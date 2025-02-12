using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneCube : MonoBehaviour
{
    [SerializeField] private GameObject _panel;
    private bool _active = false;   
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player") && _active == false)
        {
            _active = true;
            _panel.SetActive(true);
            StartCoroutine(DesactivePanel());
        }
    }

    private IEnumerator DesactivePanel()
    {
        yield return new WaitForSeconds(3);
        _panel.SetActive(false);
        _active = false;
    }
}
