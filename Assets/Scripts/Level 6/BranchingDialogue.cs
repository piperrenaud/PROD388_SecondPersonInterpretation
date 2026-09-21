using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[Serializable]
public class DialogueLine
{
    [TextArea(2, 5)] public string text;

    [Header("Optional")]
    public UnityEvent onLineStart;
}

[Serializable]
public class DialogueChoice
{
    [TextArea(1, 3)] public string choiceText;

    [Tooltip("the ID of the dialogue node this choice leads to")]
    public string nextNodeID;

    [Header("Optional")]
    public UnityEvent onChoiceSelected;
}

[Serializable]
public class DialogueNode
{
    [Header("Node ID")]
    public string nodeID;

    [Header("Dialogue")]
    public DialogueLine[] lines;

    [Header("Player Choices")]
    public DialogueChoice[] choices;

    [Header("After Dialogue")]
    [Tooltip("Used when this node has no choices")]
    public string nextNodeID;
}

public class BranchingDialogue : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text dialogueText;

    [Header("Choice Buttons")]
    [SerializeField] private Button[] choiceButtons;
    [SerializeField] private TMP_Text[] choiceTexts;

    [Header("Dialogue")]
    [SerializeField] private DialogueNode[] dialogueNodes;
    [SerializeField] private string startingNodeID;

    [Header("Typewriter")]
    [SerializeField] private float typeSpeed = 0.03f;
    [SerializeField] private float timeAfterText = 2.5f;
    [SerializeField] private AudioSource typingAudioSource;
    [SerializeField] private AudioClip[] clips;
    [SerializeField] private float typingVolume = 1f;

    private DialogueNode currentNode;
    private int currentLineIndex;

    private bool dialogueRunning;
    private bool displayingChoices;

    private Coroutine advanceCoroutine;
    private Coroutine typewriterCoroutie;

    private void Awake()
    {
        HideChoices();

        if (dialoguePanel != null ) dialoguePanel.SetActive(false);
    }

    private void Start()
    {
        StartDialogue();
    }

    private void Update()
    {
        if (!dialogueRunning) return;

        if (Keyboard.current.spaceKey.wasPressedThisFrame && !displayingChoices)
        {
            AdvanceDialogue();
        }
    }

    public void StartDialogue()
    {
        StartDialogue(startingNodeID);
    }

    public void StartDialogue(string nodeID)
    {
        DialogueNode node = FindNode(nodeID);

        if (node == null)
        {
            Debug.LogError("Could not find dialogue node: " + nodeID);
            return;
        }

        dialogueRunning = true;
        dialoguePanel.SetActive(true);

        LoadNode(node);
    }

    private void LoadNode(DialogueNode node)
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        dialoguePanel.SetActive(true);

        currentNode = node;
        currentLineIndex = 0;
        displayingChoices = false;

        HideChoices();

        DisplayCurrentLine();
    }

    private void DisplayCurrentLine()
    {
        if (currentNode == null || currentNode.lines == null || currentNode.lines.Length == 0)
        {
            FinishNode();
            return;
        }

        DialogueLine line = currentNode.lines[currentLineIndex];

        if (typewriterCoroutie != null)
        {
            StopCoroutine(typewriterCoroutie);
        }

        if (advanceCoroutine != null)
        {
            StopCoroutine(advanceCoroutine);
        }

        typewriterCoroutie = StartCoroutine(TypeLine(line));

        if (line.onLineStart != null)
        {
            line.onLineStart.Invoke();
        }
    }

    private IEnumerator TypeLine(DialogueLine line)
    {
        dialogueText.text = "";

        foreach (char letter in line.text)
        {
            dialogueText.text += letter;

            if (letter != ' ' && typingAudioSource != null && clips != null)
            {
                PlaySound();
            }

            yield return new WaitForSeconds(typeSpeed);
        }

        typewriterCoroutie = null;
        advanceCoroutine = StartCoroutine(WaitAndAdvance());
    }

    private IEnumerator WaitAndAdvance()
    {
        yield return new WaitForSeconds(timeAfterText);
        advanceCoroutine = null;
        AdvanceDialogue();
    }

    private void PlaySound()
    {
        if (clips == null || clips.Length == 0 || typingAudioSource == null) return;

        AudioClip clip = clips[UnityEngine.Random.Range(0, clips.Length)];

        if (clip == null) return;

        typingAudioSource.pitch = UnityEngine.Random.Range(0.95f, 1.05f);
        typingAudioSource.spatialBlend = 0f;
        typingAudioSource.rolloffMode = AudioRolloffMode.Logarithmic;
        typingAudioSource.dopplerLevel = 0f;

        typingAudioSource.PlayOneShot(clip, typingVolume);
    }

    private void AdvanceDialogue()
    {
        currentLineIndex++;

        if (currentLineIndex < currentNode.lines.Length)
        {
            DisplayCurrentLine();
            return;
        }

        if (currentNode.choices != null && currentNode.choices.Length > 0)
        {
            ShowChoices();
        }
        else
        {
            FinishNode();
        }
    }

    private void ShowChoices()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        displayingChoices = true;

        dialogueText.text = "";
        dialoguePanel.SetActive(false);

        HideChoices();

        for (int i = 0; i < currentNode.choices.Length; i++)
        {
            if (i >= choiceButtons.Length) break;

            DialogueChoice choice = currentNode.choices[i];

            choiceButtons[i].gameObject.SetActive(true);
            choiceTexts[i].text = choice.choiceText;

            int choiceIndex = i;

            choiceButtons[i].onClick.RemoveAllListeners();
            choiceButtons[i].onClick.AddListener(() =>
            {
                SelectChoice(choiceIndex);
            });
        }
    }

    private void SelectChoice(int choiceIndex)
    {
        if (choiceIndex < 0 || choiceIndex >= currentNode.choices.Length) return;

        DialogueChoice choice = currentNode.choices[choiceIndex];

        if (choice.onChoiceSelected != null)
        {
            choice.onChoiceSelected.Invoke();
        }

        HideChoices();
        displayingChoices = false;

        DialogueNode nextNode = FindNode(choice.nextNodeID);

        if (nextNode == null)
        {
            Debug.LogError("Could not find dialogue node: " + choice.nextNodeID);

            EndDialogue();
            return;
        }

        LoadNode(nextNode);
    }

    private void FinishNode()
    {
        if (!string.IsNullOrEmpty(currentNode.nextNodeID))
        {
            DialogueNode nextNode = FindNode(currentNode.nextNodeID);

            if (nextNode != null)
            {
                LoadNode(nextNode);
                return;
            }

            Debug.LogError("Could not find next dialogue node: " + currentNode.nextNodeID);
        }

        EndDialogue();
    }

    private void EndDialogue()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        dialogueRunning = false;
        displayingChoices = false;

        HideChoices();

        if (dialoguePanel != null) dialoguePanel.SetActive(false);
    }

    private DialogueNode FindNode(string nodeID)
    {
        foreach (DialogueNode node in dialogueNodes)
        {
            if (node.nodeID == nodeID) return node;
        }

        return null;
    }

    private void HideChoices()
    {
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (choiceButtons[i] != null)
            {
                choiceButtons[i].gameObject.SetActive(false);
            }
        }
    }

    public bool IsDialogueRunning()
    {
        return dialogueRunning;
    }

    public void ShowDebug(string message)
    {
        Debug.Log(message);
    }
}
