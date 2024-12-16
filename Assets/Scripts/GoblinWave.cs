using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GoblinWave : MonoBehaviour
{

    [SerializeField] GameObject _gob;
    [SerializeField] GameObject _spawn;
    private List<GameObject> _rocks;
    [HideInInspector] public bool isWave;
    private void OnTriggerEnter(Collider other)
    {

        if (Utils.Component.TryGetInParent<Rock>(other, out var rock))
        {
            try
            {
                _rocks.Add(other.gameObject);
            }
            catch
            {
                //
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        try
        {
            _rocks.Remove(other.gameObject);
        }
        catch
        {
            //
        }
    }

    private void Wave()
    {
        for (int i = 0; i < 7; i++)
        {
            Instantiate(_gob, _spawn.transform.position, Quaternion.identity);
        }
    }

    private void Update()
    {
        if (isWave)
        {
            for (int i = 0; i < _rocks.Count; i++)
            {
                try
                {
                    Destroy(_rocks[i].GetComponentInParent<Rock>().gameObject);
                }
                catch
                {
                    //
                }
            }
            isWave = false;
            Wave();
        }
    }

}
