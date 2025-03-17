using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifierBase : MonoBehaviour
{
    public string Name { get; protected set; }
    public float Duration { get; protected set; } // 지속 시간 (0이면 영구적)
    public float elapsedTime = 0f; // 경과 시간
    public bool isActive = false;

    public ModifierBase(string name, float duration)
    {
        Name = name;
        Duration = duration;
    }

    public virtual void ApplyMod() // 효과 적용
    { 
        isActive = true; 
    }

    public virtual void RemoveMod() // 효과 제거
    {
        isActive = false;
    }

    public virtual void UpdateMod()  // 지속 효과 갱신
    {
        if (!isActive) return;
            elapsedTime += Time.deltaTime;

        if (Duration > 0 && elapsedTime >= Duration)
            RemoveMod(); // 지속 시간이 지나면 제거
    }
}
