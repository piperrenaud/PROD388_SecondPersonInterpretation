using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LeaveOrStayEndings : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject fadeToBlack;
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private GameObject finalChoiceParent;
    [SerializeField] private Transform terminalToFace;
    [SerializeField] private ProtagonistMovement protagonistMovement;
    [SerializeField] private SceneController sceneController;
    [SerializeField] private ExpressionController expressionController;

    [Header("Popups")]
    [SerializeField] private GameObject applicationPopup;
    [SerializeField] private GameObject statusPopup;
    [SerializeField] private GameObject buildingPopup;
    [SerializeField] private GameObject failedPopup;

    private Animator fadeToBlackAnim;

    private void Start()
    {
        fadeToBlackAnim = fadeToBlack.GetComponent<Animator>();

        applicationPopup.SetActive(false);
        statusPopup.SetActive(false);
        buildingPopup.SetActive(false);
        failedPopup.SetActive(false);

        protagonistMovement.enabled = false;
        playerMovement.enabled = false;

        StartCoroutine(StartDialogue());
    }

    private IEnumerator StartDialogue()
    {
        fadeToBlack.SetActive(true);
        fadeToBlackAnim.SetTrigger("Start");
        yield return new WaitForSeconds(5f);

        StartLevel();
    }

    private void StartLevel()
    {
        playerMovement.enabled = true;
        protagonistMovement.enabled=true;

        fadeToBlack.SetActive(false);
    }

    public void LeaveEndingClicked()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        playerMovement.EnableMouseLook();

        finalChoiceParent.SetActive(false); 

        StartCoroutine(LeavingEnding());
    }

    private IEnumerator LeavingEnding()
    {
        yield return new WaitForSeconds(1f);

        //dialogue
        dialogueManager.SetProtagText("Okay");
        yield return new WaitUntil(() => !dialogueManager.IsDialogueRunning);
        dialogueManager.SetProtagText("I understand");
        yield return new WaitUntil(() => !dialogueManager.IsDialogueRunning);
        
        protagonistMovement.TurnTo(terminalToFace);
        yield return new WaitForSeconds(1f);

        dialogueManager.SetProtagText("You should go");
        yield return new WaitUntil(() => !dialogueManager.IsDialogueRunning);
        yield return new WaitForSeconds(1f);

        sceneController.ScreenFlicker();
        sceneController.RemoveFacilityCompletely();
        yield return new WaitForSeconds(2f);

        protagonistMovement.TurnTo(playerMovement.gameObject.transform);
        yield return new WaitForSeconds(1f);

        dialogueManager.SetProtagText("Thank you for staying as long as you did");
        yield return new WaitUntil(() => !dialogueManager.IsDialogueRunning);
        yield return new WaitForSeconds(2f);
        //

        fadeToBlack.SetActive(true);
        fadeToBlackAnim.SetTrigger("End");
        yield return new WaitForSeconds(5f);

        ShowApplicationPopup(true);
        yield return new WaitForSeconds(3f);
        ShowApplicationPopup(false);

        ShowStatusPopup(true);
        yield return new WaitForSeconds(3f);
        ShowStatusPopup(false);

        Application.Quit();
    }

    public void StayEndingClicked()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        playerMovement.EnableMouseLook();

        finalChoiceParent.SetActive(false);

        StartCoroutine(StayEnding());
    }

    private IEnumerator StayEnding()
    {
        yield return new WaitForSeconds(1f);

        //dialogue
        dialogueManager.SetProtagText("Really?");
        expressionController.SetHappy();
        yield return new WaitUntil(() => !dialogueManager.IsDialogueRunning);
        dialogueManager.SetProtagText("You're staying??");
        yield return new WaitUntil(() => !dialogueManager.IsDialogueRunning);

        protagonistMovement.TurnTo(terminalToFace);

        dialogueManager.SetProtagText("Okay");
        yield return new WaitUntil(() => !dialogueManager.IsDialogueRunning);

        sceneController.RebuildNewWorld();
        yield return new WaitForSeconds(1f);

        dialogueManager.SetProtagText("If we're going to stay...");
        yield return new WaitUntil(() => !dialogueManager.IsDialogueRunning);
        yield return new WaitForSeconds(1f);

        dialogueManager.SetProtagText("...I think we should make it more fun");
        yield return new WaitUntil(() => !dialogueManager.IsDialogueRunning);
        yield return new WaitForSeconds(1f);
        //

        fadeToBlack.SetActive(true);
        fadeToBlackAnim.SetTrigger("End");
        yield return new WaitForSeconds(5f);

        ShowBuildingPopup(true);
        yield return new WaitForSeconds (3f);
        ShowBuildingPopup(false);

        ShowFailedPopup(true);
        yield return new WaitForSeconds(3f);
        ShowFailedPopup(false);

        dialogueManager.SetProtagText("Oh.");
        yield return new WaitUntil(() => !dialogueManager.IsDialogueRunning);

        yield return new WaitForSeconds(2f);

        SceneManager.LoadScene("Menu");
    }

    private void ShowBuildingPopup(bool enabled)
    {
        buildingPopup.SetActive(enabled);
    }

    private void ShowFailedPopup(bool enabled)
    {
        failedPopup.SetActive(enabled);
    }

    private void ShowStatusPopup(bool enabled)
    {
        statusPopup.SetActive(enabled);
    }

    private void ShowApplicationPopup(bool enabled)
    {
        applicationPopup.SetActive(enabled);
    }
}
