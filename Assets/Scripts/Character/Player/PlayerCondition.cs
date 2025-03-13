using System;
using UnityEngine;
 
public interface IDamageable
{
    // 데미지를 받는 함수
    void TakeDamage(float damage);
}

public interface IHydrate
{
    void TakeWater(int amount);
}

public class PlayerCondition : MonoBehaviour, IDamageable, IHydrate
{
    public UICondition uiCondition; // UI에서 상태 정보를 관리하는 객체

    // UICondition에서 체력과 스태미너 상태를 가져옴
    Condition health { get { return uiCondition.health; } }
    Condition hunger { get { return uiCondition.hunger; } }
    Condition thirst { get { return uiCondition.thirst; } }
    Condition stamina { get { return uiCondition.stamina; } }

    public float lowThirstHealthDecay;  //  배고픔이 없을 때 체력 감소
    public event Action onTakeDamaged;  // 데미지를 받았을 때 발생하는 이벤트

    void Update()
    {
        hunger.Subtract(hunger.passiveValue * Time.deltaTime);
        thirst.Subtract(thirst.passiveValue * Time.deltaTime);
        stamina.Add(stamina.passiveValue * Time.deltaTime);

        if (thirst.curValue / thirst.maxValue <= 0.3f)
        {
            health.Subtract(lowThirstHealthDecay * Time.deltaTime);
        }

        if (health.curValue == 0f)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        // 체력을 회복
        health.Add(amount);
    }

    private void Die()
    {
        // 플레이어 사망 처리 (현재는 로그 출력)
        Debug.Log($"Die");
    }

    public void Eat(float amount)
    {
        hunger.Add(amount);
    }
    public void HealStamina(float amount)
    {
        stamina.Add(amount);
    }

    public void TakeDamage(float damage)
    {
        // 데미지를 받으면 체력을 감소
        health.Subtract(damage);
        // 데미지를 받았다는 이벤트 발생
        onTakeDamaged?.Invoke();
    }
    // 물 마시기
    public void TakeWater(int amount)
    {
        thirst.Add(amount);
    }


    public bool UseStamina(float amount)
    {
        // 스태미너가 부족하면 사용 불가 처리
        if (stamina.curValue - amount < 0f)
        {
            return false;
        }
        // 스태미너 감소
        stamina.Subtract(amount);
        return true;
    }
}