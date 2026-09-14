using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectMover : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera playerCam;

    [Header("Movement")]
    [SerializeField] private float moveSmoothness = 15f;
    [SerializeField] private float holdDistance = 2.5f;

    private MoveableObject heldObject;
    private Rigidbody heldRigidbody;

    private Quaternion originalRotation;
    private Collider[] heldColliders;

    private void Awake()
    {
        if (playerCam == null) playerCam = Camera.main;
    }

    private void Update()
    {
        if (Keyboard.current == null || playerCam == null) return;

        //start holding object
        if (Keyboard.current.eKey.wasPressedThisFrame && heldObject == null)
        {
            TryPickupObject();
        }

        //move object while e held
        if (heldObject != null)
        {
            if (Keyboard.current.eKey.isPressed)
            {
                MoveHeldObject();
            }
            else
            {
                DropObject();
            }
        }
    }

    private void TryPickupObject()
    {
        Ray ray = playerCam.ScreenPointToRay(
            new Vector3(Screen.width / 2f, Screen.height / 2f));

        if (!Physics.Raycast(ray, out RaycastHit hit, 100f)) return;

        MoveableObject movable = hit.collider.GetComponentInParent<MoveableObject>();

        if (movable == null) return;

        heldObject = movable;
        heldRigidbody = heldObject.GetComponent<Rigidbody>();

        originalRotation = heldObject.transform.rotation;

        //get all colliders on object
        heldColliders = heldObject.GetComponentsInChildren<Collider>();

        //ignore held objects colliders
        foreach (Collider collider in heldColliders)
        {
            collider.enabled = false;
        }

        //stop physics from fighting movement
        if (heldRigidbody != null)
        {
            heldRigidbody.linearVelocity = Vector3.zero;
            heldRigidbody.angularVelocity = Vector3.zero;
            heldRigidbody.isKinematic = true;
        }

        //move object infront of cam
        heldObject.transform.position =
            playerCam.transform.position +
            playerCam.transform.forward * holdDistance;

        heldObject.transform.rotation = originalRotation;
    }

    private void MoveHeldObject()
    {
        if (heldObject == null) return;

        //position directly infront of cam
        Vector3 targetPosition = 
            playerCam.transform.position + 
            playerCam.transform.forward * holdDistance;

        //follow cam
        heldObject.transform.position = Vector3.Lerp(
            heldObject.transform.position,
            targetPosition,
            moveSmoothness * Time.deltaTime );

        //keep original rotation
        heldObject.transform.rotation = originalRotation;
    }

    private void DropObject()
    {
        if (heldObject == null) return;

        //turn colliders back on
        if (heldColliders != null)
        {
            foreach (Collider col  in heldColliders)
            {
                if (col != null) col.enabled = true;
            }
        }

        //re enable physiocs
        if (heldRigidbody != null)
        {
            heldRigidbody.isKinematic = false;
        }

        heldObject = null;
        heldRigidbody = null;
        heldColliders = null;
    }
}
