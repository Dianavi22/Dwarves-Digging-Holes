using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallLimit : MonoBehaviour
{
    private Pickaxe _pickaxe;
    private void OnTriggerEnter(Collider other)
    {
        if (Utils.Component.TryGetInParent<Pickaxe>(other, out var pickaxe))
        {
            pickaxe.HandleDestroy();
        }
    }

    
}
