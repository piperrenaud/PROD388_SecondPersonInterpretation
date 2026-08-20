using TMPro;
using System.Collections;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    [Header("Dialogue")]
    [SerializeField] private GameObject dialogueParent;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private float timeAfterText = 2.5f;
    [SerializeField] private float typeSpeed = 0.05f;

    [Header("Audio")]
    [SerializeField] private AudioClip[] clips;
    [SerializeField][Range(0f, 1f)] private float volume = 1f;
    [SerializeField] private float minPitch = 0.95f;
    [SerializeField] private float maxPitch = 1.05f;

    private AudioSource source;
    private Coroutine dialogueCoroutine;

    public void Awake()
    {
        dialogueParent.SetActive(false);
        source = GetComponent<AudioSource>();
    }

    public void SetText(string text)
    {
        if (dialogueCoroutine != null)
        {
            StopCoroutine(dialogueCoroutine);
        }

        dialogueCoroutine = StartCoroutine(ShowDialogue(text));
    }

    private IEnumerator ShowDialogue(string text)
    {
        dialogueParent.SetActive(true);
        dialogueText.text = "";

        //typewriter
        foreach (char letter in text)
        {
            dialogueText.text += letter;
            PlaySound();

            yield return new WaitForSeconds(typeSpeed);
        }

        yield return new WaitForSeconds(timeAfterText);

        dialogueText.text = "";
        dialogueParent.SetActive(false);

        dialogueCoroutine = null;
    }

    private void PlaySound()
    {
        if (clips == null || clips.Length == 0 || source == null) return;

        AudioClip clip = clips[Random.Range(0, clips.Length)];

        if (clip == null) return;

        source.pitch = Random.Range(minPitch, maxPitch);
        source.spatialBlend = 0f;
        source.rolloffMode = AudioRolloffMode.Logarithmic;
        source.dopplerLevel = 0f;

        source.PlayOneShot(clip, volume);
    }
}
