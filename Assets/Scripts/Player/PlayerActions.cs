using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerActions : Player
{
    [SerializeField] private float throwForce;
    [SerializeField] private float pickupRange;
    [SerializeField] private float updateCheckBaseAction;
    [SerializeField] private GameObject forward;
    [SerializeField] private Transform _scale;
    [SerializeField] ParticleSystem _HurtPart;

    public GameObject heldObject;
    public bool IsHoldingObject => heldObject != null;
    private Tween rotationTween;

    private bool isBaseActionToggle = false;
    private float _lastCheckBaseAction;

    private bool _isHit = false;

    public bool carried = false;
    public Transform objectSlot;
    public GameObject pivot;

    public LayerMask layerMask;

    public float vertical;

    private bool isTaunt = false;

    private void Start()
    {
        _lastCheckBaseAction = Time.time;
    }

    private void Update()
    {
        if (isBaseActionToggle && Time.time - _lastCheckBaseAction >= updateCheckBaseAction && CheckHitRaycast(out var hits))
        {
            // Pickaxe
            if (heldObject.TryGetComponent<Pickaxe>(out var pickaxe))
            {
                if (fatigue.ReduceMiningFatigue(10))
                {
                    pickaxe.Hit(hits.First().collider.gameObject);
                }
            }
        }
    }

    public void Hit()
    {
        if (_isHit) return;

        _HurtPart.Play();
        _isHit = true;
        rb.constraints = RigidbodyConstraints.FreezeAll;

        DOVirtual.DelayedCall(1f, () =>
        {
            rb.constraints &= ~(RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY);
            _isHit = false;
        });
    }

    #region EVENTS 
    // Appel� lorsque le bouton de ramassage/lancer est press�
    public void OnCatch(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && !carried && !UIPauseManager.Instance.isPaused)
        {
            if (IsHoldingObject)
                ThrowObject();
            else
                TryPickUpObject();
        }
    }

    public void OnTaunt(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && !carried && !UIPauseManager.Instance.isPaused)
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
            isBaseActionToggle = true;
            StartAnimation();
        }

        if (context.canceled) //the key has been released
        {
            isBaseActionToggle = false;
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

    private bool CheckHitRaycast(out List<RaycastHit> hits)
    {
        Vector3 rayDirection;
        float distance = 1.6f;

        switch (vertical)
        {
            case 1: // UP case
                rayDirection = transform.up;
                break;

            case 0: // BASE case
                rayDirection = -transform.right;
                break;

            case -1: // DOWN case
                rayDirection = -transform.up;
                break;

            default:
                // If the pivot angle is not relevant, exit early
                hits = new List<RaycastHit>();
                return false;
        }

        // Perform the raycast
        // ! You can hit further forward
        hits = CastConeRay(transform.position, rayDirection, 90f, distance, 10);
        return hits.Count > 0;
    }

    List<RaycastHit> CastConeRay(Vector3 origin, Vector3 direction, float angle, float maxDistance, int numRays)
    {
        List<RaycastHit> allhits = new();
        Debug.DrawRay(origin, direction * maxDistance, Color.red);
        for (int i = 0; i < numRays; i++)
        {
            float currentAngle = Mathf.Lerp(-angle / 2, angle / 2, i / (float)(numRays - 1));
            Vector3 rayDirection = Quaternion.Euler(0, 0, currentAngle) * direction;

            if (Physics.Raycast(origin, rayDirection, out RaycastHit hit, maxDistance, layerMask) && hit.collider.transform.root.gameObject != gameObject.transform.root.gameObject)
            {
                Debug.Log(hit.collider.gameObject.name);
                Debug.DrawRay(origin, rayDirection * hit.distance, Color.green);
                allhits.Add(hit);
            }
            else
            {
                Debug.DrawRay(origin, rayDirection * maxDistance, Color.red);
            }
        }
        return allhits;
    }

    // Tente de ramasser un objet � port�e
    public void TryPickUpObject()
    {
        if (IsHoldingObject) return;
        // D�tection des objets � port�e autour du joueur
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, pickupRange);
        foreach (var hitCollider in hitColliders)
        {
            GameObject parentGameobject = Utils.GetCollisionGameObject(hitCollider);

            if (Utils.TryGetParentComponent<PlayerActions>(parentGameobject, out var player))
            {
                if (player.IsHoldingObject) continue;
                parentGameobject = player.gameObject;
            }
            // V�rifie que l'objet est �tiquet� comme "Throwable" ou "Player"
            if (parentGameobject != null && (parentGameobject.CompareTag("Throwable") || parentGameobject.CompareTag("Player")) && !parentGameobject.Equals(gameObject))
            {
                PickupObject(parentGameobject);
                break;
            }

            if (Utils.TryGetParentComponent<GoldChariot>(parentGameobject, out var chariot))
            {
                heldObject = chariot.gameObject;
                chariot.TryJoinPlayer(rb);
            }
        }
    }
    public void PickupObject(GameObject _object)
    {
        heldObject = _object;

        if (Utils.TryGetParentComponent<Enemy>(heldObject, out var enemy))
        {
            enemy.hasFocus = false;
            enemy.isGrabbed = true;
        }

        SetObjectState(heldObject, false);
        heldObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
    }

    // <summary>
    // Set In State as carried
    // </summary>
    // <param name="obj"></param>
    // <param name="state"></param>
    // <param name="forced"></param>
    private void SetObjectState(GameObject obj, bool state, bool forced = false)
    {
        if (obj.TryGetComponent<Renderer>(out var objRenderer))
        {
            objRenderer.enabled = state;
        }
        if (obj.CompareTag("Player") || obj.CompareTag("Throwable"))
        {
            Collider[] colliders = obj.GetComponentsInChildren<Collider>();
            for (int i = 0; i < colliders.Length; i++)
            {
                colliders[i].enabled = state;
            }
        }
        else
        {
            if (Utils.TryGetParentComponent<Collider>(obj, out var objCollider) && !objCollider.isTrigger)
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
                float pivotAngle = Mathf.Clamp(pivot.transform.localEulerAngles.z, -45f, 45f);
                if (pivotAngle > 180) pivotAngle -= 360;

                pivotAngle = Mathf.Clamp(pivotAngle, -35f, 35f);

                float launchAngle = Mathf.Lerp(70f, 20f, (pivotAngle + 35f) / 70f);
                float radians = pivotAngle * Mathf.Deg2Rad;
                Vector3 throwDirection = (-transform.right * Mathf.Cos(radians)) + (transform.up * Mathf.Sin(radians));
                float force = throwForce * (obj.CompareTag("Player") ? 1.5f : 1);
                rb.gameObject.transform.rotation = Quaternion.identity;
                rb.AddForce(throwDirection * force, ForceMode.Impulse);
            }

            if (forced)
            {
                rb.AddForce(transform.up * (throwForce * 0.5f), ForceMode.Impulse);
            }
        }

        // Player
        if (obj.TryGetComponent<Player>(out var obPlayer))
        {
            obPlayer.GetMovement().forceDetachFunction = ForceDetach;
            if (!state)
            {
                obPlayer.GetMovement().carried = !state;
            }
            else
            {
                DOVirtual.DelayedCall(0.25f, () => { obPlayer.GetMovement().canStopcarried = true; });
            }

            obPlayer.GetActions().carried = !state;
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
        
        //Pickaxe
        if (obj.TryGetComponent<Pickaxe>(out var pickaxe))
        {
            if (!state)
            {
                pickaxe.throwOnDestroy = () => { EmptyHands(); StopAnimation(); isBaseActionToggle = false; };
            }
            else
            {
                StopAnimation();
                isBaseActionToggle = false;
            }
        }
        obj.transform.SetParent(state ? null : objectSlot);
    }

    public void ForceDetach()
    {
        ThrowObject(true);
    }

    private void EmptyHands()
    {
        heldObject = null;
    }

    private void ThrowObject(bool forced = false)
    {
        if (!IsHoldingObject || GameManager.Instance.isGameOver) return;

        if (Utils.TryGetParentComponent<Enemy>(heldObject, out var enemy))
        {
            enemy.isGrabbed = false;
            enemy.transform.rotation = Quaternion.Euler(new Vector3(enemy.transform.rotation.x, enemy.transform.rotation.y, 0));
        }
        if (heldObject.TryGetComponent<GoldChariot>(out var goldChariot))
        {
            goldChariot.EmptyJoin();
            EmptyHands();
            return;
        }
        SetObjectState(heldObject, true, forced);
        EmptyHands();
    }

    private IEnumerator Taunt()
    {
        isTaunt = true;
        _scale.localScale = new Vector3(_scale.localScale.x, _scale.localScale.y - 0.3f, _scale.localScale.z);
        yield return new WaitForSeconds(0.2f);
        _scale.localScale = new Vector3(_scale.localScale.x, _scale.localScale.y + 0.3f, _scale.localScale.z);
        isTaunt = false;
    }
}
