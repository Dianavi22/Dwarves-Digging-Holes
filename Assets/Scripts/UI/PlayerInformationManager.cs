using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerInformationManager : MonoBehaviour
{
    public Image cartsFatigueBar;
    public Image miningFatigueBar;

    public TMP_Text cartsFatigueText;
    public TMP_Text miningFatigueText;

    public Image imagePlayer;
    public Image imageIcon;
    public Sprite beerSprite;
    public Sprite pickaxeSprite;
    public Sprite chariotSprite;

    public Player _player;
    public void Initialize(Player player)
    {
        _player = player;

        if (_player.GetFatigue() != null)
        {
            _player.GetFatigue().onCartsFatigueChanged.AddListener(UpdateCartsFatigueUI);
            _player.GetFatigue().onMiningFatigueChanged.AddListener(UpdateMiningFatigueUI);

            UpdateCartsFatigueUI(_player.GetFatigue().currentCartsFatigue, _player.GetFatigue().maxCartsFatigue);
            UpdateMiningFatigueUI(_player.GetFatigue().currentMiningFatigue, _player.GetFatigue().maxMiningFatigue);
        }
    }

    private void OnDestroy()
    {
        if (_player.GetFatigue() != null)
        {
            _player.GetFatigue().onCartsFatigueChanged.RemoveListener(UpdateCartsFatigueUI);
            _player.GetFatigue().onMiningFatigueChanged.RemoveListener(UpdateMiningFatigueUI);
        }
    }


    public void UpdateCartsFatigueUI(float currentFatigue, float maxFatigue)
    {
        if (cartsFatigueBar != null)
        {
            UpdateBar(currentFatigue, maxFatigue, cartsFatigueBar, cartsFatigueText);
        }
    }

    public void UpdateMiningFatigueUI(float currentFatigue, float maxFatigue)
    {
        if (miningFatigueBar != null)
        {
            UpdateBar(currentFatigue, maxFatigue, miningFatigueBar, miningFatigueText);
        }
    }

    public void UpdateBar(float currentValue, float maxValue, Image bar, TMP_Text text)
    {
        float ratio = currentValue / maxValue;
        bar.fillAmount = Mathf.MoveTowards(bar.fillAmount, ratio, Time.deltaTime * maxValue);
        text.text = ((int)currentValue).ToString();
    }

    // ^ Functions for test buttons. Its functions do not harm anyone. Please keep them ! :3
    public void IncreaseCartsFatigue(float amount)
    {
        _player.GetFatigue().IncreaseCartsFatigue(amount);
    }

    public void IncreaseMiningFatigue(float amount)
    {
        _player.GetFatigue().IncreaseMiningFatigue(amount);
    }

    public void ReduceCartsFatigueButtom(float amount)
    {
        _player.GetFatigue().ReduceCartsFatigue(amount);
    }

    public void ReduceMiningbutton(float amount)
    {
        _player.GetFatigue().ReduceMiningFatigue(amount);
    }
    
    // ^ Functions for test buttons ICONS.
    public void UpdateImageIcon(Sprite iconSprite)
    {
        imageIcon.enabled = true;  
        imageIcon.sprite = iconSprite; 
    }

    public void ImageIconBeer()
    {
        UpdateImageIcon(beerSprite);
    }

    public void ImageIconPickaxe()
    {
        UpdateImageIcon(pickaxeSprite);
    }

    public void ImageIconChariot()
    {   
        UpdateImageIcon(chariotSprite);
    }

    public void DisableImageIcon()
    {
        imageIcon.enabled = false;
    }

}
