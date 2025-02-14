using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TheEndObj : MonoBehaviour
{

    public ParticleSystem _lavaFloorPart;
    public ParticleSystem _breakFlorPart;
    public ParticleSystem _winGoldPart;
    public ParticleSystem _winGoldCornerPart;
    public int emission;
    public  GameObject _pointDirectionGoldWinPart;

    public ShakyCame _sc;
    private bool _isLavaMoving = false;

    private void PartRate()
    {
        emission = TargetManager.Instance.GetComponent<GoldChariot>()._currentGoldCount;
        var emissionModule = _winGoldPart.emission;
        emissionModule.rateOverTime = emission;
    }

    private void Update()
    {
        _winGoldPart.transform.LookAt(_pointDirectionGoldWinPart.transform);

    }
}
