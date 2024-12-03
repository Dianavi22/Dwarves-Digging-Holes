using System.Collections;
using DG.Tweening;
using UnityEditor.SearchService;
using UnityEngine;

public class Enemy : Entity
{
    [SerializeField] private float jumpForce = 0.5f;
    [SerializeField] public GameObject raycastDetectHitWall;
    [SerializeField] ParticleSystem _destroyGobPart;
    [SerializeField] GameObject _gfx;

    public GoldChariot _goldChariot;
    private bool _isTouchChariot;
    public bool canSteal = true;

    public bool IsTouchingChariot
    {
        get => _isTouchChariot;
        set
        {
            if (_isTouchChariot == value) return;
            if (value) _goldChariot.NbGoblin++;
            else _goldChariot.NbGoblin--;
            _isTouchChariot = value;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        _goldChariot = TargetManager.Instance.GetGameObject<GoldChariot>(Target.GoldChariot);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_goldChariot.gameObject.Equals(collision.gameObject))
        {
            if(!IsGrabbed){_rb.isKinematic = true;};
            IsTouchingChariot = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (_goldChariot.gameObject.Equals(collision.gameObject))
        {
            IsTouchingChariot = false;
            if(!IsGrabbed){_rb.isKinematic = false; };
        }
    }

    public IEnumerator HitChariot()
    {
        _goldChariot.GoldCount -= 1;
        canSteal = false;
        yield return new WaitForSeconds(1);
        canSteal = true;
    }

    public void KillGobs()
    {
        _gfx.SetActive(false);
        _rb.velocity = Vector3.zero;
    }

    public IEnumerator DestroyByLava()
    {
        _rb.velocity = Vector3.zero;
        TargetManager.Instance.GetGameObject<ShakyCame>(Target.ShakyCame).ShakyCameCustom(0.3f, 0.3f);
        _rb.isKinematic = true;
        _gfx.SetActive(false);
        _destroyGobPart.Play();
        yield return new WaitForSeconds(2);
        Destroy(this.gameObject);
    }
    public override void HandleCarriedState(Player player, bool grabbed) {
        if(grabbed) {
            IsTouchingChariot = false;
            base.HandleCarriedState(player, grabbed);
        }
        else {
            DOVirtual.DelayedCall(1f, () =>  base.HandleCarriedState(player, grabbed));
        }
        
        _rb.mass = grabbed ? 1f : 5f;
    } 
    override public void HandleDestroy()
    {
        StartCoroutine(DestroyByLava());

    }
}
