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
    [SerializeField] int Test;
    [SerializeField] int _currentInt;
    void Start()
    {
       //StartCoroutine(GoldCountWin());
    }

    void Update()
    {

    }

    public IEnumerator GoldCountWin()
    {
        _panelEnd.SetActive(true);
           yield return new WaitForSeconds(0.5f);
        _currentInt = 0;
        for (int i = 0; i < _gc._currentGoldCount; i++)
        {

            _currentInt = i + 1;
            _goldCountText.text = "";
            _goldCountText.text = _currentInt.ToString();
            
            _animator.SetTrigger("Boing");
            _goldPart.Play();
            if (_currentInt == Test)
            {
                EndSentence();
                yield return new WaitForSeconds(1);
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

            }
            yield return new WaitForSeconds(0.08f);
        }
    }

    public void LevelCompleteButton()
    {
        _levelComplete.SetActive(true);
        _panelEnd.SetActive(false);
        _buttonNext.SetActive(false);
        EventSystem.current.SetSelectedGameObject(_retryButton);
    }

    public void EndSentence()
    {
        if (_currentInt < 11)
        {
            _phrase.text = "De quoi payer qu'une chambre à l'auberge !";
        }
        else if (_currentInt >= 11 && _currentInt < 21)
        {
            _phrase.text = "Au moins vous pourrrez payer votre loyer, mais sans chauffer votre chaumière";

        }
        else if (_currentInt >= 21 && _currentInt < 31)
        {
            _phrase.text = "Tournée générale et avec un banquet s'il vous plait !";

        }
        else if (_currentInt >= 31 && _currentInt < 41)
        {
            _phrase.text = "Attention à ne pas rendre les dragons jaloux ;)";
        }
        else
        {
            _phrase.text = "Si on rachetait la forêt des elfs sylvains ?";
        }
    }
}
