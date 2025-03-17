using System;
using System.Collections;
using System.Collections.Generic;
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
        if (Input.GetKeyDown(KeyCode.K))
        {
            foreach (var modifier in playerModifiers)
            {
                AddModifier(modifier);
            }
        }

        UpdateModifiers();
    }

    public void AddModifier(ModifierBase modifier)
    {
        activeModifiers.Add(modifier);
        modifier.ApplyMod();        
    }

    public void RemoveModifier(ModifierBase modifier)
    {        
        modifier.elapsedTime = 0f;
        activeModifiers.Remove(modifier);                
    }

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
}
