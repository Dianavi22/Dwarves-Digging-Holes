using UnityEngine;
using FMODUnity;

public class GoldToChariot : MonoBehaviour
{
    [SerializeField] float _speed;
    [SerializeField] Vector3 _direction;
    private GoldChariot _goldChariot;
    private ParticleSystem _takeGoldPart;
    private GameObject _pointOneGoldDirection;
    private Score _score;
    [SerializeField] private int _goldScore = 1;
    private bool _taken = false;

    [SerializeField] private EventReference pickUpANuggetSound;
    [SerializeField] private EventReference nuggetInTheCartSound;


    void Start()
    {
        _score = TargetManager.Instance.GetGameObject<Score>();
        _takeGoldPart = GameObject.Find("TakeGoldInChariot_PART").GetComponent<ParticleSystem>();
        _goldChariot = TargetManager.Instance.GetGameObject<GoldChariot>();
        _pointOneGoldDirection = _goldChariot.hbTakeGold;

        PickUpANuggetSoundSound();
    }

    void Update()
    {
        if (_pointOneGoldDirection != null)
        {
            transform.LookAt(_pointOneGoldDirection.transform);
            Vector3 directionToTarget = (_pointOneGoldDirection.transform.position - transform.position).normalized;
            transform.position += directionToTarget * _speed * Time.deltaTime;
        }
    }
       
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "PointOneGoldDirection" && !_taken)
        {
            NuggetInTheCartSoundSound();
            
            _taken = true;
            _goldChariot.TakeNugget();
            _score.ScoreCounter += _goldScore;
            _takeGoldPart.Play();
            _goldChariot.StartAnimation();
            Destroy(this.gameObject);
        }
    }

    #region Sounds
    private void PickUpANuggetSoundSound()
    {
        RuntimeManager.PlayOneShot(pickUpANuggetSound, transform.position);
    }

    private void NuggetInTheCartSoundSound()
    {
        RuntimeManager.PlayOneShot(nuggetInTheCartSound, transform.position);
    }
    #endregion
 
}
