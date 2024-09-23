using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerInformationManager : MonoBehaviour
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


    void Start()
    {

    }

    void Update()
    {
        UpdateBar(playerHealth.currentHealth, playerHealth._maxHealth, lifeBar, lifeText);
        UpdateBar(playerFatigue.currentCartsFatigue, playerFatigue.maxCartsFatigue, cartsFatigueBar, cartsFatigueText);
        UpdateBar(playerFatigue.currentMiningFatigue, playerFatigue.maxMiningFatigue, miningFatigueBar, miningFatigueText);
    }

    public void UpdateBar(float currentValue, float maxValue, Image bar, TMP_Text text)
    {
        float ratio = currentValue / maxValue;
        bar.fillAmount = Mathf.MoveTowards(bar.fillAmount, ratio, Time.deltaTime);
        text.text = ((int)currentValue).ToString();
    }


    public void damage(int amount)
    {
        playerHealth.Damage(amount);
    }
    public void health(int amount)
    {
        playerHealth.Health(amount);
    }

    public bool ReduceCartsFatigue(float amount)
    {
        return playerFatigue.ReduceCartsFatigue(amount);
    }

    public void IncreaseCartsFatigue(float amount)
    {
        playerFatigue.IncreaseCartsFatigue(amount);
    }

    public bool ReduceMiningFatigue(float amount)
    {
        return playerFatigue.ReduceMiningFatigue(amount);
    }

    public void IncreaseMiningFatigue(float amount)
    {
        playerFatigue.IncreaseMiningFatigue(amount);
    }


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





    // ^ Functions for test buttons. Bool doesn't work with buttons. So I return a void.

    public void ReduceCartsFatigueButtom(float amount)
    {
        ReduceCartsFatigue(amount);
    }

    public void ReduceMiningbutton(float amount)
    {
        ReduceMiningFatigue(amount);
    }

}
