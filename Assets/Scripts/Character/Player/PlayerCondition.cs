using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public interface IDamageable
{
    void TakeDamage(float damage);
}

public interface IHydrate
{
    void TakeWater(int amount);
}

public class PlayerCondition : MonoBehaviour, IDamageable, IHydrate
{
    public UICondition uiCondition;
    public Image indicatorImage;
    public Animator indicatorAnimator;
    public float thirstWarningValue;
    public float hurtFromThirstWarningValue;
    public float hungerWarningValue;

    Condition health { get { return uiCondition.health; } }
    Condition hunger { get { return uiCondition.hunger; } }
    Condition thirst { get { return uiCondition.thirst; } }
    Condition stamina { get { return uiCondition.stamina; } }

    private PlayerController controller;

    public float slowSpeed; // hunger가 낮아 느려진 이동속도
    public float tooSlowSpeed; // hunger 5퍼 이하일 때 이동속도

    public float lowThirstHealthDecay;  // thirst 일정량 이하일 때 체력 감소
    public event Action onTakeDamaged;

    // 물 마시기
    public bool isInHydrateLocation = false;
    public bool isDrinking = false;

    void Start()
    {
        controller = CharacterManager.Instance.Player.controller;
    }

    void Update()
    {
        // hunger 70퍼센트 이상 : passiveValue절반만큼 감소, 70 ~ 일정량 이상 : passiveValue만큼 감소, 일정량 이하 : passiveValue의 1.5배만큼 감소
        HungerWeightSubtract();

        // thirst 70퍼센트 이상 : passiveValue절반만큼 감소, 70 ~ 일정량 이상 : passiveValue만큼 감소, 일정량 이하 : passiveValue의 1.5배만큼 감소
        ThirstWeightSubtract();

        // hunger, thirst 일정량 이하 stamina 회복 정지, 둘 다 아니라면 정상 회복
        HealthyStaminaAdd();

        // hunger, thirst 일정량 이하 인디케이터 표시
        ThirstFlash();
        HurtFromThirstFlash(); // thirst 위험 수준 이하 체력 감소, health 인디케이터 표시
        HungerFlash();
        ThirstHungerFlash(); // thirst, hunger 동시에 일정량 이하 인디케이터에 번갈아 표시

        if (health.curValue == 0f)
        {
            Die();
            indicatorImage.gameObject.SetActive(false);
        }
    }

    public void Heal(float amount)
    {
        health.Add(amount);
    }

    private void Die()
    {
        // 플레이어 죽음, 현재는 Debug.Log
        Debug.Log($"Die");
    }

    public void Eat(float amount)
    {
        hunger.Add(amount);
    }

    public void Hydrate(float amount)
    {
        thirst.Add(amount);
    }

    public void HealStamina(float amount)
    {
        stamina.Add(amount);
    }

    public void TakeDamage(float damage)
    {
        // 실제 플레이어 데미지 입는 부분
        health.Subtract(damage);
        // onTakeDamaged 이벤트
        onTakeDamaged?.Invoke();
    }

    // 물 마시기
    public void TakeWater(int amount)
    {
        thirst.Add(amount);
    }

    public bool UseStamina(float amount)
    {
        if (stamina.curValue - amount < 0f)
        {
            return false;
        }

        stamina.Subtract(amount);
        return true;
    }

    // hunger 70퍼센트 이상 : passiveValue절반만큼 감소, 70 ~ 일정량 이상 : passiveValue만큼 감소, 일정량 이하 : passiveValue의 1.5배만큼 감소
    void HungerWeightSubtract()
    {
        if (hunger.curValue / hunger.maxValue >= 0.7f)
            hunger.Subtract(hunger.passiveValue * 0.5f * Time.deltaTime);
        else if (hunger.curValue / hunger.maxValue < 0.7f && hunger.curValue / hunger.maxValue > hungerWarningValue)
            hunger.Subtract(hunger.passiveValue * Time.deltaTime);
        else
            hunger.Subtract(hunger.passiveValue * 1.5f * Time.deltaTime);
    }

    // thirst 70퍼센트 이상 : passiveValue절반만큼 감소, 70 ~ 일정량 이상 : passiveValue만큼 감소, 일정량 이하 : passiveValue의 1.5배만큼 감소
    void ThirstWeightSubtract()
    {
        if (thirst.curValue / thirst.maxValue >= 0.7f)
            thirst.Subtract(thirst.passiveValue * 0.5f * Time.deltaTime);
        else if (thirst.curValue / thirst.maxValue < 0.7f && thirst.curValue / thirst.maxValue > thirstWarningValue)
            thirst.Subtract(thirst.passiveValue * Time.deltaTime);
        else
            thirst.Subtract(thirst.passiveValue * 1.5f * Time.deltaTime);
    }

    // thirst 일정량 이하 인디케이터에 표시
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

    // thirst 위험 수준 이하 체력 감소, health 인디케이터 표시
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

    // hunger 일정량 이하 인디케이터에 표시
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

    // thirst, hunger 동시에 일정량 이하 인디케이터에 번갈아 표시
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

    // hunger, thirst 일정량 이하 stamina 회복 정지, 둘 다 아니라면 정상 회복
    void HealthyStaminaAdd()
    {
        if (hunger.curValue / hunger.maxValue <= hungerWarningValue || thirst.curValue / thirst.maxValue <= thirstWarningValue)
            stamina.Add(0);
        else stamina.Add(stamina.passiveValue * Time.deltaTime);
    }

    // 물 마시기 InputAction
    public void OnDrinking(InputAction.CallbackContext context)
    {
        if (isInHydrateLocation && context.phase == InputActionPhase.Started)
        {
            isDrinking = true;
            Invoke("StopDrinking", 3f);
        }
    }

    void StopDrinking()
    {
        isDrinking = false;
    }

    // hunger 일정량 이하 이동속도 감소
    public void SlowFromHunger()
    {
        if (hunger.curValue / hunger.maxValue <= hungerWarningValue && hunger.curValue / hunger.maxValue > 0.05f)
        {
            controller.moveSpeed = slowSpeed;
        }
        else if (hunger.curValue / hunger.maxValue <= 0.05f)
        {
            controller.moveSpeed = tooSlowSpeed;
        }
    }
}
