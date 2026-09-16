using UnityEngine;

public class Key : MonoBehaviour
{
    [Header("Key Body")]
    [SerializeField] private GameObject body;

    [Header("Door to unlock")]
    [SerializeField] private OpenDoor door;
    [SerializeField] private GameObject[] blockers;
    [SerializeField] private GameObject switchSidesTrigger;

    [Header("Audio")]
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip clip;

    public void PickUp()
    {
        body.SetActive(false);

        SetDoorUnlocked();
    }

    private void SetDoorUnlocked()
    {
        foreach (GameObject go in blockers)
        {
            go.SetActive(false);
        }

        switchSidesTrigger.SetActive(true);

        door.KeyPickedUp();

        source.PlayOneShot(clip);
    }
}
