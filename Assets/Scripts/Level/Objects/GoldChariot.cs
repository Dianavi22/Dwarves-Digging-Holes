using FMOD.Studio;
using FMODUnity;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Utils;
using DG.Tweening;
using System.Linq;
using System;

public class GoldChariot : MonoBehaviour, IGrabbable
{
    [Header("Sound effect")]
    [SerializeField] private EventReference chariotSound;
    [SerializeField] private EventReference chariotSound2;
    private EventInstance _chariotEventInstance;
    private EventInstance _chariotEventInstance2;
    private bool _isSoundPlaying = false;
    private float currentVolume = 0f;
    private float targetVolume = 0f;
    [SerializeField] private float fadeTime = 0.2f;


    [SerializeField] private TMP_Text _goldCountText;
    [SerializeField] private ParticleSystem _lostGoldPart;
    [SerializeField] private ParticleSystem _sparksPart;

    public ParticleSystem oneLostPart;
    [SerializeField] GameObject _gfx;
    [SerializeField] List<GameObject> _goldEtages;

    private List<Sequence> _nearDeathExperienceSequence = new();

    private Rigidbody _rb;
    [SerializeField] EventManager _eventManager;
    private bool _isPlayed = false;


    private int _nbGolbinOnChariot;
    public int NbGoblin
    {
        get => _nbGolbinOnChariot;
        set
        {
            _nbGolbinOnChariot = value;
            //  UpdateParticle();
        }
    }

    public int goldLostValue;

    private int _currentGoldCount = 10;
    public int GoldCount
    {
        get => _currentGoldCount;
        set
        {
            _currentGoldCount = Math.Max(0, value);
            UpdateText();
        }
    }
    public ParticleSystem GetParticleLostGold() => _lostGoldPart;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        ChariotSound();


        if (Vector3.Distance(transform.position, TargetManager.Instance.GetGameObject<Lava>().transform.position) - 4 < 5 || GoldCount <= 3)
        {
            if (!_isPlayed)
            {
                _isPlayed = true;
                _sparksPart.Play();

            }
            if (!_nearDeathExperienceSequence.Any()) NearDeathExperience();
        }
        else
        {
            _isPlayed = false;
            _sparksPart.Stop();

            if (_nearDeathExperienceSequence.Any())
            {
                foreach (Sequence item in _nearDeathExperienceSequence)
                {
                    item.Kill();
                }
                _nearDeathExperienceSequence = new();
            }
        }

