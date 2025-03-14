using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuildObjectType
{
    Base,
    Final,
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
    public RequireResourceAmount[] requireResources;
    
    public Action OnClick;

}