using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using Utils;
using static UnityEngine.GraphicsBuffer;

public class GoldToChariot : MonoBehaviour
{
    [SerializeField] float _speed;
    [SerializeField] Vector3 _direction;
    private GoldChariot _goldChariot;
    private ParticleSystem _takeGoldPart;
    private GameObject _pointOneGoldDirection;
    private Score _score;
    [SerializeField] private int _goldScore = 1;



    void Start()
    {
        
        if (!GameManager.Instance.isInMainMenu)
        {
            _score = TargetManager.Instance.GetGameObject<Score>();
        }
        _takeGoldPart = GameObject.Find("TakeGoldInChariot_PART").GetComponent<ParticleSystem>();
        _goldChariot = TargetManager.Instance.GetGameObject<GoldChariot>();
        _pointOneGoldDirection = _goldChariot.hbTakeGold;
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
        if (other.gameObject.name == "PointOneGoldDirection")
        {
            BreakGold();
            _takeGoldPart.Play();
            TargetManager.Instance.GetGameObject<GoldChariot>().StartAnimation();
            Destroy(this.gameObject);
        }
    }

    public void BreakGold()
    {
        TargetManager.Instance.GetGameObject<GoldChariot>().TakeNugget();
        _score.ScoreCounter += _goldScore;
    }
}
