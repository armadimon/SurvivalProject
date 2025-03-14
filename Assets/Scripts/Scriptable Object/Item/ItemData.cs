using UnityEngine;
public enum ItemType
{
    Resouce,        // ?癒?뜚
    Equipable,      // ?貫??
    Consumable,     // ???돩
    interactable,   // ?怨뱀깈?臾믪뒠
}

public enum ConsumableType
{
    Health,
    Hunger,
    Thirst,
    Stamina,
}

public enum RequireResourceType
{
    Wood,
    Stone,
}

[System.Serializable]
public class RequireResourceAmount
{
    public RequireResourceType type;
    public int value;
}

[System.Serializable]
public class ItemDataConsumable
{
    public ConsumableType type;
    public float value;
}

[CreateAssetMenu(fileName = "Item", menuName = "New Item")]
public class ItemData : ScriptableObject
{
    [Header("Item Info")]
    public string disPlayName;  // ?쒖떆???꾩씠???대쫫
    public string description;  // ?꾩씠???ㅻ챸
    public ItemType type;
    public Sprite icon;
    public GameObject dropPrefab;

    [Header("Setting")]
    public bool canStack;
    public int maxStackAmount;

    [Header("Consumable")]
    public ItemDataConsumable[] consumables;

    [Header("Eqip")]
    public GameObject equipPrefab;
    
    [Header("ResourceType")]
    public RequireResourceType resourceType;
}