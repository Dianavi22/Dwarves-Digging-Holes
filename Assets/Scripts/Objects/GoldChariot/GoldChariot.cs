using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldChariot : MonoBehaviour
{
    public int goldCount;
    
    public void addGold(int value)
    {
        goldCount += value;
    }
    
    public void removeGold(int value)
    {
        goldCount -= value;
    }
}
