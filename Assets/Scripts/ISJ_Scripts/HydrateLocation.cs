using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HydrateLocation : MonoBehaviour
{
    public int amount;
    public float addRate;

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
            CharacterManager.Instance.Player.controller.isInHydrateLocation = true;
            if (CharacterManager.Instance.Player.controller.isDrinking)
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
            CharacterManager.Instance.Player.controller.isInHydrateLocation = false;
            things.Clear();
        }
    }
}
