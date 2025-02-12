using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneCube : MonoBehaviour
{
    [SerializeField] private GameObject _panel;
    [SerializeField] private Animator _cubeAnim;
    private bool _active = false;   
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && _active == false)
        {
            _active = true;
            _cubeAnim.SetTrigger("Trigger");
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
