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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            collision.collider.gameObject.GetComponent<PlayerHealth>().TakeDamage();
        }

        //if (collision.collider.CompareTag("Enemy"))
        //{
        //    print("Collision");
        //    Destroy(collision.collider.gameObject);
        //}

        if (collision.collider.GetComponent<GoldChariot>())
        {
            _gameManager.GameOver();
        }
    }

}