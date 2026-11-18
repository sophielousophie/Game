using System.IO;
using UnityEditor;
using UnityEngine;

internal static class Level2CoastalVisualUpdate
{
    [MenuItem("Tools/Ponyo/Apply island soil and clean stair vegetation")]
    internal static void Apply()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            throw new System.InvalidOperationException("Exit Play Mode before updating the coastal visuals.");
        }
        foreach (string name in new[] { "Island_Headland", "Island_WoodedCove", "Island_RockyShoal" })
        {
            Level2IslandSoil.ApplyTo(AssetDatabase.LoadAssetAtPath<GameObject>($"Assets/Prefabs/Level2/Section1/{name}.prefab"));
        }
        Level2Layout layout = Level2SectionOne.LoadOrCreate();
        Undo.RecordObject(layout, "Remove lower stair vegetation");
        int removed = layout.sections[0].elements.RemoveAll(item =>
            item.name.StartsWith("CoastalStairHerbs_", System.StringComparison.Ordinal)
            || item.name.StartsWith("WildGrass_Stair_", System.StringComparison.Ordinal)
                && !item.name.StartsWith("WildGrass_Stair_7_", System.StringComparison.Ordinal)
            || item.name == "Steps_Grass" || item.name == "Steps_Bush");
        ApplyStairTexture();
        EditorUtility.SetDirty(layout);
        AssetDatabase.SaveAssets();
        Level2Builder.Build();
        Debug.Log($"COASTAL_VISUALS_OK lowerStairPlantsRemoved={removed}");
    }

    [MenuItem("Tools/Ponyo/Apply stair stone texture")]
    internal static void ApplyStairTexture()
    {
        string source = File.Exists("3d_assets/stair_stone.png") ? "3d_assets/stair_stone.png"
            : "3d_assets/6d7fa9b3844ebb1431a3471f2db9474f.jpg";
        string target = $"Assets/Art/Level2/Terrain/SurfaceTextures/CoastalStairStone{Path.GetExtension(source)}";
        if (!File.Exists(source))
        {
            Debug.Log("STAIR_TEXTURE_PENDING expected 3d_assets/stair_stone.png");
            return;
        }
        File.Copy(source, target, true);
        AssetDatabase.ImportAsset(target, ImportAssetOptions.ForceSynchronousImport);
        var importer = (TextureImporter)AssetImporter.GetAtPath(target);
        importer.wrapMode = TextureWrapMode.Repeat;
        importer.filterMode = FilterMode.Trilinear;
        importer.mipmapEnabled = true;
        importer.SaveAndReimport();
        Material material = Level2CoastalFronts.LoadMaterial("CoastalMossyStone");
        material.mainTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(target);
        EditorUtility.SetDirty(material);
        AssetDatabase.SaveAssets();
        Debug.Log("STAIR_TEXTURE_OK new stone texture applied to treads, risers and landing");
    }
}
