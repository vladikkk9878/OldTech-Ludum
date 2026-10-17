using UnityEngine;
using UnityEngine.InputSystem;

public class LidarScanner : MonoBehaviour
{
    [Header("Ссылки")] public ParticleSystem lidarParticleSystem;
    private Camera mainCamera;

    [Header("Настройки лучей")] public float scanDistance = 50f;
    public float particleSize = 0.04f;
    public Gradient distanceColors;

    [Header("Режим ЛКМ (Плотный луч)")] public int leftClickRays = 150;
    public float leftClickSpread = 3f;

    [Header("Режим ПКМ (Рассеянный веер)")]
    public int rightClickRays = 300;

    public float rightClickSpread = 45f;

    private ParticleSystem.ColorOverLifetimeModule colorModule;

    void Start()
    {
        mainCamera = Camera.main;

        if (lidarParticleSystem != null)
        {
            colorModule = lidarParticleSystem.colorOverLifetime;
            colorModule.enabled = true;

            Gradient fadeGradient = new Gradient();

            GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
            alphaKeys[0] = new GradientAlphaKey(1.0f, 0.0f);
            alphaKeys[1] = new GradientAlphaKey(0.0f, 1.0f);

            GradientColorKey[] colorKeys = new GradientColorKey[2];
            colorKeys[0] = new GradientColorKey(Color.white, 0.0f);
            colorKeys[1] = new GradientColorKey(Color.white, 1.0f);

            fadeGradient.SetKeys(colorKeys, alphaKeys);
            colorModule.color = fadeGradient;
        }
    }

    void Update()
    {
        if (Mouse.current == null || mainCamera == null) return;

        if (Mouse.current.leftButton.isPressed)
        {
            Scan(leftClickRays, leftClickSpread);
        }
        else if (Mouse.current.rightButton.isPressed)
        {
            Scan(rightClickRays, rightClickSpread);
        }
    }

    void Scan(int raysCount, float currentSpread)
    {
        if (lidarParticleSystem == null || mainCamera == null) return;

        ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams();

        for (int i = 0; i < raysCount; i++)
        {
            Vector3 rayDirection = Quaternion.Euler(
                Random.Range(-currentSpread, currentSpread),
                Random.Range(-currentSpread, currentSpread),
                0
            ) * transform.forward;

            Ray ray = new Ray(transform.position, rayDirection);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, scanDistance))
            {
                float normalizedDistance = hit.distance / scanDistance;
                Color pointColor = distanceColors.Evaluate(normalizedDistance);

                emitParams.position = hit.point;
                emitParams.velocity = Vector3.zero;
                emitParams.startColor = pointColor;
                emitParams.startSize = particleSize;

                lidarParticleSystem.Emit(emitParams, 1);
            }
        }
    }
}
