using UnityEngine;
public enum ItemType
{
    Resouce,        // ?먯썝
    Equipable,      // ?λ퉬
    Consumable,     // ?뚮퉬
    interactable,   // ?곹샇?묒슜
}

public enum ConsumableType
{
    Health,
    Hunger,
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
    public string disPlayName;  // 표시될 아이템 이름
    public string description;  // 아이템 설명
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