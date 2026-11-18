using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

internal static class Level2CoastalWater
{
    private const string AssetsPath = "Assets/Art/Level2/Terrain/SurfaceTextures";
    // The three hexagon-kit models share this water swatch in their color atlas.
    private static readonly Vector2 WaterSwatch = new(0.15625f, 0.125f);

    internal static void AddSurface(Transform root)
    {
        var vertices = new List<Vector3>();
        var indices = new List<int>();
        foreach (MeshFilter source in root.GetComponentsInChildren<MeshFilter>())
        {
            if (source.name == "TexturedWaterSurface")
            {
                continue;
            }
            MeshFilter original = PrefabUtility.GetCorrespondingObjectFromOriginalSource(source);
            Mesh mesh = original != null ? original.sharedMesh : source.sharedMesh;
            Vector3[] positions = mesh.vertices;
            Vector2[] uv = mesh.uv;
            int[] triangles = mesh.triangles;
            Matrix4x4 toRoot = root.worldToLocalMatrix * source.transform.localToWorldMatrix;
            for (int i = 0; i < triangles.Length; i += 3)
            {
                int a = triangles[i], b = triangles[i + 1], c = triangles[i + 2];
                if (!IsWater(uv[a]) || !IsWater(uv[b]) || !IsWater(uv[c]))
                {
                    continue;
                }
                Vector3 first = toRoot.MultiplyPoint3x4(positions[a]);
                Vector3 second = toRoot.MultiplyPoint3x4(positions[b]);
                Vector3 third = toRoot.MultiplyPoint3x4(positions[c]);
                Vector3 normal = Vector3.Cross(second - first, third - first).normalized;
                if (normal.y < 0.9f)
                {
                    continue;
                }
                Vector3 offset = normal * 0.005f;
                Subdivide(first + offset, second + offset, third + offset, 4);
            }
            RemoveFlatWater(source, mesh, root.name);
        }
        if (vertices.Count == 0)
        {
            throw new InvalidOperationException($"No water surface found in {root.name}.");
        }

        string name = $"{root.name}Surface";
        string path = $"{AssetsPath}/{name}.asset";
        Mesh surfaceMesh = AssetDatabase.LoadAssetAtPath<Mesh>(path);
        bool create = surfaceMesh == null;
        if (create)
        {
            surfaceMesh = new Mesh { name = name };
        }
        surfaceMesh.Clear();
        surfaceMesh.indexFormat = vertices.Count > 65535 ? IndexFormat.UInt32 : IndexFormat.UInt16;
        surfaceMesh.SetVertices(vertices);
        surfaceMesh.SetTriangles(indices, 0);
        Bounds animatedBounds = surfaceMesh.bounds;
        animatedBounds.Expand(new Vector3(0f, 1.20f, 0f));
        surfaceMesh.bounds = animatedBounds;
        if (create)
        {
            AssetDatabase.CreateAsset(surfaceMesh, path);
        }
        else
        {
            EditorUtility.SetDirty(surfaceMesh);
        }

        Transform existing = root.Find("TexturedWaterSurface");
        var surface = existing != null ? existing.gameObject : new GameObject("TexturedWaterSurface");
        surface.transform.SetParent(root, false);
        (surface.GetComponent<MeshFilter>() ?? surface.AddComponent<MeshFilter>()).sharedMesh = surfaceMesh;
        var renderer = surface.GetComponent<MeshRenderer>() ?? surface.AddComponent<MeshRenderer>();
        renderer.sharedMaterial = GetMaterial();
        renderer.shadowCastingMode = ShadowCastingMode.Off;

        void Subdivide(Vector3 a, Vector3 b, Vector3 c, int depth)
        {
            if (depth == 0)
            {
                indices.Add(vertices.Count);
                indices.Add(vertices.Count + 1);
                indices.Add(vertices.Count + 2);
                vertices.Add(a);
                vertices.Add(b);
                vertices.Add(c);
                return;
            }
            Vector3 ab = (a + b) * 0.5f;
            Vector3 bc = (b + c) * 0.5f;
            Vector3 ca = (c + a) * 0.5f;
            Subdivide(a, ab, ca, depth - 1);
            Subdivide(ab, b, bc, depth - 1);
            Subdivide(ca, bc, c, depth - 1);
            Subdivide(ab, bc, ca, depth - 1);
        }
    }

    private static void RemoveFlatWater(MeshFilter filter, Mesh original, string prefabName)
    {
        Vector2[] uv = original.uv;
        bool hasWater = Array.Exists(uv, IsWater);
        if (!hasWater)
        {
            return;
        }
        string path = $"{AssetsPath}/{prefabName}_{original.name}_Hull.asset";
        Mesh hull = AssetDatabase.LoadAssetAtPath<Mesh>(path);
        if (hull == null)
        {
            hull = UnityEngine.Object.Instantiate(original);
            hull.name = $"{prefabName}_Hull";
            for (int submesh = 0; submesh < original.subMeshCount; submesh++)
            {
                int[] source = original.GetTriangles(submesh);
                var kept = new List<int>();
                for (int i = 0; i < source.Length; i += 3)
                {
                    if (!IsWater(uv[source[i]]) || !IsWater(uv[source[i + 1]]) || !IsWater(uv[source[i + 2]]))
                    {
                        kept.Add(source[i]);
                        kept.Add(source[i + 1]);
                        kept.Add(source[i + 2]);
                    }
                }
                hull.SetTriangles(kept, submesh);
            }
            AssetDatabase.CreateAsset(hull, path);
        }
        filter.sharedMesh = hull;
        PrefabUtility.RecordPrefabInstancePropertyModifications(filter);
    }

