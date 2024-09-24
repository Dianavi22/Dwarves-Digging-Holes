using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class FatigueChangedEvent : UnityEvent<float, float> { }

public class PlayerFatigue : MonoBehaviour
{
    public FatigueChangedEvent onCartsFatigueChanged;
    public FatigueChangedEvent onMiningFatigueChanged;

    public float maxCartsFatigue = 60;
    public float maxMiningFatigue = 60;

    public float regenSpeedCartsFatigue = 0.05f;
    public float regenSpeedMiningFatigue = 0.05f;

    public float currentCartsFatigue;
    public float currentMiningFatigue;


    void Start()
    {
        currentCartsFatigue = maxCartsFatigue;
        currentMiningFatigue = maxMiningFatigue;  

        onCartsFatigueChanged.Invoke(currentCartsFatigue, maxCartsFatigue);
        onMiningFatigueChanged.Invoke(currentMiningFatigue, maxMiningFatigue);
    }

    void Update()
    {
        UpdateFatigue(ref currentCartsFatigue, maxCartsFatigue, regenSpeedCartsFatigue);
        UpdateFatigue(ref currentMiningFatigue, maxMiningFatigue, regenSpeedMiningFatigue);

        currentCartsFatigue = Mathf.Clamp(currentCartsFatigue, 0, maxCartsFatigue);
        currentMiningFatigue = Mathf.Clamp(currentMiningFatigue, 0, maxMiningFatigue);
    
        onCartsFatigueChanged.Invoke(currentCartsFatigue, maxCartsFatigue);
        onMiningFatigueChanged.Invoke(currentMiningFatigue, maxMiningFatigue);
    }


    private void UpdateFatigue(ref float currentFatigue, float maxFatigue, float regenSpeed)
    {
        if (currentFatigue < maxFatigue)
        {
            currentFatigue = Mathf.MoveTowards(currentFatigue, maxFatigue, Time.deltaTime * regenSpeed * maxFatigue);
        }
    }

    private bool ReduceFatigue(ref float currentFatigue, float maxFatigue, float amount)
    {
        if (amount <= currentFatigue)
        {
            currentFatigue -= amount;
            return true;
        }
        return false;
    }

    private void ReduceFatigueOverTime(ref float currentFatigue, float maxFatigue, float reductionRatePerSecond)
    {
        if (currentFatigue > 0)
        {
            currentFatigue -= reductionRatePerSecond * Time.deltaTime;

            currentFatigue = Mathf.Max(currentFatigue, 0);
        }
    }


    private bool IncreaseFatigue(ref float currentFatigue, float maxFatigue, float amount)
    {
        currentFatigue += amount;
        if (currentFatigue > maxFatigue)
        {
            currentFatigue = maxFatigue;
            return true;
        }
        return false;
    }


    public bool ReduceCartsFatigue(float amount)
    {
        return ReduceFatigue(ref currentCartsFatigue, maxCartsFatigue, amount);
    }

    public

    public void IncreaseCartsFatigue(float amount)
    {
        IncreaseFatigue(ref currentCartsFatigue, maxCartsFatigue, amount);
    }

    public bool ReduceMiningFatigue(float amount)
    {
        return ReduceFatigue(ref currentMiningFatigue, maxMiningFatigue, amount);
    }

    public void IncreaseMiningFatigue(float amount)
    {
        IncreaseFatigue(ref currentMiningFatigue, maxMiningFatigue, amount);
    }




}
