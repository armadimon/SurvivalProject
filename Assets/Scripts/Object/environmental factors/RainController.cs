using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class RainController : MonoBehaviour
{
    [SerializeField]
    private GameObject rainSystem; // Inspector에서 할당 (비활성화 상태여도 할당 가능)

    private ParticleSystem rainPS;
    private ParticleSystem ripplePS;

    // Start is called before the first frame update
    void Start()
    {
        if (rainSystem == null)
        {
            Debug.LogError("RainSystem 오브젝트가 할당되어 있지 않습니다.");
            return;
        }

        // RainSystem을 활성화하여 하위 오브젝트에 접근 가능하게 함
        rainSystem.SetActive(true);

        Transform rainPSTrans = rainSystem.transform.Find("Rain_PS");
        Transform ripplePSTrans = rainSystem.transform.Find("Ripple_PS");

        if (rainPSTrans == null || ripplePSTrans == null)
        {
            Debug.LogError("Rain_PS 또는 Ripple_PS 오브젝트를 찾을 수 없습니다.");
            return;
        }

        rainPS = rainPSTrans.GetComponent<ParticleSystem>();
        ripplePS = ripplePSTrans.GetComponent<ParticleSystem>();

        if (rainPS == null || ripplePS == null)
        {
            Debug.LogError("Rain_PS 또는 Ripple_PS 파티클 시스템을 찾을 수 없습니다.");
            return;
        }

        ToggleRain();
    }

    // 보스가 리스폰될 때 OnEnable이 호출되므로, 이곳에서 ToggleRain을 호출하여 비 효과를 재실행합니다.
    void OnEnable()
    {
        ToggleRain();
    }

    void ToggleRain()
    {
        // "Boss" 태그의 경우 비 효과 재생
        if (tag == "Boss")
        {
            rainSystem.SetActive(true);
            rainPS.gameObject.SetActive(true);
            ripplePS.gameObject.SetActive(true);

            rainPS.Play();
            ripplePS.Play();
        }
        else
        {
            // Boss 태그가 아닐 경우 비 효과 중지 및 RainSystem 비활성화
            rainPS.Stop();
            ripplePS.Stop();
            rainSystem.SetActive(false);
        }
    }

    // 보스가 죽으면 비 효과 중지하는 메서드
    public void OnBossDeath()
    {
        if (tag == "Boss")
        {
            rainPS.Stop();
            ripplePS.Stop();
            rainSystem.SetActive(false);
        }
    }
}
