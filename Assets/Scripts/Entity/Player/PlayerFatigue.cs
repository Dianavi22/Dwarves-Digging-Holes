using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class FatigueChangedEvent : UnityEvent<float, float> { }

public class PlayerFatigue : MonoBehaviour
{
    [HideInInspector] public FatigueChangedEvent onCartsFatigueChanged;
    [HideInInspector] public FatigueChangedEvent onMiningFatigueChanged;

    public float maxCartsFatigue = 60;
    public float maxMiningFatigue = 60;

    public float regenSpeedCartsFatigue = 0.05f;
    public float regenSpeedMiningFatigue = 0.05f;

    public float reduceSpeedCartsFatigue = 0.05f;
    public float reduceSpeedMiningFatigue = 0.5f;

    public bool isReducingCartsFatigue = false;
    public bool isCartsFatigue = false;

    public float currentCartsFatigue;
    public float currentMiningFatigue;
    private float regenDelayCartsFatigue = 0f;
    private float regenDelayMiningFatigue = 0f;

    [SerializeField] private float regenDelayCartsFatigueMax = 2f;
    [SerializeField] private float regenDelayMiningFatiMax = 2f;

    void Start()
    {
        currentCartsFatigue = maxCartsFatigue;
        currentMiningFatigue = maxMiningFatigue;  

        InvokeOnCartsFatigueChanged();
        InvokeOnMiningFatigueChanged();
    }

    void Update()
    {
        RegenCartsFatigueOverTime();
        RegenMiningFatigueOverTime();

        if (isReducingCartsFatigue)
        {
           if (ReduceCartsFatigueOverTime()){isCartsFatigue = true; Debug.Log("isCartsFatigue = " + isCartsFatigue);}
           else isCartsFatigue = false; Debug.Log("isCartsFatigue = " + isCartsFatigue);
        }

        //ReduceCartsFatigueOverTime();
        //ReduceMiningFatigueOverTime();
        //ReduceFatigueOverTime(ref currentCartsFatigue, maxCartsFatigue, reduceSpeedCartsFatigue);
        //InvokeOnMiningFatigueChanged();
    }

    private void InvokeOnCartsFatigueChanged()
    {
        onCartsFatigueChanged.Invoke(currentCartsFatigue, maxCartsFatigue);
    }
    private void InvokeOnMiningFatigueChanged()
    {
        onMiningFatigueChanged.Invoke(currentMiningFatigue, maxMiningFatigue);
    }

    // * Regeneration fatigue over time

    private void RegenerationFatigueOverTime(ref float currentFatigue, float maxFatigue, float regenSpeed)
    {
        if (currentFatigue < maxFatigue)
        {
            currentFatigue = Mathf.MoveTowards(currentFatigue, maxFatigue, Time.deltaTime * regenSpeed * maxFatigue);
            //currentFatigue = Mathf.Clamp(currentFatigue + regenSpeed * Time.deltaTime, 0, maxFatigue);
        }
    }
    private void RegenCartsFatigueOverTime()
    {
        if (regenDelayCartsFatigue > 0)
        {
            regenDelayCartsFatigue -= Time.deltaTime;
        }
        else
        {
            RegenerationFatigueOverTime(ref currentCartsFatigue, maxCartsFatigue, regenSpeedCartsFatigue);
            InvokeOnCartsFatigueChanged();
        }
    }

    private void RegenMiningFatigueOverTime()
    {
        if (regenDelayMiningFatigue > 0)
        {
            regenDelayMiningFatigue -= Time.deltaTime;
        }
        else
        {
            RegenerationFatigueOverTime(ref currentMiningFatigue, maxMiningFatigue, regenSpeedMiningFatigue);
            InvokeOnMiningFatigueChanged();
        }
    }

    private void ResetDelayRegenCartsFatigue()
    {
        regenDelayCartsFatigue = regenDelayCartsFatigueMax;
    }

    private void ResetDelayRegenMiningFatigue()
    {
        regenDelayMiningFatigue = regenDelayMiningFatiMax;
    }


    // * Reduce Fatigue OverTime

    private bool ReduceFatigueOverTime(ref float currentFatigue, float maxFatigue, float reductionRatePerSecond)
    {
        if (currentFatigue > 0)
        {
            currentFatigue = Mathf.Clamp(currentFatigue - reductionRatePerSecond * Time.deltaTime, 0, maxFatigue);
            //Debug.Log("ReduceFatigueOverTime activé");

            return true;
        }
        //Debug.Log("ReduceFatigueOverTime désactiver");
        return false;
    }

    public bool ReduceCartsFatigueOverTime()
    {
        if (ReduceFatigueOverTime(ref currentCartsFatigue, maxCartsFatigue, reduceSpeedCartsFatigue))
        {
            InvokeOnCartsFatigueChanged();
            ResetDelayRegenCartsFatigue();
            //Debug.Log("ReduceCartsFatigueOverTime activé");
            return true;
        }
        //Debug.Log("ReduceCartsFatigueOverTime désactivé");
        return false;
    }

    public void StartReducingCartsFatigue()
    {
        isReducingCartsFatigue = true;
    }

    public void StopReducingCartsFatigue()
    {
        isReducingCartsFatigue = false;
    }


    public bool ReduceMiningFatigueOverTime()
    {
        if (ReduceFatigueOverTime(ref currentMiningFatigue, maxMiningFatigue, reduceSpeedMiningFatigue))
        {
            RegenMiningFatigueOverTime();
            ResetDelayRegenMiningFatigue();
            //Debug.Log("ReduceFatigueOverTime activé");
            return true;
        }
        //Debug.Log("ReduceFatigueOverTime désactivé");
        return false;
    }


    // * Increases fatigue instantly

    private bool IncreaseFatigue(ref float currentFatigue, float maxFatigue, float amount)
    {
        currentFatigue = Mathf.Min(currentFatigue + amount, maxFatigue);
        return true;
    }

    public bool IncreaseCartsFatigue(float amount)
    {
        if (IncreaseFatigue(ref currentCartsFatigue, maxCartsFatigue, amount))
        {
            InvokeOnCartsFatigueChanged();
            return true;
        }
        return false;
    }

    public bool IncreaseMiningFatigue(float amount)
    {

        if (IncreaseFatigue(ref currentMiningFatigue, maxMiningFatigue, amount))
        {
            InvokeOnMiningFatigueChanged();
            return true;
        }
        return false;
    }


    // * Reduces fatigue instantly

    private bool ReduceFatigue(ref float currentFatigue, float amount)
    {
        if (amount <= currentFatigue)
        {
            currentFatigue -= amount;
            return true;
        }
        return false;
    }

    public bool ReduceCartsFatigue(float amount)
    {
        if (ReduceFatigue(ref currentCartsFatigue, amount))
        {
            InvokeOnCartsFatigueChanged();
            ResetDelayRegenCartsFatigue();
            return true;
        }
        return false;
    }

    public bool ReduceMiningFatigue(float amount)
    { 
        if (ReduceFatigue(ref currentMiningFatigue, amount))
        {
            InvokeOnMiningFatigueChanged();
            ResetDelayRegenMiningFatigue();
            return true;
        }
        return false;
    }
}
