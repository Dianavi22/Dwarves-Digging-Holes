using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetRock : MonoBehaviour
{

    [SerializeField] private GameObject _rock3DModel;
    [SerializeField]  List<int> positions = new List<int>();
    [SerializeField] List<Material> rocksMaterials = new List<Material>();
    [SerializeField] bool _isGoldRock;
    void Start()
    {

        if (!_isGoldRock)
        {
            _rock3DModel.gameObject.GetComponent<MeshRenderer>().material = rocksMaterials[Random.Range(0, rocksMaterials.Count - 1)];

        }
        _rock3DModel.gameObject.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, positions[Random.Range(0, positions.Count-1)]));
    }

    void Update()
    {
        
    }
}
