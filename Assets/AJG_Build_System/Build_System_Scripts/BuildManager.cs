using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BuildManager : MonoBehaviour
{
    private static BuildManager _instance;
    public static BuildManager Instance
    {
        get { return _instance; }
    }

    public GameObject Enviornment;

    public GameObject buildMenu;
    public BuildObjectInfo buildInfoBG;
    public Transform buildMenuContent;
    public BuildObject[] buildObjects;
    public List<BuildMenuItem> BuildMenuItems = new List<BuildMenuItem>();
    public BuildMenuItem buildMenuItemPrefabs;
    public BuildController buildController;
    public Action OnClick;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    
    void Start()
    {
        int index = 0;
        foreach (BuildObject buildObject in buildObjects)
        {
            BuildMenuItem buildItem = Instantiate(buildMenuItemPrefabs, buildMenuContent);
            BuildMenuItems.Add(buildItem);
            buildItem.SetData(buildObject, index);
            index++;
        }
    }

    public void CheckSufficientResources(BuildObject buildObject, int index)
    {
        RequireResourceAmount[] requireResources = buildObject.data.requireResources;
        
        foreach (var requireResource in requireResources)
        {
            if (!HasResourceAmount(requireResource, false))
            {
                NotificationManager.Instance.ShowNotification("자원이 부족합니다!!");
                return ;
            }
        }

        foreach (var requireResource in requireResources)
        {
            HasResourceAmount(requireResource, true);
        }
        buildController.SetBuildObject(buildObject);
    }

    public void OnBuildMode(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            CharacterManager.Instance.Player.controller.playerInput.SwitchCurrentActionMap("BuildMode");
            buildController.OnBuildMode(context);
        }
    }
    
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
        return false;
    }
}
