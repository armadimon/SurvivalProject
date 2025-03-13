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
    public LayerMask layerMask;     // ?????????繹먮굟瑗?????繹먮냱??

    // ????썹땟????醫딆┫????????濚????????醫딆쓧??嚥싳쇎紐???????鍮????곗뵯??
    public GameObject curInteractGameObject;
    private IInteractable curInteractable; // ??醫딆┫?????????鍮????곗뵯???IInteractable ?癲ル슢?뤸뤃?????볥궙???

    public TextMeshProUGUI promptText; // ????濚??????????? ?????뱥??UI
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

            // ???됰Ŧ六??嚥싳쉶瑗??꾧틡?????????繹먮굟瑗㎫솾?????????ㅻ샑筌?????????袁⑦꺙
            Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, maxCheckDistance, layerMask))
            {
                // ????濚???????????鍮????곗뵯?????醫딆┫?????繹먮굛???嚥▲굧?????????욍걛???ш끽維??
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
