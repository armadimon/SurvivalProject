using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuildObjectType
{
    Base,
    Final,
}

[CreateAssetMenu(fileName = "Build_Object", menuName = "New Build_Object")]
public class BuildObjectData : ScriptableObject
{
    [Header("Info")]
    public string displayName;
    public string description;
    public int maxHealth;
    public float maxSlopeAngle = 45f;
    public float minSlopeAngle = 10f;
    public BuildObjectType type;
    public Sprite icon;
    public GameObject prefabs;
    public RequireResourceAmount[] requireResources;
    
    public Action OnClick;

}