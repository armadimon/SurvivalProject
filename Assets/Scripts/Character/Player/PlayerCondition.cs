using System;
using UnityEngine;
using UnityEngine.UI;

public interface IDamageable
{
    // 占쏙옙占쏙옙占쏙옙占쏙옙 占쌨댐옙 占쌉쇽옙
    void TakeDamage(float damage);
}

public interface IHydrate
{
    void TakeWater(int amount);
}

public class PlayerCondition : MonoBehaviour, IDamageable, IHydrate
{
    public UICondition uiCondition; // UI占쏙옙占쏙옙 占쏙옙占쏙옙 占쏙옙占쏙옙占쏙옙 占쏙옙占쏙옙占싹댐옙 占쏙옙체
    public Image indicatorImage;
    public Animator indicatorAnimator;
    public float thirstWarningValue;
    public float hurtFromThirstWarningValue;
    public float hungerWarningValue;

    // UICondition占쏙옙占쏙옙 체占승곤옙 占쏙옙占승미놂옙 占쏙옙占승몌옙 占쏙옙占쏙옙占쏙옙
    Condition health { get { return uiCondition.health; } }
    Condition hunger { get { return uiCondition.hunger; } }
    Condition thirst { get { return uiCondition.thirst; } }
    Condition stamina { get { return uiCondition.stamina; } }

    public float lowThirstHealthDecay;  //  占쏙옙占쏙옙占쏙옙占?占쏙옙占쏙옙 占쏙옙 체占쏙옙 占쏙옙占쏙옙
    public event Action onTakeDamaged;  // 占쏙옙占쏙옙占쏙옙占쏙옙 占쌨억옙占쏙옙 占쏙옙 占쌩삼옙占싹댐옙 占싱븝옙트

    void Update()
    {
        // hunger 70占쏙옙 占싱삼옙 : passiveValue占쏙옙 占쏙옙占쌥몌옙큼, 70 ~ 占쏙옙占?: passiveValue占쏙옙큼, 占쏙옙占?占쏙옙占쏙옙 : passiveValue占쏙옙 1.5占썼만큼 占쏙옙占쏙옙
        HungerWeightSubtract();

        // thirst 70占쏙옙 占싱삼옙 : passiveValue占쏙옙 占쏙옙占쌥몌옙큼, 70 ~ 占쏙옙占?: passiveValue占쏙옙큼, 占쏙옙占?占쏙옙占쏙옙 : passiveValue占쏙옙 1.5占썼만큼 占쏙옙占쏙옙
        ThirstWeightSubtract();

        // hunger, thirst 占쏙옙占?占싱삼옙占쏙옙 占쏙옙占쏙옙 stamina 회占쏙옙, 占쏙옙 占쏙옙 占싹놂옙占쏙옙 占쏙옙占?占쏙옙占쏙옙占쏙옙 占쏙옙占?회占쏙옙 占쏙옙占쏙옙
        HealthyStaminaAdd();

        // hunger, thirst 占쏙옙占?占쏙옙占싹뤄옙 占쏙옙占쏙옙占쏙옙占쏙옙 占쏙옙 占싸듸옙占쏙옙占쏙옙占쏙옙 占쏙옙占쏙옙占쏙옙
        ThirstFlash();
        HurtFromThirstFlash(); // thirst占쏙옙 占쏙옙占쏙옙 占쏙옙占쏙옙占쏙옙占쏙옙 占쏙옙占쏙옙占쏙옙占쏙옙 체占승듸옙 占쏙옙占쏙옙 占쏙옙占쏙옙, Indicator占쏙옙 체占쏙옙 占쏙옙占쏙옙 占쏙옙占쏙옙占쏙옙
        HungerFlash();
        ThirstHungerFlash(); // thirst占쏙옙 hunger 占쏙옙占시울옙 占쏙옙占?占쏙옙占쏙옙占쏙옙 占쏙옙 Indicator占쏙옙 占쌉뀐옙 표占쏙옙

        if (health.curValue == 0f)
        {
            Die();
            indicatorImage.gameObject.SetActive(false);
        }
    }

    public void Heal(float amount)
    {
        // 체占쏙옙占쏙옙 회占쏙옙
        health.Add(amount);
    }

