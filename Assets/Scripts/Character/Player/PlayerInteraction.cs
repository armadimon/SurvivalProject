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
    public LayerMask layerMask;     // ?????????嚥싲갭큔?댁옃紐?????嚥싲갭큔???

    // ?????諛몃마??????ル늉????????????????????ル늉????關??濡녹춻?????????????⑥ル럯??
    public GameObject curInteractGameObject;
    private IInteractable curInteractable; // ????ル늉????????????????⑥ル럯???IInteractable ??轅붽틓??筌뚮챶夷?????癰궽블뀮???

    public TextMeshProUGUI promptText; // ???????????????? ?????獄??UI
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

            // ????嫄?彛???關???꾨き??熬곥룊??????????嚥싲갭큔?댁옃紐??щ뀋???????????닿튃癲??????????ш끽維귞댆?
            Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, maxCheckDistance, layerMask))
            {
                // ??????????????????????⑥ル럯???????ル늉??????嚥싲갭큔?????棺堉?뤃???????????살숲??????밸븶???
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
