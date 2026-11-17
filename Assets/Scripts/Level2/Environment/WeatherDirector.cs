using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public sealed class WeatherDirector : MonoBehaviour
{
    [SerializeField] private Material skybox;
    [SerializeField] private Light sun;
    [SerializeField] private Color clearFog = new(0.62f, 0.82f, 0.88f);
    [SerializeField] private Color stormFog = new(0.30f, 0.37f, 0.45f);
    [SerializeField, Min(0f)] private float clearFogDensity = 0.008f;
    [SerializeField, Min(0f)] private float stormFogDensity = 0.035f;
    [SerializeField] private Color clearSun = new(1f, 0.94f, 0.82f);
    [SerializeField] private Color stormSun = new(0.63f, 0.72f, 0.87f);
    [SerializeField, Min(0f)] private float clearSunIntensity = 1.25f;
    [SerializeField, Min(0f)] private float stormSunIntensity = 0.35f;
    [SerializeField] private ParticleSystem rain;
    [SerializeField] private ParticleSystem splashes;
    [SerializeField, Min(0f)] private float stormRainRate = 120f;
    [SerializeField, Min(0f)] private float stormSplashRate = 24f;
    [SerializeField, Range(0f, 1f)] private float storm;

    private void OnEnable()
    {
        SceneManager.activeSceneChanged += OnActiveSceneChanged;
        Apply();
    }

    private void OnDisable() => SceneManager.activeSceneChanged -= OnActiveSceneChanged;
    private void OnActiveSceneChanged(Scene previous, Scene current) => Apply();

    public void Configure(Material sky, Light directionalLight)
    {
        skybox = sky;
        sun = directionalLight;
        Apply();
    }

    public void SetStorm(float amount)
    {
        storm = Mathf.Clamp01(amount);
        Apply();
    }

    private void Apply()
    {
        if (gameObject.scene != SceneManager.GetActiveScene())
        {
            return;
        }

        RenderSettings.skybox = skybox;
        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.Exponential;
        RenderSettings.fogColor = Color.Lerp(clearFog, stormFog, storm);
        RenderSettings.fogDensity = Mathf.Lerp(clearFogDensity, stormFogDensity, storm);
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
        RenderSettings.ambientSkyColor = Color.Lerp(clearFog, stormFog, storm);
        RenderSettings.ambientEquatorColor = Color.Lerp(new Color(0.58f, 0.60f, 0.57f), stormFog, storm);
        RenderSettings.ambientGroundColor = new Color(0.38f, 0.40f, 0.48f);
        RenderSettings.sun = sun;
        if (sun != null)
        {
            sun.color = Color.Lerp(clearSun, stormSun, storm);
            sun.intensity = Mathf.Lerp(clearSunIntensity, stormSunIntensity, storm);
        }

        SetEmission(rain, storm * stormRainRate);
        SetEmission(splashes, storm * stormSplashRate);
    }

    private static void SetEmission(ParticleSystem particles, float rate)
    {
        if (particles == null)
        {
            return;
        }

        ParticleSystem.EmissionModule emission = particles.emission;
        emission.enabled = rate > 0f;
        emission.rateOverTime = rate;
    }
}
