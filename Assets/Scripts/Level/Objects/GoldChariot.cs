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
using System.Collections;

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
    [SerializeField] private float fadeInTime = 0.1f;
    [SerializeField] private float fadeOutTime = 1f;

    [SerializeField] private TMP_Text _goldCountText;

    [Header("Particle System")]
    [SerializeField] private ParticleSystem _lostGoldPart;
    [SerializeField] private ParticleSystem _sparksPart;
    public ParticleSystem oneLostPart;

    [SerializeField] private ParticleSystem _dustPart;

    [Header("Gold Pile")]
    [SerializeField] int _maxGoldStep;
    [SerializeField] MoreGold _goldStepPrefab;
    [SerializeField] Transform _defaultSpawnNuggetPosition;
    [SerializeField] Pepite nugget;

    [Header("Other")]
    [SerializeField] GameObject _defaultGFX;
    [SerializeField] GameObject _brokenGFX;
    public GameObject hbTakeGold;

    List<MoreGold> _goldStepList = new();
    private List<Sequence> _nearDeathExperienceSequence = new();
    [SerializeField] private Animator _takeGoldAnim;
    [SerializeField] private Collider _hitBox;

    private Rigidbody _rb;

    private float goblinTimer = 0f;
    public float goblinInterval = 1f;

    private int _nbGolbinOnChariot;
    public int NbGoblin
    {
        get => _nbGolbinOnChariot;
        set
        {
            _nbGolbinOnChariot = value;
            UpdateParticle();
        }
    }

    public int _currentGoldCount = 10;

    public ParticleSystem GetParticleLostGold() => _lostGoldPart;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();

        var velocity = _dustPart.velocityOverLifetime;
        velocity.x = -GameManager.Instance.CurrentScrollingSpeed;
        UpdateText();

       
    }

    public void StartAnimation()
    {
        _takeGoldAnim.SetBool("isNewGold", true);
        Invoke(nameof(ResetAnim), 0.25f);
    }

    
    private void ResetAnim()
    {
        _takeGoldAnim.SetBool("isNewGold", false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            TakeNugget();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            LostGoldByGoblin();
        }

        if (!_dustPart.isPlaying && _rb.velocity != Vector3.zero)
        {
            _dustPart.Play();
        }
        if(_rb.velocity == Vector3.zero)
        {
            _dustPart.Stop();

        }


        if (_currentGoldCount <= 0 && !GameManager.Instance.isInMainMenu && !GameManager.Instance.isGameOver)
        {
            StopChariotSound();
            StartCoroutine(GameManager.Instance.GameOver(Message.NoGold, false));
        }


        if (_currentGoldCount > 0)
        {
            try
            {
                if (NbGoblin > 0)
                {
                    goblinTimer += Time.deltaTime;

                    if (goblinTimer >= goblinInterval)
                    {
                        LostGoldByGoblin();
                        goblinTimer = 0f;
                    }
                }
                else
                {
                    goblinTimer = 0f;
                }
            }
            catch
            {
                return;
            }

        }

        ChariotSound();

        if (Vector3.Distance(transform.position, TargetManager.Instance.GetGameObject<Lava>().transform.position) - 4 < 5 || _currentGoldCount <= 3)
        {
            if (!_sparksPart.isPlaying)
            {
                _sparksPart.Play();
            }
            if (!_nearDeathExperienceSequence.Any()) NearDeathExperience();
        }
        else if (_nearDeathExperienceSequence.Any())
        {
            _sparksPart.Stop();

            foreach (Sequence item in _nearDeathExperienceSequence)
            {
                item.Kill();
            }
            _nearDeathExperienceSequence = new();
        }

    }

    private void NearDeathExperience()
    {
        //& Color Grading
        if (TargetManager.Instance.GetGameObject<PostProcessVolume>().profile.TryGetSettings(out ColorGrading colorGrading))
        {
            Sequence _nearDeathExperienceColorGrading = AnimSequence.Chariot.NearDeathSequenceColorGrading(colorGrading);

            _nearDeathExperienceSequence.Add(_nearDeathExperienceColorGrading);
        }

        //& Vignette
        if (TargetManager.Instance.GetGameObject<PostProcessVolume>().profile.TryGetSettings(out Vignette vignette))
        {
            Sequence _nearDeathExperienceVignette = AnimSequence.Chariot.NearDeathSequenceVignette(vignette);
            _nearDeathExperienceVignette.SetLoops(-1);
            _nearDeathExperienceVignette.OnKill(() => vignette.intensity.value = 0.35f);

            _nearDeathExperienceSequence.Add(_nearDeathExperienceVignette);
        }
    }

    private void UpdateText()
    {
        _goldCountText.text = _currentGoldCount.ToString();
        // oneLostPart.Play();
    }

    private void UpdateParticle()
    {
        if (NbGoblin > 0 && !_lostGoldPart.isPlaying)
        {
            _lostGoldPart.Play();
        }
        else if (NbGoblin <= 0)
        {
            _lostGoldPart.Stop();
        }
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
            targetVolume = Mathf.Clamp01(targetVolume);
        }
        else
        {
            _dustPart.Stop();
            targetVolume = 0f;
        }

        if (_chariotEventInstance.isValid())
        {
            float fadeDuration = (targetVolume > currentVolume) ? fadeInTime : fadeOutTime;

            currentVolume = Mathf.MoveTowards(currentVolume, targetVolume, Time.deltaTime * (1f / fadeDuration));
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

    #region IGrabbable
    public void HandleCarriedState(Player currentPlayer, bool isGrabbed)
    {
        Tuto tuto = TargetManager.Instance.GetGameObject<Tuto>();
        if (tuto.isPushChariot)
        {
            tuto.isTakeEnemy = true;
        }
        currentPlayer.GetMovement().canFlip = !isGrabbed;
        currentPlayer.GetAnimator().SetBool("isGrabbing", isGrabbed);
        currentPlayer.GetPlayerMovements().isGrabbingChariot = isGrabbed;
    }

    public void HandleDestroy()
    {

        StartCoroutine(GameManager.Instance.GameOver(Message.Lava, true));
        StopChariotSound();
    }
    public GameObject GetGameObject() { return gameObject; }
    #endregion

    private void SetBrokenGFX(bool isChariotBroken)
    {
        _defaultGFX.SetActive(!isChariotBroken);
        _brokenGFX.SetActive(isChariotBroken);
    }

    public void HideGfx(bool isGoldChariotDestroyed)
    {
        for (int i = 0; i < _goldStepList.Count; i++)
        {
            _goldStepList[i].gameObject.SetActive(false);
        }
        _goldCountText.gameObject.SetActive(false);
        if (!isGoldChariotDestroyed) SetBrokenGFX(true);
        else _defaultGFX.SetActive(false);
    }
    public void StopParticle()
    {
        _lostGoldPart.Stop();
    }

    #region Gold Related
    public void GoldEvent()
    {
        int _lostGoldEvent = Math.Abs(_currentGoldCount / 3);
        if (_currentGoldCount <= 1) return;
        else
        {
            for (int i = 0; i < Math.Abs(_lostGoldEvent); i++)
            {
                LostGoldByGoblin();
            }
            SpawnMultipleNugget(_lostGoldEvent, this.transform);
        }
    }

    #region Stack Gold

    public void TakeNugget()
    {
        _currentGoldCount++;
        UpdateText();
        for (int i = _goldStepList.Count - 1; i >= 0; i--)
        {
            if (_goldStepList[i] == null)
            {
                _goldStepList.RemoveAt(i);
            }
        }
        if (_currentGoldCount > (_goldStepList.Count + 1) * 10 && _goldStepList.Count <= _maxGoldStep)
        {
            Vector3 position = transform.position + transform.up + (Vector3.up * _currentGoldCount / 10);
            MoreGold g = Instantiate(_goldStepPrefab, position, Quaternion.identity, transform);
            g.Instanciate(_goldStepList.Count);
            _goldStepList.Add(g);
        }


    }

    public void DamageByFallRock()
    {
        for (int i = 0; i < 5; i++)
        {
            if (_currentGoldCount < 0)
            {
                return;
            }
            LostGoldByGoblin();
        }
        SpawnMultipleNugget(5, this.transform);

    }

    private void LostGoldByGoblin()
    {
        _currentGoldCount--;
        UpdateText();


        if (_currentGoldCount % 10 == 0)
        {
            GameObject _currentGO = _goldStepList[_goldStepList.Count - 1].gameObject;
            _goldStepList.RemoveAt(_goldStepList.Count - 1);
            StartCoroutine(_currentGO.GetComponent<MoreGold>().DespawnBlock());

        }
    }
    public void LostGoldStage(int idStep)
    {
        for (int i = _goldStepList.Count - 1; i >= 0; i--)
        {
            if (_goldStepList[i].IDGoldStep >= idStep)
            {
                if (_goldStepList[i].IDGoldStep != idStep)
                {
                    SpawnMultipleNugget(5, this.transform);
                }
                StartCoroutine(_goldStepList[i].DespawnBlock());
                _goldStepList.RemoveAt(i);

            }
        }

        if (_goldStepList.Count == 0)
        {
            _currentGoldCount = 10;
            UpdateText();
        }
        else if (_goldStepList.Count == 1)
        {
            _currentGoldCount = 11;
            UpdateText();
        }
        else
        {
            _currentGoldCount = _goldStepList.Count * 10;
            UpdateText();
        }
    }

    public void SpawnMultipleNugget(int nb, Transform position)
    {
        for (int i = 0; i <= nb - 1; i++)
        {
            SpawnNugget(position);
        }
    }

    public void SpawnNugget(Transform transform)
    {
        Pepite spawnedObject = Instantiate(nugget, transform.position, Quaternion.identity);

        if (spawnedObject.TryGetComponent<Rigidbody>(out var rb))
        {
            Vector3 direction = DRandom.DirectionInCone(transform.forward, 15f);
            rb.AddForce(direction * UnityEngine.Random.Range(10f, 30f), ForceMode.Impulse);
        }
        UpdateText();
    }
    #endregion
    #endregion
}