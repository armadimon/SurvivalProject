using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResourceItemUI : MonoBehaviour
{
    public TextMeshProUGUI resourceTypeText;
    public TextMeshProUGUI resourceAmountText;
    public void SetResource(RequireResourceType type, int amount)
    {
        if (type == RequireResourceType.Stone)
            resourceTypeText.text = "돌";
        else if (type == RequireResourceType.Wood)
            resourceTypeText.text = "나무";
        // 리소스 타입 추가되면 점차 확장. 타입이 확정되면 text가 아니라 이미지로 표현할 수 있도록 교체할 예정
        resourceAmountText.text = amount.ToString();
    }
}
