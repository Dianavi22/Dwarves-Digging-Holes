using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    private bool _readyToEvent = true;
    void Start()
    {
        
    }
    
    void Update()
    {
        if (_readyToEvent)
        {
          //  StartCoroutine(Event());
        }
    }

    private IEnumerator Event()
    {
        _readyToEvent = false;
        yield return new WaitForSeconds(3);
        print("EVENT");
        EventPickaxe();
        yield return new WaitForSeconds(3);
        print("END EVENT");
        _readyToEvent = true;
    }

    //Event 1 : All pickaxes are Destroyed
    //Event 2 : if gold is > 10 : gold/2
    //Event 3 : Lava getting cloth
    //Event 4 : Too many goblins

    private void EventPickaxe()
    {
        var _pickaxeInScene = FindObjectsOfType<Pickaxe>();
        for (int i = 0; i < _pickaxeInScene.Length; i++)
        {
            _pickaxeInScene[i].HandleDestroy();
        }
    }
}
