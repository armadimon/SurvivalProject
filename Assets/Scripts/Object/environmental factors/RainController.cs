using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class RainController : MonoBehaviour
{

    private ParticleSystem rain;
    private ParticleSystem.EmissionModule emissionModule;

    // Start is called before the first frame update
    void Start()
    {
        rain = GetComponent<ParticleSystem>();
        DayNightCycle.OnNightStateChanged += ToggleRain;
    }

    // Update is called once per frame
    void ToggleRain(bool isNight)
    {
        if(isNight)
        {
            emissionModule.rateOverTime = 1000;
        }
        else
        {
            emissionModule.rateOverTime = 0;

        }
    }
}
