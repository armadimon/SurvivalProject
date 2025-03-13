using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    public float checkRate = 0.05f;
    private float lastCheckTime;
    public float maxCheckDistance;
    public LayerMask layerMask;     // ?????????源낇꼧?????源놁젳

    // ??ш끽維????좊즴????????繹????????좊읈??濚왿몾??????щ빘???됰씭肄?
    public GameObject curInteractGameObject;
    private IInteractable curInteractable; // ??좊즴????????щ빘???됰씭肄??IInteractable ?嶺뚮ㅏ援????쒓낮???

    public TextMeshProUGUI promptText; // ????繹??????????? ???뚮듋??UI
    private Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - lastCheckTime > checkRate)
        {
            lastCheckTime = Time.time;

            // ??釉먮뻤??濚욌꼬?댄꺍?????????源낇꼧癲????????⑤똾留?????????꾨탿
            Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, maxCheckDistance, layerMask))
            {
                // ????繹??????????щ빘???됰씭肄????좊즴?????源낃도 ?濡ろ뜑???????녿ぅ??熬곣뫀肄?
                if (hit.collider.gameObject != curInteractGameObject)
                {
                    curInteractGameObject = hit.collider.gameObject;
                    curInteractable = hit.collider.GetComponent<IInteractable>();

                    SetPromptText();
                }
            }
            else
            {
                curInteractGameObject = null;
                curInteractable = null;
                promptText.gameObject.SetActive(false);
            }
        }
    }

    private void SetPromptText()
    {
        promptText.gameObject.SetActive(true);
        promptText.text = curInteractable.GetInteractPrompt();
    }

    public void OnInteractInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && curInteractable != null)
        {
            curInteractable.OnInteract();
            curInteractGameObject = null;
            curInteractable = null;
            promptText.gameObject.SetActive(false);
        }
    }
}
