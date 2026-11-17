using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

internal static class Level2CoastalWater
{
    private const string AssetsPath = "Assets/Art/Level2/Terrain/SurfaceTextures";
    // The three hexagon-kit models share this water swatch in their color atlas.
    private static readonly Vector2 WaterSwatch = new(0.15625f, 0.125f);

    internal static void AddSurface(Transform root)
    {
        var vertices = new List<Vector3>();
        var indices = new List<int>();
        foreach (MeshFilter source in root.GetComponentsInChildren<MeshFilter>())
        {
            Mesh mesh = source.sharedMesh;
            Vector3[] positions = mesh.vertices;
            Vector2[] uv = mesh.uv;
            int[] triangles = mesh.triangles;
            Matrix4x4 toRoot = root.worldToLocalMatrix * source.transform.localToWorldMatrix;
            for (int i = 0; i < triangles.Length; i += 3)
            {
                int a = triangles[i], b = triangles[i + 1], c = triangles[i + 2];
                if (!IsWater(uv[a]) || !IsWater(uv[b]) || !IsWater(uv[c]))
                {
                    continue;
                }
                Vector3 first = toRoot.MultiplyPoint3x4(positions[a]);
                Vector3 second = toRoot.MultiplyPoint3x4(positions[b]);
                Vector3 third = toRoot.MultiplyPoint3x4(positions[c]);
                Vector3 normal = Vector3.Cross(second - first, third - first).normalized;
                if (normal.y < -0.001f)
                {
                    continue;
                }
                indices.Add(vertices.Count);
                indices.Add(vertices.Count + 1);
                indices.Add(vertices.Count + 2);
                Vector3 offset = normal * 0.005f;
                vertices.Add(first + offset);
                vertices.Add(second + offset);
                vertices.Add(third + offset);
            }
        }
        if (vertices.Count == 0)
        {
            throw new InvalidOperationException($"No water surface found in {root.name}.");
        }

        string name = $"{root.name}Surface";
        string path = $"{AssetsPath}/{name}.asset";
        Mesh surfaceMesh = AssetDatabase.LoadAssetAtPath<Mesh>(path);
        bool create = surfaceMesh == null;
        if (create)
        {
            surfaceMesh = new Mesh { name = name };
        }
        surfaceMesh.Clear();
        surfaceMesh.SetVertices(vertices);
        surfaceMesh.SetTriangles(indices, 0);
        if (create)
        {
            AssetDatabase.CreateAsset(surfaceMesh, path);
        }
        else
        {
            EditorUtility.SetDirty(surfaceMesh);
        }

        var surface = new GameObject("TexturedWaterSurface");
        surface.transform.SetParent(root, false);
        surface.AddComponent<MeshFilter>().sharedMesh = surfaceMesh;
        var renderer = surface.AddComponent<MeshRenderer>();
        renderer.sharedMaterial = GetMaterial();
        renderer.shadowCastingMode = ShadowCastingMode.Off;
    }

    private static bool IsWater(Vector2 uv) => (uv - WaterSwatch).sqrMagnitude < 0.000001f;

    private static Material GetMaterial()
    {
        string path = $"{AssetsPath}/CoastalWater.mat";
        Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (material != null)
        {
            return material;
        }
        material = new Material(Shader.Find("Ponyo/Level2/CoastalWater"))
        {
            mainTexture = AssetDatabase.LoadAssetAtPath<Texture2D>($"{AssetsPath}/CoastalWater.png")
        };
        AssetDatabase.CreateAsset(material, path);
        return material;
    }
}
