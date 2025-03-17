using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class RainController : MonoBehaviour
{
    private ParticleSystem rain;

    // Start is called before the first frame update
    void Start()
    {
        rain = GetComponentInChildren<ParticleSystem>();
        DayNightCycle.OnNightStateChanged += ToggleRain;
    }

    // Update is called once per frame
    void ToggleRain(bool isNight)
    {
        // "boss" 태그가 맞는지 확인
        if (tag == "boss")
        {
            // 보스 태그가 있으면 비 오브젝트를 활성화
            rain.gameObject.SetActive(true);
        }
        else
        {
            // 그 외의 경우 비를 비활성화
            rain.gameObject.SetActive(false);
        }
    }

    // 보스가 죽으면 비를 비활성화하는 메서드 추가
    public void OnBossDeath()
    {
        if (tag == "boss")
        {
            // 보스가 죽으면 비를 비활성화
            rain.gameObject.SetActive(false);
        }
    }
}