    private void Die()
    {
        // 占시뤄옙占싱억옙 占쏙옙占?처占쏙옙 (占쏙옙占쏙옙占?占싸깍옙 占쏙옙占?
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
        // 占쏙옙占쏙옙占쏙옙占쏙옙 占쏙옙占쏙옙占쏙옙 체占쏙옙占쏙옙 占쏙옙占쏙옙
        health.Subtract(damage);
        // 占쏙옙占쏙옙占쏙옙占쏙옙 占쌨았다댐옙 占싱븝옙트 占쌩삼옙
        onTakeDamaged?.Invoke();
    }

    // 占쏙옙 占쏙옙占시깍옙
    public void TakeWater(int amount)
    {
        thirst.Add(amount);
    }

    public bool UseStamina(float amount)
    {
        // 占쏙옙占승미너곤옙 占쏙옙占쏙옙占싹몌옙 占쏙옙占?占쌀곤옙 처占쏙옙
        if (stamina.curValue - amount < 0f)
        {
            return false;
        }
        // 占쏙옙占승미놂옙 占쏙옙占쏙옙
        stamina.Subtract(amount);
        return true;
    }

    // hunger 70占쏙옙 占싱삼옙 : passiveValue占쏙옙 占쏙옙占쌥몌옙큼, 70 ~ 占쏙옙占?: passiveValue占쏙옙큼, 占쏙옙占?占쏙옙占쏙옙 : passiveValue占쏙옙 1.5占썼만큼 占쏙옙占쏙옙
    void HungerWeightSubtract()
    {
        if (hunger.curValue / hunger.maxValue >= 0.7f)
            hunger.Subtract(hunger.passiveValue * 0.5f * Time.deltaTime);
        else if (hunger.curValue / hunger.maxValue < 0.7f && hunger.curValue / hunger.maxValue > hungerWarningValue)
            hunger.Subtract(hunger.passiveValue * Time.deltaTime);
        else
            hunger.Subtract(hunger.passiveValue * 1.5f * Time.deltaTime);
    }

    // thirst 70占쏙옙 占싱삼옙 : passiveValue占쏙옙 占쏙옙占쌥몌옙큼, 70 ~ 占쏙옙占?: passiveValue占쏙옙큼, 占쏙옙占?占쏙옙占쏙옙 : passiveValue占쏙옙 1.5占썼만큼 占쏙옙占쏙옙
    void ThirstWeightSubtract()
    {
        if (thirst.curValue / thirst.maxValue >= 0.7f)
            thirst.Subtract(thirst.passiveValue * 0.5f * Time.deltaTime);
        else if (thirst.curValue / thirst.maxValue < 0.7f && thirst.curValue / thirst.maxValue > thirstWarningValue)
            thirst.Subtract(thirst.passiveValue * Time.deltaTime);
        else
            thirst.Subtract(thirst.passiveValue * 1.5f * Time.deltaTime);
    }

    // thirst 占쏙옙占?占쏙옙占쏙옙 thirst 占싸듸옙占쏙옙占쏙옙占쏙옙 占쏙옙占쏙옙占쏙옙
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

    // thirst 占쏙옙占쏙옙 占쏙옙占쏙옙 체占쏙옙 占쏙옙占쏙옙 占쏙옙 health 占싸듸옙占쏙옙占쏙옙占쏙옙 占쏙옙占쏙옙占쏙옙
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

    // hunger 占쏙옙占?占쏙옙占쏙옙 thirst 占싸듸옙占쏙옙占쏙옙占쏙옙 占쏙옙占쏙옙占쏙옙
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

    // thirst, hunger 占쏙옙占시울옙 占쏙옙占?占쏙옙占쏙옙 thirst占쏙옙 hunger 占싸듸옙占쏙옙占쏙옙占쏙옙 占쏙옙占쏙옙占싣곤옙占썽서 占쏙옙占쏙옙占쏙옙
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

    // hunger, thirst 占쏙옙占?占싱삼옙占쏙옙 占쏙옙占쏙옙 stamina 회占쏙옙, 占쏙옙 占쏙옙 占싹놂옙占쏙옙 占쏙옙占?占쏙옙占쏙옙占쏙옙 占쏙옙占?회占쏙옙 占쏙옙占쏙옙
    void HealthyStaminaAdd()
    {
        if (hunger.curValue / hunger.maxValue <= hungerWarningValue || thirst.curValue / thirst.maxValue <= thirstWarningValue)
        stamina.Add(0);
        else stamina.Add(stamina.passiveValue * Time.deltaTime);
    }
}