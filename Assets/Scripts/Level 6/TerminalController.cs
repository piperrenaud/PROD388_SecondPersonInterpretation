using TMPro;
using UnityEngine;

public class TerminalController : MonoBehaviour
{
    [Header("Text")]
    [SerializeField] private TMP_Text textBox;
    [SerializeField] private GameObject terminalParent;

    [Header("Audio")]
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip computerNoise;
    [SerializeField] private float volume = 1f;

    public void SetTerminalText(string input)
    {
        if (textBox == null) return;

        string[] strings = input.Split(',');

        textBox.text = string.Join("\n", strings);

        PlaySound();
    }

    private void PlaySound()
    {
        if (computerNoise == null || source == null) return;

        source.pitch = Random.Range(0.95f, 1.05f);
        source.spatialBlend = 0f;
        source.rolloffMode = AudioRolloffMode.Logarithmic;
        source.dopplerLevel = 0f;

        source.PlayOneShot(computerNoise, volume);
    }

    public void TurnOffTerminal()
    {
        terminalParent.SetActive(false);
    }

    public void TurnOnTerminal()
    {
        terminalParent.SetActive(true);
    }
}
