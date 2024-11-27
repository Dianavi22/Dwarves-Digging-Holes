using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerActions : MonoBehaviour
{
    [SerializeField] private float throwForce;
    [SerializeField] private float pickupRange;
    [SerializeField] private Transform _scale;
    [SerializeField] private LayerMask layerHitBaseAction;
    [SerializeField] private Transform slotInventoriaObject;

    [HideInInspector] public GameObject heldObject;
    public bool IsHoldingObject => heldObject != null;
    private Tween rotationTween;

    [HideInInspector] public bool IsBaseActionActivated = false;
    private float _lastCheckBaseAction;

    public GameObject pivot;

    [HideInInspector] public float vertical;

    private bool isTaunt = false;

    private Player _p;

    private void Awake()
    {
        _p = GetComponent<Player>();
    }

    private void Start()
    {
        _lastCheckBaseAction = Time.time;
    }

    private void Update()
    {
        if (IsBaseActionActivated && CheckHitRaycast(out var hits))
        {
            // Pickaxe
            if (IsHoldingObject && heldObject.TryGetComponent<Pickaxe>(out var pickaxe) 
                && Time.time - _lastCheckBaseAction >= GameManager.Instance.Difficulty.MiningSpeed 
                && _p.GetFatigue().ReduceMiningFatigue(GameManager.Instance.Difficulty.PlayerMiningFatigue))
            {
                pickaxe.Hit(hits.Last().gameObject);
                _lastCheckBaseAction = Time.time;
            }
        }
    }

    #region EVENTS 
    // Appel� lorsque le bouton de ramassage/lancer est press�
    public void OnCatch(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && !_p.IsCarried && !UIPauseManager.Instance.isPaused)
        {
            if (IsHoldingObject)
                ThrowObject();
            else
                TryPickUpObject();
        }

        // The grab for the goldchariot is kept while the button is pressed
        if (context.canceled && IsHoldingObject && heldObject.TryGetComponent<GoldChariot>(out var goldChariot)) //the key has been released
        {
            _p.EmptyPlayerFixedJoin();
            EmptyHands();
        }
    }

    public void OnTaunt(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && !_p.IsCarried && !UIPauseManager.Instance.isPaused)
        {
            if (isTaunt) return;

            StartCoroutine(Taunt());
        }
    }

    public void OnBaseAction(InputAction.CallbackContext context)
    {
        if (UIPauseManager.Instance.isPaused) return;
        if (context.performed) // the key has been pressed
        {
            IsBaseActionActivated = true;
            if(IsHoldingObject && heldObject.TryGetComponent<Pickaxe>(out _)) StartAnimation();
        }

        if (context.canceled) //the key has been released
        {
            IsBaseActionActivated = false;
            StopAnimation();
        }
    }
    #endregion

    // Method to start the tween, connected to the Unity Event when key is pressed
    public void StartAnimation()
    {
        // Determine the target tween angle based on the current pivot angle
        float targetAngle;
        if (Mathf.Approximately(pivot.transform.localEulerAngles.z, 325f))
        {
            targetAngle = -75f;
        }
        else if (Mathf.Approximately(pivot.transform.localEulerAngles.z, 0f))
        {
            targetAngle = 40f;
        }
        else if (Mathf.Approximately(pivot.transform.localEulerAngles.z, 35f))
        {
            targetAngle = 75f;
        }
        else
        {
            // Default to 40 if pivot is not exactly -35, 0, or 35
            targetAngle = 40f;
        }
        rotationTween = pivot.transform.DOLocalRotate(new Vector3(0, 0, targetAngle), 0.2f, RotateMode.Fast)
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

    private bool CheckHitRaycast(out List<Collider> hits)
    {
        Vector3 rayDirection;
        float distance = 1.7f;

        switch (vertical)
        {
            case 1: // UP case
                rayDirection = new Vector3(0, 2f, 0) + (-transform.right);
                break;

            case 0: // BASE case
                rayDirection = -transform.right;
                break;

            case -1: // DOWN case
                rayDirection = new Vector3(0, -2f, 0) + (-transform.right);
                break;

            default:
                // If the pivot angle is not relevant, exit early
                hits = new List<Collider>();
                return false;
        }

        // Perform the raycast
        // ! You can hit further forward
        hits = Utils.ConeRayCast(transform.position, rayDirection, 45f, distance, 10, layerHitBaseAction);
        return hits.Count > 0;
    }

    #region Handle Grab Item
    public void TryPickUpObject()
    {
        // Can't pickup item if the player already has one
        if (IsHoldingObject) return;

        GoldChariot chariot = null;

        // Detect object arround the player
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, pickupRange);
        foreach (var hitCollider in hitColliders)
        {
            // Can't pickup an other item if the player already has one
            // It also prevent if there are 2 object near the player that can be picked
            if (IsHoldingObject) return;
            GameObject parentGameobject = Utils.GetCollisionGameObject(hitCollider);

            if (Utils.TryGetParentComponent<Player>(parentGameobject, out var player))
            {
                if (player.GetActions().IsHoldingObject) continue;
                parentGameobject = player.gameObject;
            }
            // V�rifie que l'objet est �tiquet� comme "Throwable" ou "Player"
            if (parentGameobject != null && (parentGameobject.CompareTag("Throwable") || parentGameobject.CompareTag("Player")) && !parentGameobject.Equals(gameObject))
            {
                PickupObject(parentGameobject);
                break;
            }

            if (Utils.TryGetParentComponent<GoldChariot>(parentGameobject, out var testchariot)) chariot = testchariot;
        }

        // With this logic, we let priority on actual object that the player can grab. If nothing else is found, then the player can grab the chariot
        if (chariot != null && !IsHoldingObject)
        {
            heldObject = chariot.gameObject;
            _p.CreatePlayerFixedJoin(chariot.GetComponent<Rigidbody>());
        }
    }
    public void PickupObject(GameObject _object)
    {
        heldObject = _object;

        SetObjectInHand(heldObject, true);
        heldObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
    }

    // <summary>
    // Set In State as carried
    // </summary>
    // <param name="obj"></param>
    // <param name="state"></param>
    // <param name="forced"></param>
    private void SetObjectInHand(GameObject obj, bool isGrabbed, bool forced = false)
    {
        if (obj.TryGetComponent<Renderer>(out var objRenderer))
        {
            objRenderer.enabled = !isGrabbed;
        }

        if (obj.CompareTag("Player") || obj.CompareTag("Throwable"))
        {
            StopAnimation();
            CancelInvoke();
            Collider[] colliders = obj.GetComponentsInChildren<Collider>();
            for (int i = 0; i < colliders.Length; i++)
            {
                colliders[i].enabled = !isGrabbed;
            }
        }
        else if (Utils.TryGetParentComponent<Collider>(obj, out var objCollider) && !objCollider.isTrigger)
        {
            objCollider.enabled = !isGrabbed;
        }

        if (obj.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.isKinematic = isGrabbed;

            rb.collisionDetectionMode = isGrabbed ? CollisionDetectionMode.Continuous : CollisionDetectionMode.Discrete ;
            if (forced)
            {
                rb.AddForce(transform.up * (throwForce * 0.5f), ForceMode.Impulse);
            }
            else if (!isGrabbed)
            {
                float pivotAngle = Mathf.Clamp(pivot.transform.localEulerAngles.z, -45f, 45f);
                if (pivotAngle > 180) pivotAngle -= 360;
                pivotAngle = Mathf.Clamp(pivotAngle, -35f, 35f);

                float launchAngle = Mathf.Lerp(70f, 20f, (pivotAngle + 35f) / 70f);
                float radians = pivotAngle * Mathf.Deg2Rad;
                Vector3 throwDirection = (-transform.right * Mathf.Cos(radians)) + (transform.up * Mathf.Sin(radians));
                rb.gameObject.transform.rotation = Quaternion.identity;
                rb.AddForce(throwDirection * throwForce, ForceMode.Impulse);
            }
        }

        // Grabbable Object
        if (obj.TryGetComponent<IGrabbable>(out var grabbable))
        {
            grabbable.HandleCarriedState(_p, isGrabbed);
        }

        obj.transform.SetParent(isGrabbed ? slotInventoriaObject : null);
    }

    public void ForceDetach()
    {
        ThrowObject(true);
    }

    public void EmptyHands()
    {
        heldObject = null;
    }

    private void ThrowObject(bool forced = false)
    {
        if (!IsHoldingObject || GameManager.Instance.isGameOver) return;

        SetObjectInHand(heldObject, false, forced);
        EmptyHands();
    }
    #endregion

    private IEnumerator Taunt()
    {
        isTaunt = true;
        _scale.localScale = new Vector3(_scale.localScale.x, _scale.localScale.y - 0.3f, _scale.localScale.z);
        yield return new WaitForSeconds(0.2f);
        _scale.localScale = new Vector3(_scale.localScale.x, _scale.localScale.y + 0.3f, _scale.localScale.z);
        isTaunt = false;
    }
}
