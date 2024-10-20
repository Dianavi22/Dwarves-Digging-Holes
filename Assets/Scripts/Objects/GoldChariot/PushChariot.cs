using UnityEngine;

public class PushChariot : MonoBehaviour
{
    [SerializeField] private float pushForce = 150;
    [SerializeField] private Rigidbody chariotRigidbody;

    private bool _isTriggerActive;
    private Player _player;


    private void FixedUpdate()
    {
        if (!_isTriggerActive || _player == null) return;

        if (_player.GetFatigue().ReduceCartsFatigue(10f * Time.deltaTime))
        {
            chariotRigidbody.AddForce(pushForce, 0, 0, ForceMode.Impulse);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Utils.TryGetParentComponent<Player>(other, out var player) && !player.GetActions().isHoldingObject)
        {
            _isTriggerActive = true;
            _player = player;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (Utils.TryGetParentComponent<Player>(other, out _))
        {
            _isTriggerActive = false;
            _player = null;
        }
    }
}
