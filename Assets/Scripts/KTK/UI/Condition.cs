using UnityEngine;
using UnityEngine.UI;

public class Condition : MonoBehaviour
{
    [Header("Condition Setting")]
    public float curValue;          // 현재 값
    public float starValue;         // 시작 값
    public float maxValue;          // 최대 값

    public float passiverValue;     // 자연 회복 속도
    public Image uiGauge;           // UI 게이지 이미지

    void Start()
    {
        // 현재 값을 시작 값으로 설정
        curValue = starValue;
    }

    void Update()
    {
        // UI 게이지의 fillAmount를 현재 값의 백분율로 설정
        uiGauge.fillAmount = GetPercentage();
    }

    private float GetPercentage()
    {
        // 현재 값과 최대 값의 비율을 반환
        return curValue / maxValue;
    }

    public void Add(float value)
    {
        // 값 증가, 단 최대 값을 초과하지 않도록 제한
        curValue = Mathf.Min(curValue + value, maxValue);
    }

    public void Subtract(float value)
    {
        // 값 감소, 단 최소 값을 0 이하로 떨어지지 않도록 제한
        curValue = Mathf.Max(curValue - value, 0.0f);
    }
}