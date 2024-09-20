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

        if (collision.collider.CompareTag("Enemy"))
        {
            var enemy = collision.collider.transform.parent.GetComponent<Enemy>();
            Destroy(enemy.gameObject);
        }

        if (collision.collider.CompareTag("Rock"))
        {
            Destroy(collision.collider.gameObject);
        }

        if (collision.collider.GetComponent<GoldChariot>())
        {
            _gameManager.GameOver();
        }
    }

}
