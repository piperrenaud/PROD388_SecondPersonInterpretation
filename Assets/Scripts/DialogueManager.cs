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

    [Header("Dialogue Type 2")]
    [SerializeField] private GameObject dialogueParent2;
    [SerializeField] private TMP_Text dialogue2Text;

    [Header("Audio")]
    [SerializeField] private AudioClip[] clips;
    [SerializeField][Range(0f, 1f)] private float volume = 1f;
    [SerializeField] private float minPitch = 0.95f;
    [SerializeField] private float maxPitch = 1.05f;

    private AudioSource source;
    private Coroutine dialogueCoroutine;
    public bool IsDialogueRunning => dialogueCoroutine != null;

    public void Awake()
    {
        if (dialogueParent2 != null) dialogueParent2.SetActive(false);

        dialogueParent.SetActive(false);
        source = GetComponent<AudioSource>();
    }

    public void SetText(string text)
    {
        //stop whatever dialogue is currently playing
        if (dialogueCoroutine != null)
        {
            StopCoroutine(dialogueCoroutine);
            dialogueCoroutine = null;
        }

        //clear old text
        dialogueText.text = "";

        //start new dialogue immediately
        dialogueCoroutine = StartCoroutine(ShowDialogue(text, dialogueParent, dialogueText));
    }

    public void SetDialogue(string text)
    {
        //stop whatever dialogue is currently playing
        if (dialogueCoroutine != null)
        {
            StopCoroutine(dialogueCoroutine);
            dialogueCoroutine = null;
        }

        //clear old text
        dialogue2Text.text = "";

        //start new dialogue immediately
        dialogueCoroutine = StartCoroutine(ShowDialogue(text, dialogueParent2, dialogue2Text));
    }

    private IEnumerator ShowDialogue(string text, GameObject dialogueParentObject, TMP_Text dialogueTextObject)
    {
        dialogueParentObject.SetActive(true);
        dialogueTextObject.text = "";

        //typewriter
        foreach (char letter in text)
        {
            dialogueTextObject.text += letter;
            PlaySound();

            yield return new WaitForSeconds(typeSpeed);
        }

        yield return new WaitForSeconds(timeAfterText);

        dialogueTextObject.text = "";
        dialogueParentObject.SetActive(false);

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

    public void SetTimeAfterText(float time)
    {
        timeAfterText = time;
    }
}
