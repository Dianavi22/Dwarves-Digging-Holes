using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerInformationManager : MonoBehaviour
{
    public Image imagePlayer;
    public Image imageIcon;


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




    void Start()
    {
        currentLife = maxLife;
        currentCartsFatigue = maxCartsFatigue;
        currentMiningFatigue = maxMiningFatigue;        
    }

    void Update()
    {

        ratioLife = currentLife / maxLife;
        lifeBar.fillAmount = Mathf.MoveTowards(lifeBar.fillAmount, ratioLife, Time.deltaTime);
        lifeText.text = "" + (int)currentLife;


        if (currentCartsFatigue < maxCartsFatigue){currentCartsFatigue = Mathf.MoveTowards(currentCartsFatigue, maxCartsFatigue, Time.deltaTime * 0.05f * maxCartsFatigue);}
        ratioCartsFatigue = currentCartsFatigue / maxCartsFatigue;
        cartsFatigueBar.fillAmount = ratioCartsFatigue;
        cartsFatigueText.text = "" + (int)currentCartsFatigue;



        if (currentMiningFatigue < maxMiningFatigue){currentMiningFatigue = Mathf.MoveTowards(currentMiningFatigue, maxMiningFatigue, Time.deltaTime * 0.05f * maxMiningFatigue);}
        ratioMiningFatigue = currentMiningFatigue / maxMiningFatigue;
        miningFatigueBar.fillAmount = ratioMiningFatigue;
        miningFatigueText.text = "" + (int)currentMiningFatigue;



        if (currentLife <= 0)
        {
            currentLife = 0;
            // ! Dead !
        }

        if (currentLife > maxLife)
        {
            currentLife = maxLife;
            // * Too much Alive !
        }

        if (currentCartsFatigue < 0)
        {
            currentCartsFatigue = 0;
            // ^ no Carts Fatigue
        }

        if (currentMiningFatigue < 0)
        {
            currentMiningFatigue = 0;
            // ^ no Mining Fatigue
        }

    }

    public void Damage(float damage)
    {
        currentLife -= damage;
    }

    public void Health (float health)
    {
        currentLife += health;
    }


    public void ReduceCartsFatigue(float reduceCartsFatigue)
    {
        if (reduceCartsFatigue <= currentCartsFatigue)
        {
            currentCartsFatigue -= reduceCartsFatigue;
            cartsFatigueBar.fillAmount -= reduceCartsFatigue / maxCartsFatigue;
        }
        else
        {
            // * Not enough Cards Fatigue !
        }
        
    }

    public void ReduceMiningFatigue(float reduceMiningFatigue)
    {
        if (reduceMiningFatigue <= currentMiningFatigue)
        {
            currentMiningFatigue -= reduceMiningFatigue;
            miningFatigueBar.fillAmount -= reduceMiningFatigue / maxMiningFatigue;
        }
        else
        {
            // * Not enough Mining Fatigue !
        }
        
    }

    public void IncreaseCartsFatigue(float increaseCartsFatigue)
    {
        currentCartsFatigue += increaseCartsFatigue;

        if(currentCartsFatigue > maxCartsFatigue)
        {
            currentCartsFatigue = maxCartsFatigue;
        }

        cartsFatigueBar.fillAmount = currentCartsFatigue / maxCartsFatigue;
        
    }

    public void IncreaseMiningFatigue(float increaseMiningFatigue)
    {
        currentMiningFatigue += increaseMiningFatigue;

        if(currentMiningFatigue > maxMiningFatigue)
        {
            currentMiningFatigue = maxMiningFatigue;
        }

        miningFatigueBar.fillAmount = currentMiningFatigue / maxMiningFatigue;
        
    }


}
