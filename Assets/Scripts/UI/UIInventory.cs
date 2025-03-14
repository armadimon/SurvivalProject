using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.UI;

public class UIInventory : MonoBehaviour
{
    public ItemSlot[] slots;
    public ItemSlot slotItemEquip;

    public Transform dropPosition;

    public Transform slotPanel;
    public GameObject inventoryWindow;
    public GameObject itemDescription;
    public GameObject itemUseImage;
    private RectTransform itemUseRect;
    private Vector2 usuallyitemUseRect;
    private Vector3 dropButtonPosition;
    private GameObject halfDropButton;

    [Header("Select Item")]
    public Image selectItemIcon;
    public TextMeshProUGUI selectItemName;
    public TextMeshProUGUI selectItemDescription;
    public TextMeshProUGUI selectItemStatName;
    public TextMeshProUGUI selectItemStatValue;
    public Button exitButton;
    public GameObject useButton;
    public GameObject equipButton;
    public GameObject unEquipButton;
    public GameObject dropButton;

    private int curEquipIndex;
    private PlayerController controller;
    private PlayerCondition condition;
    public ItemSlot selectItem;

    public bool isUseItemWindow = false;

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

        InitUI();

        UpdateUI();
    }

    // Update is called once per frame
    void Update()
    {

    }

   

    // UI 기본세팅
    void InitUI()
    {
        selectItemIcon = transform.Find("InfoBG/InfoIcon").GetComponent<Image>();
        selectItemName = transform.Find("InfoBG/ItemName").GetComponent<TextMeshProUGUI>();
        selectItemDescription = transform.Find("InfoBG/Description").GetComponent<TextMeshProUGUI>();
        selectItemStatName = transform.Find("InfoBG/StatName").GetComponent<TextMeshProUGUI>();
        selectItemStatValue = transform.Find("InfoBG/StatValue").GetComponent<TextMeshProUGUI>();

        inventoryWindow = this.gameObject;
        slotPanel = transform.Find("InventoryBG/Slots");
        itemDescription = transform.Find("InfoBG").gameObject;
        itemUseImage = transform.Find("ItemUse").gameObject;

        itemUseRect = itemUseImage.GetComponent<RectTransform>();
        usuallyitemUseRect = itemUseRect.sizeDelta;
        

        exitButton = transform.Find("InventoryBG/ExitButton").GetComponent<Button>();
        useButton = transform.Find("ItemUse/UseButton").gameObject;
        equipButton = transform.Find("ItemUse/EquipButton").gameObject;
        unEquipButton = transform.Find("ItemUse/UnEquipButton").gameObject;
        dropButton = transform.Find("ItemUse/DropButton").gameObject;
        dropButtonPosition = dropButton.GetComponent<RectTransform>().anchoredPosition;

        useButton.GetComponent<Button>().onClick.AddListener(OnUseButton);
        dropButton.GetComponent<Button>().onClick.AddListener(OnDropButton);
        equipButton.GetComponent<Button>().onClick.AddListener(OnEquipButton);
        unEquipButton.GetComponent<Button>().onClick.AddListener (OnUnEquipButton);

        slotItemEquip = transform.Find("InventoryBG/ItemEquipSlot").GetComponent<ItemSlot>();
        slotItemEquip.index = 50;
        slotItemEquip.Inventory = this;

        slots = new ItemSlot[slotPanel.childCount];
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i] = slotPanel.GetChild(i).GetComponent<ItemSlot>();
            slots[i].index = i;
            slots[i].Inventory = this;
        }

        ClearSelectItemWindow();


        itemDescription.SetActive(false);
        itemUseImage.SetActive(false);
        inventoryWindow.SetActive(false);

        exitButton.onClick.AddListener(OnExitButton);

        
    }
     
    void ClearSelectItemWindow()
    {
        selectItem = null;

        selectItemName.text = string.Empty;
        selectItemDescription.text = string.Empty;
        selectItemStatName.text = string.Empty;
        selectItemStatValue.text = string.Empty;

        useButton.SetActive(false);
        equipButton.SetActive(false);
        unEquipButton.SetActive(false);
        dropButton.SetActive(false);
    }

    public void Toggle()
    {
        if (IsOpen())
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
            }


            inventoryWindow.SetActive(false);
            itemDescription.SetActive(false);
            itemUseImage.SetActive(false);
            isUseItemWindow = itemDescription.activeSelf;
            UsuallytemUseImage();
        }
        else
        {

            inventoryWindow.SetActive(true);
        }
    }

    public bool IsOpen()
    {
        if (Cursor.lockState == CursorLockMode.None)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
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

        ThrowItem(data);
        CharacterManager.Instance.Player.item = null;


    }

    void UpdateUI()

    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item != null)
            {
                slots[i].Set();
            }
            else
            {
                slots[i].Clear();
            }
        }
        if (slotItemEquip.item != null)
        {
            slotItemEquip.Set();
        }
        else
        {
            slotItemEquip.Clear();
        }
        
        isUseItemWindow = itemDescription.activeSelf;
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

    public void ShowItemDescription(ItemSlot slot)
    {
        if (!itemDescription.activeSelf && !isUseItemWindow)
        {
            itemDescription.SetActive(true);
            SetItemDiscriptionPosition();

            selectItemIcon.sprite = slot.item.icon;
            selectItemName.text = slot.item.disPlayName;
            selectItemDescription.text = slot.item.description;

            for (int i = 0; i < slot.item.consumables.Length; i++)
            {
                selectItemStatName.text += slot.item.consumables[i].type.ToString() + "\n";
                selectItemStatValue.text += slot.item.consumables[i].value.ToString() + "\n";
            }
        }

    }

    void SetItemDiscriptionPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        RectTransform rectDescrition = itemDescription.GetComponent<RectTransform>();

        float offsetX = rectDescrition.rect.width / 2;
        float offsetY = rectDescrition.rect.height / 2;

        mousePos.x += offsetX + 20;
        for (int i = 0; i < 3; i++)
        {
            if (mousePos.y - offsetY < 0)
            {
                mousePos.y += offsetY / 2;
            }
            else if (mousePos.y + offsetY > Screen.height)
            {
                mousePos.y -= offsetY / 2;
            }
        }

        itemDescription.transform.position = mousePos;
    }

    public void HideItemDescription()
    {

        if (!IsMouseOverUI(itemDescription) && !IsMouseOverItemSlot())
            itemDescription.SetActive(false);
    }


    public bool IsMouseOverUI(GameObject gObject)
    {
        return RectTransformUtility.RectangleContainsScreenPoint
           (gObject.GetComponent<RectTransform>(), Input.mousePosition, Camera.main);
    }

    bool IsMouseOverItemSlot()
    {
        foreach (ItemSlot slot in slots)
        {
            if (IsMouseOverUI(slot.gameObject))
            {
                return true;
            }
        }

        return false;
    }


    #region OnClick

    public void OnExitButton()
    {
        inventoryWindow.SetActive(false);
        itemDescription.SetActive(false);
        itemUseImage.SetActive(false);
    }

    public void OnUseButton()
    {
        if (selectItem.item.type == ItemType.Consumable)
        {
            for (int i = 0; i < selectItem.item.consumables.Length; i++)
            {
                switch (selectItem.item.consumables[i].type)
                {
                    case ConsumableType.Health:
                        condition.Heal(selectItem.item.consumables[i].value);
                        break;
                    case ConsumableType.Hunger:
                        condition.Eat(selectItem.item.consumables[i].value);
                        break;
                    case ConsumableType.Thirst:
                        condition.Hydrate(selectItem.item.consumables[i].value);
                        break;
                    case ConsumableType.Stamina:
                        condition.HealStamina(selectItem.item.consumables[i].value);
                        break;
                }
            }
        }
        itemUseImage.SetActive(false);
    }

    public void OnEquipButton()
    {
        if (slotItemEquip.equipped)
        {
            UnEquip();
        }

        if (slotItemEquip.item != null)
        {
        }

        int saveSelectItemIndex = selectItem.index;

        ItemSlotChange(slots[selectItem.index], slotItemEquip);
        slotItemEquip.equipped = true;
        curEquipIndex = slotItemEquip.index;

        selectItem.index = saveSelectItemIndex;

        RemoveSelectItem();
        CharacterManager.Instance.Player.equipment.EquipNew(slotItemEquip.item);

        itemUseImage.SetActive(false);
        UpdateUI();
        
    }

    public void OnUnEquipButton()
    {
        UnEquip();
        itemUseImage.SetActive(false);
    }
    

    public void OnDropButton()
    {
        ThrowItem(selectItem.item);
        itemUseImage.SetActive(false);
        RemoveSelectItem();
    }

    #endregion

    void UnEquip()
    {
        slotItemEquip.equipped = false;
        ItemSlot emptySlot = GetEmptySlot();

        if (emptySlot != null)
        {
            ItemSlotChange(slotItemEquip, emptySlot);
            UpdateUI();
            CharacterManager.Instance.Player.item = null;
        }


        CharacterManager.Instance.Player.equipment.UnEquip();

    }

    void RemoveSelectItem()
    {
        slots[selectItem.index].quantity--;

        if (slots[selectItem.index].quantity <= 0)
        {
            selectItem.item = null;
            ClearSelectItemWindow();
        }

        UpdateUI();
    }

    void ItemSlotChange(ItemSlot depart, ItemSlot arrive)
    {
        ItemData tempItem = arrive.item;
        bool tempEquipped = arrive.equipped;
        int tempQuantity = arrive.quantity;

        arrive.item = depart.item;
        //arrive.icon = depart.icon;
        arrive.equipped = depart.equipped;
        arrive.quantity = depart.quantity;

        depart.item = tempItem;
        //depart.item.icon = tempItem.icon;
        depart.equipped = tempEquipped;
        depart.quantity = tempQuantity;

    }

    public void SelectItem(int index)
    {
        ItemSlot sSlot;
        if (0 <= index && index < slots.Length)
        {
            if (slots[index].item == null) return;
            else
                sSlot = slots[index];

        }
        else
        {
            if (slotItemEquip.index != index) return;
            else
                sSlot = slotItemEquip;
        }

        useButton.SetActive(selectItem.item.type == ItemType.Consumable);
        equipButton.SetActive(selectItem.item.type == ItemType.Equipable && !sSlot.equipped);
        unEquipButton.SetActive(selectItem.item.type == ItemType.Equipable && sSlot.equipped);

        if (!sSlot.equipped)
            dropButton.SetActive(true);
        else
            dropButton.SetActive(false);

        if (!useButton.activeSelf && !equipButton.activeSelf && !unEquipButton.activeSelf)
        {
            HalfItemUseImage();
        }
        else
        {
            UsuallytemUseImage();
        }

    }

    public void HalfItemUseImage()
    {
        dropButton.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 23, 0);
        itemUseRect.sizeDelta = new Vector2(usuallyitemUseRect.x, usuallyitemUseRect.y / 2);
    }

    public void UsuallytemUseImage()
    {
        dropButton.GetComponent<RectTransform>().anchoredPosition = dropButtonPosition;
        itemUseRect.sizeDelta = usuallyitemUseRect;
    }




}
