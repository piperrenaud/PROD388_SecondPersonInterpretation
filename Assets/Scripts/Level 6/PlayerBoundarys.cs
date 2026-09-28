using UnityEngine;

public class PlayerBoundarys : MonoBehaviour
{
    [SerializeField] private GameObject boundaryDoor;

    private void Start()
    {
        boundaryDoor.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            boundaryDoor.SetActive(true);
        }
    }
}
