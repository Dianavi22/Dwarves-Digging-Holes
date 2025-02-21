using DG.Tweening;
using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils;

public class PlayerActions : MonoBehaviour
{
    private Coroutine swingCoroutine;
    [SerializeField] private EventReference swingSound;
    [SerializeField] private EventReference forgeSound;

    [SerializeField] private float throwForce;
    [SerializeField] private float pickupRange;
    [SerializeField] private Transform _scale;
    [SerializeField] private LayerMask layerHitBaseAction;
    [SerializeField] private Transform slotInventoriaObject;
    [SerializeField] ParticleSystem _yeetPart;
    [SerializeField] ParticleSystem _pickaxeSpritePart;
    [SerializeField] ParticleSystem _chariotSpritePart;
    [SerializeField] private EventReference pickupSound;
    [SerializeField] private EventReference throwSound;
    //[SerializeField] private ParticleSystem _fatiguePart;

    private Tuto _tuto;
    [HideInInspector] public GameObject heldObject;
    public bool IsHoldingObject => heldObject != null;

    [HideInInspector] public bool IsBaseActionActivated = false;
    private float _lastCheckBaseAction;

    public GameObject pivot;

    [HideInInspector] public float vertical;

    private bool isTaunt = false;

    private Player _p;

    private Dictionary<int, int> previousLayer = new();

    private bool _isFirstCanPickup = true;

    private Tween buildingPickaxe;

    private Coroutine loadingCoroutuine = null;
    private Coroutine forgeSoundCoroutine;
    private bool isForging = false;


    private void Awake()
    {
        _p = GetComponent<Player>();
    }

    private void Start()
    {
        _lastCheckBaseAction = Time.time;
        if (GameManager.Instance.isInMainMenu) return;

        _tuto = TargetManager.Instance.GetGameObject<Tuto>();
    }

