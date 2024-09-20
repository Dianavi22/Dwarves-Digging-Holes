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

    public float maxLife;
    public float maxCartsFatigue;
    public float maxMiningFatigue;


    private float currentLife;
    private float currentCartsFatigue;
    private float currentMiningFatigue;

    private float ratioLife;
    private float ratioCartsFatigue;
    private float ratioMiningFatigue;



    public Image imagePlayer;
    public Image imageIcon;
    public Sprite beerSprite;
    public Sprite pickaxeSprite;
    public Sprite chariotSprite;



    void Start()
    {
        currentLife = maxLife;
        currentCartsFatigue = maxCartsFatigue;
        currentMiningFatigue = maxMiningFatigue;        
    }

    void Update()
    {
        UpdateBar(currentLife, maxLife, lifeBar, lifeText, 1f);
        UpdateBar(currentCartsFatigue, maxCartsFatigue, cartsFatigueBar, cartsFatigueText, 0.05f * maxCartsFatigue);
        UpdateBar(currentMiningFatigue, maxMiningFatigue, miningFatigueBar, miningFatigueText, 0.05f * maxMiningFatigue);

        currentLife = Mathf.Clamp(currentLife, 0, maxLife);
        currentCartsFatigue = Mathf.Clamp(currentCartsFatigue, 0, maxCartsFatigue);
        currentMiningFatigue = Mathf.Clamp(currentMiningFatigue, 0, maxMiningFatigue);

    }

    private void UpdateBar(float currentValue, float maxValue, Image bar, TMP_Text text, float smoothFactor)
    {
        float ratio = currentValue / maxValue;
        bar.fillAmount = Mathf.MoveTowards(bar.fillAmount, ratio, Time.deltaTime * smoothFactor);
        text.text = ((int)currentValue).ToString();
    }

    public void Damage(float damage)
    {
        currentLife = Mathf.Clamp(currentLife - damage, 0, maxLife);
    }

    public void Health(float health)
    {
        currentLife = Mathf.Clamp(currentLife + health, 0, maxLife);
    }

    private void AdjustFatigue(ref float currentFatigue, float maxFatigue, float amount, bool isIncreasing, Image fatigueBar)
    {
        if (isIncreasing)
        {
            currentFatigue += amount;
            if (currentFatigue > maxFatigue) currentFatigue = maxFatigue;
        }
        else
        {
            if (amount <= currentFatigue) currentFatigue -= amount;
        }
        fatigueBar.fillAmount = currentFatigue / maxFatigue;
    }

    public void ReduceCartsFatigue(float amount)
    {
        AdjustFatigue(ref currentCartsFatigue, maxCartsFatigue, amount, false, cartsFatigueBar);
    }

    public void IncreaseCartsFatigue(float amount)
    {
        AdjustFatigue(ref currentCartsFatigue, maxCartsFatigue, amount, true, cartsFatigueBar);
    }

    public void ReduceMiningFatigue(float amount)
    {
        AdjustFatigue(ref currentMiningFatigue, maxMiningFatigue, amount, false, miningFatigueBar);
    }

    public void IncreaseMiningFatigue(float amount)
    {
        AdjustFatigue(ref currentMiningFatigue, maxMiningFatigue, amount, true, miningFatigueBar);
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

}
