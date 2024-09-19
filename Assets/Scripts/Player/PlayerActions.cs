using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using Unity.VisualScripting;

public class PlayerActions : MonoBehaviour
{
    [SerializeField] private float throwForce = 500f;
    [SerializeField] private float pickupRange = 0.1f;

    private GameObject heldObject;
    private bool isHoldingObject = false;
    private Tween rotationTween;

    private Pickaxe pickaxe1;
    public bool carried = false;
    public Transform objectSlot;
    public GameObject pivot;


    #region EVENTS 
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

    public void OnBaseAction(InputAction.CallbackContext context)
    {
        if (heldObject == null)
        {
            // TODO: Action avec autre chose
            return;
        }
        // Pickaxe
        if (heldObject.TryGetComponent<Pickaxe>(out var pickaxe))
        {
            if (context.performed) // the key has been pressed
            {
                //* Animation ONLY
                StartAnimation();
                InvokeRepeating(nameof(TestMine), 0.5f, 0.5f);
                pickaxe1 = pickaxe;
            }
            if (context.canceled) //the key has been released
            {
                StopAnimation();
                CancelInvoke(nameof(TestMine));
            }
        }
    }

    // Method to start the tween, connected to the Unity Event when key is pressed
    public void StartAnimation()
    {
        rotationTween = pivot.transform.DOLocalRotate(new Vector3(0, 0, 40), 0.2f, RotateMode.FastBeyond360)
            .SetEase(Ease.InOutQuad)
            .SetLoops(-1, LoopType.Yoyo);
    }

    // Method to stop the tween, connected to the Unity Event when key is released
    public void StopAnimation()
    {
        // Stop the tween if it is active
        if (rotationTween != null && rotationTween.IsActive())
        {
            rotationTween.Rewind();
            rotationTween.Kill();
        }
    }

    private void TestMine()
    {
        // Draw the ray in the Scene view
        Debug.DrawRay(transform.position, -transform.right * 1.4f, Color.red);

        // Perform the actual raycast
        if (Physics.Raycast(transform.position, -transform.right, out RaycastHit hit, 1.4f))
        {
            pickaxe1.Hit(hit.collider.gameObject);
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

    #endregion
    public void PickupObject(GameObject heldObject)
    {
        SetObjectState(heldObject, false);
        heldObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        isHoldingObject = true;
        Debug.Log("CATCH");
    }

    /// <summary>
    /// Set In State as carried
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="state"></param>
    /// <param name="forced"></param>
    private void SetObjectState(GameObject obj, bool state, bool forced = false)
    {
        if (obj.TryGetComponent<Renderer>(out var objRenderer))
        {
            objRenderer.enabled = state;
        }

        if (obj.TryGetComponent<Collider>(out var objCollider))
        {
            if (!objCollider.isTrigger)
            {
                objCollider.enabled = state;
            }
        }

        if (obj.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.isKinematic = !state;

            rb.collisionDetectionMode = state ? CollisionDetectionMode.Discrete : CollisionDetectionMode.Continuous;
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
            else
            {
                DOVirtual.DelayedCall(0.25f, () => { objPlayerMovements.canStopcarried = true; });
            }
            if (obj.TryGetComponent<PlayerActions>(out var objPlayerActions))
            {
                objPlayerActions.carried = !state;
            }
        }

        // Beer
        if (obj.TryGetComponent<Beer>(out var objBeer))
        {
            if (state)
            {
                objBeer.throwOnDestroy = null;
                objBeer.breakable = !state;
                DOVirtual.DelayedCall(0.5f, () =>
                {
                    objBeer.breakable = state;
                });
            }
            else
            {
                objBeer.throwOnDestroy = EmptyHands;
                objBeer.breakable = !state;
            }
        }

        obj.transform.SetParent(state ? null : objectSlot);
    }

    private void ForceDetachPlayer()
    {
        ThrowObject(true);
    }

    private void EmptyHands()
    {
        heldObject = null;
        isHoldingObject = false;
    }


    private void ThrowObject(bool forced = false)
    {
        if (heldObject != null)
        {
            SetObjectState(heldObject, true, forced);
            EmptyHands();
        }
    }
}
