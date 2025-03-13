using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildObject : MonoBehaviour
{
    public Transform snapPointGroup;
    public List<Transform> snapPoints = new List<Transform>(); 
    public BuildObjectData data;
    
    private void Awake()
    {
        if (snapPointGroup != null)
        {
            snapPoints.AddRange(snapPointGroup.GetComponentsInChildren<Transform>());
        }
    }
}
