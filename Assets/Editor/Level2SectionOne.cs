using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using Group = Level2Layout.Group;

internal static partial class Level2SectionOne
{
    internal const string LayoutPath = "Assets/Art/Level2/Level2Layout.asset";
    internal const float CoastalSectionLength = 82.4f;
    private const string Art = "Assets/Art/Level2/Generated";
    private const string Prefabs = "Assets/Prefabs/Level2/Section1";
    // The coastal terrain spans y = -1.4 to y = 1.5.
    internal const float SeaSurfaceY = -1.4f + 2.9f * 0.40f;
    internal const float IslandBaseY = SeaSurfaceY - 0.20f;
    internal const float BuoyBaseY = SeaSurfaceY - 0.055f;
    internal static readonly Vector3 StairSpawn = new(3.65f, 0.91f, 0f);

    private const string Building = "building-kit";
    private const string Forest = "mini-forest_1.0";
    private const string Survival = "survival-kit";
    private const string Hex = "hexagon-kit";
    private const string Suburban = "city-kit-suburban_20";
    private const string Watercraft = "watercraft-pack";

    private static readonly (string kit, string name)[] Models =
    {
        (Building, "stairs-closed"), (Forest, "rocks-low"), (Forest, "rocks-high"),
        (Forest, "stones"), (Forest, "plant"), (Forest, "tree"), (Forest, "patch-dirt"),
        (Forest, "patch-grass"), (Survival, "grass"), (Survival, "fish"),
        (Hex, "water"), (Hex, "water-rocks"), (Hex, "grass-hill"), (Hex, "water-island"),
        (Hex, "grass-forest"), (Hex, "stone-hill"),
        (Forest, "tree-high"), (Survival, "fence"), (Survival, "grass-large"),
        (Suburban, "tree-small"), (Watercraft, "buoy"),
        (Suburban, "tree-large"), (Survival, "tree"), (Survival, "tree-tall"),
        (Suburban, "building-type-a")
    };

    internal static Level2Layout LoadOrCreate()
    {
        Level2Layout existing = AssetDatabase.LoadAssetAtPath<Level2Layout>(LayoutPath);
        if (existing != null)
        {
            return existing;
        }

        ImportModels();
        Level2Layout layout = ScriptableObject.CreateInstance<Level2Layout>();
        layout.playerSpawn = StairSpawn;
        layout.skybox = CreateSkybox();
        var beach = new Level2Layout.Section { name = "01_Beach", length = CoastalSectionLength, build = true };
        layout.sections.Add(beach);
        layout.sections.Add(new Level2Layout.Section { name = "02_Cliff", length = 80f });
        layout.sections.Add(new Level2Layout.Section { name = "03_CoastalRoad", length = 80f });
        layout.sections.Add(new Level2Layout.Section { name = "04_FloodedVillage", length = 112f });
        CreateTerrain(beach);
        CreateObstacles(beach);
        CreateNearDecor(beach);
        CreateWater(beach);
        CreateCoastalDressing(beach);
        AddRoadTransition(beach);
        Add(beach, Group.Checkpoints, "Checkpoint_S1_End", Prefab("Checkpoint", root =>
        {
            Trigger(root, new Vector2(0.4f, 8f), new Vector2(0f, 2f));
            root.AddComponent<Level2Checkpoint>();
        }), CoastalSectionLength - 2.2f, 2.12f);
        Add(beach, Group.EndTrigger, "Section1PreviewEnd", Prefab("Section1PreviewEnd", root =>
        {
            Trigger(root, new Vector2(0.4f, 8f), new Vector2(0f, 2f));
            root.AddComponent<Level2PreviewEnd>();
        }), CoastalSectionLength - 0.6f, 1.5f);
        layout.duration = (CoastalSectionLength + 272f) / layout.playerSpeed;
        AssetDatabase.CreateAsset(layout, LayoutPath);
        AssetDatabase.SaveAssets();
        return layout;
    }

