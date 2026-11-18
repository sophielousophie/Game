using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Group = Level2Layout.Group;

internal static partial class Level2SectionOne
{
    private static void AddRoadTransition(Level2Layout.Section beach)
    {
        const Group decor = Group.Decor_Proche;
        GameObject path = Prefab("CoastalUrbanDirtPath", root =>
        {
            root.AddComponent<MeshFilter>().sharedMesh = UrbanPathMesh();
            root.AddComponent<MeshRenderer>().sharedMaterial = Level2CoastalFronts.LoadMaterial("CoastalSoil");
        });
        Add(beach, decor, "UrbanApproach_DirtPath", path, 59f, 1.5f);

        GameObject rail = Prefab("WhiteHighwayGuardrail", root =>
        {
            Color white = new(0.91f, 0.94f, 0.91f);
            Primitive(root.transform, "Rail", PrimitiveType.Cube, new Vector3(0f, 0.64f, 0f),
                new Vector3(2.1f, 0.24f, 0.10f), white);
            Primitive(root.transform, "UpperFold", PrimitiveType.Cube, new Vector3(0f, 0.77f, -0.035f),
                new Vector3(2.1f, 0.045f, 0.16f), white);
            Primitive(root.transform, "LowerFold", PrimitiveType.Cube, new Vector3(0f, 0.51f, -0.035f),
                new Vector3(2.1f, 0.045f, 0.16f), white);
            Primitive(root.transform, "Post", PrimitiveType.Cube, new Vector3(0f, 0.34f, 0.08f),
                new Vector3(0.10f, 0.68f, 0.10f), new Color(0.77f, 0.83f, 0.83f));
            Primitive(root.transform, "Reflector", PrimitiveType.Cube, new Vector3(0f, 0.66f, -0.067f),
                new Vector3(0.08f, 0.07f, 0.024f), new Color(0.98f, 0.63f, 0.26f));
        });
        for (int i = 0; i < 10; i++)
        {
            Add(beach, decor, $"UrbanApproach_Rail_{i:00}", rail, 62.25f + 2.1f * i, 1.5f, 2.18f);
        }
        GameObject lamp = Prefab("CoastalRoadLamp", root =>
        {
            Color metal = new(0.57f, 0.64f, 0.65f);
            Primitive(root.transform, "Pole", PrimitiveType.Cylinder, new Vector3(0f, 1.32f, 0f),
                new Vector3(0.075f, 1.32f, 0.075f), metal);
            Primitive(root.transform, "Arm", PrimitiveType.Cube, new Vector3(-0.31f, 2.61f, 0f),
                new Vector3(0.68f, 0.06f, 0.06f), metal);
            Primitive(root.transform, "Lamp", PrimitiveType.Cube, new Vector3(-0.62f, 2.58f, 0f),
                new Vector3(0.36f, 0.10f, 0.23f), new Color(0.94f, 0.94f, 0.82f));
        });
        Add(beach, decor, "UrbanApproach_Lamp_01", lamp, 68f, 1.5f, 2.48f);
        Add(beach, decor, "UrbanApproach_Lamp_02", lamp, 79.5f, 1.5f, 2.48f);

        GameObject grass = AssetDatabase.LoadAssetAtPath<GameObject>($"{Prefabs}/CoastalMeadow.prefab");
        for (int i = 0; i < 14; i++)
        {
            Add(beach, decor, $"UrbanApproach_Grass_{i:00}", grass, 64.5f + i * 1.24f, 1.5f,
                i % 2 == 0 ? -1.09f : 1.79f, Vector3.one * (0.42f + i % 3 * 0.08f), i * 47f);
        }
        Add(beach, Group.Hazards, "UrbanApproach_Rock", AssetDatabase.LoadAssetAtPath<GameObject>($"{Prefabs}/Rock_3D.prefab"), 67.4f, 1.74f);
        Add(beach, Group.Hazards, "UrbanApproach_Shell", AssetDatabase.LoadAssetAtPath<GameObject>($"{Prefabs}/Shell_3D_PLACEHOLDER.prefab"), 71.2f, 1.615f);
        Add(beach, Group.Hazards, "UrbanApproach_Bubble", AssetDatabase.LoadAssetAtPath<GameObject>($"{Prefabs}/Bubble_3D_PLACEHOLDER.prefab"), 76.5f, 2.02f);
    }

