using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

internal static partial class Level2SectionOne
{
    private const string GrassArt = "Assets/Art/Level2/Terrain/SurfaceTextures";

    private static GameObject CoastalGrassPrefab(string name, int bladeCount, Vector3 size)
    {
        return Prefab(name, root =>
        {
            Material material = AssetDatabase.LoadAssetAtPath<Material>($"{GrassArt}/CoastalGrass.mat");
            if (material == null)
            {
                Shader shader = AssetDatabase.LoadAssetAtPath<Shader>($"{GrassArt}/CoastalGrass.shader");
                material = new Material(shader) { name = "CoastalGrass" };
                AssetDatabase.CreateAsset(material, $"{GrassArt}/CoastalGrass.mat");
            }

            GameObject visual = new("CurvedGrass");
            visual.transform.SetParent(root.transform, false);
            visual.AddComponent<MeshFilter>().sharedMesh = LoadGrassMesh(name, bladeCount, size);
            MeshRenderer renderer = visual.AddComponent<MeshRenderer>();
            renderer.sharedMaterial = material;
            renderer.shadowCastingMode = ShadowCastingMode.Off;
            renderer.receiveShadows = false;
        });
    }

    private static Mesh LoadGrassMesh(string name, int bladeCount, Vector3 size)
    {
        string path = $"{GrassArt}/{name}.asset";
        Mesh existing = AssetDatabase.LoadAssetAtPath<Mesh>(path);
        if (existing != null)
        {
            return existing;
        }

        const int segments = 5;
        var random = new System.Random(717 + bladeCount);
        var vertices = new List<Vector3>();
        var colors = new List<Color>();
        var uv = new List<Vector2>();
        var triangles = new List<int>();
        var youngGreen = new Color(0.56f, 0.82f, 0.23f);
        var matureGreen = new Color(0.31f, 0.60f, 0.19f);
        var rootGreen = new Color(0.25f, 0.42f, 0.13f);

        // Each patch has several crowns. The shorter outer blades curl over the surrounding ground.
        for (int blade = 0; blade < bladeCount; blade++)
        {
            int crown = blade % 5;
            float crownAngle = crown * (2f * Mathf.PI / 5f);
            float crownRadius = crown == 0 ? 0f : 0.25f;
            Vector3 origin = new(
                size.x * (Mathf.Cos(crownAngle) * crownRadius + Range(-0.065f, 0.065f)),
                -0.012f,
                size.z * (Mathf.Sin(crownAngle) * crownRadius + Range(-0.065f, 0.065f)));
            float angle = Range(0f, 2f * Mathf.PI);
            Vector3 forward = new(Mathf.Cos(angle), 0f, Mathf.Sin(angle));
            Vector3 side = new(-forward.z, 0f, forward.x);
            float height = size.y * Range(0.52f, 1.28f);
            float bend = Mathf.Lerp(size.z, size.x, Mathf.Abs(forward.x)) * Range(0.12f, 0.29f);
            float width = size.y * Range(0.09f, 0.19f);
            Color tipColor = Color.Lerp(matureGreen, youngGreen, Range(0f, 1f));

            for (int segment = 0; segment < segments; segment++)
            {
                float t = (float)segment / segments;
                Vector3 center = Center(t);
                float halfWidth = width * (0.45f + 1.55f * t) * Mathf.Pow(1f - t, 0.87f) * 0.5f;
                int left = vertices.Count;
                AddVertex(center - side * halfWidth, t, 0.86f);
                AddVertex(center + side * halfWidth, t, 1f);
                if (segment > 0)
                {
                    triangles.AddRange(new[] { left - 2, left, left - 1, left - 1, left, left + 1 });
                }
            }

            int tip = vertices.Count;
            AddVertex(Center(1f), 1f, 1f);
            triangles.AddRange(new[] { tip - 2, tip, tip - 1 });

            Vector3 Center(float t) =>
                origin + Vector3.up * (height * t * (1.37f - 0.49f * t)) + forward * (bend * t * t);

            void AddVertex(Vector3 position, float t, float shade)
            {
                Color color = Color.Lerp(rootGreen, tipColor, Mathf.Pow(t, 0.66f));
                vertices.Add(position);
                colors.Add(new Color(color.r * shade, color.g * shade, color.b * shade));
                uv.Add(new Vector2(0f, t));
            }
        }

        var mesh = new Mesh { name = name };
        mesh.SetVertices(vertices);
        mesh.SetColors(colors);
        mesh.SetUVs(0, uv);
        mesh.SetTriangles(triangles, 0);
        Bounds bounds = mesh.bounds;
        bounds.Expand(new Vector3(0.32f, 0f, 0.32f));
        mesh.bounds = bounds;
        AssetDatabase.CreateAsset(mesh, path);
        return mesh;

        float Range(float min, float max) => Mathf.Lerp(min, max, (float)random.NextDouble());
    }
}
