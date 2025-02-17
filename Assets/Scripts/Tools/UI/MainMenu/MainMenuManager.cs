using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using FMOD.Studio;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject _settingsWindow;
    [SerializeField] private GameObject _closeButtonSettings;
    [SerializeField] GameObject _circleTransition;
    [SerializeField] GameObject _circleTransitionIn;
    [SerializeField] GameObject _buttons;
    [SerializeField] List<GameObject> _button;
    [SerializeField] GameObject _title;
    [SerializeField] GameObject _credits;
    [SerializeField] GameObject _stopCredits;
    [SerializeField] GameObject _startButton;
    [SerializeField] GameObject _mysteryCube;
    [SerializeField] Color _bc;
    [SerializeField] private CanvasGroup _buttonsCanvaGroup;
    private bool _scaleButton = false;

    [SerializeField] GameObject _canvasSettings;
    [SerializeField] GameObject _pivotStartSettings;
    [SerializeField] GameObject _buttonSettingsStart;

    [SerializeField] GameObject _backButton;
    [SerializeField] GameObject _bbSettings;
    [SerializeField] GameObject _rockIntro;
    [SerializeField] GameObject _rocksMenuPartGO;
    [SerializeField] Toggle _toggle;
    [SerializeField] ParticleSystem _breakRockPart;
    [SerializeField] ParticleSystem _rocksMenuPart;
    [SerializeField] ParticleSystem _littleRocksPart;
    [SerializeField] ParticleSystem _buttonFallPart;
    [SerializeField] bool lerp = false;
    [SerializeField] private EventReference introMusicSound;
    [SerializeField] private EventReference introRocksSound;
    [SerializeField] private EventReference buttonFallingSound;
    [SerializeField] private EventReference mainMusicSound;
    private EventInstance mainMusicInstance; 
    [SerializeField, Range(0, 1)] private float currentVolume = 0.5f;

    public float speed = 5;
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private ShakyCame _sc;

    private float _t = 0f;
    private int _countButton;
    private GameObject _currentButtonFall;

    private void Start()
    {
        StartCoroutine(StartCanvas());
        _toggle.isOn = PlayerPrefs.GetInt("TutoActive") == 1;
        _countButton = _button.Count-1;
        _sc = TargetManager.Instance.GetGameObject<ShakyCame>();

        IntroMusicSound();
    }

    private IEnumerator StartCanvas()
    {
        Time.timeScale = 1;
        _circleTransitionIn.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        StartCoroutine(IntroMainMenu());
    }

    private void FallButtons()
    {
        if (_countButton < 0)
        {
            return;
        }
        if (!lerp)
        {
            _currentButtonFall = _button[_countButton];
            startPosition = _currentButtonFall.transform.position;
            targetPosition = new Vector3(startPosition.x, startPosition.y  -1100, startPosition.z);
            lerp = true;
            _countButton--;
        }
    }

    private IEnumerator IntroMainMenu()
    {
        RuntimeManager.PlayOneShot(introRocksSound);
        _breakRockPart.Play();
        yield return new WaitForSeconds(0.15f);
        _sc.ShakyCameCustom(0.1f, 0.1f);
        _littleRocksPart.Play();
        yield return new WaitForSeconds(0.5f);
        _sc.ShakyCameCustom(0.1f, 0.1f);
        _littleRocksPart.Play();

        yield return new WaitForSeconds(0.5f);
        _rocksMenuPartGO.SetActive(false);
        _sc.ShakyCameCustom(0.3f, 0.1f);

        _rocksMenuPart.Play();
        yield return new WaitForSeconds(0.3f);
        _rockIntro.SetActive(false);
        yield return new WaitForSeconds(1.5f);
        FallButtons();
        yield return new WaitForSeconds(1f);
        ActiveButtons();
        MainMusicSound();
    }

    public void StartParty()
    {
        IsTutoActive();
        StartCoroutine(CircleTransition());
    }

    public bool IsTutoActive()
    {
        int i;
        if (_toggle.isOn)
        {
            i = 1;
        }
        else
        {
            i = 0;

        }
        if (!PlayerPrefs.HasKey("TutoActive"))
        {
            PlayerPrefs.SetInt("TutoActive", i);
            PlayerPrefs.Save();
        }
        else
        {
            PlayerPrefs.SetInt("TutoActive", i);

        }

        return PlayerPrefs.GetInt("TutoActive") == i;
    }

    #region Button click Event
    public void StartGame()
    {
        _mysteryCube.SetActive(false);
        _scaleButton = true;
        _pivotStartSettings.SetActive(true);
        _buttons.SetActive(false);
        _title.SetActive(false);
        EventSystem.current.SetSelectedGameObject(_buttonSettingsStart);
    }

    public void LoadSettingScene()
    {
        _mysteryCube.SetActive(false);
        _bbSettings.transform.localScale = new Vector3(0.212207913f, 0.427238166f, 0.212207913f);
        _scaleButton = true;
        _title.gameObject.SetActive(false);
        _buttons.gameObject.SetActive(false);
        _buttonsCanvaGroup.interactable = false;
        _settingsWindow.SetActive(true);
        EventSystem.current.SetSelectedGameObject(_closeButtonSettings);
    }

    public void LoadCreditScene()
    {
        _mysteryCube.SetActive(false);
        _scaleButton = true;
        _buttons.SetActive(false);
        _title.SetActive(false);
        _credits.SetActive(true);
        EventSystem.current.SetSelectedGameObject(_stopCredits);
        Invoke(nameof(ActiveButtons), 30);
    }

    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
    #endregion

    public void ActiveButtons()
    {
        ShowButtons();
        _mysteryCube.SetActive(true);
        _credits.SetActive(false);
        EventSystem.current.SetSelectedGameObject(_startButton);
    }

    public void ShowButtons()
    {
        _scaleButton = true;
        _buttons.SetActive(true);
        _title.SetActive(true);
    }

    private IEnumerator CircleTransition()
    {
        _circleTransition.SetActive(true);
        yield return new WaitForSeconds(1.7f);
        SceneManager.LoadScene(2);
    }

    private void Update()
    {
        if (lerp)
        {
            if (_t < 1f)
            {
                _t += Time.deltaTime * speed * 1.5f;
                _t = Mathf.Clamp01(_t);
                _currentButtonFall.transform.position = Vector3.Lerp(startPosition, targetPosition, _t);
                if (_t >= 1)
                {
                    _t = 0;
                    lerp = false;
                    _sc.ShakyCameCustom(0.2f, 0.1f);
                    _buttonFallPart.Play();
                    ButtonFallingSound();
                    FallButtons();
                }
            }
        }

        if (_scaleButton)
        {
            UpdateButtonScale();
        }


        mainMusicInstance.setParameterByName("Volume", currentVolume);

    }

    private void UpdateButtonScale()
    {
        Vector3 targetScale = new Vector3(3.6f, 3.6f, 3.6f);

        for (int i = 0; i < _button.Count; i++)
        {
            if (_button[i] != null)
            {
                _button[i].GetComponentInChildren<TMP_Text>().color = _bc;
                _button[i].transform.localScale = targetScale;
            }
        }
        _backButton.transform.localScale = new Vector3(1, 1, 1);
        _scaleButton = false;
        _startButton.GetComponentInChildren<TMP_Text>().color = Color.white;
    }


    #region Sounds
    private void IntroMusicSound()
    {
        RuntimeManager.PlayOneShot(introMusicSound, transform.position);
    }

    private void ButtonFallingSound()
    {
        RuntimeManager.PlayOneShot(buttonFallingSound, transform.position);
    }

    private void MainMusicSound()
    {
        mainMusicInstance = RuntimeManager.CreateInstance(mainMusicSound);
        mainMusicInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform));
        mainMusicInstance.start();

        mainMusicInstance.setParameterByName("Volume", currentVolume);
    }

    public void StopMainMusicSound()
    {
        if (mainMusicInstance.isValid())
        {
            mainMusicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            mainMusicInstance.release();
        }
    }
    #endregion

}
