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

    public PlayerFatigue playerFatigue;


    public void Initialize(PlayerHealth health, PlayerFatigue fatigue)
    {
        playerFatigue = fatigue;

        if (playerFatigue != null)
        {
            playerFatigue.onCartsFatigueChanged.AddListener(UpdateCartsFatigueUI);
            playerFatigue.onMiningFatigueChanged.AddListener(UpdateMiningFatigueUI);

            UpdateCartsFatigueUI(playerFatigue.currentCartsFatigue, playerFatigue.maxCartsFatigue);
            UpdateMiningFatigueUI(playerFatigue.currentMiningFatigue, playerFatigue.maxMiningFatigue);
        }
    }

    private void OnDestroy()
    {
        if (playerFatigue != null)
        {
            playerFatigue.onCartsFatigueChanged.RemoveListener(UpdateCartsFatigueUI);
            playerFatigue.onMiningFatigueChanged.RemoveListener(UpdateMiningFatigueUI);
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
        playerFatigue.IncreaseCartsFatigue(amount);
    }

    public void IncreaseMiningFatigue(float amount)
    {
        playerFatigue.IncreaseMiningFatigue(amount);
    }

    public void ReduceCartsFatigueButtom(float amount)
    {
        playerFatigue.ReduceCartsFatigue(amount);
    }

    public void ReduceMiningbutton(float amount)
    {
        playerFatigue.ReduceMiningFatigue(amount);
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
