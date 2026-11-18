using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

internal static class Level2IslandSoil
{
    internal static void ApplyTo(GameObject prefab)
    {
        const string visualName = "IslandSoilSurface";
        if (prefab.transform.Find(visualName) != null)
        {
            return;
        }
        using var contents = new PrefabUtility.EditPrefabContentsScope(AssetDatabase.GetAssetPath(prefab));
        Transform root = contents.prefabContentsRoot.transform;
        var vertices = new List<Vector3>();
        var uv = new List<Vector2>();
        var triangles = new List<int>();
        foreach (MeshFilter source in root.GetComponentsInChildren<MeshFilter>())
        {
            if (!System.Array.Exists(source.GetComponentsInParent<Transform>(), parent =>
                parent.name == "grass-hill_Size" || parent.name == "grass-forest_Size" || parent.name == "stone-hill_Size"))
            {
                continue;
            }
            Mesh mesh = source.sharedMesh;
            Vector3[] positions = mesh.vertices;
            Vector2[] atlas = mesh.uv;
            if (atlas.Length != positions.Length)
            {
                continue;
            }
            Matrix4x4 toRoot = root.worldToLocalMatrix * source.transform.localToWorldMatrix;
            int[] indices = mesh.triangles;
            for (int i = 0; i < indices.Length; i += 3)
            {
                int a = indices[i], b = indices[i + 1], c = indices[i + 2];
                if (!IsSoil(atlas[a]) || !IsSoil(atlas[b]) || !IsSoil(atlas[c]))
                {
                    continue;
                }
                Vector3 first = toRoot.MultiplyPoint3x4(positions[a]);
                Vector3 second = toRoot.MultiplyPoint3x4(positions[b]);
                Vector3 third = toRoot.MultiplyPoint3x4(positions[c]);
                Vector3 normal = Vector3.Cross(second - first, third - first).normalized;
                Add(first, normal);
                Add(second, normal);
                Add(third, normal);
            }
        }
        if (vertices.Count == 0)
        {
            throw new System.InvalidOperationException($"No soil faces found on {prefab.name}");
        }
        string path = $"Assets/Art/Level2/Terrain/SurfaceTextures/{prefab.name}_Soil.asset";
        var surface = new Mesh { name = $"{prefab.name}_Soil" };
        surface.SetVertices(vertices);
        surface.SetUVs(0, uv);
        surface.SetTriangles(triangles, 0);
        surface.RecalculateNormals();
        AssetDatabase.CreateAsset(surface, path);
        Transform visual = new GameObject(visualName).transform;
        visual.SetParent(root, false);
        visual.gameObject.AddComponent<MeshFilter>().sharedMesh = surface;
        MeshRenderer renderer = visual.gameObject.AddComponent<MeshRenderer>();
        renderer.sharedMaterial = Level2CoastalFronts.LoadMaterial("CoastalSoil");
        renderer.shadowCastingMode = ShadowCastingMode.Off;
        Debug.Log($"ISLAND_SOIL_OK {prefab.name} triangles={triangles.Count / 3}");

        void Add(Vector3 position, Vector3 normal)
        {
            triangles.Add(vertices.Count);
            vertices.Add(position + normal * 0.008f);
            uv.Add((Mathf.Abs(normal.z) >= Mathf.Abs(normal.x)
                ? new Vector2(position.x, position.y) : new Vector2(position.z, position.y)) / 3.5f);
        }
    }

    // The orange earth band in the original hexagon-kit atlas.
    private static bool IsSoil(Vector2 uv) =>
        (uv - new Vector2(0.90625f, 0.125f)).sqrMagnitude < 0.000001f;
}
