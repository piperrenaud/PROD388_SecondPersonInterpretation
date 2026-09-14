using UnityEngine;

public class MoveableObject : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float holdDistance = 2f;
    [SerializeField] private float surfaceOffset = 0.05f;

    public float HoldDistance => holdDistance;
    public float SurfaceOffset => surfaceOffset;    
}
