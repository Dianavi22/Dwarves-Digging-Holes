using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerActions : MonoBehaviour
{


    [SerializeField] private float throwForce = 500f;
    [SerializeField] private float pickupRange = 0.1f;
    [SerializeField] private GameObject forward;


    public GameObject heldObject;
    public bool isHoldingObject = false;
    private Tween rotationTween;

    private Pickaxe pickaxe1;
    private Rigidbody _rb;
    private bool _isHit = false;

    public bool carried = false;
    public Transform objectSlot;
    public GameObject pivot;

    public float vertical;

    [SerializeField] private Transform _scale;
    private bool isTaunt = false;
    public PlayerFatigue playerFatigue;

    [SerializeField] ParticleSystem _HurtPart;


    private UIPauseManager _uiManager;
    public bool GrabThrowJustPressed { get; private set; }
    public bool BaseActionJustPressed { get; private set; }
    public bool TauntJustPressed { get; private set; }

    private void Start()
    {
        playerFatigue = GetComponent<PlayerFatigue>();
        _rb = GetComponent<Rigidbody>();
    }

    public void PrepareAction() {
        StartAnimation();
        InvokeRepeating(nameof(TestMine), 0f, 0.5f);
    }

    public void Hit()
    {
        if(!_isHit)
        {
            _HurtPart.Play();
            _isHit = true;
            _rb.constraints = RigidbodyConstraints.FreezeAll;

            DOVirtual.DelayedCall(1f, () => {
                _rb.constraints &= ~(RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY);
                _isHit = false;
            });
        }
    }


    #region EVENTS 
    // Appel� lorsque le bouton de ramassage/lancer est press�
    public void OnCatch(InputAction.CallbackContext context)
    {

        GrabThrowJustPressed = UserInput.instance.GrabThrowJustPressed;

        if (context.phase == InputActionPhase.Started && !carried && !_uiManager.isPaused)
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

    public void OnTaunt(InputAction.CallbackContext context)
    {
        TauntJustPressed = UserInput.instance.TauntJustPressed;

        if (context.phase == InputActionPhase.Started && !carried && !_uiManager.isPaused)
        {
            if (!isTaunt)
            {
                StartCoroutine(Taunt());
            }
            else
            {
                return;
            }
        }
    }

    public void OnBaseAction(InputAction.CallbackContext context)
    {
        BaseActionJustPressed = UserInput.instance.BaseActionJustPressed;

        if (heldObject == null)
        {
            // TODO: Action avec autre chose
            return;
        }
        // Pickaxe
        if (heldObject.TryGetComponent<Pickaxe>(out var pickaxe))
        {
            if (true)
            {
                if (context.performed) // the key has been pressed
                {
                    PrepareAction();
                    pickaxe1 = pickaxe;
                }
                if (context.canceled) //the key has been released
                {
                    StopAnimation();
                    CancelInvoke(nameof(TestMine));
                }
            }

        }
    }

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

    private void TestMine()
    {
        Vector3 rayDirection;
        Color rayColor = Color.red;
        float distance = 1.4f;

        switch (vertical)
        {
            case 1: // UP case
                rayDirection = transform.up;
                break;

            case 0: // BASE case
                rayDirection = -transform.right;
                distance = 1.2f;
                break;

            case -1: // DOWN case
                rayDirection = -transform.up;
                break;

            default:
                // If the pivot angle is not relevant, exit early
                return;
        }

        // Draw the debug ray in the scene view
        Debug.DrawRay(forward.transform.position, rayDirection * distance, rayColor);

        // Perform the raycast
        // ! You can hit further forward
        if (Physics.Raycast(forward.transform.position, rayDirection, out RaycastHit hit, distance))
        {
            if (playerFatigue.ReduceMiningFatigue(10)){
                pickaxe1.Hit(hit.collider.gameObject);
                Debug.Log("Minage effectué !");
            }
        //playerFatigue.ReduceMiningFatigueOverTime();
        //pickaxe1.Hit(hit.collider.gameObject);
        //Debug.Log("Minage effectué !");
        }

    }

    // Tente de ramasser un objet � port�e
    public void TryPickUpObject()
    {

        if (!_uiManager.isPaused)
        {
            // D�tection des objets � port�e autour du joueur
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, pickupRange);
            foreach (var hitCollider in hitColliders)
            {
                GameObject parentGameobject = Utils.GetCollisionGameObject(hitCollider);
                // V�rifie que l'objet est �tiquet� comme "Throwable" ou "Player"
                if (parentGameobject != null && (parentGameobject.CompareTag("Throwable") || parentGameobject.CompareTag("Player")) && !parentGameobject.Equals(gameObject) && parentGameobject.name != "TauntHitBox")
                {
                    heldObject = parentGameobject.gameObject;

                    if (heldObject != null)
                    {
                        PickupObject(heldObject);
                        break;
                    }
                }
            }
        }

    }

    #endregion

    private void Awake()
    {
        _uiManager = FindObjectOfType<UIPauseManager>();
        if (!_uiManager)
        {
            Debug.Log("ERROR: _uiManager not found");
        }
    }

    public void PickupObject(GameObject heldObject)
    {
        if (Utils.TryGetParentComponent<Enemy>(heldObject, out var enemy))
        {
            enemy.hasFocus = false;
            enemy.isGrabbed = true;
        }
        
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
        else
        {
            Collider InChild = obj.GetComponentInChildren<Collider>();
            if (InChild != null)
            {
                InChild.enabled = state;
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
            objPlayerMovements.forceDetachFunction = ForceDetach;
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
        else if (obj.TryGetComponent<Pickaxe>(out var pickaxe))
        {
            if (!state)
            {
                pickaxe.throwOnDestroy = () => { EmptyHands(); StopAnimation(); CancelInvoke(nameof(TestMine)); };
            }
            else
            {
                StopAnimation();
                CancelInvoke(nameof(TestMine));
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
        isHoldingObject = false;
    }


    private void ThrowObject(bool forced = false)
    {
        if (heldObject != null)
        {
            if (Utils.TryGetParentComponent<Enemy>(heldObject, out var enemy))
            {
                enemy.isGrabbed = false;
            }
            SetObjectState(heldObject, true, forced);
            EmptyHands();
        }
    }

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.I)){
    //        if (!isTaunt)
    //        {
    //            StartCoroutine(Taunt());

    //        }
    //    }
    //}



    private IEnumerator Taunt()
    {
        isTaunt = true;
        _scale.localScale = new Vector3(_scale.localScale.x, _scale.localScale.y - 0.3f, _scale.localScale.z);
        yield return new WaitForSeconds(0.2f);
        _scale.localScale = new Vector3(_scale.localScale.x, _scale.localScale.y + 0.3f, _scale.localScale.z);
        isTaunt = false;
    }
}
