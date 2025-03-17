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
        rain = GetComponent<ParticleSystem>();
        DayNightCycle.OnNightStateChanged += ToggleRain;
    }

    // Update is called once per frame
    void ToggleRain(bool isNight)
    {
        if(isNight)
        {
            rain.gameObject.SetActive(true);
            
        }
        else
        {
            rain.gameObject.SetActive(false);
        }
    }
}
