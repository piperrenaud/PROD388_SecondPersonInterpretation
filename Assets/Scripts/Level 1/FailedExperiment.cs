using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FailedExperiment : MonoBehaviour
{ 
    [Header("Animation")]
    [SerializeField] private ParticleSystem particles;
    [SerializeField] private AudioSource electricityAudio;
    [SerializeField] private CameraShake cameraShake;
    [SerializeField] private AudioSource rumblingAudio;
    [SerializeField] private AudioSource collapseAudio;
    [SerializeField] private AudioSource otherElectrical;

    [Header("Post Scene Dialogue")]
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private string[] lines;
    [SerializeField] private AudioSource audioToKeepPlaying;
    [SerializeField] private GameObject mechanicalArm;

    [Header("Scene")]
    [SerializeField] private string nextScene;

    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void StartParticles()
    {
        particles.Play();
    }

    public void StopParticles()
    {
        particles.Stop();
    }

    public void PlayElectricitySound()
    {
        electricityAudio.Play();
    }

    public void ShakingCamera()
    {
        cameraShake.StartShake();
    }
    
    public void StartRumbling()
    {
        rumblingAudio.Play();
    }

    public void StopRumbling()
    {
        rumblingAudio.Stop();
    }

    public void StartCollapseAudio()
    {
        collapseAudio.Play();
    }

    public void TurnOffOtherElectrical()
    {
        otherElectrical.Stop();
    }

    public void AnimationFinished()
    {
        StartCoroutine(EndingDialogue());
    }

    private IEnumerator EndingDialogue()
    {
        DisableAllAudioExceptOne();

        yield return new WaitForSeconds(2f);

        foreach (var line in lines)
        {
            dialogueManager.SetDialogue(line);
            yield return new WaitUntil(() => !dialogueManager.IsDialogueRunning);
        }

        yield return new WaitForSeconds(2f);

        SceneManager.LoadScene(nextScene);
    }

    private void DisableAllAudioExceptOne()
    {
        AudioSource[] allAudioSources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);

        foreach (AudioSource source in allAudioSources)
        {
            if (source != audioToKeepPlaying)
            {
                source.Stop();
                source.enabled = false;
            }
        }

        mechanicalArm.SetActive(false);

        audioToKeepPlaying.enabled = true;
    }
}
