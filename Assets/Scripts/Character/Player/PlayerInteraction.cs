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
    public LayerMask layerMask;     // ?????????濚밸Ŧ援잏몭?????濚밸Ŧ???

    // ?????밸븶?????ル봿?????????嚥?????????ル봿????μ떝?롳쭗????????????怨쀫뎐??
    public GameObject curInteractGameObject;
    private IInteractable curInteractable; // ???ル봿???????????????怨쀫뎐???IInteractable ??꿔꺂??琉몃쨨?????蹂κ텤???

    public TextMeshProUGUI promptText; // ????嚥??????????? ?????諭??UI
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

            // ????거?쭛???μ떝?띄몭??袁㏉떋?????????濚밸Ŧ援잏몭?レ녇??????????살깙嶺?????????熬곣뫂爰?
            Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, maxCheckDistance, layerMask))
            {
                // ????嚥????????????????怨쀫뎐??????ル봿??????濚밸Ŧ援????β뼯援??????????띻콣?????썹땟???
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
