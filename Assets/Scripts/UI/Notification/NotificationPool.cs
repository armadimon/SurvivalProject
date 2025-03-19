using System.Collections.Generic;
using UnityEngine;

public class NotificationPool : MonoBehaviour
{
    public static NotificationPool Instance;

    public GameObject notificationPrefab;
    public Transform notificationParent;
    public int poolSize = 5;

    private Queue<GameObject> pool = new Queue<GameObject>();

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(notificationPrefab, notificationParent);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    public GameObject GetNotification()
    {
        if (pool.Count > 0)
        {
            GameObject obj = pool.Dequeue();
            obj.SetActive(true);
            obj.transform.SetParent(notificationParent, false);
            obj.transform.localPosition = Vector3.zero;
            return obj;
        }
        else
        {
            GameObject obj = Instantiate(notificationPrefab, notificationParent);
            obj.transform.localPosition = Vector3.zero;
            return obj;
        }
    }

    public void ReturnNotification(GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.localPosition = Vector3.zero;
        pool.Enqueue(obj);
    }
}