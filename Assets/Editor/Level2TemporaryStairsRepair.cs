using UnityEditor;
using UnityEngine;

internal static class Level2TemporaryStairsRepair
{
    [MenuItem("Tools/Ponyo/Update Generated Stairs")]
    private static void UpdateStairs()
    {
        foreach (string name in new[] { "CoastalStairs", "CoastalGround" })
        {
            string path = $"Assets/Prefabs/Level2/Section1/{name}.prefab";
            GameObject root = PrefabUtility.LoadPrefabContents(path);
            try
            {
                foreach (Collider2D collider in root.GetComponents<Collider2D>())
                {
                    Object.DestroyImmediate(collider);
                }
                PrefabUtility.SaveAsPrefabAsset(root, path);
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(root);
            }
        }
        Level2Layout layout = AssetDatabase.LoadAssetAtPath<Level2Layout>(Level2SectionOne.LayoutPath);
        if (!layout.sections[0].elements.Exists(item => item.name == "CoastalWalkableSurface"))
        {
            layout.sections[0].elements.Add(new Level2Layout.Element
            {
                name = "CoastalWalkableSurface",
                group = Level2Layout.Group.Gameplay,
                prefab = Level2SectionOne.CreateWalkableSurface()
            });
            EditorUtility.SetDirty(layout);
            AssetDatabase.SaveAssets();
        }
        Level2Builder.Build();
    }
}