    private void Update()
    {
        if (IsBaseActionActivated && IsHoldingObject && CheckHitRaycast(out var hits))
        {
            // Pickaxe
            if (IsHoldingObject && heldObject.TryGetComponent<Pickaxe>(out var pickaxe)
                && Time.time - _lastCheckBaseAction >= GameManager.Instance.Difficulty.MiningSpeed
                && _p.GetFatigue().ReduceMiningFatigue(GameManager.Instance.Difficulty.MiningFatigue.ActionReducer))
            {
                pickaxe.Hit(hits.Last().gameObject);
                _lastCheckBaseAction = Time.time;
            }
        }
        else if (IsBaseActionActivated && !IsHoldingObject && buildingPickaxe == null)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, pickupRange, LayerMask.GetMask("Forge"));
            if (hitColliders.Any() && GameManager.Instance.CanCreatePickaxe)
            {
                Forge forge = hitColliders[0].GetComponent<Forge>();

                buildingPickaxe ??= DOTween.Sequence()
                    .AppendCallback(() => _p.GetPlayerMovements().isCreatingPickaxe = true)
                    .AppendCallback(() => StopCoroutine(loadingCoroutuine))
                    .AppendCallback(() =>
                        {
                            _p.GetAnimator().SetTrigger("forge");

                            loadingCoroutuine = StartCoroutine(forge.LoadPickaxe());

                            isForging = true;
                            forgeSoundCoroutine = StartCoroutine(PlayForgeSoundRepeatedly());
                        })
                    .AppendInterval(2f)
                    //.Append(gameObject.transform.DOLocalRotate(new Vector3(-180, 0, 0), 0.5f)
                    //.SetAutoKill(false))
                    //.AppendInterval(1f) // Wait for 1 second
                    //.Append(gameObject.transform.DOLocalRotate(Vector3.zero, 0.5f))
                    .AppendCallback(() =>
                    {
                        forge.BuildPickaxe();
                        isForging = false;
                    })
                    .OnKill(() => { if (loadingCoroutuine != null) StopCoroutine(loadingCoroutuine); gameObject.transform.DOLocalRotate(Vector3.zero, 0f); loadingCoroutuine = StartCoroutine(forge.LoadPickaxe(true)); if (forgeSoundCoroutine != null) { StopCoroutine(forgeSoundCoroutine); forgeSoundCoroutine = null; } isForging = false; _p.GetPlayerMovements().isCreatingPickaxe = false;});
            }
        }

        if (_isFirstCanPickup && GameManager.Instance.isInMainMenu || GameManager.Instance.passTuto || _tuto.startTuto)
        {
            _isFirstCanPickup = false;
        }
    }

    #region EVENTS 
    // Appel� lorsque le bouton de ramassage/lancer est press�
    public void OnCatch(InputAction.CallbackContext context)
    {
        if (IsHoldingObject && _p.IsGrabbed)
        {
            _yeetPart.Play();
            print("Handle - " + gameObject.name);
        }

        if (!_p.CanDoAnything()) return;

        if (context.phase == InputActionPhase.Started && !_p.IsGrabbed)
        {
            if (IsHoldingObject)
            {
                _yeetPart.Play();
                _p.GetActions().StopAnimation();
                _p.GetActions().CancelInvoke();
                ThrowObject();
            }
            else
                TryPickUpObject();
        }

        // The grab for the goldchariot is kept while the button is pressed
        if (context.canceled && IsHoldingObject) //the key has been released
        {
            _p.GetActions().StopAnimation();
            _p.GetActions().CancelInvoke();
            ThrowObject();
        }
    }

    public void OnTaunt(InputAction.CallbackContext context)
    {
        if (!_p.CanDoAnything()) return;

        _p.GetAnimator().SetTrigger("taunt");

        /*if (context.phase == InputActionPhase.Started && !_p.IsGrabbed)
        {
            if (isTaunt) return;
        
            StartCoroutine(Taunt());
        }*/
    }

    public void OnTauntLeft(InputAction.CallbackContext context)
    {
        if (!_p.CanDoAnything()) return;
        _p.GetAnimator().SetTrigger("tauntLeft");
        _pickaxeSpritePart.Play();
    }

    public void OnTauntRight(InputAction.CallbackContext context)
    {
        if (!_p.CanDoAnything()) return;
        _p.GetAnimator().SetTrigger("tauntRight");
        _chariotSpritePart.Play();

    }

    public void OnPassTuto(InputAction.CallbackContext context)
    {
        if (!_p.CanDoAnything()) return;

        if (_tuto.isInTuto)
        {
            if (_tuto.isYeetEnemy)
            {
                _tuto.StopTuto();
            }
            else
            {
                GameManager.Instance.SkipTuto();
                _tuto.StopTuto();
            }
        }
        else
        {
            GameManager.Instance.passTuto = true;
        }
    }

    public void OnBaseAction(InputAction.CallbackContext context)
    {
        if (!_p.CanDoAnything()) return;

        if (context.performed) // the key has been pressed
        {
            IsBaseActionActivated = true;
            if (IsHoldingObject && heldObject.TryGetComponent<Pickaxe>(out _))
            {
                StartAnimation();
            }
        }

        if (context.canceled) //the key has been released
        {
            IsBaseActionActivated = false;
            if (buildingPickaxe != null)
            {
                buildingPickaxe.Kill();
                buildingPickaxe = null;
            }

            StopAnimation();
        }

    }
    #endregion

    // Method to start the tween, connected to the Unity Event when key is pressed
    public void StartAnimation()
    {
        _p.GetAnimator().SetBool("pickaxeHit", true);

        if (swingCoroutine == null)
        {
            swingCoroutine = StartCoroutine(WooshLoop());
        }
    }

    // Method to stop the tween, connected to the Unity Event when key is released
    public void StopAnimation()
    {
        _p.GetAnimator().SetBool("pickaxeHit", false);

        if (swingCoroutine != null)
        {
            StopCoroutine(swingCoroutine);
            swingCoroutine = null;
        }

    }

    private IEnumerator WooshLoop()
    {
        while (IsBaseActionActivated)
        {
            yield return new WaitForSeconds(GameManager.Instance.Difficulty.MiningSpeed);

            if (!CheckHitRaycast(out var hits) || hits.Count == 0)
            {
                SwingSound();
            }
        }
        swingCoroutine = null;
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
        hits = DRayCast.Cone(transform.position, rayDirection, 45f, distance, 10, layerHitBaseAction);
        return hits.Count > 0;
    }

    #region Handle Grab Item
    public void TryPickUpObject()
    {
        // Can't pickup item if the player already has one
        if (IsHoldingObject || _p.IsDead) return;

        // Detect object arround the player
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, pickupRange);

        Collider mostImportant = hitColliders
            .Where(collider => Utils.Component.TryGetInParent<IGrabbable>(collider, out var a) && !a.Equals(GetComponent<IGrabbable>()))
            //.Where(collider => GetPriority(collider) > 0)
            .OrderByDescending(collider => GetPriority(collider))
            .ThenBy(collider => Vector3.Distance(transform.position, collider.transform.position))
            .FirstOrDefault();

        if (mostImportant == null) return;

        if (Utils.Component.TryGetInParent<Player>(mostImportant, out var player))
        {
            if (player.GetActions().IsHoldingObject) return;
            PickupObject(player.gameObject);
        }
        else if (Utils.Component.TryGetInParent<GoldChariot>(mostImportant, out var chariot))
        {
            if (transform.position.y > chariot.transform.position.y + 0.75f) return;
            if (chariot.HandleCarriedState(_p, true))
            {
                heldObject = chariot.gameObject;
                _p.CreateFixedJoin(chariot.GetComponent<Rigidbody>());
            }
        }
        else PickupObject(Utils.Component.GetInParent<IGrabbable>(mostImportant).GetGameObject());
    }
    public void PickupObject(GameObject _object)
    {
        if (!SetObjectInHand(_object, true)) return;

        heldObject = _object;
        heldObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

        if (GameManager.Instance.isInMainMenu) return;

        //Tuto
        if (heldObject.TryGetComponent<Pickaxe>(out _) && _tuto.startTuto)
        {
            _tuto.isBreakRock = true;
        }

        if (heldObject.TryGetComponent<Enemy>(out _) && _tuto.isTakeEnemy)
        {
            _tuto.isYeetEnemy = true;
        }
    }

    // <summary>
    // Set In State as carried
    // </summary>
    // <param name="obj"></param>
    // <param name="state"></param>
    // <param name="forced"></param>
    private bool SetObjectInHand(GameObject obj, bool isGrabbed, bool forced = false)
    {
        bool hasPickupObject = false;
        if (obj.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.isKinematic = isGrabbed;
            rb.collisionDetectionMode = isGrabbed ? CollisionDetectionMode.Continuous : CollisionDetectionMode.Discrete;

            if (forced)
            {
                rb.AddForce(transform.up * (throwForce * 0.25f), ForceMode.Impulse);
                rb.gameObject.transform.rotation = Quaternion.identity;
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
            LayerHandler(obj);
            hasPickupObject = grabbable.HandleCarriedState(_p, isGrabbed);
        }

        RuntimeManager.PlayOneShot(isGrabbed ? pickupSound : throwSound, transform.position);

        if (obj.TryGetComponent<Pickaxe>(out var pickaxe))
            obj.transform.SetParent(isGrabbed ? _p.GetModelRef().GetPickaxeSlot() : null);
        else
            obj.transform.SetParent(isGrabbed ? slotInventoriaObject : null);

        return hasPickupObject;
    }

    private void LayerHandler(GameObject obj)
    {
        int instanceId = obj.GetInstanceID();
        int newLayer;

        if (previousLayer.TryGetValue(instanceId, out int layer))
        {
            previousLayer.Remove(instanceId);
            newLayer = layer;
        }
        else
        {
            previousLayer.Add(instanceId, obj.layer);
            newLayer = 10; //Grabbed Layer
        }
        Layer.SetNewLayerObject(obj, newLayer);
    }

    private int GetPriority(Collider collider)
    {
        if (Utils.Component.TryGetInParent<Pickaxe>(collider, out _))
            return 5;
        if (Utils.Component.TryGetInParent<Enemy>(collider, out _))
            return 4;
        if (Utils.Component.TryGetInParent<GoldChariot>(collider, out _))
            return 3;
        if (collider.TryGetComponent<Player>(out _))
            return 2; // Player
        return 1;
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

        bool canThrowObject = false;

        if (heldObject.TryGetComponent<GoldChariot>(out var chariot))
        {
            _p.EmptyFixedJoin();
            canThrowObject = chariot.HandleCarriedState(_p, false);
        }
        else
        {
            canThrowObject = SetObjectInHand(heldObject, false, forced);
        }

        //if (canThrowObject)
        EmptyHands();
    }
    #endregion

    public IEnumerator Taunt()
    {
        isTaunt = true;
        _scale.localScale = new Vector3(_scale.localScale.x, _scale.localScale.y - 0.3f, _scale.localScale.z);
        yield return new WaitForSeconds(0.2f);
        _scale.localScale = new Vector3(_scale.localScale.x, _scale.localScale.y + 0.3f, _scale.localScale.z);
        isTaunt = false;
    }

    #region Sound forgeEvent
    private void SwingSound()
    {
        RuntimeManager.PlayOneShot(swingSound, transform.position);
    }

    private void ForgeSound()
    {
        RuntimeManager.PlayOneShot(forgeSound, transform.position);
    }

    private IEnumerator PlayForgeSoundRepeatedly()
    {
        while (isForging)
        {
            ForgeSound();
            yield return new WaitForSeconds(0.5f);
        }
    }


    #endregion
}
