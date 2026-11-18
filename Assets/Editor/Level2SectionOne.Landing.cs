using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using Group = Level2Layout.Group;

internal static partial class Level2SectionOne
{
    internal static void AddStairLanding(Level2Layout.Section beach)
    {
        const string name = "CoastalStairLanding";
        if (beach.elements.Exists(element => element.name == name))
        {
            return;
        }
        GameObject landing = Prefab(name, root =>
        {
            var renderer = root.AddComponent<MeshRenderer>();
            renderer.sharedMaterial = Level2CoastalFronts.LoadMaterial("CoastalMossyStone");
            renderer.shadowCastingMode = ShadowCastingMode.Off;
            root.AddComponent<MeshFilter>().sharedMesh = CreateLandingMesh();
            Model(root.transform, Forest, "stones", new Vector3(0.55f, 0.075f, 0.46f),
                new Vector3(1.64f, 0f, 0.11f), -26f);
            Model(root.transform, Forest, "stones", new Vector3(0.36f, 0.053f, 0.34f),
                new Vector3(1.30f, 0f, -0.68f), 43f);
            Model(root.transform, Survival, "grass-large", new Vector3(0.46f, 0.18f, 0.42f),
                new Vector3(0.94f, 0f, -1.03f), 24f);
            Model(root.transform, Survival, "grass-large", new Vector3(0.67f, 0.23f, 0.53f),
                new Vector3(1.00f, 0f, 1.63f), -37f);
        });
        Add(beach, Group.Decor_Proche, name, landing, 8.8f, 1.5f);
    }

    private static Mesh CreateLandingMesh()
    {
        const string path = "Assets/Art/Level2/Terrain/SurfaceTextures/CoastalStairLanding.asset";
        Mesh mesh = AssetDatabase.LoadAssetAtPath<Mesh>(path);
        if (mesh != null)
        {
            return mesh;
        }

        const float join = -0.05f;
        const float top = 0.009f;
        const float front = -1.420f;
        // The landing trails out into the grass; the visible wall ends in staggered stones.
        Vector2[] paving =
        {
            new(front, 0.72f), new(-1.08f, 0.84f), new(-0.85f, 1.14f), new(-0.47f, 1.32f),
            new(-0.09f, 1.59f), new(0.43f, 1.56f), new(0.67f, 1.38f),
            new(1.12f, 1.49f), new(1.52f, 1.10f), new(1.86f, 0.54f), new(2.12f, 0.13f)
        };
        Vector2[] exposedEdge =
        {
            new(-2.89f, 0.05f), new(-1.77f, 0.06f), new(-1.52f, 0.35f),
            new(-1.42f, 0.33f), new(-1.29f, 0.45f), new(-1.05f, 0.49f),
            new(-0.98f, 0.63f), new(-0.71f, 0.59f), new(-0.65f, 0.73f),
            new(-0.40f, 0.81f), new(-0.35f, 0.73f), new(-0.11f, 0.70f), new(top, 0.72f)
        };
        var vertices = new List<Vector3>();
        var uv = new List<Vector2>();
        var triangles = new List<int>();

        for (int i = 1; i < paving.Length; i++)
        {
            Vector2 a = paving[i - 1];
            Vector2 b = paving[i];
            Quad(new Vector3(join, top, a.x), new Vector3(join, top, b.x),
                new Vector3(b.y, top, b.x), new Vector3(a.y, top, a.x), true);
        }
        for (int i = 1; i < exposedEdge.Length; i++)
        {
            Vector2 a = exposedEdge[i - 1];
            Vector2 b = exposedEdge[i];
            Quad(new Vector3(join, a.x, front), new Vector3(join, b.x, front),
                new Vector3(b.y, b.x, front), new Vector3(a.y, a.x, front), false);
        }

        mesh = new Mesh { name = "CoastalStairLanding" };
        mesh.SetVertices(vertices);
        mesh.SetUVs(0, uv);
        mesh.SetTriangles(triangles, 0);
        mesh.RecalculateNormals();
        AssetDatabase.CreateAsset(mesh, path);
        return mesh;

        void Quad(Vector3 a, Vector3 b, Vector3 c, Vector3 d, bool horizontal)
        {
            int first = vertices.Count;
            foreach (Vector3 point in new[] { a, b, c, d })
            {
                vertices.Add(point);
                // Match the stair's planar UVs at the seam.
                uv.Add(new Vector2(point.x + 4.4f, horizontal ? point.z : point.y + 2.4f) / 2.6f);
            }
            triangles.AddRange(new[] { first, first + 1, first + 2, first, first + 2, first + 3 });
        }
    }
}
