using System;
using System.Collections.Generic;
using UnityEngine;

public class SettlementManager : MonoBehaviour
{
    public List<BuildObject> InBoundBuildObjectsList = new List<BuildObject>();
    public List<BuildObject> OutBoundBuildObjectsList = new List<BuildObject>();
    
    public Settlement settlement;
    public GameObject settlementManagementMenu;
    public BuildObjectInfo settlementBuildObjectInfo;
    public List<SettlementItem> SettlementItems = new List<SettlementItem>();
    public SettlementItem settlementItemPrefabs;
    public BuildMenuItem buildMenuItemPrefabs;
    public Transform SettlementMenuContent;
    
    
    public float damageInterval = 5f;
    public int damageAmount = 10;

    public static SettlementManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        settlement = FindObjectOfType<Settlement>();
        InvokeRepeating(nameof(ReSettingBuildObject), 5, 5);
        InvokeRepeating(nameof(ApplyDamageToObjects), damageInterval, damageInterval);
    }

    private void ReSettingBuildObject()
    {
     
        for (int i = InBoundBuildObjectsList.Count - 1; i >= 0; i--)
        {
            BuildObject obj = InBoundBuildObjectsList[i];
            if (!obj.isSafe)
            {
                InBoundBuildObjectsList.RemoveAt(i);
                if (!OutBoundBuildObjectsList.Contains(obj))
                {
                    OutBoundBuildObjectsList.Add(obj);
                }
            }
        }
        for (int i = OutBoundBuildObjectsList.Count - 1; i >= 0; i--)
        {
            BuildObject obj = OutBoundBuildObjectsList[i];
            if (obj.isSafe)
            {
                OutBoundBuildObjectsList.RemoveAt(i);
                if (!InBoundBuildObjectsList.Contains(obj))
                {
                    InBoundBuildObjectsList.Add(obj);
                }
            }
        }
    }

    private void UpdateUI()
    {
        
    }
    
    public void RegisterBuildObject(BuildObject obj, bool inSettlement)
    {
        
        if (inSettlement)
        {
            if (!InBoundBuildObjectsList.Contains(obj))
            {
                InBoundBuildObjectsList.Add(obj);
                SettlementItem sItem = Instantiate(settlementItemPrefabs, SettlementMenuContent);
                SettlementItems.Add(sItem);
                sItem.SetData(obj);
            }
        }
        else
        {
            if (!OutBoundBuildObjectsList.Contains(obj))
            {
                OutBoundBuildObjectsList.Add(obj);
            }
        }
    }

    public void RemoveBuildObject(BuildObject obj)
    {
        if (InBoundBuildObjectsList.Contains(obj))
        {
            InBoundBuildObjectsList.Remove(obj);
            Destroy(obj.gameObject);
        }
        else if (OutBoundBuildObjectsList.Contains(obj))
        {
            OutBoundBuildObjectsList.Remove(obj);
            Destroy(obj.gameObject);
        }
    }

    public void RepairBuildObject(BuildObject obj, int repairAmount)
    {
        if (InBoundBuildObjectsList.Contains(obj))
        {
            obj.Repair(repairAmount);
        }
    }

    private void ApplyDamageToObjects()
    {
        for (int i = OutBoundBuildObjectsList.Count - 1; i >= 0; i--)
        {
            OutBoundBuildObjectsList[i].TakeDamage(damageAmount);
        }
    }
    
}
