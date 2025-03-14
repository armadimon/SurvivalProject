using UnityEngine;
public enum ItemType
{
    Resouce,        // ?????
    Equipable,      // ?潁??
    Consumable,     // ?????
    interactable,   // ????繹??????
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
    public string disPlayName;  // ??戮?뻣???熬곣뫗逾?????藥?
    public string description;  // ?熬곣뫗逾?????닿뎄
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