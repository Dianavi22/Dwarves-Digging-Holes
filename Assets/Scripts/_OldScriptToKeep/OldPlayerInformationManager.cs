using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OldPlayerInformationManager : MonoBehaviour
{
    public Image lifeBar;
    public Image cartsFatigueBar;
    public Image miningFatigueBar;

    public TMP_Text lifeText;
    public TMP_Text cartsFatigueText;
    public TMP_Text miningFatigueText;

    public Image imagePlayer;
    public Image imageIcon;
    public Sprite beerSprite;
    public Sprite pickaxeSprite;
    public Sprite chariotSprite;

    public PlayerHealth playerHealth;
    public PlayerFatigue playerFatigue;


    public void Initialize(PlayerHealth health, PlayerFatigue fatigue)
    {
        playerHealth = health;
        playerFatigue = fatigue;

        if (playerHealth != null)
        {
            //playerHealth.onHealthChanged.AddListener(UpdateLifeUI);
        }

        if (playerFatigue != null)
        {
            playerFatigue.onCartsFatigueChanged.AddListener(UpdateCartsFatigueUI);
            playerFatigue.onMiningFatigueChanged.AddListener(UpdateMiningFatigueUI);

            UpdateCartsFatigueUI(playerFatigue.currentCartsFatigue, playerFatigue.MaxPushCartFatigue);
            UpdateMiningFatigueUI(playerFatigue.currentMiningFatigue, playerFatigue.MaxMiningFatigue);
        }
    }

    private void OnDestroy()
    {
        if (playerHealth != null)
        {
            //playerHealth.onHealthChanged.RemoveListener(UpdateLifeUI);
        }

        if (playerFatigue != null)
        {
            playerFatigue.onCartsFatigueChanged.RemoveListener(UpdateCartsFatigueUI);
            playerFatigue.onMiningFatigueChanged.RemoveListener(UpdateMiningFatigueUI);
        }
    }

    public void UpdateLifeUI(int currentHealth, int maxHealth)
    {
        if (lifeBar != null)
        {
        UpdateBar(currentHealth, maxHealth, lifeBar, lifeText);

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







    // ^ Functions for test buttons.
    public void damage(int amount)
    {
        //playerHealth.Damage(amount);
    }
    public void health(int amount)
    {
        //playerHealth.Health(amount);
    }

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
