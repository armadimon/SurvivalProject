using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settlement : MonoBehaviour
{
    public BuildObject settelBuildObject;
    public Collider settelCollider;
    public int checkLayer;
    private void Start()
    {
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
    
    void OnEnable()
    {
        if (settelBuildObject != null)
        {
            settelBuildObject.OnSetChanged += OnSettleBoundry;
        }
        else
        {
            Debug.LogError("build object is null");
        }
    }

    void OnDisable()
    {
        if (settelBuildObject != null)
        {
            settelBuildObject.OnSetChanged -= OnSettleBoundry;
        }
    }

    public void OnSettleBoundry()
    {
        GetComponent<Collider>().enabled = true;
    }
    
}
