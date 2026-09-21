using System.Collections;
using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    private Light pointLight;

    private float minIntensity = 0.1f;
    private float maxIntensity = 1.2f;

    private float minFlickerTime = 0.08f;
    private float maxFlickerTime = 1.8f;

    private void Start()
    {
        pointLight = GetComponent<Light>();

        StartCoroutine(Flicker());
    }

    private IEnumerator Flicker()
    {
        while (true)
        {
            pointLight.intensity = Random.Range(minIntensity, maxIntensity);

            yield return new WaitForSeconds(Random.Range(minFlickerTime, maxFlickerTime));
        }
    }
}
