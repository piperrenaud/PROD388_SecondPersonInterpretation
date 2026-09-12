using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndTrigger : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private GameObject inputTextBox;
    [SerializeField] private CameraOrbit cameraOrbit;
    [SerializeField] private ProtagonistGridMovement protagMovement;
    [SerializeField] private Animator fadeToBlackAnimator;
    [SerializeField] private RandomDialogue randomDialogue;
    [SerializeField] private MovementInput movementInput;

    [Header("Exit room")]
    [SerializeField] private DoorConnection connectingDoor;
    [SerializeField] private Vector2Int targetPosition;

    [Header("Ending text")]
    [TextArea(2, 4)]
    [SerializeField] private string[] lines;

    private void OnTriggerEnter(Collider other)
    {
        if (dialogueManager != null)
        {
            StartCoroutine(EndingBehaviour());
        }
    }

    private IEnumerator EndingBehaviour()
    {
        yield return new WaitForSeconds(3f);

        inputTextBox.SetActive(false);
        cameraOrbit.enabled = false;
        randomDialogue.enabled = false;
        movementInput.enabled = false;

        foreach (var line in lines)
        {
            dialogueManager.SetProtagText(line);
            yield return new WaitUntil(() => !dialogueManager.IsDialogueRunning);
        }

        Debug.Log("Dialogue done");
        StartCoroutine(protagMovement.MoveThroughDoor(targetPosition, connectingDoor));

        yield return new WaitForSeconds(3f);

        fadeToBlackAnimator.gameObject.SetActive(true);
        fadeToBlackAnimator.SetTrigger("End");

        yield return new WaitForSeconds(6f);

        LoadNewScene();
    }

    private void LoadNewScene()
    {
        SceneManager.LoadScene("Level 3");
    }
}
