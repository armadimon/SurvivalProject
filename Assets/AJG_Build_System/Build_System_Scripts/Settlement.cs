using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settlement : MonoBehaviour
{
    public int checkLayer;
    private void Start()
    {
        BuildObject settelBuildObject = GetComponentInParent<BuildObject>();
        settelBuildObject.isSafe = true;
        SettlementManager.Instance.RegisterBuildObject(settelBuildObject, settelBuildObject.isSafe);
        SettlementManager.Instance.settlement = this;
        checkLayer = LayerMask.NameToLayer("BuildObject");
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);
        if (other.gameObject.layer == checkLayer)
        {
            BuildObject obj = other.gameObject.GetComponent<BuildObject>();
            if (obj != null)
            {
                Debug.Log(other.gameObject.name);
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
