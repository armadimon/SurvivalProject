using System;
using UnityEngine;
using UnityEngine.UI;

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

    public float lowThirstHealthDecay;  //  thirst ?�눘�젟???�똾釉�????筌ｋ���젾 揶쏅Ŋ�꺖
    public event Action onTakeDamaged;

    void Update()
    {
        // hunger 70?�눘苑�???�똻湲� : passiveValue?�뜄而뀐쭕�슦寃� 揶쏅Ŋ�꺖, 70 ~ ?�눘�젟???�똻湲� : passiveValue筌띾슦寃� 揶쏅Ŋ�꺖, ?�눘�젟???�똾釉� : passiveValue??1.5獄쏄퀡彛�??揶쏅Ŋ�꺖
        HungerWeightSubtract();

        // thirst 70?�눘苑�???�똻湲� : passiveValue?�뜄而뀐쭕�슦寃� 揶쏅Ŋ�꺖, 70 ~ ?�눘�젟???�똻湲� : passiveValue筌띾슦寃� 揶쏅Ŋ�꺖, ?�눘�젟???�똾釉� : passiveValue??1.5獄쏄퀡彛�??揶쏅Ŋ�꺖
        ThirstWeightSubtract();

        // hunger, thirst ?�눘�젟???�똾釉� stamina ?�슢�궗 ?類�?, ?????袁⑤빍?�눖�늺 ?類ㅺ맒 ?�슢�궗
        HealthyStaminaAdd();

        // hunger, thirst ?�눘�젟???�똾釉� ?紐껊탵��냈�?�똾苑� ?�뮇�뻻
        ThirstFlash();
        HurtFromThirstFlash(); // thirst ?袁る퓮 ?�꼷? ?�똾釉� 筌ｋ���젾 揶쏅Ŋ�꺖, health ?紐껊탵��냈�?�똾苑� ?�뮇�뻻
        HungerFlash();
        ThirstHungerFlash(); // thirst, hunger ?�늿�뻻???�눘�젟???�똾釉� ?紐껊탵��냈�?�똾苑�??甕곕뜃而�???�뮇�뻻

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
        // ?�슢�쟿?�똻堉� 雅뚯럩�벉, ?袁⑹삺??Debug.Log
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
        // ?�끉�젫 ?�슢�쟿?�똻堉� ?怨�?筌왖� ?�굝�뮉 �겫���겫?
        health.Subtract(damage);
        // onTakeDamaged ?���源�??
        onTakeDamaged?.Invoke();
    }

    // �눧?筌띾뜆�뻻疫�?
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

    // hunger 70?�눘苑�???�똻湲� : passiveValue?�뜄而뀐쭕�슦寃� 揶쏅Ŋ�꺖, 70 ~ ?�눘�젟???�똻湲� : passiveValue筌띾슦寃� 揶쏅Ŋ�꺖, ?�눘�젟???�똾釉� : passiveValue??1.5獄쏄퀡彛�??揶쏅Ŋ�꺖
    void HungerWeightSubtract()
    {
        if (hunger.curValue / hunger.maxValue >= 0.7f)
            hunger.Subtract(hunger.passiveValue * 0.5f * Time.deltaTime);
        else if (hunger.curValue / hunger.maxValue < 0.7f && hunger.curValue / hunger.maxValue > hungerWarningValue)
            hunger.Subtract(hunger.passiveValue * Time.deltaTime);
        else
            hunger.Subtract(hunger.passiveValue * 1.5f * Time.deltaTime);
    }

    // thirst 70?�눘苑�???�똻湲� : passiveValue?�뜄而뀐쭕�슦寃� 揶쏅Ŋ�꺖, 70 ~ ?�눘�젟???�똻湲� : passiveValue筌띾슦寃� 揶쏅Ŋ�꺖, ?�눘�젟???�똾釉� : passiveValue??1.5獄쏄퀡彛�??揶쏅Ŋ�꺖
    void ThirstWeightSubtract()
    {
        if (thirst.curValue / thirst.maxValue >= 0.7f)
            thirst.Subtract(thirst.passiveValue * 0.5f * Time.deltaTime);
        else if (thirst.curValue / thirst.maxValue < 0.7f && thirst.curValue / thirst.maxValue > thirstWarningValue)
            thirst.Subtract(thirst.passiveValue * Time.deltaTime);
        else
            thirst.Subtract(thirst.passiveValue * 1.5f * Time.deltaTime);
    }

    // thirst ?�눘�젟???�똾釉� ?紐껊탵��냈�?�똾苑�???�뮇�뻻
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

    // thirst ?袁る퓮 ?�꼷? ?�똾釉� 筌ｋ���젾 揶쏅Ŋ�꺖, health ?紐껊탵��냈�?�똾苑� ?�뮇�뻻
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

    // hunger ?�눘�젟???�똾釉� ?紐껊탵��냈�?�똾苑�???�뮇�뻻
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

    // thirst, hunger ?�늿�뻻???�눘�젟???�똾釉� ?紐껊탵��냈�?�똾苑�??甕곕뜃而�???�뮇�뻻
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

    // hunger, thirst ?�눘�젟???�똾釉� stamina ?�슢�궗 ?類�?, ?????袁⑤빍?�눖�늺 ?類ㅺ맒 ?�슢�궗
    void HealthyStaminaAdd()
    {
        if (hunger.curValue / hunger.maxValue <= hungerWarningValue || thirst.curValue / thirst.maxValue <= thirstWarningValue)
        stamina.Add(0);
        else stamina.Add(stamina.passiveValue * Time.deltaTime);
    }    
}