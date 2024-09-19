using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lava : MonoBehaviour
{
    void Start()
    {

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
            GameManager.Instance.GameOver();
        }
    }

}
