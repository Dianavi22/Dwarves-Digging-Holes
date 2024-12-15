using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnDynamite : MonoBehaviour
{
    [SerializeField] GameObject _dynamite;
    private GameObject _currentTnt;
    private void Start()
    {
        SpawnTnt();
    }
    public void SpawnTnt()
    {
        _currentTnt = Instantiate(_dynamite, transform);
        _currentTnt.GetComponent<Dynamite>().spawnPoint = this.gameObject;
    }
}
