using UnityEngine;

public class PushChariot : MonoBehaviour
{
    [SerializeField] private float pushForce = 150;
    [SerializeField] private Rigidbody chariotRigidbody;

    private bool _isTriggerActive;
    private PlayerFatigue _playerFatigue;


    private void FixedUpdate()
    {
        if (!_isTriggerActive || _playerFatigue == null) return;

        if (_playerFatigue.ReduceCartsFatigue(10f * Time.deltaTime))
        {
            chariotRigidbody.AddForce(pushForce, 0, 0, ForceMode.Impulse);
            Debug.Log(chariotRigidbody.velocity);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Utils.TryGetParentComponent<PlayerActions>(other, out var player) && !player.isHoldingObject)
        {
            _isTriggerActive = true;
            _playerFatigue = player.playerFatigue;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (Utils.TryGetParentComponent<PlayerActions>(other, out _))
        {
            _isTriggerActive = false;
            _playerFatigue = null;
        }
    }

}
