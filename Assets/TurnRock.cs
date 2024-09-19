using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnRock : MonoBehaviour
{

   [SerializeField] private GameObject _rock3DModel;
        List<int> positions = new List<int>();
    void Start()
    {
        positions.Add(0);
        positions.Add(90);
        positions.Add(180);
        positions.Add(270);

        _rock3DModel.gameObject.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, positions[Random.Range(0,3)]));

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
