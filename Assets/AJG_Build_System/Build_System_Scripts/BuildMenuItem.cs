using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuildMenuItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image icon;
    public BuildObjectInfo infoBG;
    public TextMeshProUGUI nameText;
    public Button selectButton;
    public BuildObjectData buildObjectData;
    public RequireResourceAmount[] requireResourceTypes;
    // public BuildObject buildObject;
    public int index;

    private void Start()
    {
        // infoBG = GameObject.Find("BuildInfoBG").gameObject;
        infoBG = BuildManager.Instance.buildInfoBG;
    }
    
    public void SetData(BuildObject newBuildObject, int index)
    {  
        buildObjectData = newBuildObject.data;
        selectButton.image.sprite = newBuildObject.data.icon;
        nameText.text = newBuildObject.data.displayName;
        requireResourceTypes = newBuildObject.data.requireResources;
        // buildObject = newBuildObject;
        // buildObject.data.OnClick = () => BuildManager.Instance.buildController.SetBuildObject(buildObject);
        //
        selectButton.onClick.AddListener(() => BuildManager.Instance.CheckSufficientResources(newBuildObject, index));
    }
    
    public void SetInfoBG()
    {
        SetItemDiscriptionPosition();
        infoBG.SetBuildObjectData(buildObjectData);
    }
    void SetItemDiscriptionPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        RectTransform rectDescrition = infoBG.GetComponent<RectTransform>();

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
        infoBG.transform.position = mousePos;
    }

    
    public void OnPointerEnter(PointerEventData eventData)
    {
        infoBG.gameObject.SetActive(true);
        SetInfoBG();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        infoBG.gameObject.SetActive(false);
    }
}
