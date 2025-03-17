using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settlement : MonoBehaviour
{
    public int checkLayer;
    private void Start()
    {
        checkLayer = LayerMask.NameToLayer("BuildObject");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == checkLayer)
        {
            BuildObject obj = other.gameObject.GetComponent<BuildObject>();
            if (obj != null)
            {
                obj.isSafe = true;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == checkLayer)
        {
            BuildObject obj = other.gameObject.GetComponent<BuildObject>();
            if (obj != null)
            {
                obj.isSafe = false;
            }
        }
    }
    
}
