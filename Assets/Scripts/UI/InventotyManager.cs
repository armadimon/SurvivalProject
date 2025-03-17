using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InventotyManager : MonoBehaviour
{
    private static InventotyManager _instance;
    public static InventotyManager Instance 
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameObject("InventotyManager").AddComponent<InventotyManager>();
            }
            return _instance;
        }
    }

    public UIInventory _inventory;

    public UIInventory Inventory
    {
        get { return _inventory; }
        set { _inventory = value; }
    }

    public int maxSlots = 20;


    private void Awake()
    {
        if (_instance != null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (_instance == this)
            {
                Destroy(gameObject);

            }
        }
    }

    // 특정 재료를 보유하고 있는지 확인하고 소모시키는 메서드
    // consume이 false 라면 확인만, true라면 소모
    public bool HasResourceAmount(RequireResourceAmount requireResourceAmount, bool consume, int quanntity = 1)
    {
        ItemSlot[] slots = Inventory.slots;
        int totalAvailable = 0;             // 인벤토리에 해당 아이템의 총 개수를 표시
        List<int> useSlotIndexes = new List<int>();     // 사용할 슬롯 인덱스 저장

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == null) continue;

            if (slots[i].item.type == ItemType.Resouce && slots[i].item.resourceType == requireResourceAmount.type)
            {
                totalAvailable += slots[i].quantity;
                useSlotIndexes.Add(i);

            }
        }

        // 총 개수가 요구 개수보다 모자르다면 false
        if (totalAvailable < requireResourceAmount.value * quanntity)
            return false;

        if (consume)
        {
            // 아이템을 제거해야 할 총 수량을 저장
            int consumeQuantity = requireResourceAmount.value * quanntity;

            foreach (int index in useSlotIndexes)
            {
                if (consumeQuantity <= 0) break;    //아이템을 다 만들었다면 정지

                // 현재 슬롯에서 제거할 수 있는 최대량을 계산
                int toRemove = Mathf.Min(slots[index].quantity, consumeQuantity);
                slots[index].quantity -= toRemove;
                consumeQuantity -= toRemove;

                if (slots[index].quantity <= 0)
                {
                    slots[index].item = null;
                }

            }

        }

        Inventory.UpdateUI();

        return true;
    }

    public void AddItem(ItemData item, int quantity)
    {

        for (int i = 0; i < quantity; i++)
        {

            if (item.canStack)
            {
                ItemSlot slot = Inventory.GetItemStack(item);
                if (slot != null)
                {
                    quantity++;
                    continue;
                }
            }

            ItemSlot emptySlot = Inventory.GetEmptySlot();

            if (emptySlot != null)
            {
                emptySlot.item = item;
                emptySlot.quantity = 1;
                continue;
            }

            Inventory.ThrowItem(item);

        }
       
    }

    public void RemoveItem(ItemData item, int amount)
    {
        foreach (var slot in Inventory.slots)
        {
            if (slot.item == item)
            {
                slot.quantity -= amount;
                if (slot.quantity <= 0)
                    slot.Clear();
            }
        }
    }

    public bool HasItem(ItemData item, int amount)
    {
        int totalAmount = 0;

        foreach (var slot in Inventory.slots)
        {
            if (slot.item == item)
            {
                totalAmount += slot.quantity;
                if (totalAmount >= amount)
                {
                    return true;
                }
            }
        }
        return false;

    }




}