    private static void CreateTerrain(Level2Layout.Section beach)
    {
        GameObject stairs = Prefab("CoastalStairs", root =>
        {
            Model(root.transform, Building, "stairs-closed", new Vector3(8.8f, 2.4f, 4.1f),
                new Vector3(0f, 0f, 0.65f), 90f);
            DressStairs(root.transform);
        });
        Add(beach, Group.Gameplay, "CoastalStairs", stairs, 4.4f, -0.9f);

        GameObject ground = Prefab("CoastalGround", root =>
        {
            Primitive(root.transform, "Soil", PrimitiveType.Cube, new Vector3(0f, -1.5f, 0.65f),
                new Vector3(1f, 2.8f, 4.1f), new Color(0.70f, 0.46f, 0.27f));
            Primitive(root.transform, "GrassCap", PrimitiveType.Cube, new Vector3(0f, -0.09f, 0.65f),
                new Vector3(1f, 0.18f, 4.13f), new Color(0.36f, 0.64f, 0.25f));
            Level2CoastalFronts.AddSoil(root.transform);
        });
        Add(beach, Group.Gameplay, "CoastalGround", ground, (8.8f + CoastalSectionLength) * 0.5f, 1.5f,
            scale: new Vector3(CoastalSectionLength - 8.8f, 1f, 1f));
        AddStairLanding(beach);
        Add(beach, Group.Gameplay, "CoastalWalkableSurface", CreateWalkableSurface(), 0f, 0f);
    }

    internal static void DressStairs(Transform root)
    {
        if (root.Find("StoneFoundation") == null)
        {
            // The bottom of the terrain is at y = -1.4; the stair model starts at y = -0.9.
            Primitive(root, "StoneFoundation", PrimitiveType.Cube, new Vector3(0.11f, -0.249f, 0.65f),
                new Vector3(8.58f, 0.502f, 4.1f), new Color(0.51f, 0.54f, 0.54f));
        }
        Level2CoastalFronts.AddStairs(root);
    }

    internal static GameObject CreateWalkableSurface()
    {
        return Prefab("CoastalWalkableSurface", root =>
        {
            // One edge lets the capsule cross from the slope to the flat without a collider seam.
            EdgeCollider2D surface = root.AddComponent<EdgeCollider2D>();
            surface.points = new[] { new Vector2(0f, -0.7f), new Vector2(8.08f, 1.5f), new Vector2(CoastalSectionLength, 1.5f) };
        });
    }

    private static void CreateObstacles(Level2Layout.Section beach)
    {
        Add(beach, Group.Hazards, "Rock_01_FirstJump", Obstacle("Rock_3D", "Obstacles/Rock_PLACEHOLDER", new Vector2(0.94f, 0.48f),
            root => Model(root, Forest, "rocks-low", new Vector3(1.14f, 0.54f, 0.85f), new Vector3(0f, -0.24f, 0f), 20f)), 18f, 1.74f);
        Add(beach, Group.Hazards, "Shell_01_PLACEHOLDER", Obstacle("Shell_3D_PLACEHOLDER", "Obstacles/Shell_PLACEHOLDER", new Vector2(0.55f, 0.23f),
            root => Primitive(root, "Shell_PLACEHOLDER", PrimitiveType.Sphere, Vector3.zero, new Vector3(0.70f, 0.25f, 0.42f),
                new Color(1f, 0.70f, 0.57f))), 26f, 1.615f);
        Add(beach, Group.Hazards, "Bubble_01_PLACEHOLDER", Obstacle("Bubble_3D_PLACEHOLDER", "Obstacles/BubbleTrap_PLACEHOLDER", new Vector2(0.4f, 0.41f), root =>
        {
            Primitive(root, "Bubble_PLACEHOLDER", PrimitiveType.Sphere, Vector3.zero, Vector3.one * 0.5f,
                new Color(0.27f, 0.78f, 1f, 0.53f));
            Primitive(root, "Glint", PrimitiveType.Sphere, new Vector3(-0.095f, 0.11f, -0.215f),
                new Vector3(0.09f, 0.055f, 0.03f), Color.white);
        }), 34f, 2.02f);
        Add(beach, Group.Hazards, "Puddle_01_PLACEHOLDER", Obstacle("Puddle_3D_PLACEHOLDER", "Obstacles/DeepPuddle_PLACEHOLDER", new Vector2(3.1f, 0.30f),
            root => Primitive(root, "Puddle_PLACEHOLDER", PrimitiveType.Sphere, new Vector3(0f, -0.13f, 0f),
                new Vector3(3.35f, 0.12f, 1.8f), new Color(0.14f, 0.42f, 0.65f))), 42f, 1.68f);
        Add(beach, Group.Platforms, "FishWave_01_PLACEHOLDER", Obstacle("FishWave_3D_PLACEHOLDER", "Platforms/FishWave_PLACEHOLDER", new Vector2(2.65f, 0.38f), root =>
        {
            Primitive(root, "Wave_PLACEHOLDER", PrimitiveType.Sphere, Vector3.zero, new Vector3(2.85f, 0.42f, 1.2f),
                new Color(0.20f, 0.77f, 0.89f));
            Model(root, Survival, "fish", new Vector3(1.65f, 0.62f, 0.38f), new Vector3(0f, -0.27f, -0.41f), 180f,
                MaterialFor("FishBlue", new Color(0.22f, 0.68f, 0.90f)));
        }, root =>
        {
            Rigidbody2D body = root.AddComponent<Rigidbody2D>();
            body.bodyType = RigidbodyType2D.Kinematic;
            body.interpolation = RigidbodyInterpolation2D.Interpolate;
            root.AddComponent<PlatformEffector2D>().useOneWay = true;
            root.GetComponent<BoxCollider2D>().usedByEffector = true;
            var mover = new SerializedObject(root.GetComponent<PingPongMover2D>());
            mover.FindProperty("movement").vector2Value = new Vector2(1.25f, 0.15f);
            mover.ApplyModifiedPropertiesWithoutUndo();
        }), 54f, 2.05f);
    }

