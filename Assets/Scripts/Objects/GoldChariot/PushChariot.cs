using UnityEngine;

public class PushChariot : MonoBehaviour
{
    [SerializeField] private float pushForce = 150;
    [SerializeField] private Rigidbody rb;
    private bool _isTriggerActive;
    private PlayerFatigue _playerFatigue;

    [SerializeField] private ParticleSystem _lostGoldPart;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (!_isTriggerActive || _playerFatigue == null) return;

        if (_playerFatigue.ReduceCartsFatigue(10f * Time.deltaTime))
        {
            rb.AddForce(pushForce, 0, 0, ForceMode.Impulse);
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

    private void OnCollisionExit(Collision collision)
    {
        if (Utils.TryGetParentComponent<Enemy>(collision.collider, out var enemy))
        {
            print("FUCK");
            _lostGoldPart.Stop();
        }
    }
}
