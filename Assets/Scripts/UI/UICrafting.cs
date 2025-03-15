using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICrafting : MonoBehaviour
{
    public bool HasResourceAmount(RequireResourceAmount requireResourceAmount, bool consume)
    {
        ItemSlot[] slots = InventotyManager.Instance.Inventory.slots;
        bool resourceFound = false;

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == null)
                continue;

            if (slots[i].item.type == ItemType.Resouce && slots[i].item.resourceType == requireResourceAmount.type)
            {
                resourceFound = true;

                if (slots[i].quantity >= requireResourceAmount.value)
                {
                    if (consume)
                    {
                        slots[i].quantity -= requireResourceAmount.value;
                    }
                    return true;
                }
            }
        }

        if (!resourceFound)
        {
            Debug.Log($"?몃깽?좊━??{requireResourceAmount.type} 由ъ냼?ㅺ? ?놁뒿?덈떎!");
        }
        return false;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
