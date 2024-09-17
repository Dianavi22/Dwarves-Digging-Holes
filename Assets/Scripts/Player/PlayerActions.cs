using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerActions : MonoBehaviour
{
    [SerializeField] public Transform objectSlot;
    [SerializeField] private float throwForce = 500f;

    [SerializeField] public GameObject currentHeldObject;

    public GameObject heldObject;
    public bool isHoldingObject = false;

    void Start()
    {
    }

    public void OnCatch(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            if (isHoldingObject)
            {
                ThrowObject();
            }
            else
            {
                TryPickUpObject();
            }
        }
    }

    private void TryPickUpObject()
    {
        Collider[] hitColliders = UnityEngine.Physics.OverlapSphere(transform.position, 1f);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Throwable") || hitCollider.CompareTag("Player"))
            {
                heldObject = hitCollider.gameObject;
                if (heldObject != null)
                {
                    Renderer objRenderer = heldObject.GetComponent<Renderer>();
                    if (objRenderer != null)
                    {
                        objRenderer.enabled = false;
                    }

                    Collider objCollider = heldObject.GetComponent<Collider>();
                    if (objCollider != null)
                    {
                        objCollider.enabled = false;
                    }

                    Rigidbody rb = heldObject.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        rb.isKinematic = true;
                    }

                    heldObject.transform.SetParent(objectSlot);
                    heldObject.transform.localPosition = Vector3.zero;
                    heldObject.transform.localRotation = Quaternion.identity;

                    currentHeldObject = heldObject;
                    isHoldingObject = true;
                    print("CATCH");
                    return;
                }
            }
        }
    }

    private void ThrowObject()
    {
        if (heldObject != null)
        {
            Renderer objRenderer = heldObject.GetComponent<Renderer>();
            if (objRenderer != null)
            {
                objRenderer.enabled = true;
            }

            Collider objCollider = heldObject.GetComponent<Collider>();
            if (objCollider != null)
            {
                objCollider.enabled = true;
            }

            Rigidbody rb = heldObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.AddForce(transform.forward * throwForce);
            }

            heldObject.transform.SetParent(null);
            currentHeldObject = null; 
            heldObject = null;
            isHoldingObject = false;
            print("YET");
        }
    }
}
