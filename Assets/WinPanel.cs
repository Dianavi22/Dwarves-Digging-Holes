using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class WinPanel : MonoBehaviour
{

    [SerializeField] GoldChariot _gc;
    [SerializeField] TMP_Text _goldCountText;
    [SerializeField] ParticleSystem _goldPart;
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
    void Start()
    {
      // StartCoroutine(GoldCountWin());
    }

    void Update()
    {

    }

    public IEnumerator GoldCountWin()
    {
       // _panelEnd.transform.position = planchVector;
        _panelEnd.SetActive(true);
        _spritePlanche.SetActive(true);
           yield return new WaitForSeconds(0.5f);
        print(_gc._currentGoldCount);
        _currentInt = 0;
        for (int i = 0; i < _gc._currentGoldCount; i++)
        {

            _currentInt = i + 1;
            _goldCountText.text = "";
            _goldCountText.text = _currentInt.ToString();
            
            _animator.SetTrigger("Boing");
            _goldPart.Play();
            if (_currentInt == _gc._currentGoldCount)
            {
                EndSentence();
                yield return new WaitForSeconds(1);
                _shakyCame.ShakyCameCustom(0.1f, 0.1f);
               // _badge.transform.position = new Vector3(10.7910004f, 5.86800003f, -13.0530005f);
                _badge.SetActive(true);
                yield return new WaitForSeconds(0.15f);
                _badgePart.Play();
                yield return new WaitForSeconds(0.2f);
                _endPart.SetActive(true);
                _phrase.gameObject.SetActive(true);
                yield return new WaitForSeconds(0.08f);
                _buttonNext.SetActive(true);
                EventSystem.current.SetSelectedGameObject(_buttonNext);

            }
            yield return new WaitForSeconds(0.08f);
        }
    }

    public void LevelCompleteButton()
    {
        _levelComplete.SetActive(true);
        _panelEnd.SetActive(false);
        _spritePlanche.SetActive(false);

        _buttonNext.SetActive(false);
        GameManager.Instance.SetStatsParent();
        EventSystem.current.SetSelectedGameObject(_retryButton);
    }

    public void EndSentence()
    {
        if (_currentInt < 11)
        {
            _phrase.text = "Enough to pay for just a room at the inn!";
        }
        else if (_currentInt >= 11 && _currentInt < 21)
        {
            _phrase.text = "At least you�ll be able to pay your rent, but without heating your cottage.";

        }
        else if (_currentInt >= 21 && _currentInt < 31)
        {
            _phrase.text = "A round for everyone, and a banquet too, please!";

        }
        else if (_currentInt >= 31 && _currentInt < 41)
        {
            _phrase.text = "Careful not to make the dragons jealous!";
        }
        else
        {
            _phrase.text = "What if we bought the Sylvan Elves' forest?";
        }
    }
}
