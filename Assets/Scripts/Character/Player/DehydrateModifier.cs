using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DehydrateModifier : ModifierBase
{
    public float dehydrateDuration;
    public float damagePerSecond;

    public DehydrateModifier(float duration, float damagePerSecond)
        : base("Dehydrate", duration)
    {
        this.damagePerSecond = damagePerSecond;
    }

    public void Awake()
    {
        Duration = dehydrateDuration;
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
        if (isActive) CharacterManager.Instance.Player.condition.uiCondition.thirst.curValue -= damagePerSecond * Time.deltaTime; // 지속 데미지 적용
    }
}
