using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SceneController : MonoBehaviour
{
    [Header("Objects to be deleted/disabled")]
    [SerializeField] private GameObject debrisParent;
    [SerializeField] private GameObject experimentParent;
    [SerializeField] private GameObject facilityScifiWallsParent;
    [SerializeField] private GameObject facilityFurnitureParent;
    [SerializeField] private GameObject officeAreaParent;
    [SerializeField] private GameObject facilityWallsParent;
    [SerializeField] private GameObject facilitySetupParent;
    [SerializeField] private GameObject wholeFacilityParent;
    [SerializeField] private GameObject playerBody;
    [SerializeField] private GameObject objectsThatStay;

    [Header("Objects to Manipulate")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private ProtagonistMovement protagonistMovement;
    [SerializeField] private GameObject screenGlitch;
    [SerializeField] private GameObject errorImage;
    [SerializeField] private GameObject finalChoicesParent;

    [Header("New Room")]
    [SerializeField] private GameObject structureParent;
    [SerializeField] private GameObject baseParent;
    [SerializeField] private GameObject extraParent;

    [Header("References")]
    [SerializeField] private Transform terminalToFace;

    [Header("Audio")]
    [SerializeField] private float glitchVolumne = 1f;
    [SerializeField] private AudioClip[] glitchClips;
    [SerializeField] private float disableVolume = 1f;
    [SerializeField] private AudioClip[] disableBeep;
    [SerializeField] private float disappearVolume = 1f;
    [SerializeField] private AudioClip[] disappearBeep;
    [SerializeField] private float restoredVolume = 1f;
    [SerializeField] private AudioClip[] restoredBeep;


    private AudioSource source;

    public void Awake()
    {
        source = GetComponent<AudioSource>();

        errorImage.SetActive(false);
        screenGlitch.SetActive(false);
        finalChoicesParent.SetActive(false);

        structureParent.SetActive(false);
        baseParent.SetActive(false);
        extraParent.SetActive(false);

        objectsThatStay.SetActive(true);
    }

    public void ScreenFlicker()
    {
        screenGlitch.SetActive(true);
        StartCoroutine(GlitchingScreen());
    }

    private IEnumerator GlitchingScreen()
    {
        Animator animator = screenGlitch.GetComponent<Animator>();
        animator.SetTrigger("Glitch");

        PlaySound(glitchClips, glitchVolumne);

        yield return new WaitForSeconds(0.35f);
        screenGlitch.SetActive(false);
    }

    public void ShowMissingObjectError()
    {
        errorImage.SetActive(true);

        StartCoroutine(ErrorPopup());
    }

    private IEnumerator ErrorPopup()
    {
        Animator animator = errorImage.GetComponent<Animator>();
        animator.SetTrigger("In");

        yield return new WaitForSeconds(5f);

        animator.SetTrigger("Out");
        yield return new WaitForSeconds(0.2f);
        errorImage.SetActive(false);
    }


    public void DisableFirstObject()
    {
        protagonistMovement.TurnTo(terminalToFace);
        debrisParent.SetActive(false);

        PlaySound(disappearBeep, disappearVolume);
    }

    public void DisableSecondObject()
    {
        protagonistMovement.TurnTo(terminalToFace);
        experimentParent.SetActive(false);

        PlaySound(disappearBeep, disappearVolume);
    }

    public void DisableThridObject()
    {
        protagonistMovement.TurnTo(terminalToFace);
        facilityScifiWallsParent.SetActive(false);

        PlaySound(disappearBeep, disappearVolume);
    }

    public void EnableInitialObjects()
    {
        debrisParent.SetActive(true);
        experimentParent.SetActive(true);
        facilityScifiWallsParent.SetActive(true);
        PlaySound(restoredBeep, restoredVolume);
    }

    public void DisableWholeScene()
    {
        protagonistMovement.TurnTo(terminalToFace);
        StartCoroutine(SceneDisable());
    }

    private IEnumerator SceneDisable()
    {
        facilityFurnitureParent.SetActive(false);
        PlaySound(disappearBeep, disappearVolume);
        yield return new WaitForSeconds(1f);

        debrisParent.SetActive(false);
        PlaySound(disappearBeep, disappearVolume);
        yield return new WaitForSeconds(1f);

        experimentParent.SetActive(false);
        PlaySound(disappearBeep, disappearVolume);
        yield return new WaitForSeconds(1f);

        facilityScifiWallsParent.SetActive(false);
        PlaySound(disappearBeep, disappearVolume);
        yield return new WaitForSeconds(1f);

        officeAreaParent.SetActive(false);
        facilityWallsParent.SetActive(false);
        PlaySound(disappearBeep, disappearVolume);
        yield return new WaitForSeconds(1f);

        facilitySetupParent.SetActive(false);
        PlaySound(disappearBeep, disappearVolume);
    }

    public void SlowPlayerMovement()
    {
        //turns to look at terminal
        protagonistMovement.TurnTo(terminalToFace);
        playerMovement.SlowMovementSpeed();
        PlaySound(disableBeep, disableVolume);
    }

    public void RestorePlayerMovementSpeed()
    {
        playerMovement.RestoreMovementSpeed();
    }

    public void DisablePlayerWASDMovement()
    {
        protagonistMovement.TurnTo(terminalToFace);
        playerMovement.DisableMovement();
        PlaySound(disableBeep, disableVolume);
    }

    public void EnablePlayerWASDMovement()
    {
        protagonistMovement.TurnTo(terminalToFace);
        playerMovement.EnableMovement();
        PlaySound(restoredBeep, restoredVolume);
    }

    public void DisablePlayerMouseLook()
    {
        protagonistMovement.TurnTo(terminalToFace);
        playerMovement.DisableMouseLook();
        PlaySound(disableBeep, disableVolume);
    }

    public void EnablePlayerMouseLook()
    {
        playerMovement.EnableMouseLook();
        PlaySound(restoredBeep, restoredVolume);
    }    

    public void DisablePlayerBody()
    {
        protagonistMovement.TurnTo(terminalToFace);
        playerBody.SetActive(false);
        PlaySound(disappearBeep, disappearVolume);
    }

    public void EnablePlayerBody()
    {
        playerBody.SetActive(true);
        PlaySound(restoredBeep, restoredVolume);
    }

    public void RemoveFacilityCompletely()
    {
        protagonistMovement.TurnTo(terminalToFace);
        wholeFacilityParent.SetActive(false);
        PlaySound(disappearBeep, disappearVolume);
    }

    public void RebuildNewWorld()
    {
        StartCoroutine(Rebuild());
    }

    private IEnumerator Rebuild()
    {
        yield return new WaitForSeconds(1f);

        objectsThatStay.SetActive(false);
        structureParent.SetActive(true);
        PlaySound(restoredBeep, restoredVolume);
        yield return new WaitForSeconds(1.5f);

        baseParent.SetActive(true);
        PlaySound(restoredBeep, restoredVolume);
        yield return new WaitForSeconds(1.5f);

        extraParent.SetActive(true);
        PlaySound(restoredBeep, restoredVolume);
    }

    public void ShowFinalChoices()
    {
        finalChoicesParent.SetActive(true);

        Button[] buttons = finalChoicesParent.GetComponentsInChildren<Button>();

        foreach (Button button in buttons)
        {
            button.interactable = false;
        }
    }

    public void EnableFinalChoices()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        playerMovement.DisableMouseLook();

        Button[] buttons = finalChoicesParent.GetComponentsInChildren<Button>();

        foreach (Button button in buttons)
        {
            button.interactable = true;
        }
    }

    public void PrintDebug(string message)
    {
        Debug.Log(message);
    }

    private void PlaySound(AudioClip[] clips, float volume)
    {
        if (clips == null || clips.Length == 0 || source == null) return;

        AudioClip clip = clips[Random.Range(0, clips.Length)];

        if (clip == null) return;

        source.pitch = Random.Range(0.95f, 1.05f);
        source.spatialBlend = 0f;
        source.rolloffMode = AudioRolloffMode.Logarithmic;
        source.dopplerLevel = 0f;

        source.PlayOneShot(clip, volume);
    }
}
