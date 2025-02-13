using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneCube : MonoBehaviour
{
    [SerializeField] private GameObject _panel;
    [SerializeField] private GameObject _gfx;
    [SerializeField] private Collider _collider;
    [SerializeField] private Animator _cubeAnim;
    [SerializeField] private ParticleSystem _spawnCubePart;
    [SerializeField] private ParticleSystem _destroyRockPart;
    [SerializeField] private ParticleSystem _hitCubePart;
    private bool _active = false;   
    void Start()
    {
        _spawnCubePart.Play();

    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && _active == false)
        {
            _hitCubePart.Play();
            _active = true;
            _collider.isTrigger  = true;
            _cubeAnim.SetTrigger("Trigger");
            _panel.SetActive(true);
            StartCoroutine(DesactivePanel());
        }
    }
    
    private IEnumerator DesactivePanel()
    {
        
        yield return new WaitForSeconds(0.5f);
        _destroyRockPart.Play();
        _gfx.GetComponent<Renderer>().enabled = false;
        yield return new WaitForSeconds(3);
        _collider.isTrigger = false;

        _panel.SetActive(false);
        _gfx.GetComponent<Renderer>().enabled = true;
        _spawnCubePart.Play();
        _active = false;
    }
}
