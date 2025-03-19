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
    
    public Dictionary<string, SettlementItem> activeUIItems = new Dictionary<string, SettlementItem>();
    
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
        InvokeRepeating(nameof(UpdateUI), 5 , 5);
        InvokeRepeating(nameof(ReSettingBuildObject), 5, 5);
        InvokeRepeating(nameof(ApplyDamageToObjects), damageInterval, damageInterval);
    }

    private void ReSettingBuildObject()
    {
        if (settlement != null)
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
    }

    private void UpdateUI()
    {
        // 같은 이름을 가진 개수를 집계
        Dictionary<string, int> buildObjectCounts = new Dictionary<string, int>();

        foreach (BuildObject obj in InBoundBuildObjectsList)
        {
            if (buildObjectCounts.ContainsKey(obj.data.displayName))
            {
                buildObjectCounts[obj.data.displayName]++;
            }
            else
            {
                buildObjectCounts[obj.data.displayName] = 1;
            }
        }

        // UI 요소 추가 및 업데이트
        foreach (var entry in buildObjectCounts)
        {
            if (activeUIItems.ContainsKey(entry.Key))
            {
                // 기존 UI가 있으면 개수만 업데이트
                activeUIItems[entry.Key].SetData(entry.Key, entry.Value);
            }
            else
            {
                // 새 UI 요소 생성
                SettlementItem sItem = Instantiate(settlementItemPrefabs, SettlementMenuContent);
                sItem.SetData(entry.Key, entry.Value);
                activeUIItems[entry.Key] = sItem;
            }
        }

        // UI에서 사라진 항목 제거
        List<string> keysToRemove = new List<string>();
        foreach (var key in activeUIItems.Keys)
        {
            if (!buildObjectCounts.ContainsKey(key))
            {
                Destroy(activeUIItems[key].gameObject);
                keysToRemove.Add(key);
            }
        }
        foreach (var key in keysToRemove)
        {
            activeUIItems.Remove(key);
        }
    }

    
    public void RegisterBuildObject(BuildObject obj, bool inSettlement)
    {
        
        if (inSettlement)
        {
            if (!InBoundBuildObjectsList.Contains(obj))
            {
                InBoundBuildObjectsList.Add(obj);
                // SettlementItem sItem = Instantiate(settlementItemPrefabs, SettlementMenuContent);
                // SettlementItems.Add(sItem);
                // sItem.SetData(obj);
            }
        }
        else
        {
            if (!OutBoundBuildObjectsList.Contains(obj))
            {
                OutBoundBuildObjectsList.Add(obj);
            }
        }
        UpdateUI();
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
        UpdateUI();
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

    public void DisplaySettlementManageMenu()
    {
        if (settlementManagementMenu.activeSelf)
        {
            CharacterManager.Instance.Player.controller.ToggleCursur();
            if (Cursor.lockState == CursorLockMode.None)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
        else
        {
            CharacterManager.Instance.Player.controller.ToggleCursur();
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
            }
        }
        settlementManagementMenu.SetActive(!settlementManagementMenu.activeSelf);
    }
}
