using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetRock : MonoBehaviour
{

    [SerializeField]  List<int> positions = new List<int>();
    [SerializeField] List<Material> rocksMaterials = new List<Material>();
    [SerializeField] bool _isGoldRock;
    [SerializeField] GameObject _gfx;
    [SerializeField] MeshRenderer _meshRenderer;
    
    void Start()
    {

        if (!_isGoldRock)
        {
            _meshRenderer.material = rocksMaterials[Random.Range(0, rocksMaterials.Count - 1)];
            _gfx.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, positions[Random.Range(0, positions.Count-1)]));
        }
    }

    void Update()
    {
        
    }
}
