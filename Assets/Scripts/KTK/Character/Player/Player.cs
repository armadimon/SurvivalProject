using System;

using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerController controller;     // 플레이어 컨트롤러
    public PlayerCondition condition;       // 플레이어 상태
    public Transform dropPosition;          // 떨굴 위치
    //public Equipment equip;               // 장비 착용


    //public ItemData itemData;
    //public Action addItem;

    public Animator animator;
    public void Awake()
    {
        animator = GetComponent<Animator>();
        CharacterManager.Instance.Player = this; // 플레이어 캐릭터를 캐릭터 매니저에 등록
        controller = GetComponent<PlayerController>();
        condition = GetComponent<PlayerCondition>();
        //equip = GetComponent<Equipment>();
    }
}