using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    [HideInInspector]
    public float movementSpeed = 3f;
    
    private void Update()
    {
        transform.position -= new Vector3(movementSpeed, 0f, 0f) * Time.deltaTime;
    }
}