    private static void CreateNearDecor(Level2Layout.Section beach)
    {
        GameObject bush = ModelPrefab("Bush", Forest, "plant", new Vector3(1.30f, 0.65f, 0.85f), 18f);
        GameObject shrub = ModelPrefab("Shrub", Suburban, "tree-small", new Vector3(0.82f, 1.24f, 0.75f), 20f);
        GameObject grass = CoastalGrassPrefab("GrassTuft", 22, new Vector3(0.58f, 0.35f, 0.45f));
        GameObject stones = ModelPrefab("Pebbles", Forest, "stones", new Vector3(0.72f, 0.24f, 0.48f), 30f);
        GameObject dirtPatch = ModelPrefab("DirtPatch", Forest, "patch-dirt", new Vector3(1.52f, 0.06f, 1f), 15f);
        GameObject grassPatch = ModelPrefab("GrassPatch", Forest, "patch-grass", new Vector3(1.62f, 0.14f, 0.85f), 15f);
        GameObject flower = Prefab("Flower_PLACEHOLDER", root =>
        {
            Primitive(root.transform, "Stem", PrimitiveType.Cylinder, new Vector3(0f, 0.13f, 0f), new Vector3(0.035f, 0.13f, 0.035f), new Color(0.30f, 0.61f, 0.24f));
            for (int i = 0; i < 4; i++)
            {
                float angle = i * Mathf.PI / 2f;
                Primitive(root.transform, "Petal", PrimitiveType.Sphere, new Vector3(Mathf.Cos(angle) * 0.085f, 0.28f + Mathf.Sin(angle) * 0.085f, 0f),
                    Vector3.one * 0.14f, new Color(0.98f, 0.44f, 0.61f));
            }
            Primitive(root.transform, "Center", PrimitiveType.Sphere, new Vector3(0f, 0.28f, -0.06f), Vector3.one * 0.13f, new Color(1f, 0.83f, 0.25f));
        });

        var plants = new[] { bush, grass, flower, stones, shrub, grass, grassPatch, flower, dirtPatch };
        for (int i = 0; i < 27; i++)
        {
            GameObject plant = plants[i % plants.Length];
            Add(beach, Group.Decor_Proche, $"{plant.name}_{i + 1:00}", plant, 9.8f + i * 1.9f,
                1.5f, 0.78f + (i % 3) * 0.46f, Vector3.one * (0.84f + (i % 5) * 0.08f), i * 31f);
        }
    }

    private static void CreateWater(Level2Layout.Section beach)
    {
        GameObject water = SeaPrefab("SeaWater", "water", new Vector3(10.5f, 0.40f, 9.0f));
        GameObject waterRocks = SeaPrefab("SeaWaterRocks", "water-rocks", new Vector3(10.5f, 1.25f, 9.0f));
        GameObject buoy = ModelPrefab("Buoy", Watercraft, "buoy", new Vector3(0.75f, 1.1f, 0.75f), 0f);
        float waterY = SeaTileOriginY(water);
        float rocksY = SeaTileOriginY(waterRocks);
        for (int row = 0; row < 5; row++)
        {
            Group group = row < 2 ? Group.Decor_Intermediaire : Group.Decor_Lointain;
            for (int tile = 0; tile < 17; tile++)
            {
                GameObject prefab = row < 3 && (tile + row * 2) % 7 == 3 ? waterRocks : water;
                float spacing = 8.15f;
                Add(beach, group, $"Sea_{row}_{tile:00}", prefab, -31f + tile * spacing + (row % 2) * spacing * 0.5f,
                    prefab == water ? waterY : rocksY, 4.5f + row * 6.2f);
            }
        }

        for (int row = 0; row < 2; row++)
        {
            for (int tile = 0; tile < 12; tile++)
            {
                Add(beach, Group.Decor_Proche, $"ShoreWater_{row}_{tile:00}", water, -10f + tile * 8.15f + row * 4.075f,
                    waterY, -8.1f + row * 6.2f);
            }
        }
        Add(beach, Group.Decor_Intermediaire, "Buoy_01", buoy, 3f, BuoyBaseY, 6.2f);
        Add(beach, Group.Decor_Intermediaire, "Buoy_02", buoy, 37f, BuoyBaseY, 10.6f);
        Level2ShoreSplashes.AddTo(beach);
    }

