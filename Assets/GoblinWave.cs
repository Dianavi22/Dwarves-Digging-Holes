using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GoblinWave : MonoBehaviour
{

    [SerializeField] GameObject _gob;
    [SerializeField] GameObject _spawn;
    public bool isWave;
    private void OnTriggerEnter(Collider other)
    {
        if (Utils.Component.TryGetInParent<Rock>(other, out var rock) && isWave)
        {
            Destroy(rock.gameObject);
        }
    }

    private void Wave() {
        for (int i = 0; i < 7; i++)
        {
            Instantiate(_gob, _spawn.transform.position, Quaternion.identity);
        }
    }

    private void Update()
    {
        if (isWave)
        {
            isWave = false;
            Wave();
        }
    }

}
