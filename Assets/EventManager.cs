using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    private bool _readyToEvent = true;
    [SerializeField] GoldChariot _goldChariot;
    [SerializeField] GameObject _lava;
    private bool _isLavaMove = false;
    private bool _isLavaMoveEndEvent = false;
    void Start()
    {
        
    }
    
    void Update()
    {
        if (_readyToEvent)
        {
            StartCoroutine(Event());
        }
        if (_isLavaMove)
        {
            _lava.transform.position = Vector3.Lerp(_lava.transform.position, new Vector3(_lava.transform.position.x + 5, _lava.transform.position.y, _lava.transform.position.z), Time.deltaTime * 0.5f);
        }
        if (_isLavaMoveEndEvent)
        {
            _lava.transform.position = Vector3.Lerp(_lava.transform.position, new Vector3(_lava.transform.position.x - 5, _lava.transform.position.y, _lava.transform.position.z), Time.deltaTime * 0.5f);
        }
    }

    private IEnumerator Event()
    {
        _readyToEvent = false;
        yield return new WaitForSeconds(3);
        StartCoroutine(LavaGetingClose());
        EventGoldChariot();
        yield return new WaitForSeconds(30);
        print("END EVENT");
        _readyToEvent = true;
    }

    //Event 1 : All pickaxes are Destroyed
    //Event 2 : if gold is > 10 : gold/2
    //Event 3 : Lava getting close
    //Event 4 : Too many goblins

    private void EventPickaxe()
    {
        var _pickaxeInScene = FindObjectsOfType<Pickaxe>();
        for (int i = 0; i < _pickaxeInScene.Length; i++)
        {
            _pickaxeInScene[i].HandleDestroy();
        }
    }

    private void EventGoldChariot()
    {
        _goldChariot.GoldEvent();
    }

    private IEnumerator LavaGetingClose()
    {
        _isLavaMove = true;
        yield return new WaitForSeconds(4.5f);
        _isLavaMove = false;
        yield return new WaitForSeconds(4.5f);
        _isLavaMoveEndEvent = true;
        yield return new WaitForSeconds(4.5f);
        _isLavaMoveEndEvent = false;


    }
}
