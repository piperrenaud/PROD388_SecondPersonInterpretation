using UnityEngine;

public class LightSwitch : MonoBehaviour
{
    [Header("Narration")]
    [SerializeField] private NarratorManager narratorManager;
    [SerializeField] private int interactionsBeforeNarration = 12;
    [SerializeField] private float maxTimeBetweenInteractions = 2f;

    [Header("Lights")]
    [SerializeField] private Light[] lights;
    [SerializeField] private bool lightsOn = true;
    [SerializeField] private AudioClip lightSwitchNoise;

    private AudioSource source;

    private int interactionCount = 0;
    private float interactionTimer = 0f;

    private bool lastInteractionWasOpen = false;
    private bool hasStartedSequence = false;

    private void Awake()
    {
        foreach (Light light in lights)
        {
            light.gameObject.SetActive(lightsOn);
        }

        source = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (!hasStartedSequence)
        {
            return;
        }

        interactionTimer += Time.deltaTime;

        if (interactionTimer > maxTimeBetweenInteractions)
        {
            ResetSequence();
        }
    }

    public void ToggleLights()
    {
        lightsOn = !lightsOn;

        foreach (Light light in lights)
        {
            light.gameObject.SetActive(lightsOn);
        }

        if (lightsOn)
        {
            if (hasStartedSequence && lastInteractionWasOpen)
            {
                ResetSequence();
            }
            lastInteractionWasOpen = true;

            CheckForRepeatedInteraction();
        }
        
        if (!lightsOn)
        {
            if (hasStartedSequence && !lastInteractionWasOpen)
            {
                ResetSequence();
            }
            lastInteractionWasOpen = false;

            CheckForRepeatedInteraction();
        }

        interactionCount++;
        hasStartedSequence = true;
        interactionTimer = 0f;
        PlaySound();
    }

    private void CheckForRepeatedInteraction()
    {
        if (interactionCount >= interactionsBeforeNarration)
        {
            narratorManager.LightSequenceCompleted();
            ResetSequence();
        }
    }

    private void ResetSequence()
    {
        interactionCount = 0;
        interactionTimer = 0f;
        hasStartedSequence = false;
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
