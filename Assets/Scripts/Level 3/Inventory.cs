using UnityEngine;

public class Inventory : MonoBehaviour
{
    [Header("Has items?")]
    [SerializeField] private bool door5Key = false;
    [SerializeField] private bool cabinetKey = false;
    [SerializeField] private bool door3Key = false;

    public bool HasDoor5Key => door5Key;
    public bool HasCabinetKey => cabinetKey;
    public bool HasDoor3Key => door3Key;

    public void ObtainedItem(string item, bool obtained)
    {
        if (item == "door5Key") door5Key = obtained;
        if (item == "cabinetKey") cabinetKey = obtained;
        if (item == "door3Key") door3Key = obtained;
    }

    public bool HasItem(string item)
    {
        if (item == "door5Key")
        {
            if (door5Key) return true;
        }

        if (item == "cabinetKey")
        {
            if (cabinetKey) return true;
        }

        if (item == "door3Key")
        {
            if (door3Key) return true;
        }

        return false;
    }
}
