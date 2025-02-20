using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using FMODUnity;

public class WinPanel : MonoBehaviour
{

    [SerializeField] GoldChariot _gc;
    [SerializeField] TMP_Text _goldCountText;
    [SerializeField] ParticleSystem _goldPart;
    [SerializeField] ParticleSystem _goldBGPart;
    [SerializeField] ParticleSystem _badgePart;
    [SerializeField] TMP_Text _phrase;
    [SerializeField] Animator _animator;
    [SerializeField] ShakyCame _shakyCame;
    [SerializeField] GameObject _endPart;
    [SerializeField] GameObject _badge;
    [SerializeField] GameObject _panelEnd;
    [SerializeField] GameObject _buttonNext;
    [SerializeField] GameObject _levelComplete;
    [SerializeField] GameObject _retryButton;
    [SerializeField] GameObject _spritePlanche;
    [SerializeField] Vector3 planchVector = new Vector3(10f, 5.5f, -11.1000004f);
    [SerializeField] int Test;
    [SerializeField] int _currentInt;

    [SerializeField] private EventReference goldTakeSound;
    [SerializeField] private EventReference panelEndSound;
    [SerializeField] private EventReference badgeSound;
    [SerializeField] private EventReference victorySound;

    void Start()
    {
      // StartCoroutine(GoldCountWin());
    }

    public IEnumerator GoldCountWin()
    {
        GameOST.Instance.PauseAndSetMusicTime(2, 32);
        _panelEnd.SetActive(true);
        _spritePlanche.SetActive(true);
        PanelEndSound();
        
        yield return new WaitForSeconds(0.5f);
        VictorySound();
        _currentInt = 0;
        for (int i = 0; i < _gc._currentGoldCount; i++)
        {
            _currentInt = i + 1;
            _goldCountText.text = "";
            _goldCountText.text = _currentInt.ToString();
            goldTakeSoundSound();
            
            _animator.SetTrigger("Boing");
            _goldPart.Play();
            
            _goldBGPart.Play();
            if (_currentInt == _gc._currentGoldCount)
            {
                EndSentence();
                _goldBGPart.Play();
                yield return new WaitForSeconds(0.15f);
                BadgeSound();
                yield return new WaitForSeconds(0.85f);
                _shakyCame.ShakyCameCustom(0.1f, 0.1f);
                _badge.SetActive(true);
                yield return new WaitForSeconds(0.15f);
                _badgePart.Play();
                yield return new WaitForSeconds(0.2f);
                _endPart.SetActive(true);
                _phrase.gameObject.SetActive(true);
                yield return new WaitForSeconds(0.08f);
                _buttonNext.SetActive(true);
                EventSystem.current.SetSelectedGameObject(_buttonNext);
                GameOST.Instance.ResumeMusic();

            }
            yield return new WaitForSeconds(0.08f);
        }
    }

    public void LevelCompleteButton()
    {
        _levelComplete.SetActive(true);
        _panelEnd.SetActive(false);
        _spritePlanche.SetActive(false);
        _badge.SetActive(false);
        _buttonNext.SetActive(false);
        GameManager.Instance.SetStatsParent();
        EventSystem.current.SetSelectedGameObject(_retryButton);
    }

    public void EndSentence()
    {

        var emission = _goldBGPart.emission; 
        if (_currentInt < 11)
        {
            emission.rateOverTime = 5f;
            _phrase.text = StringManager.Instance.GetLevelCompleteSentence(LevelCompleteMessage.LittleGold);
        }
        else if (_currentInt >= 11 && _currentInt < 21)
        {
            emission.rateOverTime = 30f;
            _phrase.text = StringManager.Instance.GetLevelCompleteSentence(LevelCompleteMessage.AverageGold);
        }
        else if (_currentInt >= 21 && _currentInt < 31)
        {
            emission.rateOverTime = 50f;
            _phrase.text = StringManager.Instance.GetLevelCompleteSentence(LevelCompleteMessage.GreatGold);
        }
        else if (_currentInt >= 31 && _currentInt < 41)
        {
            emission.rateOverTime = 70f;

            _phrase.text = StringManager.Instance.GetLevelCompleteSentence(LevelCompleteMessage.ExtraGold);
        }
        else
        {
            emission.rateOverTime = 100f;
            _phrase.text = StringManager.Instance.GetLevelCompleteSentence(LevelCompleteMessage.GoldMountain);
        }
    }

    #region Sounds
    private void goldTakeSoundSound()
    {
        RuntimeManager.PlayOneShot(goldTakeSound, transform.position);
    }

    private void PanelEndSound()
    {
        RuntimeManager.PlayOneShot(panelEndSound, transform.position);
    }

    private void BadgeSound()
    {
        RuntimeManager.PlayOneShot(badgeSound, transform.position);
    }

    private void VictorySound()
    {
        RuntimeManager.PlayOneShot(victorySound, transform.position);
    }
    #endregion
}
