using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonModifier : ModifierBase
{   
    public float poisonDuration;
    public float damagePerSecond;

    public PoisonModifier(float duration, float damagePerSecond)
        : base("Poison", duration)
    {                       
        this.damagePerSecond = damagePerSecond;
    }

    public void Awake()
    {
        Duration = poisonDuration;
    }

    public override void ApplyMod() // 효과 적용
    {
        base.ApplyMod();
    }

    public override void RemoveMod() // 효과 제거
    {
        base.RemoveMod();
    }

    public override void UpdateMod()
    {
        base.UpdateMod();
        if (isActive) CharacterManager.Instance.Player.condition.uiCondition.health.curValue -= damagePerSecond * Time.deltaTime; // 지속 데미지 적용
    }
}
