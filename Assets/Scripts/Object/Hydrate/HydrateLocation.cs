using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class HydrateLocation : MonoBehaviour
{
    public int amount;
    public float addRate;
    public TextMeshProUGUI interactionText;

    List<IHydrate> things = new List<IHydrate>();

    void Start()
    {
        InvokeRepeating("Hydrate", 0, addRate);
    }

    void Hydrate()
    {
        for (int i = 0; i < things.Count; i++)
        {
            things[i].TakeWater(amount);
        }
    }

    private void OnTriggerStay(Collider other)
    {        
        if (other.TryGetComponent(out IHydrate hydrate))
        {
            interactionText.text = "Press [H]\nto drink water.";
            CharacterManager.Instance.Player.condition.isInHydrateLocation = true;
            if (CharacterManager.Instance.Player.condition.isDrinking)
            {
                things.Add(hydrate);
                things = things.Distinct().ToList();
            }
            else things.Clear();            
        }
    }

    private void OnTriggerExit(Collider other)
    {        
        if (other.TryGetComponent(out IHydrate hydrate))
        {
            interactionText.text = string.Empty;
            CharacterManager.Instance.Player.condition.isInHydrateLocation = false;
            things.Clear();
        }
    }
}
