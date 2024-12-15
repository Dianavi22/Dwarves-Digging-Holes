using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnRock : MonoBehaviour
{
    [SerializeField] GameObject _rock;
    private bool _readyToSpawn = true;
    void Start()
    {
        
    }

    void Update()
    {
        if (_readyToSpawn)
        {
            StartCoroutine(SpawnRocks());
        }
    }

    private IEnumerator SpawnRocks()
    {
        _readyToSpawn = false;
        yield return new WaitForSeconds(Random.Range(3,5));
        Instantiate(_rock, transform);
        _readyToSpawn = true;
    }

}
