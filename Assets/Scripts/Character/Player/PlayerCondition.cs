using System;
using UnityEngine;
using UnityEngine.UI;

public interface IDamageable
{
    // ?좎룞?쇿뜝?숈삕?좎룞?쇿뜝?숈삕 ?좎뙣?먯삕 ?좎뙃?쎌삕
    void TakeDamage(float damage);
}

public interface IHydrate
{
    void TakeWater(int amount);
}

public class PlayerCondition : MonoBehaviour, IDamageable, IHydrate
{
    public UICondition uiCondition; // UI?좎룞?쇿뜝?숈삕 ?좎룞?쇿뜝?숈삕 ?좎룞?쇿뜝?숈삕?좎룞???좎룞?쇿뜝?숈삕?좎떦?먯삕 ?좎룞?숈껜
    public Image indicatorImage;
    public Animator indicatorAnimator;
    public float thirstWarningValue;
    public float hurtFromThirstWarningValue;
    public float hungerWarningValue;

    // UICondition?좎룞?쇿뜝?숈삕 泥닷뜝?밴낀???좎룞?쇿뜝?밸??귥삕 ?좎룞?쇿뜝?밸챿???좎룞?쇿뜝?숈삕?좎룞??
    Condition health { get { return uiCondition.health; } }
    Condition hunger { get { return uiCondition.hunger; } }
    Condition thirst { get { return uiCondition.thirst; } }
    Condition stamina { get { return uiCondition.stamina; } }

    public float lowThirstHealthDecay;  //  ?좎룞?쇿뜝?숈삕?좎룞?쇿뜝??좎룞?쇿뜝?숈삕 ?좎룞??泥닷뜝?숈삕 ?좎룞?쇿뜝?숈삕
    public event Action onTakeDamaged;  // ?좎룞?쇿뜝?숈삕?좎룞?쇿뜝?숈삕 ?좎뙣?듭삕?좎룞???좎룞???좎뙥?쇱삕?좎떦?먯삕 ?좎떛釉앹삕??

    void Update()
    {
        // hunger 70?좎룞???좎떛?쇱삕 : passiveValue?좎룞???좎룞?쇿뜝?λ챿?숉겮, 70 ~ ?좎룞?쇿뜝?: passiveValue?좎룞?숉겮, ?좎룞?쇿뜝??좎룞?쇿뜝?숈삕 : passiveValue?좎룞??1.5?좎띁留뚰겮 ?좎룞?쇿뜝?숈삕
        HungerWeightSubtract();

        // thirst 70?좎룞???좎떛?쇱삕 : passiveValue?좎룞???좎룞?쇿뜝?λ챿?숉겮, 70 ~ ?좎룞?쇿뜝?: passiveValue?좎룞?숉겮, ?좎룞?쇿뜝??좎룞?쇿뜝?숈삕 : passiveValue?좎룞??1.5?좎띁留뚰겮 ?좎룞?쇿뜝?숈삕
        ThirstWeightSubtract();

        // hunger, thirst ?좎룞?쇿뜝??좎떛?쇱삕?좎룞???좎룞?쇿뜝?숈삕 stamina ?뚦뜝?숈삕, ?좎룞???좎룞???좎떦?귥삕?좎룞???좎룞?쇿뜝??좎룞?쇿뜝?숈삕?좎룞???좎룞?쇿뜝??뚦뜝?숈삕 ?좎룞?쇿뜝?숈삕
        HealthyStaminaAdd();

        // hunger, thirst ?좎룞?쇿뜝??좎룞?쇿뜝?밸쨪???좎룞?쇿뜝?숈삕?좎룞?쇿뜝?숈삕 ?좎룞???좎떥?몄삕?좎룞?쇿뜝?숈삕?좎룞???좎룞?쇿뜝?숈삕?좎룞??
        ThirstFlash();
        HurtFromThirstFlash(); // thirst?좎룞???좎룞?쇿뜝?숈삕 ?좎룞?쇿뜝?숈삕?좎룞?쇿뜝?숈삕 ?좎룞?쇿뜝?숈삕?좎룞?쇿뜝?숈삕 泥닷뜝?밸벝???좎룞?쇿뜝?숈삕 ?좎룞?쇿뜝?숈삕, Indicator?좎룞??泥닷뜝?숈삕 ?좎룞?쇿뜝?숈삕 ?좎룞?쇿뜝?숈삕?좎룞??
        HungerFlash();
        ThirstHungerFlash(); // thirst?좎룞??hunger ?좎룞?쇿뜝?쒖슱???좎룞?쇿뜝??좎룞?쇿뜝?숈삕?좎룞???좎룞??Indicator?좎룞???좎뙃?먯삕 ?쒎뜝?숈삕

        if (health.curValue == 0f)
        {
            Die();
            indicatorImage.gameObject.SetActive(false);
        }
    }

    public void Heal(float amount)
    {
        // 泥닷뜝?숈삕?좎룞???뚦뜝?숈삕
        health.Add(amount);
    }

