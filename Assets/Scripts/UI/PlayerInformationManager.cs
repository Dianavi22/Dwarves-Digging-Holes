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

    public float regenSpeedCartsFatigue = 0.05f;
    public float regenSpeedMiningFatigue = 0.05f;

    private float currentLife;
    private float currentCartsFatigue;
    private float currentMiningFatigue;

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
        UpdateFatigue(ref currentCartsFatigue, maxCartsFatigue, cartsFatigueBar, cartsFatigueText, regenSpeedCartsFatigue);
        UpdateFatigue(ref currentMiningFatigue, maxMiningFatigue, miningFatigueBar, miningFatigueText, regenSpeedMiningFatigue);

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

        private void UpdateFatigue(ref float currentFatigue, float maxFatigue, Image fatigueBar, TMP_Text fatigueText, float regenSpeed)
    {
        if (currentFatigue < maxFatigue)
        {
            currentFatigue = Mathf.MoveTowards(currentFatigue, maxFatigue, Time.deltaTime * regenSpeed * maxFatigue);
        }

        UpdateBar(currentFatigue, maxFatigue, fatigueBar, fatigueText, regenSpeed);
    }

        public void CheckPlayerLife()
    {
        if (currentLife == 0)
        {
            Debug.Log("The player is dead.");
            PlayerDeath();
        }
    }

    void PlayerDeath()
    {
        Debug.Log("Execution of player death actions.");
    }

    public void Damage(float damage)
    {
        currentLife = Mathf.Clamp(currentLife - damage, 0, maxLife);
        CheckPlayerLife(); 
    }

    public void Health(float health)
    {
        currentLife = Mathf.Clamp(currentLife + health, 0, maxLife);
    }

    private bool ReduceFatigue(ref float currentFatigue, float maxFatigue, float amount, Image fatigueBar)
    {
        if (amount <= currentFatigue)
        {
            currentFatigue -= amount;
            fatigueBar.fillAmount = currentFatigue / maxFatigue;
            return true;
        }
        return false;
    }
    private void IncreaseFatigue(ref float currentFatigue, float maxFatigue, float amount, Image fatigueBar)
    {
        currentFatigue += amount;
        if (currentFatigue > maxFatigue)
        {
            currentFatigue = maxFatigue;
        }

        fatigueBar.fillAmount = currentFatigue / maxFatigue;
    }

    public bool ReduceCartsFatigue(float amount)
    {
        return ReduceFatigue(ref currentCartsFatigue, maxCartsFatigue, amount, cartsFatigueBar);
    }

    public void IncreaseCartsFatigue(float amount)
    {
        IncreaseFatigue(ref currentCartsFatigue, maxCartsFatigue, amount, cartsFatigueBar);
    }

    public bool ReduceMiningFatigue(float amount)
    {
        return ReduceFatigue(ref currentMiningFatigue, maxMiningFatigue, amount, miningFatigueBar);
    }

    public void IncreaseMiningFatigue(float amount)
    {
        IncreaseFatigue(ref currentMiningFatigue, maxMiningFatigue, amount, miningFatigueBar);
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
