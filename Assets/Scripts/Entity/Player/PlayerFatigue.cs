using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class FatigueChangedEvent : UnityEvent<float, float> { }

public class PlayerFatigue : MonoBehaviour
{
    [HideInInspector] public FatigueChangedEvent onCartsFatigueChanged;
    [HideInInspector] public FatigueChangedEvent onMiningFatigueChanged;
    [HideInInspector] public float currentCartsFatigue;
    [HideInInspector] public float currentMiningFatigue;

    [SerializeField] ParticleSystem _fatiguePart;

    private PlayerFatigueData _miningData;
    private PlayerFatigueData _pushCartData;

    private float regenDelayCartsFatigue;
    private float regenDelayMiningFatigue;

    public float MaxMiningFatigue => _miningData.MaxFatigue;
    public float MaxPushCartFatigue => _pushCartData.MaxFatigue;

    void Start()
    {
        _miningData = GameManager.Instance.Difficulty.MiningFatigue;
        _pushCartData = GameManager.Instance.Difficulty.PushCartFatigue;

        currentCartsFatigue = MaxPushCartFatigue;
        currentMiningFatigue = MaxMiningFatigue;

        ResetDelayRegenCartsFatigue();
        ResetDelayRegenMiningFatigue();

        InvokeOnCartsFatigueChanged();
        InvokeOnMiningFatigueChanged();
    }

    void Update()
    {
        RegenCartsFatigueOverTime();
        RegenMiningFatigueOverTime();

        //TODO : Fix _fatiguePart
        if (currentCartsFatigue <= 50 || currentMiningFatigue <= 50)
        {
            _fatiguePart.Play();
        }
        else
        {
            _fatiguePart.Stop();
        }
    }

    private void InvokeOnCartsFatigueChanged()
    {
        onCartsFatigueChanged.Invoke(currentCartsFatigue, MaxPushCartFatigue);
    }
    private void InvokeOnMiningFatigueChanged()
    {
        onMiningFatigueChanged.Invoke(currentMiningFatigue, MaxMiningFatigue);
    }

    private void ResetDelayRegenCartsFatigue()
    {
        regenDelayCartsFatigue = _pushCartData.RegenDelay;
    }

    private void ResetDelayRegenMiningFatigue()
    {
        regenDelayMiningFatigue = _miningData.RegenDelay;
    }

    #region Regenerate fatigue over time
    private void RegenFatigueOverTime(ref float currentFatigue, float maxFatigue, float regenSpeed)
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
            RegenFatigueOverTime(ref currentCartsFatigue, MaxPushCartFatigue, _pushCartData.RegenByFrame);
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
            RegenFatigueOverTime(ref currentMiningFatigue, MaxMiningFatigue, _miningData.RegenByFrame);
            InvokeOnMiningFatigueChanged();
        }
    }
    #endregion

    #region Increases fatigue instantly
    private bool IncreaseFatigue(ref float currentFatigue, float maxFatigue, float amount)
    {
        currentFatigue = Mathf.Min(currentFatigue + amount, maxFatigue);
        return true;
    }

    public bool IncreaseCartsFatigue(float amount)
    {
        if (IncreaseFatigue(ref currentCartsFatigue, MaxPushCartFatigue, amount))
        {
            InvokeOnCartsFatigueChanged();
            return true;
        }
        return false;
    }

    public bool IncreaseMiningFatigue(float amount)
    {

        if (IncreaseFatigue(ref currentMiningFatigue, MaxMiningFatigue, amount))
        {
            InvokeOnMiningFatigueChanged();
            return true;
        }
        return false;
    }
    #endregion

    #region Reduces fatigue instantly
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
    #endregion
}
