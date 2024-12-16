using FMOD.Studio;
using FMODUnity;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Utils;
using DG.Tweening;

public class GoldChariot : MonoBehaviour, IGrabbable
{
    [SerializeField] private TMP_Text _goldCountText;
    [SerializeField] private ParticleSystem _lostGoldPart;

    [SerializeField] private GameObject _lavaPosition;
   public ParticleSystem oneLostPart;
    [SerializeField] GameObject _gfx;
    [SerializeField] Tuto _tuto;
    [SerializeField] private EventReference chariotSound;

    private bool _isSoundPlaying = false;
    private EventInstance _chariotEventInstance;
    private Sequence _nearDeathExperience;

    private Rigidbody _rb;

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

    private int _currentGoldCount = 10;
    public int GoldCount
    {
        get => _currentGoldCount;
        set
        {
            if (_currentGoldCount > 0)
            {
                _currentGoldCount = value;
                UpdateText();
            }
        }
    }
    public ParticleSystem GetParticleLostGold() => _lostGoldPart;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (_rb.velocity.x > 0)
        {
           
            PlayChariotSound();
        }
        else
        {
            PauseChariotSound();
        }
        if(Vector3.Distance(transform.position, _lavaPosition.transform.position) - 4 < 5 || GoldCount <= 3) {
            if(_nearDeathExperience == null) NearDeathExperience();
        }
        else {
            if(_nearDeathExperience != null) {
                _nearDeathExperience.Kill();
                _nearDeathExperience = null;
            }
        }

    }

    private void NearDeathExperience() {
        if (GameManager.Instance.postProcessVolume.profile.TryGetSettings(out Vignette vignette))
        {
            _nearDeathExperience = AnimSequence.Chariot.NearDeathSequence(vignette);
            _nearDeathExperience.OnKill(() => vignette.intensity.value = 0.35f);
            _nearDeathExperience.SetLoops(-1);
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

    #region SFX
    private void PlayChariotSound()
    {
        if (!_isSoundPlaying)
        {
            if (!_chariotEventInstance.isValid())
            {
                _chariotEventInstance = RuntimeManager.CreateInstance(chariotSound);
                _chariotEventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform));
                _chariotEventInstance.start();
            }
            else
            {
                StartCoroutine(Sound.UnpauseWithFade(_chariotEventInstance, 0.1f));
            }

            _isSoundPlaying = true;
        }
    }
    public void PauseChariotSound()
    {
        if (_isSoundPlaying)
        {
            FMOD.RESULT result = _chariotEventInstance.getPaused(out bool isPaused);
            if (result == FMOD.RESULT.OK && !isPaused)
            {
                StartCoroutine(Sound.PauseWithFade(_chariotEventInstance, 0.1f));
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
            _isSoundPlaying = false;
        }
    }
    #endregion
    
    public void HandleCarriedState(Player currentPlayer, bool isGrabbed)
    {

        if (_tuto.isPushChariot)
        {
            _tuto.isTakeEnemy = true;
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
            UpdateText();
        }
    }
}