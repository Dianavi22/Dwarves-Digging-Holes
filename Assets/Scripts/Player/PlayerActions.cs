using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class PlayerActions : MonoBehaviour
{
    public Transform objectSlot; // Emplacement o� placer l'objet tenu
    [SerializeField] private float throwForce = 500f; // Force de lancer
    [SerializeField] private float pickupRange = 0.1f; // Port�e pour ramasser un objet

    private GameObject heldObject; // R�f�rence � l'objet en cours de manipulation
    private bool isHoldingObject = false; // Bool�en pour savoir si un objet est tenu
    public bool carried = false;

    // Appel� lorsque le bouton de ramassage/lancer est press�
    public void OnCatch(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && !carried)
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

    // Tente de ramasser un objet � port�e
    private void TryPickUpObject()
    {
        // D�tection des objets � port�e autour du joueur
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, pickupRange);

        foreach (var hitCollider in hitColliders)
        {
            // V�rifie que l'objet est �tiquet� comme "Throwable" ou "Player"
            if ((hitCollider.CompareTag("Throwable") || hitCollider.CompareTag("Player")) && !hitCollider.gameObject.Equals(gameObject))
            {
                heldObject = hitCollider.gameObject;

                if (heldObject != null)
                {
                    PickupObject(heldObject);
                    break;

                }
            }
        }
    }

    public void PickupObject(GameObject heldObject)
    {
        SetObjectState(heldObject, false);
        heldObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        isHoldingObject = true;
        Debug.Log("CATCH");
    }

    private void SetObjectState(GameObject obj, bool state, bool forced = false)
    {
        if (obj.TryGetComponent<Renderer>(out var objRenderer))
        {
            objRenderer.enabled = state;
        }

        if (obj.TryGetComponent<Collider>(out var objCollider))
        {
            objCollider.enabled = state;
        }

        if (obj.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.isKinematic = !state;

            if (state && !forced)
            {
                float radians = 45f * Mathf.Deg2Rad;
                Vector3 throwDirection = (-transform.right * Mathf.Cos(radians)) + (transform.up * Mathf.Sin(radians));
                float force = throwForce * (obj.CompareTag("Player") ? 1.5f : 1);
                rb.AddForce(throwDirection * force, ForceMode.Impulse);
            }

            if (forced)
            {
                rb.AddForce(transform.up * (throwForce * 0.5f), ForceMode.Impulse);
            }
        }

        // Player
        if (obj.TryGetComponent<PlayerMovements>(out var objPlayerMovements))
        {
            objPlayerMovements.forceDetachFunction = ForceDetachPlayer;
            if (!state)
            {
                objPlayerMovements.carried = !state;
            }
            else {
                DOVirtual.DelayedCall(0.25f, () => {objPlayerMovements.canStopcarried = true;});
            }
            if (obj.TryGetComponent<PlayerActions>(out var objPlayerActions))
            {
                objPlayerActions.carried = !state;
            }
        }

        // Beer
        if(obj.TryGetComponent<Beer>(out var objBeer)) {
            if(state) {
                DOVirtual.DelayedCall(0.5f, () => {
                    objBeer.breakable = state;
                });
            }
            else {
                objBeer.breakable = state;
            }
        }
        
        heldObject.transform.SetParent(state ? null : objectSlot);
    }

    private void ForceDetachPlayer()
    {
        ThrowObject(true);
    }



    private void ThrowObject(bool forced = false)
    {
        if (heldObject != null)
        {
            SetObjectState(heldObject, true, forced);
            heldObject = null;
            isHoldingObject = false;
        }
    }



    // Visualiser la port�e de ramassage dans l'�diteur
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }
}