        for (int i = 0; i < _goldEtages.Count; i++)
        {
            if(_currentGoldCount > (i + 1) * 10)
            {
                _goldEtages[i].GetComponent<MoreGold>().SpawnBlock(_goldEtages[i]);

            }
            else
            {
                _goldEtages[i].GetComponent<MoreGold>().DespawnBlock(_goldEtages[i]);
            }
        }
    }

    private void NearDeathExperience()
    {
        //& Color Grading
        if (GameManager.Instance.postProcessVolume.profile.TryGetSettings(out ColorGrading colorGrading))
        {
            Sequence _nearDeathExperienceColorGrading = AnimSequence.Chariot.NearDeathSequenceColorGrading(colorGrading);

            _nearDeathExperienceSequence.Add(_nearDeathExperienceColorGrading);
        }

        //& Vignette
        if (GameManager.Instance.postProcessVolume.profile.TryGetSettings(out Vignette vignette))
        {
            Sequence _nearDeathExperienceVignette = AnimSequence.Chariot.NearDeathSequenceVignette(vignette);
            _nearDeathExperienceVignette.SetLoops(-1);
            _nearDeathExperienceVignette.OnKill(() => vignette.intensity.value = 0.35f);

            _nearDeathExperienceSequence.Add(_nearDeathExperienceVignette);
        }
    }


    private void UpdateText()
    {
        _goldCountText.text = GoldCount.ToString();
        // oneLostPart.Play();
    }

    private void UpdateParticle()
    {
        //if (_nbGolbinOnChariot > 0 && !_lostGoldPart.isPlaying)
        //{
        //    _lostGoldPart.Play();
        //}
        //else if (_nbGolbinOnChariot <= 0)
        //{
        //    _lostGoldPart.Stop();
        //}
        //Debug.Log(NbGoblin + " - isPlaying: " + _lostGoldPart.isPlaying.ToString());
    }

    #region Sound
    public void ChariotSound()
    {
        float speed = Mathf.Abs(_rb.velocity.x);

     //   Debug.Log("currentVolume : " + currentVolume);
     //   Debug.Log("speed : " + speed);

        if (speed > 0.5f)
        {
            if (!_isSoundPlaying)
            {
                if (!_chariotEventInstance.isValid())
                {
                    _chariotEventInstance = RuntimeManager.CreateInstance(chariotSound);
                    _chariotEventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform));
                    _chariotEventInstance.start();

                    _chariotEventInstance2 = RuntimeManager.CreateInstance(chariotSound2);
                    _chariotEventInstance2.set3DAttributes(RuntimeUtils.To3DAttributes(transform));
                    _chariotEventInstance2.start();
                }
                else
                {
                    _chariotEventInstance.setPaused(false);
                    _chariotEventInstance2.setPaused(false);
                }

                _isSoundPlaying = true;
            }

            targetVolume = (speed - 0.5f) / (4f - 0.5f);
        }
        else
        {
            targetVolume = 0f;
        }

        if (_chariotEventInstance.isValid())
        {
            currentVolume = Mathf.MoveTowards(currentVolume, targetVolume, Time.deltaTime * (1f / fadeTime));
            _chariotEventInstance.setParameterByName("Volume", currentVolume);
            _chariotEventInstance2.setParameterByName("Volume", currentVolume);

            if (currentVolume == 0f && _isSoundPlaying)
            {
                _chariotEventInstance.setPaused(true);
                _chariotEventInstance2.setPaused(true);
                _isSoundPlaying = false;
            }
        }

    }

    public void StopChariotSound()
    {
        if (_chariotEventInstance.isValid())
        {
            _chariotEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            _chariotEventInstance.release();

            _chariotEventInstance2.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            _chariotEventInstance2.release();
            _isSoundPlaying = false;
        }
    }

    #endregion

    public void HandleCarriedState(Player currentPlayer, bool isGrabbed)
    {
        Tuto tuto = TargetManager.Instance.GetGameObject<Tuto>();
        if (tuto.isPushChariot)
        {
            tuto.isTakeEnemy = true;
        }
        currentPlayer.GetMovement().canFlip = !isGrabbed;
        currentPlayer.GetAnimator().SetBool("hasChariot", isGrabbed);
    }

    public void HandleDestroy()
    {
        StartCoroutine(GameManager.Instance.GameOver(Message.Lava));
    }
    public void HideGfx()
    {
        _goldCountText.gameObject.SetActive(false);
        _gfx.SetActive(false);
    }
    public GameObject GetGameObject() { return gameObject; }

    public void HideChariotText()
    {
        _lostGoldPart.Stop();
    }

    public void GoldEvent()
    {
        if (_currentGoldCount <= 1) return;
        else
        {
            _currentGoldCount = (int)Mathf.Round(_currentGoldCount / 2);
            _eventManager.SpawnPepite(_currentGoldCount);
            UpdateText();
        }
    }
    public void LostGoldStage()
    {
        goldLostValue = Mathf.Abs(_currentGoldCount) % 10;
        if (goldLostValue == 0) { goldLostValue = 10; }
        if (_currentGoldCount - goldLostValue < 10)
        {
            _currentGoldCount = 10;
        }
        else
        {
            _currentGoldCount = _currentGoldCount - goldLostValue;
        }
        UpdateText();
    }
   

    public void LostGoldByRock()
    {
        _currentGoldCount = _currentGoldCount - 5;
        _eventManager.SpawnPepite(5);
        UpdateText();
    }

    public void AddGoldPepite()
    {
        _currentGoldCount = _currentGoldCount + 1;
        UpdateText();
    }

   
}