    internal static float SeaTileOriginY(GameObject prefab, float scaleY = 1f)
    {
        Transform surface = prefab.transform.Find("TexturedWaterSurface");
        float localWaterY = surface.localPosition.y
            + surface.GetComponent<MeshFilter>().sharedMesh.bounds.center.y * surface.localScale.y;
        return SeaSurfaceY - localWaterY * scaleY;
    }

    private static GameObject SeaPrefab(string prefab, string model, Vector3 size, float yaw = 0f)
    {
        return Prefab(prefab, root =>
        {
            Model(root.transform, Hex, model, size, Vector3.zero, yaw);
            Level2CoastalWater.AddSurface(root.transform);
        });
    }

    internal static void Primitive(Transform parent, string name, PrimitiveType type, Vector3 position, Vector3 scale, Color color)
    {
        GameObject obj = GameObject.CreatePrimitive(type);
        obj.name = name;
        obj.transform.SetParent(parent, false);
        obj.transform.localPosition = position;
        obj.transform.localScale = scale;
        UnityEngine.Object.DestroyImmediate(obj.GetComponent<Collider>());
        Color32 key = color;
        obj.GetComponent<Renderer>().sharedMaterial = MaterialFor($"Color_{ColorUtility.ToHtmlStringRGBA(key)}", key);
    }

    private static void Add(Level2Layout.Section section, Group group, string name, GameObject prefab, float x, float y,
        float z = 0f, Vector3? scale = null, float yaw = 0f)
    {
        section.elements.Add(new Level2Layout.Element
        {
            name = name, group = group, prefab = prefab, position = new Vector3(x, y, z),
            rotation = new Vector3(0f, yaw, 0f), scale = scale ?? Vector3.one
        });
    }

    private static GameObject Obstacle(string name, string source, Vector2 colliderSize, Action<Transform> visuals, Action<GameObject> setup = null)
    {
        return Prefab(name, root =>
        {
            // Copy the blockout's component settings while keeping the original prefab intact.
            GameObject template = AssetDatabase.LoadAssetAtPath<GameObject>($"Assets/Prefabs/Level2/{source}.prefab");
            if (template == null)
            {
                throw new InvalidOperationException($"Missing Level 2 gameplay prefab: {source}");
            }
            foreach (Component component in template.GetComponents<Component>())
            {
                if (component is not Transform && component is not Renderer)
                {
                    EditorUtility.CopySerialized(component, root.AddComponent(component.GetType()));
                }
            }
            root.GetComponent<BoxCollider2D>().size = colliderSize;
            visuals(root.transform);
            setup?.Invoke(root);
        });
    }

    private static void Trigger(GameObject root, Vector2 size, Vector2 offset)
    {
        BoxCollider2D box = root.AddComponent<BoxCollider2D>();
        box.size = size;
        box.offset = offset;
        box.isTrigger = true;
    }

    private static GameObject ModelPrefab(string name, string kit, string model, Vector3 targetSize, float yaw)
    {
        return Prefab(name, root => Model(root.transform, kit, model, targetSize, Vector3.zero, yaw));
    }

