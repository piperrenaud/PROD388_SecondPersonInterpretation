using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [Header("Shake Settings")]
    [SerializeField] private float shakeDuration = 8f;
    [SerializeField] private float startingIntensity = 0.05f;
    [SerializeField] private float maximumIntensity = 0.5f;
    [SerializeField] private float shakeSpeed = 20f;

    private float shakeTimer;
    private bool isShaking;
    private Vector3 originalLocalPosition;

    private void Awake()
    {
        originalLocalPosition = transform.localPosition;
    }
    public void Update()
    {
        if (!isShaking) return;

        shakeTimer += Time.deltaTime;

        //0-> over the duration
        float progress = Mathf.Clamp01(shakeTimer / shakeDuration);

        //increase
        float intensity = Mathf.Lerp(startingIntensity, maximumIntensity, progress);

        //perlin noise
        float x = (Mathf.PerlinNoise(Time.time * shakeSpeed, 0f) - 0.5f) * 2f;
        float y = (Mathf.PerlinNoise(0f, Time.time * shakeSpeed) - 0.5f) * 2f;

        Vector3 shakeOffset = new Vector3(x, y, 0f) * intensity;

        transform.localPosition = originalLocalPosition + shakeOffset;

        //stop after duration
        if (shakeTimer >= shakeDuration)
        {
            StopShake();
        }
    }

    public void StartShake()
    {
        shakeTimer = 0f;
        isShaking = true;

        transform.localPosition = originalLocalPosition;
    }

    public void StopShake()
    {
        isShaking = false;
        shakeTimer = 0f;

        transform.localPosition = originalLocalPosition;
    }
}
