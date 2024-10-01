using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class Beer : MonoBehaviour
{

    public bool breakable = false;
    public Action throwOnDestroy;

    /// <summary>
    /// This function is called when the MonoBehaviour will be destroyed.
    /// </summary>
    void OnDestroy()
    {
        throwOnDestroy?.Invoke();
    }

    /// <summary>
    /// OnTriggerEnter is called when the Collider other enters the trigger.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    void OnTriggerEnter(Collider other)
    {
        if (!breakable)
        {
            return;
        }


        /*
        * A voir qu'est ce qui pourrait briser la bière
        */
        // if(other.gameObject.CompareTag("Ground")) {
        //     BreakBeer();
        // }

        if (other.gameObject.CompareTag("Player"))
        {
            // TODO: Faire l'effet de la bière ?
            Debug.Log("Effect");
            BreakBeer();

        }
        else
        {
            BreakBeer();
        }
    }

    void BreakBeer()
    {
        /*
        TODO: Play Animation
        */
        Destroy(gameObject);
    }
}
