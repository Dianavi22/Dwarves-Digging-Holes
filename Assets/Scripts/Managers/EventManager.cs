using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using Utils;

public class EventManager : MonoBehaviour
{

    [Header("Text Event")]
    [SerializeField] TMP_Text _eventText;
    [SerializeField] TMP_Text _littleText;
    [SerializeField] TypeSentence _ts;
    [SerializeField] ParticleSystem _showTextPart;

    [Header("Pickaxe Event")]
    // Display with the text event
    [SerializeField] List<MeshRenderer> _pickaxesModels;
    [SerializeField] List<ParticleSystem> _pickaxesPart;

    [Header("Lava Event")]
    [SerializeField] ParticleSystem _lavaPartUI;
    [SerializeField] ParticleSystem _lavaRain;

    [Header("Gold Event")]
    [SerializeField] ParticleSystem _goldChariotPart;
    [SerializeField] ParticleSystem _goldChariotUIPart;

    [Header("Goblin Event")]
    [SerializeField] GoblinWave _goblinWave;


    [SerializeField] ParticleSystem _forgeBrokenPart;
    [SerializeField] ParticleSystem _deleteForgePanelEventPart;
    [SerializeField] ParticleSystem _panelNoForgePart;
    [SerializeField] Animator _forgePanelAnimator;
    [SerializeField] Animator _littleTextAnim;

    [Header("Other")]
    public float rocksHealth;
    public float rocksWithGoldHealth;
    public bool isForgeEvent = false;

    public bool isRockEvent = false;

    private Lava _lava;
    private Vector3 _lavaOldPosition;
    private ShakyCame _sc;
    private GoldChariot _goldChariot;
    private bool _readyToEvent = false;
    private bool _isLavaMove = false;
    private bool _isLavaMoveEndEvent = false;
    [SerializeField] private GameObject _bubbleForgeBroken;
    [SerializeField] private GameObject _forgePanel;

    public static EventManager Instance; // A static reference to the GameManager instance
    void Awake()
    {
        if (Instance == null) // If there is no instance already
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
    }

    void Start()
    {
        _goldChariot = TargetManager.Instance.GetGameObject<GoldChariot>();
        _sc = TargetManager.Instance.GetGameObject<ShakyCame>();
        _lava = TargetManager.Instance.GetGameObject<Lava>();
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
    public void LaunchEvent()
    {
        _readyToEvent = true;
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
        //ChooseEvent(Random.Range(0, 5));
         ChooseEvent(4);
        yield return new WaitForSeconds(30);
        _readyToEvent = true;
    }

    //Event 1 : All pickaxes are Destroyed
    //Event 2 : if gold is > 10 : gold/2 (No fun)
    //Event 3 : Lava getting close
    //Event 4 : Too many goblins
    //Event 5 : Unbreackables Rocks
    private void ChooseEvent(int i)
    {
        switch (i)
        {
            case 0:
                StartCoroutine(EventPickaxe());
                break;
            case 3:
                StartCoroutine(EventGoldChariot());
                break;
            case 2:
                StartCoroutine(LavaGettingClose());
                break;
            case 1:
                StartCoroutine(GoblinWave());
                break;
            case 4:
                StartCoroutine(NoForge());
                break;
            default:
                break;
        }
    }

    #region Describe Event
    private IEnumerator EventPickaxe()
    {
        StartCoroutine(TextEvent(StringManager.Instance.GetSentence(Message.PickaxeEvent)));
        _ts.WriteMachinEffect("Pickaxes aren't what they used to be, elf work!", _littleText, 0.02f);

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
        _littleText.text = "";

    }

    private IEnumerator EventGoldChariot()
    {
        StartCoroutine(TextEvent(StringManager.Instance.GetSentence(Message.TaxeEvent)));
        _ts.WriteMachinEffect("All this gold... you make even the gods jealous!", _littleText, 0.02f);
        yield return new WaitForSeconds(1f);

        _goldChariotUIPart.Play();
        yield return new WaitForSeconds(1.5f);
        print(_goldChariot.transform.position);
        _goldChariotPart.Play();
        _sc.ShakyCameCustom(0.3f, 0.2f);
        _goldChariot.GoldEvent();
        _littleText.text = "";

    }

    private IEnumerator NoForge()
    {
        
        StartCoroutine(TextEvent(StringManager.Instance.GetSentence(Message.ForgeEvent)));
        _littleText.text = "";
        _littleText.text = "I knew I should have taken the warranty on this forge";
        _littleText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2);
        _forgePanel.SetActive(true);
        _forgeBrokenPart.Play();
        _bubbleForgeBroken.SetActive(true);
        isForgeEvent = true;
        _littleTextAnim.SetTrigger("OutLittleText");
        yield return new WaitForSeconds(0.20f);
        _sc.ShakyCameCustom(0.2f, 0.2f);
        _panelNoForgePart.Play();
        yield return new WaitForSeconds(5);
        _bubbleForgeBroken.SetActive(false);
        _forgePanelAnimator.SetTrigger("EndForgeEvent");
        _forgeBrokenPart.Stop();
        _littleText.gameObject.SetActive(false);

        isForgeEvent = false;
       

        yield return new WaitForSeconds(0.31f);
        _sc.ShakyCameCustom(0.2f, 0.2f);

        _deleteForgePanelEventPart.Play();
        _forgePanel.SetActive(false);


    }

    private IEnumerator LavaGettingClose()
    {
        StartCoroutine(TextEvent(StringManager.Instance.GetSentence(Message.LavaEvent)));
        _ts.WriteMachinEffect("Your legs may be short, but you can still cover 7.5m!", _littleText, 0.02f);

        _lavaPartUI.Play();
        _lavaRain.Play();
        _sc.ShakyCameCustom(0.2f, 0.2f);
        yield return new WaitForSeconds(2);
        _littleText.text = "";

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
        _ts.WriteMachinEffect("After not seeing their buddies come back, they show up in a gang!", _littleText, 0.02f);

        yield return new WaitForSeconds(2f);
        _sc.ShakyCameCustom(0.3f, 0.2f);
        _goblinWave.GenerateWave();
        _littleText.text = "";

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
    #endregion
}
