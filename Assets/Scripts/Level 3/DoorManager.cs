using UnityEngine;

public class DoorManager : MonoBehaviour
{
    public static DoorManager Instance;

    private MehcnaicalDoors currentOpenDoor;

    public MehcnaicalDoors CurrentOpenDoor => currentOpenDoor;

    private void Awake()
    {
        Instance = this;
    }

    public void SetOpenDoor(MehcnaicalDoors newDoor)
    {
        if (currentOpenDoor != null && currentOpenDoor != newDoor)
        {
            currentOpenDoor.CloseDoor(false);
        }

        currentOpenDoor = newDoor;

        CharacterDoorController character = FindFirstObjectByType<CharacterDoorController>();

        if (character != null)
        {
            character.CheckOpenDoor();
        }
    }

    public void ClearOpenDoor(MehcnaicalDoors door)
    {
        if (currentOpenDoor != door) return;

        currentOpenDoor = null;

        CharacterDoorController character = FindFirstObjectByType<CharacterDoorController>();

        if (character != null)
        {
            character.CheckOpenDoor();
        }
    }
}
