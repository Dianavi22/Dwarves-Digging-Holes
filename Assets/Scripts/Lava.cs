using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lava : MonoBehaviour
{
    private GameManager _gameManager;
    void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
    }

    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.gameObject.GetComponentInParent<Dwarf>().gameObject.GetComponentInParent<PlayerHealth>().TakeDamage();

        }
        if (other.CompareTag("EndingCondition"))
        {
            _gameManager.GameOver();
        }

        if (other.CompareTag("Rock"))
        {
            Destroy(other.gameObject);

        }
        if (other.CompareTag("Enemy"))
        {
            Destroy(other.gameObject.GetComponentInParent<Enemy>().gameObject);
        }
        if (other.CompareTag("Pickaxe"))
        {
            print("PICKAXE IN A FUCKING LAVA");
            Destroy(other.gameObject);

        }
    }
}
