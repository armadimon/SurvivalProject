using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildObject : MonoBehaviour
{
    public Transform snapPointGroup;
    public List<Transform> snapPoints = new List<Transform>(); 
    public BuildObjectData data;
    public Quaternion originalRotation;
    public bool isSet = false;
    
    private void Awake()
    {
        originalRotation = transform.rotation;
        if (snapPointGroup != null)
        {
            snapPoints.AddRange(snapPointGroup.GetComponentsInChildren<Transform>());
        }
    }
}
