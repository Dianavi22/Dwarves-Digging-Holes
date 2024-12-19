using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GoblinWave : MonoBehaviour
{
    [SerializeField] int _nbGoblin;
    [SerializeField] Enemy _gob;
    [SerializeField] Transform _spawnPoint;

    private List<Rock> _rocks = new();

    private void OnTriggerEnter(Collider other)
    {
        if (Utils.Component.TryGetInParent<Rock>(other, out var rock))
        {
            _rocks.Add(rock);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (Utils.Component.TryGetInParent<Rock>(other, out var rock))
        {
            _rocks.Remove(rock);
        }
    }

    public void GenerateWave()
    {
        for (int i = 0; i < _rocks.Count; i++)
        {
            if (_rocks[i].IsDestroyed()) _rocks.Remove(_rocks[i]);
            else Destroy(_rocks[i].gameObject);
        }

        for (int i = 0; i < _nbGoblin; i++)
        {
            Instantiate(_gob, _spawnPoint.position, Quaternion.identity);
        }
    }
}
