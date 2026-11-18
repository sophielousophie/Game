using UnityEngine;
using Group = Level2Layout.Group;

internal static partial class Level2SectionOne
{
    private static void CreateCoastalDressing(Level2Layout.Section beach)
    {
        AddMainlandDressing(beach);
        AddWildGrass(beach);
        AddIslandDressing(beach);
    }

    private static void AddMainlandDressing(Level2Layout.Section beach)
    {
        const Group group = Group.Decor_Proche;
        GameObject fence = ModelPrefab("CoastalWoodFence", Survival, "fence", new Vector3(1.85f, 0.72f, 0.16f), 0f);
        GameObject tallTree = ModelPrefab("CoastalTallTree", Forest, "tree-high", new Vector3(1.58f, 2.85f, 1.30f), 22f);
        GameObject shrub = ModelPrefab("Shrub", Suburban, "tree-small", new Vector3(0.82f, 1.24f, 0.75f), 20f);
        GameObject bush = ModelPrefab("Bush", Forest, "plant", new Vector3(1.30f, 0.65f, 0.85f), 18f);
        GameObject meadow = CoastalGrassPrefab("CoastalMeadow", 36, new Vector3(0.87f, 0.34f, 0.78f));

        (float start, int count, float depth)[] runs =
        {
            (11.5f, 3, 2.34f), (21.2f, 2, 2.18f), (30.6f, 3, 2.30f),
            (44.5f, 4, 2.32f), (56.8f, 2, 2.20f)
        };
        for (int run = 0; run < runs.Length; run++)
        {
            var (start, count, depth) = runs[run];
            for (int part = 0; part < count; part++)
            {
                Add(beach, group, $"CoastalFence_{run + 1:00}_{part + 1:00}", fence, start + 1.78f * part, 1.5f, depth);
            }
        }

        float[] groves = { 10.1f, 16.1f, 23.8f, 29f, 37.4f, 46.5f, 52f, 60.3f };
        for (int i = 0; i < groves.Length; i++)
        {
            float x = groves[i];
            float direction = i % 2 == 0 ? 1f : -1f;
            Add(beach, group, $"CoastalGrove_Tree_{i:00}", tallTree, x, 1.5f, 2.04f,
                Vector3.one * (0.62f + i % 3 * 0.15f), 37f * i);
            Add(beach, group, $"CoastalGrove_Shrub_{i:00}", shrub, x + direction * 0.92f, 1.5f, 1.94f,
                Vector3.one * (0.72f + i % 3 * 0.14f), 61f * i);
            Add(beach, group, $"CoastalGrove_Bush_{i:00}", bush, x - direction * 0.61f, 1.5f, 1.61f, Vector3.one * 0.66f, 47f * i);
        }

        var random = new System.Random(1732);
        for (int i = 0; i < 38; i++)
        {
            float x = 9.4f + i * 1.40f + (float)random.NextDouble() * 0.48f;
            float z = i % 3 == 0 ? -0.93f : 1.1f + (float)random.NextDouble() * 0.75f;
            Add(beach, group, $"CoastalMeadow_{i:00}", meadow, x, 1.5f, z,
                Vector3.one * (0.38f + (float)random.NextDouble() * 0.37f), random.Next(360));
        }

    }

    private static void AddIslandDressing(Level2Layout.Section beach)
    {
        GameObject cliff = Prefab("Island_Headland", root =>
        {
            Model(root.transform, Hex, "stone-hill", new Vector3(8.5f, 4.4f, 6.3f), new Vector3(-1.6f, 0f, 0.35f), -9f);
            Model(root.transform, Hex, "grass-hill", new Vector3(7.3f, 4.2f, 5.8f), new Vector3(-0.9f, 0.32f, 0.65f), 21f);
            Model(root.transform, Hex, "grass-forest", new Vector3(6.9f, 2.65f, 5.5f), new Vector3(3.3f, 0.04f, 0.3f), 39f);
            Model(root.transform, Forest, "rocks-high", new Vector3(3.2f, 1.55f, 2.6f), new Vector3(-4.3f, -0.04f, -1.7f), 13f);
        });
        GameObject wooded = Prefab("Island_WoodedCove", root =>
        {
            Model(root.transform, Hex, "grass-forest", new Vector3(8.8f, 3.5f, 6.5f), new Vector3(-2.25f, 0f, 0.25f), 19f);
            Model(root.transform, Hex, "grass-forest", new Vector3(6.4f, 2.35f, 5.6f), new Vector3(3f, 0.03f, -0.15f), -29f);
            Model(root.transform, Forest, "rocks-low", new Vector3(4.4f, 0.77f, 2.65f), new Vector3(4.55f, -0.09f, -1.25f), 41f);
        });
        GameObject shoal = Prefab("Island_RockyShoal", root =>
        {
            Model(root.transform, Forest, "rocks-low", new Vector3(6.6f, 1.35f, 3.8f), new Vector3(-1.55f, 0f, 0f), 19f);
            Model(root.transform, Hex, "grass-forest", new Vector3(4.35f, 1.53f, 3.45f), new Vector3(0.8f, 0.04f, 0.3f), -19f);
            Model(root.transform, Forest, "rocks-low", new Vector3(2.2f, 0.59f, 1.6f), new Vector3(3.7f, -0.16f, -0.55f), -37f);
        });
        VegetateIsland(cliff, 0);
        VegetateIsland(wooded, 1);
        VegetateIsland(shoal, 2);
        Level2IslandSoil.ApplyTo(cliff);
        Level2IslandSoil.ApplyTo(wooded);
        Level2IslandSoil.ApplyTo(shoal);

        void Place(string name, GameObject prefab, Group group, float x, float z, Vector3 scale, float yaw) =>
            Add(beach, group, $"CoastIsland_{name}", prefab, x, IslandBaseY, z, scale, yaw);

        const Group mid = Group.Decor_Intermediaire;
        const Group far = Group.Decor_Lointain;
        Place("WestHeadland", cliff, mid, -9f, 17f, new Vector3(1.03f, 0.90f, 0.87f), -9f);
        Place("WestShoal", shoal, mid, 6f, 19.5f, new Vector3(0.74f, 0.64f, 0.9f), -18f);
        Place("SouthCove", wooded, mid, 25f, 16.5f, new Vector3(1.05f, 0.82f, 1.0f), 14f);
        Place("EastShoal", shoal, mid, 44f, 14.5f, new Vector3(1.02f, 0.62f, 0.86f), 34f);
        Place("EastHeadland", cliff, mid, 67f, 18f, new Vector3(0.95f, 1.05f, 0.95f), 20f);
        Place("FarCove", wooded, far, -23f, 30f, new Vector3(1.65f, 0.95f, 1.15f), -4f);
        Place("FarHeadland", cliff, far, 14f, 29f, new Vector3(1.18f, 1.03f, 1.02f), -15f);
        Place("FarWoodedRidge", wooded, far, 49f, 32f, new Vector3(1.38f, 0.95f, 1.2f), -28f);
        Place("FarEastShoal", CreateHouseIsland(shoal), far, 85f, 28f, new Vector3(1.63f, 1.19f, 1.16f), 20f);
    }
}
