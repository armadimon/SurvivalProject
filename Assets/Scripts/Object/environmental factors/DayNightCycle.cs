using System;
using DG.Tweening;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    public static event Action<bool> OnNightStateChanged; // true: 밤, false: 낮

    private bool isNight = false; // 현재 밤인지 낮인지 여부

    [Range(0.0f, 1.0f)]
    public float time; // 현재 시간 (0.0f ~ 1.0f 범위) - 0.0f은 자정, 0.5f는 정오
    public float fullDayLength; // 하루의 길이 (초 단위)
    public float startTime = 0.4f; // 게임 시작 시 초기 시간 (0.5f는 정오, 0.4f는 아침)
    private float timeRate; // 시간 변화 속도 (초당 변화량)
    public Vector3 noon; // 정오일 때 태양의 방향 (보통 (90, 0, 0) 설정)

    [Header("Sun")]
    public Light sun; // 태양 조명
    public Gradient sunColor; // 태양 색상을 시간에 따라 변화시키는 Gradient
    public AnimationCurve sunIntensity; // 태양의 강도를 시간에 따라 변화시키는 AnimationCurve

    [Header("Moon")]
    public Light moon; // 달 조명
    public Gradient moonColor; // 달 색상을 시간에 따라 변화시키는 Gradient
    public AnimationCurve moonIntensity; // 달의 강도를 시간에 따라 변화시키는 AnimationCurve

    [Header("Other Lighting")]
    public AnimationCurve lightingIntensityMultiplier; // 전체적인 조명 강도 조절 AnimationCurve
    public AnimationCurve reflectionIntensityMultiplier; // 반사광 강도 조절 AnimationCurve

    void Start()
    {
        // 하루의 길이에 따른 시간 변화 속도 설정 (1.0f을 하루 길이로 나눠서 초당 변화량 계산)
        timeRate = 1.0f / fullDayLength;

        // 게임 시작 시 초기 시간 설정
        time = startTime;
    }

    void Update()
    {
        // 현재 시간을 업데이트 (시간이 1을 초과하면 0으로 순환)
        time = (time + timeRate * Time.deltaTime) % 1.0f;

        bool newIsNight = time >= 0.75f; // 현재 밤인지 낮인지 여부 판단

        if (newIsNight != isNight)
        {
            isNight = newIsNight;
            OnNightStateChanged?.Invoke(isNight); // 밤 상태 변경 이벤트 발생

            if(isNight)
            {
                // 밤이 되었을 때 처리
                NotificationManager.Instance.ShowNotification("밤이 되었습니다.");
                // 3초 후에 두 번째 알림 표시
                DOVirtual.DelayedCall(3f, () =>
                {
                    NotificationManager.Instance.ShowNotification("동물들을 조심하세요.");
                });

            }
            else
            {
                // 낮이 되었을 때 처리
                NotificationManager.Instance.ShowNotification("낮이 되었습니다.");
            }
        }


        // 태양과 달의 조명을 업데이트
        UpdateLighting(sun, sunColor, sunIntensity);
        UpdateLighting(moon, moonColor, moonIntensity);

        // 전체적인 조명과 반사광의 강도를 AnimationCurve를 이용해 조절
        RenderSettings.ambientIntensity = lightingIntensityMultiplier.Evaluate(time);
        RenderSettings.reflectionIntensity = reflectionIntensityMultiplier.Evaluate(time);
    }

    void UpdateLighting(Light lightSource, Gradient gradient, AnimationCurve intensityCurve)
    {
        // 현재 시간(time)에 따른 조명의 강도를 계산
        float intensity = intensityCurve.Evaluate(time);

        // 조명의 회전 각도를 설정
        // - 태양(sun)은 기준 시간에서 0.25를 빼고, 달(moon)은 0.75를 뺌
        // - noon 벡터를 곱하고, 4를 곱하여 최종 회전 각도를 결정
        lightSource.transform.eulerAngles = (time - (lightSource == sun ? 0.25f : 0.75f)) * noon * 4f;

        // 시간에 따른 색상 변경 적용
        lightSource.color = gradient.Evaluate(time);

        // 빛의 강도 설정
        lightSource.intensity = intensity;

        // 빛의 활성화 상태 조절 (빛이 없을 경우 오브젝트 비활성화)
        GameObject go = lightSource.gameObject;
        if (lightSource.intensity == 0 && go.activeInHierarchy)
        {
            go.SetActive(false);
        }
        else if (lightSource.intensity > 0 && !go.activeInHierarchy)
        {
            go.SetActive(true);
        }
    }
}