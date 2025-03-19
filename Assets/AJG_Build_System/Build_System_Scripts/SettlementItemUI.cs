using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettlementItem : MonoBehaviour
{
    public TextMeshProUGUI resourceTypeText;
    public TextMeshProUGUI resourceAmountText;
    public Button selectButton;
        
    public void SetData(string type, int amount)
    {
        resourceTypeText.text = type;
        resourceAmountText.text = amount.ToString();
        
        // selectButton.onClick.AddListener(() => SettlementManager.Instance.CheckSufficientResources(newBuildObject, false));
    }
}
