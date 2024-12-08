using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class GoldToChariot : MonoBehaviour
{
    [SerializeField] Transform _goldChariot;
    [SerializeField] float _speed;
    void Start()
    {
        
    }

    void Update()
    {
        transform.LookAt(_goldChariot);
        transform.position += (_goldChariot.position - transform.position).normalized * _speed * Time.deltaTime;
        transform.LookAt(_goldChariot, Vector3.left);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Utils.Component.TryGetInParent<GoldChariot>(other, out var pickaxe))
        {
            Destroy(this.gameObject);
        }
    }
}
