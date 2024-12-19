using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using Utils;

public class EventManager : MonoBehaviour
{
    private bool _readyToEvent = false;
    private GoldChariot _goldChariot;
    private Lava _lava;
    private Vector3 _lavaOldPosition;
    private ShakyCame _sc;
    public float rocksHealth;
    public float rocksWithGoldHealth;
    private bool _isLavaMove = false;
    private bool _isLavaMoveEndEvent = false;
    [SerializeField] ParticleSystem _showTextPart;
    [SerializeField] TMP_Text _eventText;

    [SerializeField] List<MeshRenderer> _pickaxesModels;
    [SerializeField] List<ParticleSystem> _pickaxesPart;

    [SerializeField] ParticleSystem _lavaPartUI;
    [SerializeField] ParticleSystem _lavaRain;
    [SerializeField] ParticleSystem _goldChariotPart;
    [SerializeField] ParticleSystem _goldChariotUIPart;

    [SerializeField] GoblinWave _goblinWave;

    public bool  isRockEvent = false;

    void Start()
    {
        _goldChariot = TargetManager.Instance.GetGameObject<GoldChariot>();
        _sc = TargetManager.Instance.GetGameObject<ShakyCame>();
        _lava = TargetManager.Instance.GetGameObject<Lava>();
    }

    public void LaunchEvent()
    {
        _readyToEvent = true;
    }

    void Update()
    {
        if (_readyToEvent && !GameManager.Instance.isDisableEventManager)
        {
            StartCoroutine(Event());
        }
        if (_isLavaMove)
        {
            _lava.transform.position = Vector3.Lerp(_lava.transform.position, new Vector3(_lava.transform.position.x + 4, _lava.transform.position.y, _lava.transform.position.z), Time.deltaTime * 0.5f);
        }
        if (_isLavaMoveEndEvent)
        {
            _lava.transform.position = Vector3.Lerp(_lava.transform.position, new Vector3(_lava.transform.position.x - 4, _lava.transform.position.y, _lava.transform.position.z), Time.deltaTime * GameManager.Instance.CurrentScrollingSpeed / 2);
            if (_lava.transform.position.x <= _lavaOldPosition.x)
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
        yield return new WaitForSeconds(1.5f);
        _eventText.gameObject.SetActive(false);

    }

    private IEnumerator Event()
    {
        _readyToEvent = false;
        yield return new WaitForSeconds(10);
        ChooseEvent(Random.Range(0, 4));
      //  ChooseEvent(1);
        yield return new WaitForSeconds(30);
        _readyToEvent = true;
    }

    //Event 1 : All pickaxes are Destroyed
    //Event 2 : if gold is > 10 : gold/2
    //Event 3 : Lava getting close
    //Event 4 : Too many goblins
    //Event 5 : Unbreackables Rocks

    private void ChooseEvent(int i)
    {
        if (i == 0)
        {
            StartCoroutine(EventPickaxe());
        }
        else if (i == 1)
        {
            StartCoroutine(EventGoldChariot());
        }
        else if (i == 2)
        {
            StartCoroutine(LavaGettingClose());

        }
        else if (i == 3)
        {
            StartCoroutine(GoblinWave());
        }
       
        else
        {
            //
        }

    }

    private IEnumerator EventPickaxe()
    {
        StartCoroutine(TextEvent(StringManager.Instance.GetSentence(Message.PickaxeEvent)));
        yield return new WaitForSeconds(0.2f);
        for (int i = 0; i < _pickaxesModels.Count; i++)
        {
            _pickaxesModels[i].enabled = true;
        }
        yield return new WaitForSeconds(1.5f);
        for (int i = 0; i < _pickaxesModels.Count; i++)
        {
            _pickaxesModels[i].enabled = false;
        }
        for (int i = 0; i < _pickaxesPart.Count; i++)
        {
            _pickaxesPart[i].Play();
        }
        yield return new WaitForSeconds(2);
        var _pickaxeInScene = FindObjectsOfType<Pickaxe>();
        for (int i = 0; i < _pickaxeInScene.Length; i++)
        {
            _pickaxeInScene[i].HandleDestroy();
        }
        _sc.ShakyCameCustom(0.2f, 0.2f);
    }

    private IEnumerator EventGoldChariot()
    {
        StartCoroutine(TextEvent(StringManager.Instance.GetSentence(Message.TaxeEvent)));
        yield return new WaitForSeconds(0.5f);
        _goldChariotUIPart.Play();
        yield return new WaitForSeconds(1.5f);
        print(_goldChariot.transform.position);
        _goldChariotPart.Play();
        _sc.ShakyCameCustom(0.3f, 0.2f);
        _goldChariot.GoldEvent();

    }

    private IEnumerator LavaGettingClose()
    {
        StartCoroutine(TextEvent(StringManager.Instance.GetSentence(Message.LavaEvent)));
        _lavaPartUI.Play();
        _lavaRain.Play();
        _sc.ShakyCameCustom(0.2f, 0.2f);
        yield return new WaitForSeconds(2);
        _sc.ShakyCameCustom(4f, 0.2f);
        _lavaOldPosition = _lava.transform.position;
        _isLavaMove = true;
        yield return new WaitForSeconds(4.5f);
        _isLavaMove = false;
        _lavaRain.Stop();
        yield return new WaitForSeconds(4.5f);
        _isLavaMoveEndEvent = true;
    }

    private IEnumerator GoblinWave()
    {
        StartCoroutine(TextEvent(StringManager.Instance.GetSentence(Message.GoblinEvent)));
        yield return new WaitForSeconds(1);
        _sc.ShakyCameCustom(0.3f, 0.2f);
        _goblinWave.isWave = true;

    }

    private IEnumerator DurabilityRocks()
    {
        StartCoroutine(TextEvent("Durability Rocks"));
        yield return new WaitForSeconds(1);
        _sc.ShakyCameCustom(0.3f, 0.2f);
        isRockEvent = true;
        yield return new WaitForSeconds(10);
        isRockEvent = false;


    }
}
