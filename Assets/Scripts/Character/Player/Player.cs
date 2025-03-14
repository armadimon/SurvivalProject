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
    private void Awake()
    {
        CharacterManager.Instance.Player = this;
        controller = GetComponent<PlayerController>();
        condition = GetComponent<PlayerCondition>();
        equipment = GetComponent<Equipment>();
    }
}
