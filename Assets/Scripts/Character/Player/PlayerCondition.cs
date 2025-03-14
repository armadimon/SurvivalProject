using System;
using UnityEngine;
using UnityEngine.UI;

public interface IDamageable
{
    // �������� �޴� �Լ�
    void TakeDamage(float damage);
}

public interface IHydrate
{
    void TakeWater(int amount);
}

public class PlayerCondition : MonoBehaviour, IDamageable, IHydrate
{
    public UICondition uiCondition; // UI���� ���� ������ �����ϴ� ��ü
    public Image indicatorImage;
    public Animator indicatorAnimator;
    public float thirstWarningValue;
    public float hurtFromThirstWarningValue;
    public float hungerWarningValue;

    // UICondition���� ü�°� ���¹̳� ���¸� ������
    Condition health { get { return uiCondition.health; } }
    Condition hunger { get { return uiCondition.hunger; } }
    Condition thirst { get { return uiCondition.thirst; } }
    Condition stamina { get { return uiCondition.stamina; } }

    public float lowThirstHealthDecay;  //  ������� ���� �� ü�� ����
    public event Action onTakeDamaged;  // �������� �޾��� �� �߻��ϴ� �̺�Ʈ

    void Update()
    {
        // hunger 70�� �̻� : passiveValue�� ���ݸ�ŭ, 70 ~ ��� : passiveValue��ŭ, ��� ���� : passiveValue�� 1.5�踸ŭ ����
        HungerWeightSubtract();

        // thirst 70�� �̻� : passiveValue�� ���ݸ�ŭ, 70 ~ ��� : passiveValue��ŭ, ��� ���� : passiveValue�� 1.5�踸ŭ ����
        ThirstWeightSubtract();

        // hunger, thirst ��� �̻��� ���� stamina ȸ��, �� �� �ϳ��� ��� ������ ��� ȸ�� ����
        HealthyStaminaAdd();

        // hunger, thirst ��� ���Ϸ� �������� �� �ε������� ������
        ThirstFlash();
        HurtFromThirstFlash(); // thirst�� ���� �������� �������� ü�µ� ���� ����, Indicator�� ü�� ���� ������
        HungerFlash();
        ThirstHungerFlash(); // thirst�� hunger ���ÿ� ��� ������ �� Indicator�� �Բ� ǥ��

        if (health.curValue == 0f)
        {
            Die();
            indicatorImage.gameObject.SetActive(false);
        }
    }

    public void Heal(float amount)
    {
        // ü���� ȸ��
        health.Add(amount);
    }

    private void Die()
    {
        // �÷��̾� ��� ó�� (����� �α� ���)
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
        // �������� ������ ü���� ����
        health.Subtract(damage);
        // �������� �޾Ҵٴ� �̺�Ʈ �߻�
        onTakeDamaged?.Invoke();
    }

    // �� ���ñ�
    public void TakeWater(int amount)
    {
        thirst.Add(amount);
    }

    public bool UseStamina(float amount)
    {
        // ���¹̳ʰ� �����ϸ� ��� �Ұ� ó��
        if (stamina.curValue - amount < 0f)
        {
            return false;
        }
        // ���¹̳� ����
        stamina.Subtract(amount);
        return true;
    }

    // hunger 70�� �̻� : passiveValue�� ���ݸ�ŭ, 70 ~ ��� : passiveValue��ŭ, ��� ���� : passiveValue�� 1.5�踸ŭ ����
    void HungerWeightSubtract()
    {
        if (hunger.curValue / hunger.maxValue >= 0.7f)
            hunger.Subtract(hunger.passiveValue * 0.5f * Time.deltaTime);
        else if (hunger.curValue / hunger.maxValue < 0.7f && hunger.curValue / hunger.maxValue > hungerWarningValue)
            hunger.Subtract(hunger.passiveValue * Time.deltaTime);
        else
            hunger.Subtract(hunger.passiveValue * 1.5f * Time.deltaTime);
    }

    // thirst 70�� �̻� : passiveValue�� ���ݸ�ŭ, 70 ~ ��� : passiveValue��ŭ, ��� ���� : passiveValue�� 1.5�踸ŭ ����
    void ThirstWeightSubtract()
    {
        if (thirst.curValue / thirst.maxValue >= 0.7f)
            thirst.Subtract(thirst.passiveValue * 0.5f * Time.deltaTime);
        else if (thirst.curValue / thirst.maxValue < 0.7f && thirst.curValue / thirst.maxValue > thirstWarningValue)
            thirst.Subtract(thirst.passiveValue * Time.deltaTime);
        else
            thirst.Subtract(thirst.passiveValue * 1.5f * Time.deltaTime);
    }

    // thirst ��� ���� thirst �ε������� ������
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

    // thirst ���� ���� ü�� ���� �� health �ε������� ������
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

    // hunger ��� ���� thirst �ε������� ������
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

    // thirst, hunger ���ÿ� ��� ���� thirst�� hunger �ε������� �����ư��鼭 ������
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

    // hunger, thirst ��� �̻��� ���� stamina ȸ��, �� �� �ϳ��� ��� ������ ��� ȸ�� ����
    void HealthyStaminaAdd()
    {
        if (hunger.curValue / hunger.maxValue <= hungerWarningValue || thirst.curValue / thirst.maxValue <= thirstWarningValue)
        stamina.Add(0);
        else stamina.Add(stamina.passiveValue * Time.deltaTime);
    }
}