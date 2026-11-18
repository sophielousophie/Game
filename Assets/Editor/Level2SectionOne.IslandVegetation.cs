using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

internal static partial class Level2SectionOne
{
    private readonly struct IslandTriangle
    {
        internal readonly Vector3 a, b, c;
        internal readonly float area;

        internal IslandTriangle(Vector3 a, Vector3 b, Vector3 c)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            area = Mathf.Abs(Vector3.Cross(b - a, c - a).y) * 0.5f;
        }
    }

    [MenuItem("Tools/Ponyo/Add vegetation to background islands")]
    private static void VegetateBackgroundIslands()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            throw new InvalidOperationException("Exit Play Mode before dressing the islands.");
        }
        ImportModels();
        string[] names = { "Island_Headland", "Island_WoodedCove", "Island_RockyShoal" };
        for (int i = 0; i < names.Length; i++)
        {
            VegetateIsland(AssetDatabase.LoadAssetAtPath<GameObject>($"{Prefabs}/{names[i]}.prefab"), i);
        }
        AssetDatabase.SaveAssets();
        Debug.Log("ISLAND_VEGETATION_OK three island prefabs updated");
    }

    private static void VegetateIsland(GameObject prefab, int variant)
    {
        if (prefab.transform.Find("IslandVegetation") != null)
        {
            return;
        }
        using var contents = new PrefabUtility.EditPrefabContentsScope(AssetDatabase.GetAssetPath(prefab));
        GameObject root = contents.prefabContentsRoot;
        var ground = new List<IslandTriangle>();
        var planting = new List<IslandTriangle>();
        float totalArea = 0f;
        foreach (MeshFilter filter in root.GetComponentsInChildren<MeshFilter>())
        {
            Mesh mesh = filter.sharedMesh;
            Matrix4x4 matrix = root.transform.worldToLocalMatrix * filter.transform.localToWorldMatrix;
            Vector3[] vertices = mesh.vertices;
            int[] indices = mesh.triangles;
            Transform[] ancestors = filter.GetComponentsInParent<Transform>();
            bool forest = Array.Exists(ancestors, parent => parent.name == "grass-forest_Size");
            bool hill = Array.Exists(ancestors, parent => parent.name == "grass-hill_Size");
            // Forest tiles include baked trees. Plant only on their low grassy terrace.
            float terrace = mesh.bounds.min.y + mesh.bounds.size.y * 0.51f;
            for (int i = 0; i < indices.Length; i += 3)
            {
                Vector3 a = vertices[indices[i]];
                Vector3 b = vertices[indices[i + 1]];
                Vector3 c = vertices[indices[i + 2]];
                var triangle = new IslandTriangle(matrix.MultiplyPoint3x4(a), matrix.MultiplyPoint3x4(b), matrix.MultiplyPoint3x4(c));
                if (triangle.area < 0.001f)
                {
                    continue;
                }
                ground.Add(triangle);
                Vector3 normal = Vector3.Cross(triangle.b - triangle.a, triangle.c - triangle.a).normalized;
                if (normal.y > 0.65f && (hill || forest && Mathf.Max(a.y, b.y, c.y) <= terrace))
                {
                    planting.Add(triangle);
                    totalArea += triangle.area;
                }
            }
        }
        if (planting.Count == 0)
        {
            throw new InvalidOperationException($"No grassy planting surfaces found on {prefab.name}");
        }

        Transform foliage = new GameObject("IslandVegetation").transform;
        foliage.SetParent(root.transform, false);
        var random = new System.Random(11817 + variant);
        var treePositions = new List<Vector3>();
        int[] counts = variant == 2 ? new[] { 4, 9, 22, 10 } : new[] { 13 + variant * 3, 18, 38, 16 };
        var flower = AssetDatabase.LoadAssetAtPath<GameObject>($"{Prefabs}/Flower_PLACEHOLDER.prefab");
        var grass = CoastalGrassPrefab("CoastalMeadow", 36, new Vector3(0.87f, 0.34f, 0.78f));
        for (int kind = 0; kind < counts.Length; kind++)
        {
            int placed = 0;
            for (int attempt = 0; attempt < counts[kind] * 35 && placed < counts[kind]; attempt++)
            {
                float pick = Range(0f, totalArea);
                IslandTriangle selected = planting[planting.Count - 1];
                foreach (IslandTriangle triangle in planting)
                {
                    pick -= triangle.area;
                    if (pick <= 0f)
                    {
                        selected = triangle;
                        break;
                    }
                }
                float u = Mathf.Sqrt(Range(0f, 1f));
                float v = Range(0f, 1f);
                Vector3 point = selected.a * (1f - u) + selected.b * (u * (1f - v)) + selected.c * (u * v);
                if (ground.Exists(triangle => SurfaceAbove(triangle, point))
                    || kind == 0 && treePositions.Exists(other => Vector2.Distance(new Vector2(other.x, other.z), new Vector2(point.x, point.z)) < 1.12f))
                {
                    continue;
                }
                point.y -= 0.018f;
                float yaw = Range(0f, 360f);
                float size = Range(0.78f, 1.22f);
                if (kind == 0)
                {
                    string[] kits = { Forest, Suburban, Survival, Survival };
                    string[] models = { "tree", "tree-large", "tree", "tree-tall" };
                    int type = (placed + variant) % models.Length;
                    float height = type == 3 ? 2.20f : 1.65f;
                    Model(foliage, kits[type], models[type], new Vector3(1.08f, height, 1.02f) * size, point, yaw);
                    treePositions.Add(point);
                }
                else if (kind == 1)
                {
                    Model(foliage, Forest, "plant", new Vector3(0.88f, 0.46f, 0.75f) * size, point, yaw);
                }
                else
                {
                    var plant = (GameObject)PrefabUtility.InstantiatePrefab(kind == 2 ? grass : flower, foliage);
                    plant.transform.localPosition = point;
                    plant.transform.localRotation = Quaternion.Euler(0f, yaw, 0f);
                    plant.transform.localScale = Vector3.one * size * (kind == 2 ? 1.25f : 1.85f);
                    PrefabUtility.RecordPrefabInstancePropertyModifications(plant.transform);
                }
                placed++;
            }
            Debug.Log($"ISLAND_PLANTS {prefab.name} kind={kind} placed={placed}");
        }
        float Range(float min, float max) => Mathf.Lerp(min, max, (float)random.NextDouble());
    }

    private static bool SurfaceAbove(IslandTriangle triangle, Vector3 point)
    {
        Vector3 ab = triangle.b - triangle.a;
        Vector3 ac = triangle.c - triangle.a;
        Vector3 ap = point - triangle.a;
        float determinant = ab.x * ac.z - ac.x * ab.z;
        float u = (ap.x * ac.z - ac.x * ap.z) / determinant;
        float v = (ab.x * ap.z - ap.x * ab.z) / determinant;
        return u >= 0f && v >= 0f && u + v <= 1f
            && triangle.a.y + u * ab.y + v * ac.y > point.y + 0.035f;
    }
}
