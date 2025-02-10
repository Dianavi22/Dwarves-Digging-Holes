using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitGoldByChariot : MonoBehaviour
{
    [SerializeField] ParticleSystem _hitGold;

    private void Start()
    {
        _hitGold = GameObject.Find("PlayerHitGold_PART").GetComponent<ParticleSystem>();
    }
    public void HitByPlayer(Vector3 pos)
    {
        _hitGold.transform.position = pos;
        _hitGold.Play();
    }
}