    [MenuItem("Tools/Ponyo/Apply animated model water")]
    internal static void ApplyAnimatedModelWater()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            throw new InvalidOperationException("Exit Play Mode before updating the water.");
        }
        ConfigureMotion();
        ConfigureShoreline(Level2SectionOne.LoadOrCreate());
        foreach (string name in new[] { "SeaWater", "SeaWaterRocks" })
        {
            using var contents = new PrefabUtility.EditPrefabContentsScope($"Assets/Prefabs/Level2/Section1/{name}.prefab");
            AddSurface(contents.prefabContentsRoot.transform);
        }
        Level2ShoreSplashes.Apply();
        AssetDatabase.SaveAssets();
        Debug.Log("MODEL_WATER_OK referenceAnimation=16.6667s frames=128");
    }

    private static void ConfigureMotion()
    {
        const string path = "Assets/Art/Level2/Terrain/WaterWaves/WaterWavesMotion.asset";
        Texture2DArray motion = AssetDatabase.LoadAssetAtPath<Texture2DArray>(path);
        if (motion == null)
        {
            using var stream = File.OpenRead("Assets/Art/Level2/Terrain/WaterWaves/WaterWavesMotion.bytes");
            using var reader = new BinaryReader(stream);
            if (reader.ReadUInt32() != 0x56415750)
            {
                throw new InvalidDataException("Invalid wave animation data.");
            }
            int width = reader.ReadInt32();
            int height = reader.ReadInt32();
            int frames = reader.ReadInt32();
            reader.ReadSingle();
            motion = new Texture2DArray(width, height, frames, TextureFormat.RHalf, false, true)
            {
                name = "WaterWavesMotion", wrapMode = TextureWrapMode.Repeat, filterMode = FilterMode.Bilinear
            };
            for (int frame = 0; frame < frames; frame++)
            {
                motion.SetPixelData(reader.ReadBytes(width * height * 2), 0, frame);
            }
            motion.Apply(false, true);
            AssetDatabase.CreateAsset(motion, path);
        }
        Material material = GetMaterial();
        material.SetTexture("_WaveMotion", motion);
        material.SetFloat("_WaveHeight", 0.32f);
        material.SetFloat("_WaveTileSize", 18f);
        Material sky = Level2SectionOne.LoadOrCreate().skybox;
        material.SetTexture("_ReflectionMap", sky.GetTexture("_MainTex"));
        material.SetFloat("_ReflectionRotation", sky.GetFloat("_Rotation"));
        EditorUtility.SetDirty(material);
    }

    private static bool IsWater(Vector2 uv) => (uv - WaterSwatch).sqrMagnitude < 0.000001f;

    [MenuItem("Tools/Ponyo/Fix submerged stair foam")]
    private static void FixSubmergedStairFoam()
    {
        ConfigureShoreline(Level2SectionOne.LoadOrCreate());
        AssetDatabase.SaveAssets();
    }

    internal static void ConfigureShoreline(Level2Layout layout)
    {
        Level2Layout.Element stairs = layout.sections[0].elements.Find(item => item.name == "CoastalStairs");
        Matrix4x4 placement = Matrix4x4.TRS(stairs.position + Vector3.up * layout.coastalElevationOffset,
            Quaternion.Euler(stairs.rotation), stairs.scale);
        float firstDryX = float.PositiveInfinity;
        foreach (MeshFilter filter in stairs.prefab.GetComponentsInChildren<MeshFilter>())
        {
            Matrix4x4 toWorld = placement * stairs.prefab.transform.worldToLocalMatrix * filter.transform.localToWorldMatrix;
            foreach (Vector3 vertex in filter.sharedMesh.vertices)
            {
                Vector3 position = toWorld.MultiplyPoint3x4(vertex);
                if (position.y > Level2SectionOne.SeaSurfaceY + 0.01f)
                {
                    firstDryX = Mathf.Min(firstDryX, position.x);
                }
            }
        }
        if (float.IsPositiveInfinity(firstDryX))
        {
            throw new InvalidOperationException("The stairs have no emerged surface for the shoreline.");
        }
        float end = layout.sections[0].length;
        Material material = GetMaterial();
        material.SetVector("_ShoreBounds", new Vector4((firstDryX + end) * 0.5f, 0.65f,
            (end - firstDryX) * 0.5f, 2.05f));
        EditorUtility.SetDirty(material);
        Debug.Log($"STAIR_FOAM_OK firstEmergedX={firstDryX}");
    }

    private static Material GetMaterial()
    {
        string path = $"{AssetsPath}/CoastalWater.mat";
        Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (material != null)
        {
            return material;
        }
        material = new Material(Shader.Find("Ponyo/Level2/CoastalWater"));
        material.SetTexture("_WaveMotion", AssetDatabase.LoadAssetAtPath<Texture2DArray>(
            "Assets/Art/Level2/Terrain/WaterWaves/WaterWavesMotion.asset"));
        material.SetTexture("_ReflectionMap", AssetDatabase.LoadAssetAtPath<Texture2D>(
            "Assets/Art/Level2/Generated/kenney_skyboxes/skybox-day.png"));
        material.SetFloat("_ReflectionRotation", 115f);
        AssetDatabase.CreateAsset(material, path);
        return material;
    }
}
