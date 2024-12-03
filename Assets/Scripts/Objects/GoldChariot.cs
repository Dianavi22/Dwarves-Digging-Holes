using FMOD.Studio;
using FMODUnity;
using TMPro;
using UnityEngine;

public class GoldChariot : MonoBehaviour, IGrabbable
{
    [SerializeField] private TMP_Text _goldCountText;
    [SerializeField] private ParticleSystem _lostGoldPart;
    [SerializeField] private Score _score;
    [SerializeField] private int _goldScore;
    [SerializeField] GameObject _gfx;

    [SerializeField] private EventReference chariotSound;
    private SoundUtils _soundUtils;
    private bool _isSoundPlaying = false;
    private EventInstance _chariotEventInstance;

    private Rigidbody _rb;

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

    private int _currentGoldCount = 10;
    public int GoldCount
    {
        get => _currentGoldCount;
        set
        {
           if (_currentGoldCount > 0)
                {
                _score.AddScoreOnce(_currentGoldCount < value ? _goldScore : -(_goldScore / 2));
                _currentGoldCount = value;
                UpdateText();
            }
          
        }
    }
    public ParticleSystem GetParticleLostGold() => _lostGoldPart;

    private void Start()
    {
        _soundUtils = GetComponent<SoundUtils>();
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
    }

    private void UpdateText()
    {
        _goldCountText.text = GoldCount.ToString();
    }

    private void UpdateParticle()
    {
        if (_nbGolbinOnChariot > 0 && !_lostGoldPart.isPlaying)
        {
            _lostGoldPart.Play();
        }
        else if (_nbGolbinOnChariot <= 0)
        {
            _lostGoldPart.Stop();
        }
        Debug.Log(NbGoblin + " - isPlaying: " + _lostGoldPart.isPlaying.ToString());
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
                _soundUtils.UnpauseWithFade(_chariotEventInstance, 0.1f);
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
                _soundUtils.PauseWithFade(_chariotEventInstance, 0.1f);
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
        currentPlayer.GetMovement().canFlip = !isGrabbed;
        currentPlayer.GetAnimator().SetBool("hasChariot", isGrabbed); 
    }

    public void HandleDestroy()
    {
        //@todo Make the gameover call only after a certain time in the lava not instantly
        GameManager.Instance.GameOver(DeathMessage.Lava);
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
}