using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChariotWin : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("GoldChariot"))
        {
            print("HERE");
            GameManager.Instance.isChariotWin = true;
        }
    }

}
