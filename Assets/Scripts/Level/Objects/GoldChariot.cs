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

    [Header("Gold Pile")]
    [SerializeField] int _maxGoldStep;
    [SerializeField] MoreGold _goldStepPrefab;
    [SerializeField] Transform _defaultSpawnNuggetPosition;
    [SerializeField] Pepite nugget;

    [Header("Other")]
    [SerializeField] GameObject _defaultGFX;
    [SerializeField] GameObject _brokenGFX;


    List<MoreGold> _goldStepList = new();
    private List<Sequence> _nearDeathExperienceSequence = new();

    private Rigidbody _rb;

    public int NbGoblin
    {
        get => _nbGolbinOnChariot;
        set
        {
            _nbGolbinOnChariot = value;
            UpdateParticle();
        }
    }
    private int _nbGolbinOnChariot;

    public int GoldCount
    {
        get => _currentGoldCount;
        set
        {
            _currentGoldCount = Math.Max(0, value);
            UpdateText();

            if (_currentGoldCount > (_goldStepList.Count + 1) * 10 && _goldStepList.Count <= _maxGoldStep)
            {
                Vector3 position = transform.position + transform.up + (Vector3.up * _currentGoldCount / 10);
                MoreGold g = Instantiate(_goldStepPrefab, position, Quaternion.identity, transform);
                g.Instanciate(_goldStepList.Count);
                _goldStepList.Add(g);
            }
            else if (_currentGoldCount > 0 && _currentGoldCount <= _goldStepList.Count * 10)
            {
                StartCoroutine(_goldStepList[_goldStepList.Count - 1].DespawnBlock());
                _goldStepList.RemoveAt(_goldStepList.Count - 1);
            }

            // GameOver
            GameManager gm = GameManager.Instance;
            if (_currentGoldCount <= 0 && !gm.isInMainMenu && !gm.isGameOver && !gm.debugMode)
                StartCoroutine(gm.GameOver(Message.NoGold, false));
        }
    }
    private int _currentGoldCount = 10;

    private Transform NuggetSpawnPoint => _goldStepList.Count == 0 ? _defaultSpawnNuggetPosition : _goldStepList.Last().GetSpawnPoint;
    public int GetHighestIndexStepList => _goldStepList.Count - 1;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        ChariotSound();

        if (Vector3.Distance(transform.position, TargetManager.Instance.GetGameObject<Lava>().transform.position) - 4 < 5 || GoldCount <= 3)
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
        _goldCountText.text = GoldCount.ToString();
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
        currentPlayer.GetAnimator().SetBool("hasChariot", isGrabbed);
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
        if (GoldCount <= 1) return;
        else
        {
            GoldCount = (int)Mathf.Round(GoldCount / 2);
            SpawnMultipleNugget(GoldCount, NuggetSpawnPoint);
        }
    }
    public void LostGoldStage(int idStep)
    {
        int goldLostValue = Mathf.Abs(GoldCount) % 10;
        if (goldLostValue == 0)
            goldLostValue = 10;
        if (GoldCount - goldLostValue < 10)
            GoldCount = 10;
        else
            GoldCount -= goldLostValue;

        SpawnMultipleNugget(goldLostValue, _goldStepList[idStep].GetSpawnPoint);
    }

    public void SpawnMultipleNugget(int nb, Transform position)
    {
        for (int i = 0; i <= nb; i++)
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
}