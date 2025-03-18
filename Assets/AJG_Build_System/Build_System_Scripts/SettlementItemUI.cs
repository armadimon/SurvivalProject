
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SettlementItem : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI DescriptionText;
    public GameObject infoBG;
    public Button selectButton;
    public RequireResourceAmount[] requireResourceTypes;
    // public BuildObject buildObject;
    public int index;

    public void SetData(BuildObject newBuildObject)
    {  
        icon.sprite = newBuildObject.data.icon;
        nameText.text = newBuildObject.data.displayName;
        DescriptionText.text = newBuildObject.data.description;
        requireResourceTypes = newBuildObject.data.requireResources;
        // buildObject = newBuildObject;
        // buildObject.data.OnClick = () => BuildManager.Instance.buildController.SetBuildObject(buildObject);
        //
        selectButton.onClick.AddListener(() => BuildManager.Instance.CheckSufficientResources(newBuildObject, false));
    }

    public void SetInfoBG()
    {
        
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        infoBG.SetActive(true);
        SetInfoBG(); // 데이터를 설정하는 함수 호출
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        infoBG.SetActive(false);
    }
}
