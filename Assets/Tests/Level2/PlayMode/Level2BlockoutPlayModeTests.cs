using System.Collections;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public sealed class Level2BlockoutPlayModeTests
{
    private const string ScenePath = "Assets/Scenes/Level2_Ponyo.unity";
    private const float ExpectedRunSpeed = 3.68f;

    [UnityTest]
    public IEnumerator LoweredCoastBringsSeaCloserAndSpawnStartsAboveIt()
    {
        yield return OpenLevel();
        Bounds soil = GameObject.Find("CoastalGround").transform.Find("Soil").GetComponent<Renderer>().bounds;
        float groundTop = GameObject.Find("CoastalGround").transform.Find("GrassCap").GetComponent<Renderer>().bounds.max.y;
        float expectedWaterline = Mathf.Lerp(soil.min.y, groundTop, 0.52f);
        Assert.That(groundTop, Is.EqualTo(1.152f).Within(0.002f));
        Assert.That(Camera.main.fieldOfView, Is.EqualTo(28.58f).Within(0.02f));

        foreach (string tile in new[] { "ShoreWater_0_00", "Sea_0_00", "Sea_0_03" })
        {
            float waterline = GameObject.Find(tile).transform.Find("TexturedWaterSurface").GetComponent<Renderer>().bounds.center.y;
            Assert.That(waterline, Is.EqualTo(expectedWaterline).Within(0.002f), tile);
        }

        Transform spawn = GameObject.Find("Checkpoint_S1_Start").transform;
        var player = Object.FindAnyObjectByType<PonyoMovement2D>();
        Assert.That(spawn.position.x, Is.InRange(3.3f, 4.4f));
        Assert.That(player.GetComponent<Collider2D>().bounds.min.y, Is.GreaterThan(expectedWaterline + 0.2f));
    }

    [UnityTest]
    public IEnumerator PonyoRunsFromStairsOntoLandWithoutJumping()
    {
        yield return OpenLevel();
        GameObject landing = GameObject.Find("CoastalStairLanding");
        Assert.That(landing, Is.Not.Null);
        Bounds paving = landing.GetComponent<MeshRenderer>().bounds;
        Assert.That(paving.min.x, Is.LessThan(8.8f));
        Assert.That(paving.max.x, Is.GreaterThan(9.8f));
        Assert.That(landing.GetComponentsInChildren<Collider2D>(), Is.Empty);

        var player = Object.FindAnyObjectByType<PonyoMovement2D>().GetComponent<Rigidbody2D>();
        yield return FixedSteps(105);
        Assert.That(player.position.x, Is.GreaterThan(10f));
        Assert.That(player.position.y, Is.EqualTo(FlatPlayerY).Within(0.12f));
    }

    [UnityTest]
    public IEnumerator Scene1AutoRunAndTimelineMarkersAreValid()
    {
        yield return OpenLevel();

        GameObject player = GameObject.Find("Ponyo_PLACEHOLDER");
        Assert.That(player, Is.Not.Null, "The scene must contain the Level 2 player placeholder.");
        Assert.That(player.TryGetComponent(out PonyoMovement2D movement), Is.True);
        Assert.That(movement.RunSpeed, Is.EqualTo(ExpectedRunSpeed).Within(0.001f));

        float startX = player.transform.position.x;
        for (int index = 0; index < 15; index++)
        {
            yield return new WaitForFixedUpdate();
        }

        Assert.That(player.transform.position.x, Is.GreaterThan(startX + 0.5f), "Auto-run must advance Ponyo to the right.");

        AssertMarker("S1_END_25.75s_x82.4", 82.4f);
        AssertMarker("S2_END_50.75s_x162.4", 162.4f);
        AssertMarker("S3_END_75.75s_x242.4", 242.4f);
        AssertMarker("LEVEL_END_110.75s_x354.4", 354.4f);
        Assert.That(Object.FindAnyObjectByType<BubbleTrap>(), Is.Not.Null);
        Assert.That(Object.FindAnyObjectByType<PuddleSlowZone>(), Is.Not.Null);
        Assert.That(Object.FindAnyObjectByType<BounceSurface2D>(), Is.Not.Null);
    }

    [UnityTest]
    public IEnumerator IslandsStayAnchoredAsCameraFollowsPlayer()
    {
        yield return OpenLevel();
        var mid = GameObject.Find("CoastIsland_WestHeadland").transform;
        var far = GameObject.Find("CoastIsland_FarHeadland").transform;
        Transform camera = Camera.main.transform;
        Vector3 originalMid = mid.position;
        Vector3 originalFar = far.position;
        float originalCameraX = camera.position.x;

        yield return FixedSteps(120);

        Assert.That(camera.position.x, Is.GreaterThan(originalCameraX + 2f));
        Assert.That(mid.position, Is.EqualTo(originalMid));
        Assert.That(far.position, Is.EqualTo(originalFar));
    }

    [UnityTest]
    public IEnumerator HeldJumpClearsFirstBubble()
    {
        yield return OpenLevel();
        var player = Object.FindAnyObjectByType<PonyoMovement2D>().GetComponent<Rigidbody2D>();
        float bubbleX = Object.FindAnyObjectByType<BubbleTrap>().transform.position.x;
        player.position = new Vector2(bubbleX - 1.2f, FlatPlayerY);
        yield return FixedSteps(4);

        Keyboard keyboard = InputSystem.AddDevice<Keyboard>();
        try
        {
            InputSystem.QueueStateEvent(keyboard, new KeyboardState(Key.Space));
            yield return FixedSteps(29);
            Assert.That(player.position.x, Is.GreaterThan(bubbleX + 0.6f),
                "A held jump should clear the tutorial bubble without a respawn.");
        }
        finally
        {
            InputSystem.RemoveDevice(keyboard);
        }
    }

    [UnityTest]
    public IEnumerator Section1ClimbsRespawnsSlowsBouncesAndStops()
    {
        yield return OpenLevel();
        var player = Object.FindAnyObjectByType<PonyoMovement2D>().GetComponent<Rigidbody2D>();

        yield return FixedSteps(210);
        Assert.That(player.position.x, Is.GreaterThan(10f), "Auto-run should climb the stairs and continue on the flat without a jump.");
        Assert.That(player.position.y, Is.EqualTo(FlatPlayerY).Within(0.15f));

        player.position = new Vector2(33.7f, FlatPlayerY);
        yield return FixedSteps(30);
        Assert.That(player.position.x, Is.LessThan(5f), "The first bubble should send Ponyo back to the start.");

        player.position = new Vector2(41f, FlatPlayerY);
        yield return FixedSteps(8);
        Assert.That(player.linearVelocity.x, Is.EqualTo(ExpectedRunSpeed * 0.55f).Within(0.05f));
        player.position = new Vector2(46f, FlatPlayerY);
        yield return FixedSteps(8);
        Assert.That(player.linearVelocity.x, Is.EqualTo(ExpectedRunSpeed).Within(0.05f));

        var wave = Object.FindAnyObjectByType<BounceSurface2D>().GetComponent<Rigidbody2D>();
        player.position = wave.position + new Vector2(-0.35f, 1.55f);
        player.linearVelocity = new Vector2(ExpectedRunSpeed, -4f);
        bool bounced = false;
        for (int step = 0; step < 24; step++)
        {
            yield return new WaitForFixedUpdate();
            bounced |= player.linearVelocity.y > 5f;
        }
        Assert.That(bounced, Is.True, "Landing from above should bounce Ponyo on the moving wave.");

        player.position = new Vector2(79.5f, FlatPlayerY);
        player.linearVelocity = Vector2.zero;
        yield return FixedSteps(48);
        Assert.That(player.position.x, Is.InRange(81.1f, 81.9f));
        Assert.That(player.linearVelocity, Is.EqualTo(Vector2.zero), "The section preview should stop before the ground ends.");
    }

    [UnityTest]
    public IEnumerator ExtendedApproachHasFiveExtraSecondsAndStopsOnSolidGround()
    {
        yield return OpenLevel();
        var movement = Object.FindAnyObjectByType<PonyoMovement2D>();
        Transform finish = GameObject.Find("Section1PreviewEnd").transform;
        Assert.That((finish.position.x - 63.4f) / movement.RunSpeed, Is.EqualTo(5f).Within(0.001f));
        Assert.That(GameObject.Find("UrbanApproach_Rail_09").GetComponentsInChildren<Collider2D>(), Is.Empty);
        var body = movement.GetComponent<Rigidbody2D>();
        body.position = new Vector2(78.8f, FlatPlayerY);
        body.linearVelocity = Vector2.zero;
        yield return FixedSteps(60);
        Assert.That(body.position.x, Is.InRange(81.1f, 81.9f));
        Assert.That(body.position.y, Is.EqualTo(FlatPlayerY).Within(0.12f));
        Assert.That(body.linearVelocity, Is.EqualTo(Vector2.zero));
    }

    private static float FlatPlayerY =>
        GameObject.Find("CoastalGround").transform.Find("GrassCap").GetComponent<Renderer>().bounds.max.y + 0.575f;

    private static IEnumerator OpenLevel()
    {
        AsyncOperation operation = EditorSceneManager.LoadSceneAsyncInPlayMode(
            ScenePath, new LoadSceneParameters(LoadSceneMode.Single));
        while (!operation.isDone)
        {
            yield return null;
        }
    }

    private static IEnumerator FixedSteps(int count)
    {
        for (int step = 0; step < count; step++)
        {
            yield return new WaitForFixedUpdate();
        }
    }

    private static void AssertMarker(string name, float expectedX)
    {
        GameObject marker = GameObject.Find(name);
        Assert.That(marker, Is.Not.Null, $"Missing timeline marker {name}.");
        Assert.That(marker.transform.position.x, Is.EqualTo(expectedX).Within(0.001f));
    }
}
