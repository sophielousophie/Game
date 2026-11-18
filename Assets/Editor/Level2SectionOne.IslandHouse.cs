using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

internal static partial class Level2SectionOne
{
    private static GameObject CreateHouseIsland(GameObject shoal)
    {
        return Prefab("Island_HouseShoal", root =>
        {
            PrefabUtility.InstantiatePrefab(shoal, root.transform);
            Model(root.transform, Suburban, "building-type-a", new Vector3(2.50f, 2.35f, 2.10f),
                new Vector3(-1.55f, 1.37f, -0.25f), 160f);

            // The terrace of the forest tile sits at half its full height, below its baked trees.
            Model(root.transform, Suburban, "tree-large", new Vector3(1.25f, 1.85f, 1.15f),
                new Vector3(1.45f, 0.83f, 0.60f), 28f);
            Model(root.transform, Survival, "tree-tall", new Vector3(0.90f, 2.25f, 0.85f),
                new Vector3(0.35f, 0.83f, 1.15f), -31f);
            Model(root.transform, Forest, "tree", new Vector3(1.05f, 1.65f, 0.95f),
                new Vector3(2.05f, 0.83f, -0.30f), 67f);
            DressHouseBase(root);
        });
    }

    private static void DressHouseBase(GameObject root)
    {
        Transform house = root.transform.Find("building-type-a_Size");
        Vector3 position = house.localPosition;
        position.y = IslandRockHeight(root.transform, -1.55f, -0.25f) - 0.06f;
        house.localPosition = position;
        Transform support = root.transform.Find("stones_Size");
        if (support != null)
        {
            UnityEngine.Object.DestroyImmediate(support.gameObject);
        }
        if (root.transform.Find("HouseLeftVegetation") != null)
        {
            return;
        }
        Transform plants = new GameObject("HouseLeftVegetation").transform;
        plants.SetParent(root.transform, false);
        Plant(Suburban, "tree-small", new Vector3(0.70f, 0.95f, 0.65f), -3.65f, 0.55f, 21f);
        Plant(Forest, "plant", new Vector3(0.68f, 0.36f, 0.56f), -3.30f, -0.65f, 58f);
        Plant(Forest, "plant", new Vector3(0.50f, 0.28f, 0.42f), -4.03f, -0.13f, -24f);
        for (int i = 0; i < 4; i++)
        {
            Plant(Survival, "grass", new Vector3(0.43f, 0.24f, 0.37f),
                -3.05f - i * 0.27f, -0.95f + i * 0.38f, i * 57f);
        }

        void Plant(string kit, string model, Vector3 size, float x, float z, float yaw)
        {
            float height = IslandRockHeight(root.transform, x, z);
            Model(plants, kit, model, size, new Vector3(x, height - 0.025f, z), yaw);
        }
    }

    private static float IslandRockHeight(Transform root, float x, float z)
    {
        float height = float.NegativeInfinity;
        foreach (MeshFilter filter in root.GetComponentsInChildren<MeshFilter>())
        {
            if (!Array.Exists(filter.GetComponentsInParent<Transform>(), parent => parent.name == "rocks-low_Size"))
            {
                continue;
            }
            Matrix4x4 matrix = root.worldToLocalMatrix * filter.transform.localToWorldMatrix;
            Vector3[] vertices = filter.sharedMesh.vertices;
            int[] triangles = filter.sharedMesh.triangles;
            for (int i = 0; i < triangles.Length; i += 3)
            {
                Vector3 a = matrix.MultiplyPoint3x4(vertices[triangles[i]]);
                Vector3 ab = matrix.MultiplyPoint3x4(vertices[triangles[i + 1]]) - a;
                Vector3 ac = matrix.MultiplyPoint3x4(vertices[triangles[i + 2]]) - a;
                float determinant = ab.x * ac.z - ac.x * ab.z;
                if (Mathf.Abs(determinant) < 0.00001f)
                {
                    continue;
                }
                float u = ((x - a.x) * ac.z - ac.x * (z - a.z)) / determinant;
                float v = (ab.x * (z - a.z) - (x - a.x) * ab.z) / determinant;
                if (u >= 0f && v >= 0f && u + v <= 1f)
                {
                    height = Mathf.Max(height, a.y + u * ab.y + v * ac.y);
                }
            }
        }
        if (float.IsNegativeInfinity(height))
        {
            throw new InvalidOperationException($"No rock under the island plant at {x}, {z}.");
        }
        return height;
    }

    [MenuItem("Tools/Ponyo/Refine island house base")]
    private static void RefineIslandHouseBase()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            throw new InvalidOperationException("Exit Play Mode before adjusting the island house.");
        }
        using (var contents = new PrefabUtility.EditPrefabContentsScope($"{Prefabs}/Island_HouseShoal.prefab"))
        {
            DressHouseBase(contents.prefabContentsRoot);
        }
        AssetDatabase.SaveAssets();
        Debug.Log("ISLAND_HOUSE_BASE_OK embeddedInGreyRock=0.06 leftPlants=7");
    }

    [MenuItem("Tools/Ponyo/Add house to final background island")]
    private static void AddFinalIslandHouse()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            throw new InvalidOperationException("Exit Play Mode before dressing the final island.");
        }
        ImportModels();
        GameObject shoal = AssetDatabase.LoadAssetAtPath<GameObject>($"{Prefabs}/Island_RockyShoal.prefab");
        GameObject houseIsland = CreateHouseIsland(shoal);
        Level2Layout layout = LoadOrCreate();
        Level2Layout.Element item = layout.sections[0].elements.Find(element => element.name == "CoastIsland_FarEastShoal")
            ?? throw new InvalidOperationException("The final background island is missing from the layout.");
        Undo.RecordObject(layout, "Add island house");
        item.prefab = houseIsland;
        EditorUtility.SetDirty(layout);

        Scene scene = SceneManager.GetSceneByPath(Level2Builder.ScenePath);
        if (!scene.IsValid() || !scene.isLoaded)
        {
            scene = EditorSceneManager.OpenScene(Level2Builder.ScenePath, OpenSceneMode.Additive);
        }
        Transform background = null;
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            if (root.name == "Generated_Level2")
            {
                background = root.transform.Find("Decor_Lointain");
            }
        }
        if (background == null)
        {
            throw new InvalidOperationException("The Level 2 background group is missing.");
        }
        Transform existing = background.Find(item.name);
        Vector3 position = existing != null ? existing.localPosition : item.position;
        Quaternion rotation = existing != null ? existing.localRotation : Quaternion.Euler(item.rotation);
        Vector3 scale = existing != null ? existing.localScale : item.scale;
        if (existing != null)
        {
            Undo.DestroyObjectImmediate(existing.gameObject);
        }
        var island = (GameObject)PrefabUtility.InstantiatePrefab(houseIsland, background);
        Undo.RegisterCreatedObjectUndo(island, "Add island house");
        island.name = item.name;
        island.transform.localPosition = position;
        island.transform.localRotation = rotation;
        island.transform.localScale = scale;
        PrefabUtility.RecordPrefabInstancePropertyModifications(island.transform);
        AssetDatabase.SaveAssets();
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Selection.activeGameObject = island;
        Debug.Log("ISLAND_HOUSE_OK final background island has a Suburban house and three additional trees");
    }
}
