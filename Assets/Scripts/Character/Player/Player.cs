using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerController controller;

    public ItemData item;

    public Action addItem;

    public Transform dropPosition;

    public PlayerCondition condition;

    public Equipment equipment;

    public List<ModifierBase> playerModifiers;
    [SerializeField] private List<ModifierBase> activeModifiers = new List<ModifierBase>();   

    private void Awake()
    {
        CharacterManager.Instance.Player = this;
        controller = GetComponent<PlayerController>();
        condition = GetComponent<PlayerCondition>();
        equipment = GetComponent<Equipment>();
        playerModifiers = new List<ModifierBase>(GetComponents<ModifierBase>());             
    }

    private void Update()
    {        
        if (OnPoisoned())
        {
            for (int i = 0; i < playerModifiers.Count; i++)
            {
                if (playerModifiers[i].TryGetComponent(out PoisonModifier posion))
                {
                    AddModifier(playerModifiers[i]);
                    activeModifiers = activeModifiers.Distinct().ToList();
                }
            }
        }

        UpdateModifiers();
    }

    // modifier 활성화
    public void AddModifier(ModifierBase modifier)
    {
        activeModifiers.Add(modifier);
        modifier.ApplyMod();        
    }

    // modifier 비활성화
    public void RemoveModifier(ModifierBase modifier)
    {        
        modifier.elapsedTime = 0f;
        activeModifiers.Remove(modifier);                
    }

    // modifier 상태업데이트(duration 지나면 자동 비활성화도 포함)
    private void UpdateModifiers()
    {
        if (activeModifiers.Count > 0)
        {
            for (int i = 0; i < activeModifiers.Count; i++)
            {
                activeModifiers[i].UpdateMod();                

                if (activeModifiers[i].TryGetComponent(out PoisonModifier poison))
                {                    
                    if (activeModifiers[i].isActive) condition.indicatorAnimator.SetBool("OnPoison", true);
                    else condition.indicatorAnimator.SetBool("OnPoison", false);
                }

                if (!activeModifiers[i].isActive) RemoveModifier(activeModifiers[i]);
            }
        }
    }
    
    // 중독상태 bool 메서드 (확률 25%)
    public bool OnPoisoned()
    {
        if (condition.isHealing && condition.poisonProbabilityInt < 24) return true;            
        else return false;
    }
}
