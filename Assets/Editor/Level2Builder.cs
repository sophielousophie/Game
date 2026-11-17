using System;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Group = Level2Layout.Group;
using Object = UnityEngine.Object;

public static class Level2Builder
{
    internal const string ScenePath = "Assets/Scenes/Level2_Ponyo.unity";
    private const string RootName = "Generated_Level2";
    private const string BuildLabel = "Build Level 2";

    [MenuItem("Tools/Ponyo/Build Level 2")]
    public static void Build()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            throw new InvalidOperationException("Exit Play Mode before building Level 2.");
        }
        if (AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath) == null)
        {
            throw new InvalidOperationException($"Level 2 scene not found: {ScenePath}");
        }

        Scene previous = SceneManager.GetActiveScene();
        Scene scene = SceneManager.GetSceneByPath(ScenePath);
        bool alreadyOpen = scene.IsValid() && scene.isLoaded;
        if (alreadyOpen && scene.isDirty)
        {
            throw new InvalidOperationException("Save your open Level 2 scene before rebuilding it.");
        }
        if (!alreadyOpen)
        {
            scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Additive);
        }

        SceneManager.SetActiveScene(scene);
        try
        {
            Level2Layout layout = Level2SectionOne.LoadOrCreate();
            Validate(layout);
            Rebuild(scene, layout);
            Debug.Log($"LEVEL2_BUILD_OK scene={ScenePath} distance={layout.TotalDistance:F1}u");
        }
        finally
        {
            if (previous.IsValid() && previous.isLoaded && previous != scene)
            {
                SceneManager.SetActiveScene(previous);
            }
            if (!alreadyOpen)
            {
                EditorSceneManager.CloseScene(scene, true);
            }
        }
    }

    private static void Validate(Level2Layout layout)
    {
        if (!(layout.playerSpeed > 0f) || !(layout.duration > 0f) || layout.sections.Count == 0
            || layout.sections.Any(section => section == null || !(section.length > 0f))
            || !Mathf.Approximately(layout.sections.Sum(section => section.length), layout.TotalDistance))
        {
            throw new InvalidOperationException("The section lengths must sum to the player speed multiplied by the duration.");
        }
        if (layout.skybox == null || !layout.sections.Any(section => section.build))
        {
            throw new InvalidOperationException("Assign a skybox and enable at least one section in the layout.");
        }
        foreach (Level2Layout.Element item in layout.sections.Where(section => section.build).SelectMany(section => section.elements))
        {
            if (item == null || item.prefab == null || !PrefabUtility.IsPartOfPrefabAsset(item.prefab)
                || !Enum.IsDefined(typeof(Group), item.group))
            {
                throw new InvalidOperationException($"Invalid layout prefab or group: {item?.name}");
            }
        }
    }

    private static void Rebuild(Scene scene, Level2Layout layout)
    {
        GameObject[] sceneRoots = scene.GetRootGameObjects();
        GameObject[] previousRoots = sceneRoots.Where(root => root.name == RootName).ToArray();
        Undo.IncrementCurrentGroup();
        int undoGroup = Undo.GetCurrentGroup();
        Undo.SetCurrentGroupName(BuildLabel);
        GameObject generated = new(RootName);
        try
        {
            Transform[] groups = new Transform[Enum.GetValues(typeof(Group)).Length];
            foreach (Group group in Enum.GetValues(typeof(Group)))
            {
                groups[(int)group] = Child(generated.transform, group.ToString());
            }

            Transform Get(Group group) => groups[(int)group];
            Transform timeline = Child(Get(Group.Gameplay), "Timeline");
            Child(timeline, "S1_START_0s").localPosition = new Vector3(0f, 5f, 2f);
            float offset = 0f;
            for (int i = 0; i < layout.sections.Count; i++)
            {
                Level2Layout.Section section = layout.sections[i];
                if (section.build)
                {
                    foreach (Level2Layout.Element element in section.elements)
                    {
                        var instance = (GameObject)PrefabUtility.InstantiatePrefab(element.prefab, Get(element.group));
                        instance.name = element.name;
                        instance.transform.SetLocalPositionAndRotation(element.position + Vector3.right * offset, Quaternion.Euler(element.rotation));
                        instance.transform.localScale = element.scale;
                        PrefabUtility.RecordPrefabInstancePropertyModifications(instance);
                        PrefabUtility.RecordPrefabInstancePropertyModifications(instance.transform);
                    }
                }
                offset += section.length;
                string markerName = FormattableString.Invariant(
                    $"{(i == layout.sections.Count - 1 ? "LEVEL" : $"S{i + 1}")}_END_{offset / layout.playerSpeed:0.##}s_x{offset:0.##}");
                Child(timeline, markerName).localPosition = new Vector3(offset, 5f, 2f);
            }

            Transform checkpoint = Child(Get(Group.Checkpoints), "Checkpoint_S1_Start");
            checkpoint.localPosition = layout.playerSpawn;
            Transform player = BuildPlayer(Get(Group.Gameplay), checkpoint, layout);
            BuildCamera(Get(Group.Gameplay), player);
            BuildWeather(Get(Group.Gameplay), layout);

            Undo.RegisterCreatedObjectUndo(generated, BuildLabel);
            RemoveLegacyBlockout(sceneRoots, previousRoots.Length == 0);
            foreach (GameObject oldRoot in previousRoots)
            {
                Undo.DestroyObjectImmediate(oldRoot);
            }

            EditorSceneManager.MarkSceneDirty(scene);
            if (!EditorSceneManager.SaveScene(scene))
            {
                throw new InvalidOperationException($"Unity could not save {ScenePath}.");
            }
            Undo.CollapseUndoOperations(undoGroup);
        }
        catch
        {
            Undo.RevertAllDownToGroup(undoGroup);
            if (generated != null)
            {
                Object.DestroyImmediate(generated);
            }
            throw;
        }
    }

    private static void RemoveLegacyBlockout(GameObject[] sceneRoots, bool replaceSectionOne)
    {
        const string legacyRoot = "LEVEL_2 — 105s @ 3.2u/s — grounded feel v2 — MVP art v1";
        string[] ownedChildren =
        {
            "S1_START_0s", "S1_END_20s_x64", "S2_END_45s_x144", "S3_END_70s_x224", "LEVEL_END_105s_x336",
            "Checkpoint_S1_Start", "Ponyo_PLACEHOLDER", "Main Camera", "BACKGROUND_FAR_Parallax",
            "BACKGROUND_MIDDLE_Parallax", "TERRAIN_CoastalSteps_And_Grass", "GAMEPLAY_S1_TutorialSequence",
            "DECORATION_PLACEHOLDERS_SEPARATE"
        };
        foreach (GameObject root in sceneRoots.Where(root => root.name == legacyRoot || root.name == legacyRoot.Replace("—", "-")))
        {
            Transform section = root.transform.Find("SECTION_01_Beach_0-20s");
            if (replaceSectionOne && section != null)
            {
                foreach (string name in ownedChildren)
                {
                    Transform child = section.Find(name);
                    if (child != null)
                    {
                        Undo.DestroyObjectImmediate(child.gameObject);
                    }
                }
                RemoveEmpty(section);
            }
            foreach ((string name, string marker) in new[]
            {
                ("SECTION_02_Cliff_20-45s", "S2_END_45s_x144"),
                ("SECTION_03_CoastalRoad_45-70s", "S3_END_70s_x224"),
                ("SECTION_04_FloodedVillage_70-105s", "LEVEL_END_105s_x336")
            })
            {
                Transform emptySection = root.transform.Find($"{name}_BLOCKOUT_PENDING");
                if (emptySection != null)
                {
                    RemoveEmpty(emptySection.Find(marker));
                    RemoveEmpty(emptySection);
                }
            }
            RemoveEmpty(root.transform);
        }
    }

    private static void RemoveEmpty(Transform candidate)
    {
        if (candidate != null && candidate.childCount == 0 && candidate.GetComponents<Component>().Length == 1)
        {
            Undo.DestroyObjectImmediate(candidate.gameObject);
        }
    }

    private static Transform BuildPlayer(Transform parent, Transform checkpoint, Level2Layout layout)
    {
        Transform player = Child(parent, "Ponyo_PLACEHOLDER");
        player.localPosition = layout.playerSpawn;
        player.gameObject.layer = 2;
        Level2SectionOne.Primitive(player, "Ponyo_CoralCapsule_PLACEHOLDER", PrimitiveType.Capsule, Vector3.zero,
            new Vector3(0.75f, 0.575f, 0.68f), new Color(1f, 0.36f, 0.35f));
        Rigidbody2D body = player.gameObject.AddComponent<Rigidbody2D>();
        body.freezeRotation = true;
        body.gravityScale = 3f;
        body.interpolation = RigidbodyInterpolation2D.Interpolate;
        body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        player.gameObject.AddComponent<CapsuleCollider2D>().size = new Vector2(0.75f, 1.15f);
        Transform probe = Child(player, "GroundProbe");
        probe.localPosition = new Vector3(0f, -0.65f, 0f);

        PonyoMovement2D movement = player.gameObject.AddComponent<PonyoMovement2D>();
        var settings = new SerializedObject(movement);
        settings.FindProperty("groundProbe").objectReferenceValue = probe;
        settings.FindProperty("groundMask").intValue = ~(1 << 2);
        settings.FindProperty("runSpeed").floatValue = layout.playerSpeed;
        settings.ApplyModifiedPropertiesWithoutUndo();
        player.gameObject.AddComponent<CheckpointRespawner>().SetCheckpoint(checkpoint);
        return player;
    }

    private static void BuildCamera(Transform parent, Transform player)
    {
        Transform rig = Child(parent, "Main Camera");
        rig.localPosition = new Vector3(5.3f, 2.05f, -14.5f);
        rig.localRotation = Quaternion.Euler(6f, 0f, 0f);
        rig.gameObject.tag = "MainCamera";
        rig.gameObject.AddComponent<AudioListener>();
        Camera camera = rig.gameObject.AddComponent<Camera>();
        camera.fieldOfView = 34f;
        camera.clearFlags = CameraClearFlags.Skybox;
        camera.farClipPlane = 100f;
        var follow = new SerializedObject(rig.gameObject.AddComponent<CameraFollow2D>());
        follow.FindProperty("target").objectReferenceValue = player;
        follow.FindProperty("offset").vector2Value = new Vector2(4.65f, 1.95f);
        follow.FindProperty("minimumX").floatValue = 5.3f;
        follow.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void BuildWeather(Transform parent, Level2Layout layout)
    {
        Transform weather = Child(parent, "Weather");
        Transform sun = Child(weather, "Directional Light");
        sun.localRotation = Quaternion.Euler(44f, -32f, 0f);
        Light light = sun.gameObject.AddComponent<Light>();
        light.type = LightType.Directional;
        light.shadows = LightShadows.Soft;
        weather.gameObject.AddComponent<WeatherDirector>().Configure(layout.skybox, light);
    }

    private static Transform Child(Transform parent, string name)
    {
        Transform transform = new GameObject(name).transform;
        transform.SetParent(parent, false);
        return transform;
    }
}
