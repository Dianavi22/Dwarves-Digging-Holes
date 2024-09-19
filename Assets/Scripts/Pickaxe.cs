using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickaxe : MonoBehaviour
{
    [SerializeField]
    private int _healthPoint = 20;

    public void Hit(GameObject hit) {
        Debug.Log($"Hit {hit.name}");

        //^ Currently, it breaks whatever it touch
        //if(hit.TryGetComponent<Rock>(out rock)){}
        _healthPoint -= 1;
        Debug.Log(_healthPoint);
    }

}
