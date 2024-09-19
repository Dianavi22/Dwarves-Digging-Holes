using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Beer : MonoBehaviour
{

    public bool breakable = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// OnCollisionEnter is called when this collider/rigidbody has begun
    /// touching another rigidbody/collider.
    /// </summary>
    /// <param name="other">The Collision data associated with this collision.</param>
    void OnCollisionEnter(Collision other)
    {
        if(!breakable) {
            return;
        }
        
        /*
        * A voir qu'est ce qui pourrait briser la bière
        */
        // if(other.gameObject.CompareTag("Ground")) {
        //     BreakBeer();
        // }

            if(other.gameObject.CompareTag("Player")) {
                // TODO: Faire l'effet de la bière ?
                Debug.Log("Effect");
                BreakBeer();

            }
            else {
                BreakBeer();
            }
    }

    void BreakBeer() {
        /*
        TODO: Play Animation
        */
        Destroy(gameObject);
    }
}
