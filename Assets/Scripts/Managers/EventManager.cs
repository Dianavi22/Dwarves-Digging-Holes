using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    private bool _readyToEvent = false;
    private GoldChariot _goldChariot;
    public float scrollSpeed;
    [SerializeField] GameObject _lava;
    private bool _isLavaMove = false;
    private bool _isLavaMoveEndEvent = false;
    private Vector3 _lavaPosition;
    [SerializeField] ParticleSystem _showTextPart;
    [SerializeField] TMP_Text _eventText;

    void Start()
    {
        _goldChariot = TargetManager.Instance.GetGameObject<GoldChariot>(Target.GoldChariot);
        Invoke("LaunchEvent", 10);
    }

    private void LaunchEvent()
    {
        _readyToEvent = true;
    }

    void Update()
    {
        if (_readyToEvent && !GameManager.Instance.isDisableEventManager)
        {
            print("EventCoroutine");
            StartCoroutine(Event());
        }
        if (_isLavaMove)
        {
            _lava.transform.position = Vector3.Lerp(_lava.transform.position, new Vector3(_lava.transform.position.x + 4, _lava.transform.position.y, _lava.transform.position.z), Time.deltaTime * 0.5f);
        }
        if (_isLavaMoveEndEvent)
        {
            _lava.transform.position = Vector3.Lerp(_lava.transform.position, new Vector3(_lava.transform.position.x - 4, _lava.transform.position.y, _lava.transform.position.z), Time.deltaTime * scrollSpeed / 2);
            if (_lava.transform.position.x <= _lavaPosition.x)
            {
                _isLavaMoveEndEvent = false;
            }
        }
    }

    private IEnumerator TextEvent(string message)
    {
        _eventText.text = "";
        _showTextPart.Play();
        yield return new WaitForSeconds(0.2f);
        _eventText.gameObject.SetActive(true);
        _eventText.text = message;
        yield return new WaitForSeconds(1);
        _eventText.gameObject.SetActive(false);

    }

    private IEnumerator Event()
    {
        _readyToEvent = false;
        yield return new WaitForSeconds(70); 
        ChooseEvent(Random.Range(0, 5));
        yield return new WaitForSeconds(30);
        _readyToEvent = true;
    }

    //Event 1 : All pickaxes are Destroyed
    //Event 2 : if gold is > 10 : gold/2
    //Event 3 : Lava getting close
    //Event 4 : Too many goblins

    private void ChooseEvent(int i)
    {
        print("ChooseEvent");

        if (i == 0)
        {
            EventPickaxe();
        }
        else if (i == 1)
        {

            EventGoldChariot();
        }
        else if (i == 2)
        {
            StartCoroutine(LavaGettingClose());
        }
        else
        {
            //
        }

    }

    private void EventPickaxe()
    {
        StartCoroutine(TextEvent("PICKAXES !!"));
        var _pickaxeInScene = FindObjectsOfType<Pickaxe>();
        for (int i = 0; i < _pickaxeInScene.Length; i++)
        {
            _pickaxeInScene[i].HandleDestroy();
        }
    }

    private void EventGoldChariot()
    {
        StartCoroutine(TextEvent("GOOOLD !!"));
        _goldChariot.GoldEvent();
    }

    private IEnumerator LavaGettingClose()
    {
        print("LAVAEvent");

        StartCoroutine(TextEvent("LAVAAA !!"));

        _lavaPosition = _lava.transform.position;
        _isLavaMove = true;
        yield return new WaitForSeconds(4.5f);
        _isLavaMove = false;
        yield return new WaitForSeconds(4.5f);
        _isLavaMoveEndEvent = true;


    }
}
