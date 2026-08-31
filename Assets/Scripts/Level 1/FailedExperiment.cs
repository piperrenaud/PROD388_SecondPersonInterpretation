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
        SceneManager.LoadScene(nextScene);
    }
}
