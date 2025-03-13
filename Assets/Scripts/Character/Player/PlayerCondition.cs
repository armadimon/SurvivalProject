using System;
using UnityEngine;
using UnityEngine.UI;

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
    public Image indicatorImage;
    public Animator indicatorAnimator;
    public float thirstWarningValue;
    public float hurtFromThirstWarningValue;
    public float hungerWarningValue;

    // UICondition에서 체력과 스태미너 상태를 가져옴
    Condition health { get { return uiCondition.health; } }
    Condition hunger { get { return uiCondition.hunger; } }
    Condition thirst { get { return uiCondition.thirst; } }
    Condition stamina { get { return uiCondition.stamina; } }

    public float lowThirstHealthDecay;  //  배고픔이 없을 때 체력 감소
    public event Action onTakeDamaged;  // 데미지를 받았을 때 발생하는 이벤트

    void Update()
    {
        // hunger 70퍼 이상 : passiveValue의 절반만큼, 70 ~ 경고량 : passiveValue만큼, 경고량 이하 : passiveValue의 1.5배만큼 감소
        HungerWeightSubtract();

        // thirst 70퍼 이상 : passiveValue의 절반만큼, 70 ~ 경고량 : passiveValue만큼, 경고량 이하 : passiveValue의 1.5배만큼 감소
        ThirstWeightSubtract();

        // hunger, thirst 경고량 이상일 때만 stamina 회복, 둘 중 하나라도 경고량 이하일 경우 회복 정지
        HealthyStaminaAdd();

        // hunger, thirst 경고량 이하로 내려갔을 때 인디케이터 깜빡임
        ThirstFlash();
        HurtFromThirstFlash(); // thirst는 위험 수준으로 떨어지면 체력도 같이 감소, Indicator에 체력 감소 깜빡임
        HungerFlash();
        ThirstHungerFlash(); // thirst와 hunger 동시에 경고량 이하일 때 Indicator에 함께 표시

        if (health.curValue == 0f)
        {
            Die();
            indicatorImage.gameObject.SetActive(false);
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

    // hunger 70퍼 이상 : passiveValue의 절반만큼, 70 ~ 경고량 : passiveValue만큼, 경고량 이하 : passiveValue의 1.5배만큼 감소
    void HungerWeightSubtract()
    {
        if (hunger.curValue / hunger.maxValue >= 0.7f)
            hunger.Subtract(hunger.passiveValue * 0.5f * Time.deltaTime);
        else if (hunger.curValue / hunger.maxValue < 0.7f && hunger.curValue / hunger.maxValue > hungerWarningValue)
            hunger.Subtract(hunger.passiveValue * Time.deltaTime);
        else
            hunger.Subtract(hunger.passiveValue * 1.5f * Time.deltaTime);
    }

    // thirst 70퍼 이상 : passiveValue의 절반만큼, 70 ~ 경고량 : passiveValue만큼, 경고량 이하 : passiveValue의 1.5배만큼 감소
    void ThirstWeightSubtract()
    {
        if (thirst.curValue / thirst.maxValue >= 0.7f)
            thirst.Subtract(thirst.passiveValue * 0.5f * Time.deltaTime);
        else if (thirst.curValue / thirst.maxValue < 0.7f && thirst.curValue / thirst.maxValue > thirstWarningValue)
            thirst.Subtract(thirst.passiveValue * Time.deltaTime);
        else
            thirst.Subtract(thirst.passiveValue * 1.5f * Time.deltaTime);
    }

    // thirst 경고량 이하 thirst 인디케이터 깜빡임
    void ThirstFlash()
    {
        if (thirst.curValue / thirst.maxValue <= thirstWarningValue)
        {            
            indicatorAnimator.SetBool("OnThirst", true);
        }
        else
        {            
            indicatorAnimator.SetBool("OnThirst", false);
        }
    }

    // thirst 위험 수준 체력 감소 및 health 인디케이터 깜빡임
    void HurtFromThirstFlash()
    {
        if (thirst.curValue / thirst.maxValue <= hurtFromThirstWarningValue)
        {
            health.Subtract(lowThirstHealthDecay * Time.deltaTime);
            indicatorAnimator.SetBool("OnHurt", true);
        }
        else
        {            
            indicatorAnimator.SetBool("OnHurt", false);
        }
    }

    // hunger 경고량 이하 thirst 인디케이터 깜빡임
    void HungerFlash()
    {
        if (hunger.curValue / hunger.maxValue <= hungerWarningValue)
        {            
            indicatorAnimator.SetBool("OnHunger", true);
        }
        else
        {            
            indicatorAnimator.SetBool("OnHunger", false);
        }
    }

    // thirst, hunger 동시에 경고량 이하 thirst와 hunger 인디케이터 번갈아가면서 깜빡임
    void ThirstHungerFlash()
    {
        if (thirst.curValue / thirst.maxValue <= thirstWarningValue && hunger.curValue / hunger.maxValue <= hungerWarningValue)
        {
            indicatorAnimator.SetBool("OnThirstHunger", true);
        }
        else
        {
            indicatorAnimator.SetBool("OnThirstHunger", false);
        }
    }

    // hunger, thirst 경고량 이상일 때만 stamina 회복, 둘 중 하나라도 경고량 이하일 경우 회복 정지
    void HealthyStaminaAdd()
    {
        if (hunger.curValue / hunger.maxValue <= hungerWarningValue || thirst.curValue / thirst.maxValue <= thirstWarningValue)
        stamina.Add(0);
        else stamina.Add(stamina.passiveValue * Time.deltaTime);
    }
}