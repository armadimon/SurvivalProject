using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrapBase : MonoBehaviour
{
    public BuildObject buildObject;
    public LayerMask layerMask;
    public SpikeTrap spikeTrap;

    public Collider collider;
    private void OnTriggerEnter(Collider other)
    {
        if ((layerMask.value & (1 << other.gameObject.layer)) != 0)
        {
            spikeTrap.Activate();
        }
    }
    
    void OnEnable()
    {
        if (buildObject != null)
        {
            buildObject.OnSetChanged += OnSpikeTrap;
        }
        else
        {
            Debug.LogError("build object is null");
        }
    }

    void OnDisable()
    {
        if (buildObject != null)
        {
            buildObject.OnSetChanged -= OnSpikeTrap;
        }
    }

    public void OnSpikeTrap()
    {
        collider.enabled = true;
    }
}
