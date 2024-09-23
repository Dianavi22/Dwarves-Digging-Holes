using UnityEngine;

public class Lava : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.gameObject.transform.root.GetComponent<PlayerHealth>().TakeDamage();
        }

        if (other.CompareTag("EndingCondition"))
        {
            GameManager.Instance.GameOver();
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
