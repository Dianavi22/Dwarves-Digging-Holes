using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class GoldToChariot : MonoBehaviour
{
    [SerializeField] GoldChariot _goldChariot;
    [SerializeField] float _speed;
    [SerializeField] Vector3 _direction;
    void Start()
    {
        _goldChariot = TargetManager.Instance.GetGameObject<GoldChariot>(Target.GoldChariot);
        StartCoroutine(GoldPosition());
    }

    void Update()
    {
        transform.LookAt(_goldChariot.transform);
        transform.position += ( _direction - transform.position).normalized * _speed * Time.deltaTime;
        transform.LookAt(_goldChariot.transform, Vector3.left);
    }

    private IEnumerator GoldPosition() {
        _direction = new Vector3(Random.Range(this.transform.position.x-100, this.transform.position.x + 100), Random.Range(this.transform.position.y - 100, this.transform.position.y + 100), 0 );
        yield return new WaitForSeconds(0.1f);
        _direction = new Vector3(_goldChariot.transform.position.x, _goldChariot.transform.position.y, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Utils.Component.TryGetInParent<GoldChariot>(other, out var pickaxe))
        {
            Destroy(this.gameObject);
        }
    }
}
