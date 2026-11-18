using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Group = Level2Layout.Group;

internal static partial class Level2SectionOne
{
    internal static bool IsCoastalLand(Level2Layout.Element element) =>
        element.group == Group.Gameplay || element.group == Group.Hazards
        || element.group == Group.Platforms || element.group == Group.Checkpoints
        || element.group == Group.EndTrigger
        || element.group == Group.Decor_Proche && !element.name.StartsWith("ShoreWater_", StringComparison.Ordinal);

    internal static float CameraFieldOfView(float zoom) =>
        2f * Mathf.Atan(Mathf.Tan(17f * Mathf.Deg2Rad) / zoom) * Mathf.Rad2Deg;

    [MenuItem("Tools/Ponyo/Apply coastal framing")]
    internal static void ApplyCoastalFraming()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            throw new InvalidOperationException("Exit Play Mode before adjusting the coastal framing.");
        }
        Level2Layout layout = LoadOrCreate();
        Undo.RecordObject(layout, "Adjust coastal framing");
        layout.cameraZoom = 1.2f;
        // Previously exposed height was 1.5 - (-0.24) = 1.74 units.
        layout.coastalElevationOffset = -(1.5f - SeaSurfaceY) * 0.20f;
        Scene scene = SceneManager.GetSceneByPath(Level2Builder.ScenePath);
        if (!scene.IsValid() || !scene.isLoaded)
        {
            scene = EditorSceneManager.OpenScene(Level2Builder.ScenePath, OpenSceneMode.Additive);
        }
        Transform generated = null;
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            if (root.name == "Generated_Level2")
            {
                generated = root.transform;
            }
        }
        if (generated == null)
        {
            throw new InvalidOperationException("Build the Level 2 scene before adjusting the framing.");
        }
        foreach (Level2Layout.Element element in layout.sections[0].elements)
        {
            if (!IsCoastalLand(element))
            {
                continue;
            }
            Transform instance = generated.Find($"{element.group}/{element.name}");
            if (instance == null)
            {
                throw new InvalidOperationException($"Missing coastal element {element.name}");
            }
            Undo.RecordObject(instance, "Lower coastal terrain");
            instance.localPosition = element.position + Vector3.up * layout.coastalElevationOffset;
            PrefabUtility.RecordPrefabInstancePropertyModifications(instance);
        }
        Vector3 spawn = layout.playerSpawn + Vector3.up * layout.coastalElevationOffset;
        Transform player = generated.Find("Gameplay/Ponyo_PLACEHOLDER");
        Transform checkpoint = generated.Find("Checkpoints/Checkpoint_S1_Start");
        Undo.RecordObjects(new UnityEngine.Object[] { player, checkpoint }, "Lower coastal spawn");
        player.localPosition = spawn;
        checkpoint.localPosition = spawn;
        Camera camera = generated.Find("Gameplay/Main Camera").GetComponent<Camera>();
        Undo.RecordObjects(new UnityEngine.Object[] { camera, camera.transform }, "Adjust coastal camera");
        camera.fieldOfView = CameraFieldOfView(layout.cameraZoom);
        camera.transform.localPosition = new Vector3(5.3f, 2.05f + layout.coastalElevationOffset, -14.5f);
        Level2CoastalWater.ConfigureShoreline(layout);
        EditorUtility.SetDirty(layout);
        AssetDatabase.SaveAssets();
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log($"COASTAL_FRAMING_OK zoom={layout.cameraZoom} elevation={layout.coastalElevationOffset}");
    }
}
