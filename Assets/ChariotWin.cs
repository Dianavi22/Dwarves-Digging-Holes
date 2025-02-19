using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChariotWin : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("GoldChariot"))
        {
            GameManager.Instance.isChariotWin = true;
        }
    }

}