    private static void Model(Transform parent, string kit, string name, Vector3 targetSize, Vector3 basePosition, float yaw, Material material = null)
    {
        Transform fit = new GameObject($"{name}_Size").transform;
        fit.SetParent(parent, false);
        Transform facing = new GameObject("Orientation").transform;
        facing.SetParent(fit, false);
        facing.localRotation = Quaternion.Euler(0f, yaw, 0f);
        GameObject asset = AssetDatabase.LoadAssetAtPath<GameObject>($"{Art}/kenney_{kit}/{name}.fbx");
        if (asset == null)
        {
            throw new InvalidOperationException($"Missing imported Kenney FBX: {kit}/{name}");
        }
        var model = (GameObject)PrefabUtility.InstantiatePrefab(asset, facing);
        Renderer[] renderers = model.GetComponentsInChildren<Renderer>();
        Bounds bounds = renderers[0].bounds;
        material ??= MaterialFor($"Kenney_{kit}", Color.white,
            AssetDatabase.LoadAssetAtPath<Texture2D>($"{Art}/kenney_{kit}/Textures/colormap.png"));
        foreach (Renderer renderer in renderers)
        {
            bounds.Encapsulate(renderer.bounds);
            Material[] slots = renderer.sharedMaterials;
            Array.Fill(slots, material);
            renderer.sharedMaterials = slots;
            PrefabUtility.RecordPrefabInstancePropertyModifications(renderer);
        }

        Vector3 size = bounds.size;
        Vector3 scale = new(targetSize.x / size.x, targetSize.y / size.y, targetSize.z / size.z);
        Vector3 origin = fit.InverseTransformPoint(new Vector3(bounds.center.x, bounds.min.y, bounds.center.z));
        fit.localScale = scale;
        fit.localPosition = basePosition - Vector3.Scale(origin, scale);
    }

    private static GameObject Prefab(string name, Action<GameObject> configure)
    {
        string path = $"{Prefabs}/{name}.prefab";
        GameObject existing = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (existing != null)
        {
            return existing;
        }
        GameObject root = new(name);
        try
        {
            configure(root);
            return PrefabUtility.SaveAsPrefabAsset(root, path)
                ?? throw new IOException($"Could not save Level 2 prefab: {path}");
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(root);
        }
    }

    private static Material MaterialFor(string name, Color color, Texture texture = null)
    {
        string path = $"{Art}/Materials/{name}.mat";
        Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (material != null)
        {
            return material;
        }
        material = new Material(Shader.Find("Universal Render Pipeline/Lit")) { color = color, mainTexture = texture };
        material.SetFloat("_Smoothness", 0.18f);
        if (color.a < 1f)
        {
            material.SetOverrideTag("RenderType", "Transparent");
            material.SetFloat("_Surface", 1f);
            material.SetFloat("_SrcBlend", (float)BlendMode.SrcAlpha);
            material.SetFloat("_DstBlend", (float)BlendMode.OneMinusSrcAlpha);
            material.SetFloat("_ZWrite", 0f);
            material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            material.SetShaderPassEnabled("ShadowCaster", false);
            material.renderQueue = (int)RenderQueue.Transparent;
        }
        AssetDatabase.CreateAsset(material, path);
        return material;
    }

    private static Material CreateSkybox()
    {
        string path = $"{Art}/Materials/CoastalDay.mat";
        Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (material != null)
        {
            return material;
        }
        material = new Material(Shader.Find("Skybox/Panoramic"));
        material.SetTexture("_MainTex", AssetDatabase.LoadAssetAtPath<Texture2D>($"{Art}/kenney_skyboxes/skybox-day.png"));
        material.SetFloat("_Rotation", 115f);
        AssetDatabase.CreateAsset(material, path);
        return material;
    }

    private static void ImportModels()
    {
        Directory.CreateDirectory(Prefabs);
        Directory.CreateDirectory($"{Art}/Materials");
        foreach ((string kit, string name) in Models)
        {
            string source = $"3d_assets/kenney_{kit}";
            string destination = $"{Art}/kenney_{kit}";
            CopyIfMissing($"{source}/Models/FBX format/{name}.fbx", $"{destination}/{name}.fbx");
            CopyIfMissing($"{source}/Models/FBX format/Textures/colormap.png", $"{destination}/Textures/colormap.png");
            CopyIfMissing($"{source}/License.txt", $"{destination}/License.txt");
        }
        CopyIfMissing("3d_assets/kenney_skyboxes/Skyboxes/skybox-day.png", $"{Art}/kenney_skyboxes/skybox-day.png");
        CopyIfMissing("3d_assets/kenney_skyboxes/License.txt", $"{Art}/kenney_skyboxes/License.txt");
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
    }

    private static void CopyIfMissing(string source, string destination)
    {
        if (File.Exists(destination))
        {
            return;
        }
        Directory.CreateDirectory(Path.GetDirectoryName(destination));
        File.Copy(source, destination);
    }
}
