using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerActions : MonoBehaviour
{
    [SerializeField] private Transform objectSlot; // Emplacement où placer l'objet tenu
    [SerializeField] private float throwForce = 500f; // Force de lancer
    [SerializeField] private float pickupRange = 0.1f; // Portée pour ramasser un objet

    private GameObject heldObject; // Référence à l'objet en cours de manipulation
    private bool isHoldingObject = false; // Booléen pour savoir si un objet est tenu

    public bool carried = false;

    // Appelé lorsque le bouton de ramassage/lancer est pressé
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

    // Tente de ramasser un objet à portée
    private void TryPickUpObject()
    {
        // Détection des objets à portée autour du joueur
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, pickupRange);

        foreach (var hitCollider in hitColliders)
        {
            // Vérifie que l'objet est étiqueté comme "Throwable" ou "Player"
            if ((hitCollider.CompareTag("Throwable") || hitCollider.CompareTag("Player")) && !hitCollider.gameObject.Equals(gameObject))
            {
                heldObject = hitCollider.gameObject;

                if (heldObject != null)
                {
                    SetObjectState(heldObject, false);
                    heldObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                    isHoldingObject = true;
                    Debug.Log("CATCH");
                    break;

                }
            }
        }
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

            if(state && !forced)
            {
                float radians = 35f * Mathf.Deg2Rad;
                Vector3 throwDirection = (-transform.right * Mathf.Cos(radians)) + (transform.up * Mathf.Sin(radians));
                float force = throwForce * (obj.CompareTag("Player") ? 1.5f : 1);
                rb.AddForce(throwDirection * force, ForceMode.Impulse);
            }

            if(forced)
            {
                rb.AddForce(transform.up * (throwForce * 0.25f), ForceMode.Impulse);
            }
        }

        if (obj.TryGetComponent<PlayerMovements>(out var objPlayerMovements))
        {
            objPlayerMovements.forceDetachFunction = ForceDetachPlayer;
            if (state)
            {
                DOVirtual.DelayedCall(1f, () =>
                {
                    objPlayerMovements.carried = !state;
                });
            }
            else
            {
                objPlayerMovements.carried = !state;
            }
        }

        if (obj.TryGetComponent<PlayerActions>(out var objPlayerActions))
        {
            objPlayerActions.carried = !state;
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



    // Visualiser la portée de ramassage dans l'éditeur
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }
}
