using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GoldChariot : MonoBehaviour
{
    public int goldCount;
    [SerializeField] private TMP_Text _goldCoundText;

    private void Update()
    {
        _goldCoundText.text = goldCount.ToString();
    }

    public void addGold(int value)
    {
        goldCount += value;
    }
    
    public void removeGold(int value)
    {
        goldCount -= value;
    }
}
