using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Group = Level2Layout.Group;

internal static partial class Level2SectionOne
{
    private static void AddWildGrass(Level2Layout.Section beach)
    {
        beach.elements.RemoveAll(item => item.name.StartsWith("WildGrass_", StringComparison.Ordinal));
        GameObject meadow = CoastalGrassPrefab("CoastalMeadow", 36, new Vector3(0.87f, 0.34f, 0.78f));
        var random = new System.Random(11732);
        float[] bankDepths = { -1.02f, 1.95f };
        // Low, separated patches frame the path and leave small props visible.
        for (int row = 0; row < bankDepths.Length; row++)
        {
            for (int i = 0; i < 75; i++)
            {
                float x = 9.15f + i * 0.72f + Range(-0.18f, 0.18f);
                float z = bankDepths[row] + Range(-0.15f, 0.15f);
                float size = Range(0.63f, 0.98f);
                float height = row == 0 ? Range(0.30f, 0.50f) : Range(0.48f, 0.82f);
                Place($"WildGrass_Bank_{row}_{i:000}", x, 1.5f, z, new Vector3(size, height, size));
            }
        }
        const int step = 7;
        for (int patch = 0; patch < 3; patch++)
        {
            float x = (step + 0.5f) * 1.1f + Range(-0.29f, 0.29f);
            float y = -0.9f + (0.4f + 0.3f * step) * 0.96f;
            float z = patch == 0 ? Range(-1.10f, -0.85f) : Range(1.50f, 2.03f);
            Place($"WildGrass_Stair_{step}_{patch}", x, y, z,
                new Vector3(Range(0.56f, 0.80f), Range(0.32f, 0.60f), Range(0.52f, 0.76f)));
        }

        float Range(float min, float max) => Mathf.Lerp(min, max, (float)random.NextDouble());

        void Place(string name, float x, float y, float z, Vector3 scale)
        {
            float yaw = Range(0f, 360f);
            bool coversProp = beach.elements.Exists(item =>
                (item.group == Group.Hazards || item.group == Group.Platforms)
                    && Mathf.Abs(item.position.x - x) < 1.9f
                || item.prefab != null
                    && (item.prefab.name == "Flower_PLACEHOLDER" || item.prefab.name == "Pebbles")
                    && Mathf.Abs(item.position.x - x) < 0.85f);
            if (!coversProp)
            {
                Add(beach, Group.Decor_Proche, name, meadow, x, y, z, scale, yaw);
            }
        }
    }

    [MenuItem("Tools/Ponyo/Apply coastal grass distribution")]
    internal static void DensifyCoastalGrass()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            throw new InvalidOperationException("Exit Play Mode before adding coastal grass.");
        }
        Scene scene = SceneManager.GetSceneByPath(Level2Builder.ScenePath);
        if (!scene.IsValid() || !scene.isLoaded)
        {
            scene = EditorSceneManager.OpenScene(Level2Builder.ScenePath, OpenSceneMode.Additive);
        }
        Transform decor = null;
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            if (root.name == "Generated_Level2")
            {
                decor = root.transform.Find("Decor_Proche");
            }
        }
        if (decor == null)
        {
            throw new InvalidOperationException("Build the Level 2 scene before adding coastal grass.");
        }
        Level2Layout layout = LoadOrCreate();
        Undo.RecordObject(layout, "Densify coastal grass");
        AddWildGrass(layout.sections[0]);
        for (int i = decor.childCount - 1; i >= 0; i--)
        {
            GameObject child = decor.GetChild(i).gameObject;
            if (child.name.StartsWith("WildGrass_", StringComparison.Ordinal))
            {
                Undo.DestroyObjectImmediate(child);
            }
        }
        int added = 0;
        foreach (Level2Layout.Element item in layout.sections[0].elements)
        {
            if (!item.name.StartsWith("WildGrass_", StringComparison.Ordinal) || decor.Find(item.name) != null)
            {
                continue;
            }
            var instance = (GameObject)PrefabUtility.InstantiatePrefab(item.prefab, decor);
            Undo.RegisterCreatedObjectUndo(instance, "Densify coastal grass");
            instance.name = item.name;
            instance.transform.localPosition = item.position + Vector3.up * layout.coastalElevationOffset;
            instance.transform.localEulerAngles = item.rotation;
            instance.transform.localScale = item.scale;
            PrefabUtility.RecordPrefabInstancePropertyModifications(instance.transform);
            added++;
        }
        EditorUtility.SetDirty(layout);
        AssetDatabase.SaveAssets();
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log($"COASTAL_WILD_GRASS_OK added={added}");
    }
}
