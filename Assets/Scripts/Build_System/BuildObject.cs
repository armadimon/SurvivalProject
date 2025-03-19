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
    public bool isSafe = false;
    public string Category { get; private set; }
    public int Health { get; private set; } = 100;
    public bool IsInsideSettlement { get; set; }
    
    public event Action OnSetChanged;

    private bool _isSet = false;
    public bool IsSet
    {
        get { return _isSet; }
        set
        {
            if (_isSet != value)
            {
                _isSet = value;
                OnSetChanged?.Invoke(); // 이벤트 발생
            }
        }
    }
    

    private void Awake()
    {
        originalRotation = transform.rotation;
    }
    public void TakeDamage(int amount)
    {
        Health -= amount;
        if (Health <= 0)
        {
            SettlementManager.Instance.RemoveBuildObject(this);
            Invoke("InvokeDestroy", 1f);
        }
    }

    private void InvokeDestroy()
    {
        Destroy(gameObject);
    }

    public void Repair(int amount)
    {
        Health += amount;
    }
}
