using UnityEngine;
using UnityEngine.Events;

public class TriggerNPCMove : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private UnityEvent onTriggered;

    private bool hasTriggered = false;
    private void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered)
        {
            if (other.CompareTag("Player"))
            {
                onTriggered.Invoke();
                hasTriggered = true;
            }
        }
    }
}
