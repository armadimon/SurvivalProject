using UnityEngine;
using TMPro;
using DG.Tweening;

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager Instance;
    public Transform notificationParent;

    void Awake()    
    {
        Instance = this;
    }

    public void ShowNotification(string message)
    {
        GameObject notification = NotificationPool.Instance.GetNotification();
        notification.transform.SetParent(notificationParent, false);
        notification.SetActive(true);

        TextMeshProUGUI text = notification.GetComponentInChildren<TextMeshProUGUI>();
        text.text = message;

        RectTransform rectTransform = notification.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(0, -100);
        rectTransform.DOAnchorPosY(0, 0.5f).SetEase(Ease.OutBack); 

        CanvasGroup canvasGroup = notification.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1f;
        DOVirtual.DelayedCall(2f, () =>
        {
            canvasGroup.DOFade(0, 0.5f).OnComplete(() =>
            {
                notification.SetActive(false);
                NotificationPool.Instance.ReturnNotification(notification);
            });
        });
    }
}