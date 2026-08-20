using UnityEngine;

public class LightSwitch : MonoBehaviour
{
    [Header("Lights")]
    [SerializeField] private Light[] lights;
    [SerializeField] private bool lightsOn = true;
    [SerializeField] private AudioClip lightSwitchNoise;

    private AudioSource source;

    private void Awake()
    {
        foreach (Light light in lights)
        {
            light.gameObject.SetActive(lightsOn);
        }

        source = GetComponent<AudioSource>();
    }

    public void ToggleLights()
    {
        lightsOn = !lightsOn;

        foreach (Light light in lights)
        {
            light.gameObject.SetActive(lightsOn);
            Debug.Log(light.gameObject.name + " is On? " + lightsOn);
        }

        PlaySound();
    }

    private void PlaySound()
    {
        if (lightSwitchNoise == null || source == null) return;

        source.pitch = Random.Range(0.95f, 1.05f);
        source.spatialBlend = 1f;
        source.rolloffMode = AudioRolloffMode.Logarithmic;
        source.minDistance = 1.5f;
        source.maxDistance = 12f;
        source.dopplerLevel = 0f;

        source.PlayOneShot(lightSwitchNoise, 0.5f);
    }    
}
