using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class RainController : MonoBehaviour
{
    public ParticleSystem rainPS;
    public ParticleSystem ripplePS;

    private int bossCount = 0;

    private void Start()
    {
        // 게임 시작 시 비를 멈추도록 설정
        rainPS.Stop();
        ripplePS.Stop();
    }

    private void OnEnable()
    {
        // 이벤트 구독
        EntitySpawner.OnBossSpawned += StartRain;
        EntityTracker.OnBossDestroyed += StopRain;
    }

    private void OnDisable()
    {
        // 이벤트 구독 해제
        EntitySpawner.OnBossSpawned -= StartRain;
        EntityTracker.OnBossDestroyed -= StopRain;
    }

    private void StartRain()
    {
        bossCount++;
        if (bossCount == 1) // 첫 보스가 등장하면 비 시작
        {
            rainPS.Play();
            ripplePS.Play();
        }
    }

    private void StopRain()
    {
        bossCount--;
        if (bossCount <= 0) // 모든 보스가 사라지면 비 멈춤
        {
            bossCount = 0;
            rainPS.Stop();
            ripplePS.Stop();
        }
    }
}