    private void Die()
    {
        // ?좎떆琉꾩삕?좎떛?듭삕 ?좎룞?쇿뜝?泥섇뜝?숈삕 (?좎룞?쇿뜝?숈삕???좎떥源띿삕 ?좎룞?쇿뜝?
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
        // ?좎룞?쇿뜝?숈삕?좎룞?쇿뜝?숈삕 ?좎룞?쇿뜝?숈삕?좎룞??泥닷뜝?숈삕?좎룞???좎룞?쇿뜝?숈삕
        health.Subtract(damage);
        // ?좎룞?쇿뜝?숈삕?좎룞?쇿뜝?숈삕 ?좎뙣?섎떎?먯삕 ?좎떛釉앹삕???좎뙥?쇱삕
        onTakeDamaged?.Invoke();
    }

    // ?좎룞???좎룞?쇿뜝?쒓퉵??
    public void TakeWater(int amount)
    {
        thirst.Add(amount);
    }

    public bool UseStamina(float amount)
    {
        // ?좎룞?쇿뜝?밸??덇낀???좎룞?쇿뜝?숈삕?좎떦紐뚯삕 ?좎룞?쇿뜝??좎?怨ㅼ삕 泥섇뜝?숈삕
        if (stamina.curValue - amount < 0f)
        {
            return false;
        }
        // ?좎룞?쇿뜝?밸??귥삕 ?좎룞?쇿뜝?숈삕
        stamina.Subtract(amount);
        return true;
    }

    // hunger 70?좎룞???좎떛?쇱삕 : passiveValue?좎룞???좎룞?쇿뜝?λ챿?숉겮, 70 ~ ?좎룞?쇿뜝?: passiveValue?좎룞?숉겮, ?좎룞?쇿뜝??좎룞?쇿뜝?숈삕 : passiveValue?좎룞??1.5?좎띁留뚰겮 ?좎룞?쇿뜝?숈삕
    void HungerWeightSubtract()
    {
        if (hunger.curValue / hunger.maxValue >= 0.7f)
            hunger.Subtract(hunger.passiveValue * 0.5f * Time.deltaTime);
        else if (hunger.curValue / hunger.maxValue < 0.7f && hunger.curValue / hunger.maxValue > hungerWarningValue)
            hunger.Subtract(hunger.passiveValue * Time.deltaTime);
        else
            hunger.Subtract(hunger.passiveValue * 1.5f * Time.deltaTime);
    }

    // thirst 70?좎룞???좎떛?쇱삕 : passiveValue?좎룞???좎룞?쇿뜝?λ챿?숉겮, 70 ~ ?좎룞?쇿뜝?: passiveValue?좎룞?숉겮, ?좎룞?쇿뜝??좎룞?쇿뜝?숈삕 : passiveValue?좎룞??1.5?좎띁留뚰겮 ?좎룞?쇿뜝?숈삕
    void ThirstWeightSubtract()
    {
        if (thirst.curValue / thirst.maxValue >= 0.7f)
            thirst.Subtract(thirst.passiveValue * 0.5f * Time.deltaTime);
        else if (thirst.curValue / thirst.maxValue < 0.7f && thirst.curValue / thirst.maxValue > thirstWarningValue)
            thirst.Subtract(thirst.passiveValue * Time.deltaTime);
        else
            thirst.Subtract(thirst.passiveValue * 1.5f * Time.deltaTime);
    }

    // thirst ?좎룞?쇿뜝??좎룞?쇿뜝?숈삕 thirst ?좎떥?몄삕?좎룞?쇿뜝?숈삕?좎룞???좎룞?쇿뜝?숈삕?좎룞??
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

    // thirst ?좎룞?쇿뜝?숈삕 ?좎룞?쇿뜝?숈삕 泥닷뜝?숈삕 ?좎룞?쇿뜝?숈삕 ?좎룞??health ?좎떥?몄삕?좎룞?쇿뜝?숈삕?좎룞???좎룞?쇿뜝?숈삕?좎룞??
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

    // hunger ?좎룞?쇿뜝??좎룞?쇿뜝?숈삕 thirst ?좎떥?몄삕?좎룞?쇿뜝?숈삕?좎룞???좎룞?쇿뜝?숈삕?좎룞??
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

    // thirst, hunger ?좎룞?쇿뜝?쒖슱???좎룞?쇿뜝??좎룞?쇿뜝?숈삕 thirst?좎룞??hunger ?좎떥?몄삕?좎룞?쇿뜝?숈삕?좎룞???좎룞?쇿뜝?숈삕?좎떍怨ㅼ삕?좎띂???좎룞?쇿뜝?숈삕?좎룞??
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

    // hunger, thirst ?좎룞?쇿뜝??좎떛?쇱삕?좎룞???좎룞?쇿뜝?숈삕 stamina ?뚦뜝?숈삕, ?좎룞???좎룞???좎떦?귥삕?좎룞???좎룞?쇿뜝??좎룞?쇿뜝?숈삕?좎룞???좎룞?쇿뜝??뚦뜝?숈삕 ?좎룞?쇿뜝?숈삕
    void HealthyStaminaAdd()
    {
        if (hunger.curValue / hunger.maxValue <= hungerWarningValue || thirst.curValue / thirst.maxValue <= thirstWarningValue)
        stamina.Add(0);
        else stamina.Add(stamina.passiveValue * Time.deltaTime);
    }
}