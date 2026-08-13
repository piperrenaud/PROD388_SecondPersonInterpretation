using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class CameraSwitcher : MonoBehaviour
{
    [Header("Cameras")]
    [SerializeField] private Camera[] cameras;
    [SerializeField] private TMP_Text cameraText;

    private int currentCameraIndex = 0;

    private void Start()
    {
        if (cameras == null || cameras.Length == 0)
        {
            Debug.LogWarning("CameraSwitcher: no cameras assigned");
            return;
        }

        SetActiveCamera(currentCameraIndex);
    }

    private void Update()
    {
        if (Keyboard.current == null) return;
        
        if (Keyboard.current.aKey.wasPressedThisFrame)
        {
            SwitchCamera(-1);
        }

        if (Keyboard.current.dKey.wasPressedThisFrame)
        {
            SwitchCamera(1);
        }
    }

    private void SwitchCamera(int direction)
    {
        currentCameraIndex += direction;

        //wrap around
        if (currentCameraIndex < 0) currentCameraIndex = cameras.Length - 1;
        if (currentCameraIndex >= cameras.Length) currentCameraIndex = 0;

        SetActiveCamera(currentCameraIndex);
    }

    private void SetActiveCamera(int index)
    {
        for (int i = 0; i < cameras.Length; i++)
        {
            cameras[i].gameObject.SetActive(i == index);
        }

        cameraText.SetText("CAM " + (currentCameraIndex + 1));
    }
}
