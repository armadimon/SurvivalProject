using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public PlayerController controller;

    public ItemData item;

    public Action addItem;

    public Transform dropPosition;

    public PlayerCondition condition;

    public Equipment equipment;

    public List<ModifierBase> playerModifiers;
    public List<ModifierBase> activeModifiers = new List<ModifierBase>();

    public GameObject poisonedIcon;
    public GameObject dehydrateIcon;

    [Range(0, 100)] public int poisonProbability;
    [Range(0, 100)] public int dehydrateProbability;  

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
        PoisonModifier();
        DehydrateModifier();

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
        activeModifiers.Remove(modifier);
        modifier.elapsedTime = 0f;
    }

    // modifier 상태업데이트(duration 지나면 자동 비활성화도 포함)
    private void UpdateModifiers()
    {
        if (activeModifiers.Count > 0)
        {
            for (int i = 0; i < activeModifiers.Count; i++)
            {
                activeModifiers[i].UpdateMod();                

                if (activeModifiers[i] is PoisonModifier)
                {                    
                    if (activeModifiers[i].isActive)
                    {
                        condition.indicatorAnimator.SetBool("OnPoison", true);
                        poisonedIcon.transform.SetAsFirstSibling();
                        poisonedIcon.SetActive(true);
                    }
                    else
                    {
                        condition.indicatorAnimator.SetBool("OnPoison", false);
                        poisonedIcon.SetActive(false);
                        RemoveModifier(activeModifiers[i]);
                    }
                }
                
                if (activeModifiers[i] is DehydrateModifier)
                {
                    if (activeModifiers[i].isActive)
                    {
                        condition.indicatorAnimator.SetBool("OnThirst", true);
                        dehydrateIcon.transform.SetAsFirstSibling();
                        dehydrateIcon.SetActive(true);
                    }
                    else
                    {
                        condition.indicatorAnimator.SetBool("OnThirst", false);
                        dehydrateIcon.SetActive(false);
                        RemoveModifier(activeModifiers[i]);
                    }
                }
            }
        }
        else return;
    }
    
    // 중독상태 bool 메서드 (확률 25%)
    public bool OnPoisoned()
    {
        if (condition.isHealing && condition.poisonProbabilityInt < poisonProbability) return true;            
        else return false;
    }

    // activeModifiers에 중독 이미 있는지 확인
    public bool PoisonAlready()
    {
        for (int i = 0; i < activeModifiers.Count; i++)
        {
            if (activeModifiers[i] is PoisonModifier) return true;
        }

        return false;
    }

    // 중독 modifier 적용
    public void PoisonModifier()
    {
        if (OnPoisoned() && !PoisonAlready())
        {
            for (int i = 0; i < playerModifiers.Count; i++)
            {
                if (playerModifiers[i] is PoisonModifier)
                {
                    AddModifier(playerModifiers[i]);
                }
            }
        }
    }

    // 탈수상태 bool 메서드 (확률 25%)
    public bool OnDehydrated()
    {
        if (condition.isHydrating && condition.dehydrateProbabilityInt < dehydrateProbability) return true;
        else return false;
    }

    // activeModifiers에 중독 이미 있는지 확인
    public bool DehydrateAlready()
    {
        for (int i = 0; i < activeModifiers.Count; i++)
        {
            if (activeModifiers[i] is DehydrateModifier) return true;
        }

        return false;
    }

    // 탈수 modifier 적용
    public void DehydrateModifier()
    {
        if (OnDehydrated() && !DehydrateAlready())
        {
            for (int i = 0; i < playerModifiers.Count; i++)
            {
                if (playerModifiers[i] is DehydrateModifier)
                {
                    AddModifier(playerModifiers[i]);                    
                }
            }
        }
    }
}
