using UnityEngine;

public class TriggerNPCMove : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ProtagMovement protagMovement;

    private bool hasTriggered = false;
    private void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered)
        {
            if (other.CompareTag("Player"))
            {
                protagMovement.MoveToNextPoint();
                Debug.Log("Moving Protag to next point");
                hasTriggered = true;
            }
        }
    }
}
