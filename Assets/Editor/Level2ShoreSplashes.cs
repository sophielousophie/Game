using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

internal static class Level2ShoreSplashes
{
    internal static void AddTo(Level2Layout.Section section)
    {
        const string path = "Assets/Prefabs/Level2/Section1/CoastalShoreSplash.prefab";
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (prefab == null)
        {
            GameObject root = new("CoastalShoreSplash");
            try
            {
                Configure(root);
                prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
            }
            finally
            {
                Object.DestroyImmediate(root);
            }
        }
        for (int i = 0; i < 16; i++)
        {
            string name = $"ShoreSplash_{i:00}";
            if (section.elements.Exists(element => element.name == name))
            {
                continue;
            }
            section.elements.Add(new Level2Layout.Element
            {
                name = name,
                group = Level2Layout.Group.Decor_Intermediaire,
                prefab = prefab,
                position = new Vector3(3.9f + i * 5.1f, Level2SectionOne.SeaSurfaceY + 0.015f, -1.49f)
            });
        }
    }

    internal static void Apply()
    {
        Level2Layout layout = Level2SectionOne.LoadOrCreate();
        Undo.RecordObject(layout, "Add shore splashes");
        AddTo(layout.sections[0]);
        EditorUtility.SetDirty(layout);
        AssetDatabase.SaveAssets();
        Level2Builder.Build();
    }

    private static void Configure(GameObject root)
    {
        const string materialPath = "Assets/Art/Level2/Terrain/SurfaceTextures/CoastalSplash.mat";
        Material material = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
        if (material == null)
        {
            material = new Material(Shader.Find("Ponyo/Level2/CoastalSplash")) { name = "CoastalSplash" };
            AssetDatabase.CreateAsset(material, materialPath);
        }
        ParticleSystem spray = root.AddComponent<ParticleSystem>();
        spray.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        var main = spray.main;
        main.duration = 4.1f;
        main.loop = true;
        main.startDelay = new ParticleSystem.MinMaxCurve(0.1f, 3.9f);
        main.startLifetime = new ParticleSystem.MinMaxCurve(0.35f, 0.70f);
        main.startSpeed = 0f;
        main.startSize = new ParticleSystem.MinMaxCurve(0.025f, 0.055f);
        main.startColor = new Color(0.82f, 0.94f, 0.95f, 0.80f);
        main.gravityModifier = 0.12f;
        main.maxParticles = 12;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.cullingMode = ParticleSystemCullingMode.Pause;
        var emission = spray.emission;
        emission.rateOverTime = 0f;
        emission.SetBursts(new[] { new ParticleSystem.Burst(0f, (short)2, (short)4) { probability = 0.85f } });
        var shape = spray.shape;
        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale = new Vector3(0.42f, 0.012f, 0.06f);
        var velocity = spray.velocityOverLifetime;
        velocity.enabled = true;
        velocity.space = ParticleSystemSimulationSpace.World;
        velocity.x = new ParticleSystem.MinMaxCurve(-0.18f, 0.18f);
        velocity.y = new ParticleSystem.MinMaxCurve(0.55f, 0.95f);
        velocity.z = new ParticleSystem.MinMaxCurve(-0.24f, -0.08f);
        var fade = spray.colorOverLifetime;
        fade.enabled = true;
        var gradient = new Gradient();
        gradient.SetKeys(
            new[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(Color.white, 1f) },
            new[] { new GradientAlphaKey(0.25f, 0f), new GradientAlphaKey(1f, 0.13f), new GradientAlphaKey(0f, 1f) });
        fade.color = gradient;
        var renderer = spray.GetComponent<ParticleSystemRenderer>();
        renderer.sharedMaterial = material;
        renderer.renderMode = ParticleSystemRenderMode.Billboard;
        renderer.shadowCastingMode = ShadowCastingMode.Off;
        renderer.receiveShadows = false;
    }
}
