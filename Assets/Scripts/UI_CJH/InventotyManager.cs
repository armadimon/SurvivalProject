using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventotyManager : MonoBehaviour
{
    public static InventotyManager Instance { get; private set; }

    public List<ItemSlot> slots = new List<ItemSlot>();
    public int maxSlots = 21;

    private void Awake()
    {
        if (Instance != null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool AddItem(ItemData item, int amount)
    {
        foreach (ItemSlot slot in slots)
        {
            
        }


        return false;
    }

}
