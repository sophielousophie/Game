using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

internal static class Level2CoastalFronts
{
    private const string AssetsPath = "Assets/Art/Level2/Terrain/SurfaceTextures";
    private const float FaceOffset = 0.005f;

    internal static void AddStairs(Transform root)
    {
        AddFront(root, root.GetComponentsInChildren<MeshFilter>(), "CoastalMossyStone",
            new Vector2(1f, 1f) / 2.6f, includeStairTreads: true);
    }

    internal static void AddSoil(Transform root)
    {
        AddFront(root, new[] { root.Find("Soil").GetComponent<MeshFilter>() }, "CoastalSoil",
            new Vector2(Level2SectionOne.CoastalSectionLength - 8.8f, 1f) / 3.5f);
    }

    private static void AddFront(Transform root, MeshFilter[] sources, string name, Vector2 uvScale,
        bool includeStairTreads = false)
    {
        string objectName = $"{name}_Front";
        Transform existing = root.Find(objectName);
        Transform foundation = root.Find("StoneFoundation");
        var vertices = new List<Vector3>();
        var uv = new List<Vector2>();
        var indices = new List<int>();
        foreach (MeshFilter source in sources)
        {
            if (source.transform == existing)
            {
                continue;
            }
            Mesh mesh = source.sharedMesh;
            Vector3[] positions = mesh.vertices;
            Matrix4x4 toRoot = root.worldToLocalMatrix * source.transform.localToWorldMatrix;
            int[] triangles = mesh.triangles;
            for (int i = 0; i < triangles.Length; i += 3)
            {
                Vector3 a = toRoot.MultiplyPoint3x4(positions[triangles[i]]);
                Vector3 b = toRoot.MultiplyPoint3x4(positions[triangles[i + 1]]);
                Vector3 c = toRoot.MultiplyPoint3x4(positions[triangles[i + 2]]);
                Vector3 normal = Vector3.Cross(b - a, c - a).normalized;
                bool tread = includeStairTreads && normal.y > 0.999f && source.transform != foundation;
                bool riser = includeStairTreads && normal.x < -0.999f;
                if (!tread && !riser && normal.z > -0.999f)
                {
                    continue;
                }

                AddVertex(a, normal, tread, riser);
                AddVertex(b, normal, tread, riser);
                AddVertex(c, normal, tread, riser);
            }
        }
        if (vertices.Count == 0)
        {
            throw new InvalidOperationException($"No front-facing triangles for {name}.");
        }

        string meshPath = $"{AssetsPath}/{name}Front.asset";
        Mesh frontMesh = AssetDatabase.LoadAssetAtPath<Mesh>(meshPath);
        bool create = frontMesh == null;
        if (create)
        {
            frontMesh = new Mesh { name = $"{name}Front" };
        }
        frontMesh.Clear();
        frontMesh.SetVertices(vertices);
        frontMesh.SetUVs(0, uv);
        frontMesh.SetTriangles(indices, 0);
        frontMesh.RecalculateNormals();
        if (create)
        {
            AssetDatabase.CreateAsset(frontMesh, meshPath);
        }
        else
        {
            EditorUtility.SetDirty(frontMesh);
        }

        GameObject front = existing != null ? existing.gameObject : new GameObject(objectName);
        front.transform.SetParent(root, false);
        front.transform.localPosition = Vector3.zero;
        front.transform.localScale = Vector3.one;
        MeshFilter filter = front.GetComponent<MeshFilter>() ?? front.AddComponent<MeshFilter>();
        filter.sharedMesh = frontMesh;
        MeshRenderer renderer = front.GetComponent<MeshRenderer>() ?? front.AddComponent<MeshRenderer>();
        renderer.sharedMaterial = LoadMaterial(name);
        renderer.shadowCastingMode = ShadowCastingMode.Off;

        void AddVertex(Vector3 position, Vector3 normal, bool tread, bool riser)
        {
            indices.Add(vertices.Count);
            vertices.Add(position + normal * FaceOffset);
            Vector2 projected = tread ? new Vector2(position.x, position.z)
                : riser ? new Vector2(position.z, position.y) : new Vector2(position.x, position.y);
            uv.Add(Vector2.Scale(projected, uvScale));
        }
    }

    internal static Material LoadMaterial(string name)
    {
        string path = $"{AssetsPath}/{name}Front.mat";
        Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (material != null)
        {
            return material;
        }

        Texture2D texture = name == "CoastalMossyStone"
            ? AssetDatabase.LoadAssetAtPath<Texture2D>($"{AssetsPath}/CoastalStairStone.png") : null;
        if (texture == null && name == "CoastalMossyStone")
        {
            texture = AssetDatabase.LoadAssetAtPath<Texture2D>($"{AssetsPath}/CoastalStairStone.jpg");
        }
        texture ??= AssetDatabase.LoadAssetAtPath<Texture2D>($"{AssetsPath}/{name}.png");
        if (texture == null)
        {
            throw new InvalidOperationException($"Missing front texture: {name}.");
        }
        material = new Material(Shader.Find("Universal Render Pipeline/Lit")) { mainTexture = texture };
        material.SetFloat("_Smoothness", 0f);
        AssetDatabase.CreateAsset(material, path);
        return material;
    }
}
