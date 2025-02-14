using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
        for (int i = 0; i < Test; i++)
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

            }
            yield return new WaitForSeconds(0.08f);
        }
    }

    public void EndSentence()
    {
        if (_currentInt < 11)
        {
            _phrase.text = "De quoi payer qu'une chambre � l'auberge !";
        }
        else if (_currentInt >= 11 && _currentInt < 21)
        {
            _phrase.text = "Au moins vous pourrrez payer votre loyer, mais sans chauffer votre chaumi�re";

        }
        else if (_currentInt >= 21 && _currentInt < 31)
        {
            _phrase.text = "Tourn�e g�n�rale et avec un banquet s'il vous plait !";

        }
        else if (_currentInt >= 31 && _currentInt < 41)
        {
            _phrase.text = "Attention � ne pas rendre les dragons jaloux ;)";
        }
        else
        {
            _phrase.text = "Si on rachetait la for�t des elfs sylvains ?";
        }
    }
}
