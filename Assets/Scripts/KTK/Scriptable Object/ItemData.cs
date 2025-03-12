using UnityEngine;
public enum ItemType
{
    Resouce,        // 자원
    Equipable,      // 장비
    Consumable,     // 소비
    interactable,   // 상호작용
}

public enum ConsumableType
{
    Health,
    Hunger,
    Stamina,
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
    public string disPlayName;  // 보여지는 이름
    public string description;  // 설명
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
}