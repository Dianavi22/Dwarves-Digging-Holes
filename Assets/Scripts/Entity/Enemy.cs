using System.Collections;
using DG.Tweening;
using UnityEditor.SearchService;
using UnityEngine;

public class Enemy : Entity
{
    [SerializeField] private float jumpForce = 0.5f;
    [SerializeField] public GameObject raycastDetectHitWall;

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
}