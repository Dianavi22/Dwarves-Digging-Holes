using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyPart : MonoBehaviour
{

    [SerializeField] ParticleSystem _particleSystem;
    void Start()
    {
        StartCoroutine(DestroyParticule());
    }

    private IEnumerator DestroyParticule()
    {
        yield return new WaitForSeconds(_particleSystem.main.duration + 0.5f);
        Destroy(this.gameObject);
    }

}
