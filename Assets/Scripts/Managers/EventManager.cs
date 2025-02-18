using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using Utils;
using FMODUnity;

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
    [SerializeField] private EventReference eventPickaxeSound;
    [SerializeField] private EventReference EventPickaxeBreak;

    [Header("Lava Event")]
    [SerializeField] ParticleSystem _lavaPartUI;
    [SerializeField] ParticleSystem _lavaRain;
    [SerializeField] private EventReference eventGoldChariotSound;

    [Header("Gold Event")]
    [SerializeField] ParticleSystem _goldChariotPart;
    [SerializeField] ParticleSystem _goldChariotUIPart;
    [SerializeField] private EventReference lavaGettingCloseSound;
    [SerializeField] private EventReference lightningSound;

    [Header("Goblin Event")]
    [SerializeField] GoblinWave _goblinWave;
    [SerializeField] private EventReference goblinWaveSound;

    [SerializeField] private EventReference noForgeSound;


    [SerializeField] ParticleSystem _forgeBrokenPart;
    [SerializeField] ParticleSystem _deleteForgePanelEventPart;
    [SerializeField] ParticleSystem _panelNoForgePart;
    [SerializeField] ParticleSystem _goblinWaveSpawnPart;
    [SerializeField] Animator _forgePanelAnimator;
    [SerializeField] Animator _littleTextAnim;

    [Header("Other")]
    public float rocksHealth;
    public float rocksWithGoldHealth;
    public bool isForgeEvent = false;

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
        if (_readyToEvent && !GameManager.Instance.isDisableEventManager && !GameManager.Instance.isGameOver && !GameManager.Instance.isEnding)
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
        if (!GameManager.Instance.isDisableEventManager && !GameManager.Instance.isGameOver && !GameManager.Instance.isEnding)
        {
            _eventText.text = "";
            _showTextPart.Play();
            yield return new WaitForSeconds(0.2f);
            _eventText.gameObject.SetActive(true);
            _eventText.text = message;
            yield return new WaitForSeconds(1.5f);
            _eventText.gameObject.SetActive(false);
        }

    }

    private IEnumerator Event()
    {
        if (!GameManager.Instance.isDisableEventManager && !GameManager.Instance.isGameOver && !GameManager.Instance.isEnding)
        {
            _readyToEvent = false;
            yield return new WaitForSeconds(10);
            ChooseEvent(Random.Range(0, 5));
            //ChooseEvent(1);
            yield return new WaitForSeconds(30);
            _readyToEvent = true;
        }

    }

    //Event 1 : All pickaxes are Destroyed
    //Event 2 : if gold is > 10 : gold/2 (No fun)
    //Event 3 : Lava getting close
    //Event 4 : Too many goblins
    //Event 5 : Unbreackables Rocks
    private void ChooseEvent(int i)
    {
        if (!GameManager.Instance.isDisableEventManager && !GameManager.Instance.isGameOver && !GameManager.Instance.isEnding)
        {
            _littleText.text = "";
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
           
    }


    #region Describe Event
    private IEnumerator EventPickaxe()
    {
        StartCoroutine(TextEvent(StringManager.Instance.GetSentence(Message.PickaxeEvent)));
        _littleText.gameObject.SetActive(false);
        EventPickaxeSound();

        _littleText.text = StringManager.Instance.GetSentence(Message.PickaxeEventDesc);
        _littleText.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        for (int i = 0; i < _pickaxesModels.Count; i++)
        {
            _pickaxesModels[i].enabled = true;
        }
        yield return new WaitForSeconds(1.5f);
        _littleTextAnim.SetTrigger("OutLittleText");
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
        EventPickaxeBreakSound();
        _sc.ShakyCameCustom(0.2f, 0.2f);
    }

    private IEnumerator EventGoldChariot()
    {
        StartCoroutine(TextEvent(StringManager.Instance.GetSentence(Message.TaxEvent)));
        _littleText.gameObject.SetActive(false);
        EventGoldChariotSound();

        _littleText.text = StringManager.Instance.GetSentence(Message.TaxEventDesc);
        _littleText.gameObject.SetActive(true);

        yield return new WaitForSeconds(2f);
        _littleTextAnim.SetTrigger("OutLittleText");
        _goldChariotUIPart.Play();
        yield return new WaitForSeconds(1.5f);
        LightningSound();
        _goldChariotPart.Play();
        _sc.ShakyCameCustom(0.3f, 0.2f);
        _goldChariot.GoldEvent();
    }

    private IEnumerator NoForge()
    {

        StartCoroutine(TextEvent(StringManager.Instance.GetSentence(Message.ForgeEvent)));
        _littleText.gameObject.SetActive(false);
        NoForgeSound();

        _littleText.text = StringManager.Instance.GetSentence(Message.ForgeEventDesc);
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
        _littleText.gameObject.SetActive(false);
        LavaGettingCloseSound();

        _littleText.text = StringManager.Instance.GetSentence(Message.LavaEventDesc);
        _littleText.gameObject.SetActive(true);

        _lavaPartUI.Play();
        _lavaRain.Play();
        _sc.ShakyCameCustom(0.2f, 0.2f);
        yield return new WaitForSeconds(2);
        _littleTextAnim.SetTrigger("OutLittleText");


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
        _littleText.gameObject.SetActive(false);
        GoblinWaveSound();

        _littleText.text = StringManager.Instance.GetSentence(Message.GoblinEventDesc);
        _littleText.gameObject.SetActive(true);

        yield return new WaitForSeconds(2f);
        _littleTextAnim.SetTrigger("OutLittleText");
        _sc.ShakyCameCustom(0.3f, 0.2f);
        _goblinWaveSpawnPart.Play();
        yield return new WaitForSeconds(1f);

        _goblinWave.GenerateWave();
    }
    #endregion

    #region Sounds     eventPickaxeSound eventGoldChariotSound lavaGettingCloseSound goblinWaveSound noForgeSound   lightning
    private void EventPickaxeSound()
    {
        RuntimeManager.PlayOneShot(eventPickaxeSound, transform.position);
    }
    private void EventPickaxeBreakSound()
    {
        RuntimeManager.PlayOneShot(EventPickaxeBreak, transform.position);
    }
    private void EventGoldChariotSound()
    {
        RuntimeManager.PlayOneShot(eventGoldChariotSound, transform.position);
    }
    private void LightningSound()
    {
        RuntimeManager.PlayOneShot(lightningSound, transform.position);
    }
    private void LavaGettingCloseSound()
    {
        RuntimeManager.PlayOneShot(lavaGettingCloseSound, transform.position);
    }
    private void GoblinWaveSound()
    {
        RuntimeManager.PlayOneShot(goblinWaveSound, transform.position);
    }
    private void NoForgeSound()
    {
        RuntimeManager.PlayOneShot(noForgeSound, transform.position);
    }
    #endregion
}
