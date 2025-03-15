using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Range(0.0f, 1.0f)]
    public float time;
    public float fullDayLength;
    public float startTime = 0.4f; // time = 0.5f 일때 정오
    private float timeRate;
    public Vector3 noon;    // Vector 90, 0, 0

    [Header("Sun")]
    public Light sun;
    public Gradient sunColor;   // Gradient 클래스는 색상을 시간에 따라 변화시키기 위한 클래스입니다.
    public AnimationCurve sunIntensity; // AnimationCurve 클래스는 시간에 따라 값이 변화하는 값을 저장하기 위한 클래스입니다.

    [Header("Moon")]
    public Light moon;
    public Gradient moonColor;
    public AnimationCurve moonIntensity;

    [Header("other Lighting")]
    public AnimationCurve lightingIntensityMultiplier;
    public AnimationCurve reflectionIntensityMultiplier;


    // Start is called before the first frame update
    void Start()
    {
        timeRate = 1.0f / fullDayLength;
        time = startTime;
    }

    // Update is called once per frame
    void Update()
    {
        time = (time + timeRate * Time.deltaTime) % 1.0f;

        UpdateLighting(sun, sunColor, sunIntensity);
        UpdateLighting(moon, moonColor, moonIntensity);

        RenderSettings.ambientIntensity = lightingIntensityMultiplier.Evaluate(time);
        RenderSettings.reflectionIntensity = reflectionIntensityMultiplier.Evaluate(time);
    }

    void UpdateLighting(Light lightSource, Gradient gradirent, AnimationCurve intensityCurve)
    {
        // 현재 시간(time)에 따른 조명의 강도를 계산합니다.
        float intensity = intensityCurve.Evaluate(time);

        // 조명의 회전 각도를 설정합니다.
        // 태양(sun)일 경우 0.25를 빼고, 달(moon)일 경우 0.75를 뺍니다.
        // noon 벡터와 곱한 후 4를 더하여 최종 회전 각도를 계산합니다.
        lightSource.transform.eulerAngles = (time - (lightSource == sun ? 0.25f : 0.75f)) * noon * 4f;
        lightSource.color = gradirent.Evaluate(time);
        lightSource.intensity = intensity;
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