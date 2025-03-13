using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuildObjectType
{
    Base,
    Final,
}

public enum RequireResourceType
{
    Wood,
    Stone,
}

[Serializable]
public class BuildDataRequireResourceAmount
{
    public RequireResourceType type;
    public float value;
}


[CreateAssetMenu(fileName = "Item", menuName = "New Item")]
public class BuildObjectData : ScriptableObject
{
    [Header("Info")]
    public string displayName;
    public string description;
    public BuildObjectType type;
    public Sprite icon;
    public GameObject prefabs;
    public BuildDataRequireResourceAmount[] requireResources;
    
    public Action OnClick;

}