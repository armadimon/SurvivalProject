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
    public LayerMask layerMask;     // ?癒?퉳????됱뵠????쇱젟

    // ?袁⑹삺 揶쏅Ŋ????怨뱀깈?臾믪뒠 揶쎛?館釉???삵닏??븍뱜
    public GameObject curInteractGameObject;
    private IInteractable curInteractable; // 揶쏅Ŋ?????삵닏??븍뱜??IInteractable ?紐낃숲??륁뵠??

    public TextMeshProUGUI promptText; // ?怨뱀깈?臾믪뒠 ??덇땀 ?얜㈇??UI
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

            // ?遺얇늺 餓λ쵐釉?癒?퐣 ??됱뵠筌?Ŋ?????곴퐣 ?癒?퉳??띾┛
            Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, maxCheckDistance, layerMask))
            {
                // ?怨뱀깈?臾믪뒠 ??삵닏??븍뱜??揶쏅Ŋ???됱뱽 野껋럩????낅쑓??꾨뱜
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
