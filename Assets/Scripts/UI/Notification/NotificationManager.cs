using UnityEngine;
using TMPro;
using DG.Tweening;

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager Instance;
    public Transform notificationParent;

    void Awake()    
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        else
            Instance = this;
    }

    public void ShowNotification(string message)
    {
        GameObject notification = NotificationPool.Instance.GetNotification();
        if(notification != null)
        {
            notification.transform.SetParent(notificationParent, false);
            notification.SetActive(true);
        }
        
        TextMeshProUGUI text = notification.GetComponentInChildren<TextMeshProUGUI>();
        text.text = message;

        RectTransform rectTransform = notification.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = new Vector2(0, -100);
            rectTransform.DOAnchorPosY(0, 0.5f).SetEase(Ease.OutBack);
        }

        CanvasGroup canvasGroup = notification.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1f;
        if (canvasGroup != null)
        {
            DOVirtual.DelayedCall(2f, () =>
            {
                if (canvasGroup != null)
                {
                    canvasGroup.DOFade(0, 0.5f).OnComplete(() =>
                    {
                        notification.SetActive(false);
                        NotificationPool.Instance.ReturnNotification(notification);
                    });
                }
            });
        }
    }
}