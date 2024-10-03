using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetRock : MonoBehaviour
{

    [SerializeField] List<int> positions;
    [SerializeField] List<Material> rocksMaterials;
    [SerializeField] GameObject _gfx;
    [SerializeField] MeshRenderer _meshRenderer;
    
    void Start()
    {
        _meshRenderer.material = rocksMaterials[Random.Range(0, rocksMaterials.Count - 1)];
        _gfx.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, positions[Random.Range(0, positions.Count - 1)]));
    }
}
