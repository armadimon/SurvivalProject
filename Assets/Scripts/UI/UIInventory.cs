using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIInventory : MonoBehaviour
{
    public ItemSlot[] slots;

    public Transform dropPosition;

    public Transform slotPanel;
    public GameObject inventoryWindow;
    public GameObject itemDescription;
    public GameObject itemUseImage;

    [Header("Select Item")]
    public Image selectItemIcon;
    public TextMeshProUGUI selectItemName;
    public TextMeshProUGUI selectItemDescription;
    public TextMeshProUGUI selectItemStatName;
    public TextMeshProUGUI selectItemStatValue;
    public GameObject exitButton;
    public GameObject useButton;
    public GameObject equipButton;
    public GameObject unEquipButton;
    public GameObject dropButton;

    private PlayerController controller;
    private PlayerCondition condition;

    private void Awake()
    {
        InventotyManager.Instance.Inventory = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        controller = CharacterManager.Instance.Player.controller;
        condition = CharacterManager.Instance.Player.condition;
        dropPosition = CharacterManager.Instance.Player.dropPosition;

        controller.Inventory += Toggle;
        CharacterManager.Instance.Player.addItem += AddItem;

        SelectItemSetting();

        slots = new ItemSlot[slotPanel.childCount];
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i] = slotPanel.GetChild(i).GetComponent<ItemSlot>();
            slots[i].index = i;
            slots[i].Inventory = this;
        }
        ClearSelectedItemWindow();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ClearSelectedItemWindow()
    {
        selectItemIcon = null;
        selectItemName.text = string.Empty;
        selectItemDescription.text = string.Empty;
        selectItemStatName.text = string.Empty;
        selectItemStatValue.text = string.Empty;

        exitButton.SetActive(false);
        useButton.SetActive(false);
        equipButton.SetActive(false);
        unEquipButton.SetActive(false);
        dropButton.SetActive(false);
    }

    void SelectItemSetting()
    {
        selectItemIcon = transform.Find("InfoBG/Icon").GetComponent<Image>();
        selectItemName = transform.Find("InfoBG/ItemName").GetComponent<TextMeshProUGUI>();
        selectItemDescription = transform.Find("InfoBG/Description").GetComponent<TextMeshProUGUI>();
        selectItemStatName = transform.Find("InfoBG/StatName").GetComponent<TextMeshProUGUI>();
        selectItemStatValue = transform.Find("InfoBG/StatValue").GetComponent<TextMeshProUGUI>();

        inventoryWindow = this.gameObject;
        slotPanel = transform.Find("InventoryBG/Slots");
        itemDescription = transform.Find("InfoBG").gameObject;
        itemUseImage = transform.Find("ItemUse").gameObject;

        exitButton = transform.Find("InventoryBG/ExitButton").gameObject;
        useButton = transform.Find("ItemUse/UseButton").gameObject;
        equipButton = transform.Find("ItemUse/EquipButton").gameObject;
        unEquipButton = transform.Find("ItemUse/UnEquipButton").gameObject;
        dropButton = transform.Find("ItemUse/DropButton").gameObject;


        itemDescription.SetActive(false);
        itemUseImage.SetActive(false);
        inventoryWindow.SetActive(false);
    }

    public void Toggle()
    {
        if (IsOpen())
        {
            inventoryWindow.SetActive(false);
        }
        else
        {
            inventoryWindow.SetActive(true);
        }
    }

    public bool IsOpen()
    {
        return inventoryWindow.activeInHierarchy;
    }

    void AddItem()
    {
        ItemData data = CharacterManager.Instance.Player.item;

        if (data.canStack)
        {
            ItemSlot slot = GetItemStack(data);
            if (slot != null)
            {
                slot.quantity++;
                UpdateUI();
                CharacterManager.Instance.Player.item = null;
                return;
            }
        }

        ItemSlot emptySlot = GetEmptySlot();

        if (emptySlot != null)
        {
            emptySlot.item = data;
            emptySlot.quantity = 1;
            UpdateUI();
            CharacterManager.Instance.Player.item = null;
            return;
        }
        
    }

    void UpdateUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] != null)
            {
                slots[i].Set();
            }
            else
            {
                slots[i].Clear();
            }
        }
    }

    ItemSlot GetItemStack(ItemData data)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == data && slots[i].quantity < data.maxStackAmount)
            {
                return slots[i];
            }
        }
        return null;
    }

    ItemSlot GetEmptySlot()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == null)
            {
                return slots[i];
            }
        }

        return null;
    }

    void ThrowItem(ItemData data)
    {
        Instantiate(data.dropPrefab, dropPosition.position, Quaternion.Euler(Vector3.one * Random.value * 360));
    }

}