    private static Mesh UrbanPathMesh()
    {
        const string path = "Assets/Art/Level2/Terrain/SurfaceTextures/CoastalUrbanDirtPath.asset";
        Mesh mesh = AssetDatabase.LoadAssetAtPath<Mesh>(path);
        if (mesh != null)
        {
            return mesh;
        }
        var vertices = new List<Vector3>();
        var uv = new List<Vector2>();
        var triangles = new List<int>();
        for (int i = 0; i <= 24; i++)
        {
            float x = (CoastalSectionLength - 59f) * i / 24f;
            float width = Mathf.Lerp(0.06f, 1.13f, Mathf.SmoothStep(0f, 1f, x / 6f));
            float irregularity = Mathf.Sin(i * 2.7f) * 0.13f * (1f - Mathf.Clamp01(x / 12f));
            vertices.Add(new Vector3(x, 0.012f, 0.23f - width + irregularity));
            vertices.Add(new Vector3(x, 0.012f, 0.23f + width - irregularity * 0.7f));
            uv.Add(new Vector2(x / 3.5f, (0.23f - width) / 3.5f));
            uv.Add(new Vector2(x / 3.5f, (0.23f + width) / 3.5f));
            if (i > 0)
            {
                int start = i * 2;
                triangles.AddRange(new[] { start - 2, start - 1, start, start - 1, start + 1, start });
            }
        }
        mesh = new Mesh { name = "CoastalUrbanDirtPath" };
        mesh.SetVertices(vertices);
        mesh.SetUVs(0, uv);
        mesh.SetTriangles(triangles, 0);
        mesh.RecalculateNormals();
        AssetDatabase.CreateAsset(mesh, path);
        return mesh;
    }

    [MenuItem("Tools/Ponyo/Extend coastal section and add urban approach")]
    internal static void ExtendCoastalSection()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            throw new InvalidOperationException("Exit Play Mode before extending the coast.");
        }
        Level2Layout layout = LoadOrCreate();
        Level2Layout.Section beach = layout.sections[0];
        Undo.RecordObject(layout, "Extend coastal section");
        beach.length = CoastalSectionLength;
        layout.duration = layout.sections.Sum(section => section.length) / layout.playerSpeed;
        Level2Layout.Element ground = beach.elements.Find(item => item.name == "CoastalGround");
        ground.position.x = (8.8f + CoastalSectionLength) * 0.5f;
        ground.scale.x = CoastalSectionLength - 8.8f;
        beach.elements.Find(item => item.name == "Section1PreviewEnd").position.x = CoastalSectionLength - 0.6f;
        beach.elements.Find(item => item.name == "Checkpoint_S1_End").position.x = CoastalSectionLength - 2.2f;
        beach.elements.RemoveAll(item => item.name.StartsWith("UrbanApproach_", StringComparison.Ordinal));
        AddRoadTransition(beach);
        using (var contents = new PrefabUtility.EditPrefabContentsScope($"{Prefabs}/CoastalWalkableSurface.prefab"))
        {
            EdgeCollider2D edge = contents.prefabContentsRoot.GetComponent<EdgeCollider2D>();
            Vector2[] points = edge.points;
            points[points.Length - 1].x = CoastalSectionLength;
            edge.points = points;
        }
        using (var contents = new PrefabUtility.EditPrefabContentsScope($"{Prefabs}/CoastalGround.prefab"))
        {
            Level2CoastalFronts.AddSoil(contents.prefabContentsRoot.transform);
        }
        Level2CoastalWater.ConfigureShoreline(layout);
        EditorUtility.SetDirty(layout);
        AssetDatabase.SaveAssets();
        Level2Builder.Build();
        Debug.Log("COASTAL_EXTENSION_OK extraRunSeconds=5 end=82.4 whiteGuardrail=true");
    }
}
