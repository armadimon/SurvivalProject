using System.Collections;
using UnityEngine;

public class Respawn : MonoBehaviour
{
    public Resource resource;

    private float respawnTime = 5f;
    public void StartRespawn()
    { 
        StartCoroutine(RespawnResource());
    }

    IEnumerator RespawnResource()
    {
        yield return new WaitForSeconds(respawnTime);

        resource.capacity = resource.maxCapacity;
        resource.gameObject.SetActive(true);
    }
}
