using System.IO;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class Level2BlockoutBuilder
{
    private const string ScenePath = Level2Builder.ScenePath;

    public static void BuildScene1Blockout() => Level2Builder.Build();

    public static void CaptureScene1Preview()
    {
        EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        Camera camera = GameObject.Find("Main Camera")?.GetComponent<Camera>();
        if (camera == null)
        {
            throw new MissingReferenceException("Main Camera was not found in the Level 2 scene.");
        }

        const int width = 1600;
        const int height = 900;
        RenderTexture renderTexture = new RenderTexture(width, height, 24);
        Texture2D capture = new Texture2D(width, height, TextureFormat.RGB24, false);
        camera.targetTexture = renderTexture;
        RenderTexture.active = renderTexture;
        Shader.WarmupAllShaders();
        camera.Render();
        GL.Flush();
        camera.Render();
        GL.Flush();
        camera.Render();
        capture.ReadPixels(new Rect(0f, 0f, width, height), 0, 0);
        capture.Apply();

        Directory.CreateDirectory("Artifacts");
        File.WriteAllBytes("Artifacts/Level2Scene1Mvp.png", capture.EncodeToPNG());
        camera.targetTexture = null;
        RenderTexture.active = null;
        Object.DestroyImmediate(renderTexture);
        Object.DestroyImmediate(capture);
        Debug.Log("LEVEL2_CAPTURE_OK path=Artifacts/Level2Scene1Mvp.png");
    }
}
