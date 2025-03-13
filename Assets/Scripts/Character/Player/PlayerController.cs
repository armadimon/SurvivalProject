using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    private Vector2 _curMoveInput;
    public float jumpPower;
    public LayerMask groundLayerMask;
    public bool isGrounded;
    public float playerHeight;

    public float runSpeedMultiplier;    // ?щ━湲????대룞 ?띾룄 諛곗닔
    public float runStamina;            // ?뚮え?섎뒗 ?щ━湲??ㅽ뀒誘몃굹
    private float originMoveSpeed;      // 珥덇린 ?대룞 ?띾룄 (蹂듦뎄??

    [Header("Look")]
    public Transform cameraContainer;
    public float minXLook;
    public float maxXLook;
    private float camCurXRot;
    public float lookSensitivity;
    private Vector2 mouseDelta;
    public bool canLook = true;

    private Rigidbody _rigidbody;
    private BuildController _buildController;

    // 臾?留덉떆湲?
    public bool isInHydrateLocation = false;
    public bool isDrinking = false;


    public Action Inventory;            // ?몃깽?좊━ ?닿린 ?대깽??
    private PlayerCondition playerCondition; // PlayerCondition 而댄룷?뚰듃 (?ㅽ깭誘몃굹 ???곹깭 愿由?


    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        playerCondition = GetComponent<PlayerCondition>();
        originMoveSpeed = moveSpeed;                // 珥덇린 ?대룞 ?띾룄 ???
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _buildController = GetComponent<BuildController>();
    }

    private void Update()
    {
        LimitSpeed();
    }

    private void FixedUpdate()
    {
        IsGrounded();
        Move();
    }

    private void LateUpdate()
    {
        // 移대찓???뚯쟾 泥섎━瑜??섑뻾 (留덉슦???낅젰 諛섏쁺)
        if (canLook)
        {
            CameraLook();
        }
        //Debug.DrawRay(transform.position + (transform.forward * 0.2f) + (transform.up * 0.01f), Vector3.down * 1.5f, Color.red);
    }

    void Move()
    {
        Vector3 dir = transform.forward * _curMoveInput.y + transform.right * _curMoveInput.x;
        // dir.y = _rigidbody.velocity.y;
        
        _rigidbody.AddForce(dir.normalized * (moveSpeed * 10f), ForceMode.Force);
    }

    void CameraLook()
    {
        camCurXRot += mouseDelta.y * lookSensitivity;
        camCurXRot = Mathf.Clamp(camCurXRot, minXLook, maxXLook);
        cameraContainer.localEulerAngles = new Vector3(-camCurXRot, 0, 0);
        
        transform.eulerAngles += new Vector3(0, mouseDelta.x * lookSensitivity, 0);
    }
    
    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            _curMoveInput = context.ReadValue<Vector2>();
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            _curMoveInput = Vector2.zero;
        }
    }


    // ?щ━湲??낅젰 泥섎━ (?ㅽ깭誘몃굹 ?뚮え 諛??대룞 ?띾룄 利앷?)
    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            Debug.Log("a");
            // ?щ━湲??쒖옉: ?ㅽ깭誘몃굹瑜??뚮え?섍퀬 ?대룞 ?띾룄瑜??믪엫
            if (playerCondition.UseStamina(runStamina))
            {
                moveSpeed *= runSpeedMultiplier;
                StartCoroutine(RunStaminaDrain());
            }
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            // ?щ━湲?以묒? ??珥덇린 ?대룞 ?띾룄濡?蹂듦뎄
            moveSpeed = originMoveSpeed;
        }
    }

    // ?щ━湲???吏?띿쟻?쇰줈 ?ㅽ깭誘몃굹 ?뚮え?섎뒗 肄붾（??
    private IEnumerator RunStaminaDrain()
    {
        while (moveSpeed > originMoveSpeed)
        {
            if (!playerCondition.UseStamina(runStamina * Time.deltaTime))
            {
                // ?ㅽ깭誘몃굹 遺議????대룞 ?띾룄瑜?珥덇린??
                moveSpeed = originMoveSpeed;
                break;
            }
            yield return null;
        }
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        mouseDelta = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        
        if (context.phase == InputActionPhase.Started && IsGrounded())
        {
            _rigidbody.AddForce(Vector2.up * jumpPower, ForceMode.Impulse);
        }
    }
    

    void ToggleCursur()
    {
        bool toggle = Cursor.lockState == CursorLockMode.Locked;
        Cursor.lockState = toggle ? CursorLockMode.None : CursorLockMode.Locked;
        canLook = !toggle;
    }
    bool IsGrounded()
    {
        Ray[] rays = new Ray[4]
        {
            new Ray(transform.position + (transform.forward * 0.2f), Vector3.down),
            new Ray(transform.position + (-transform.forward * 0.2f), Vector3.down),
            new Ray(transform.position + (transform.right * 0.2f), Vector3.down),
            new Ray(transform.position + (-transform.right * 0.2f), Vector3.down),
        };
        for (int i = 0; i < rays.Length; ++i)
        {
            Debug.DrawRay(rays[i].origin, rays[i].direction * playerHeight, Color.red);
            if (Physics.Raycast(rays[i], playerHeight + 0.2f, groundLayerMask))
            {
                return true;
            }
        }
        return false;
    }

    private void LimitSpeed()
    {
        Vector3 flatVelocity = new Vector3(_rigidbody.velocity.x, 0, _rigidbody.velocity.z);

        if (flatVelocity.magnitude > moveSpeed)
        {
            Vector3 limitVelocity = flatVelocity.normalized * moveSpeed;
            _rigidbody.velocity = new Vector3(limitVelocity.x, _rigidbody.velocity.y, limitVelocity.z);
        }
    }

    // ?몃깽?좊━ ?몄텧 ?낅젰 泥섎━ (?몃깽?좊━ UI ?쒖떆)
    public void OnInventory(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            Inventory?.Invoke();
            ToggleCursur(); // ?몃깽?좊━ ?ㅽ뵂 ??而ㅼ꽌瑜??쒖떆
        }
    }

    public void OnDrinking(InputAction.CallbackContext context)
    {
        if (isInHydrateLocation && context.phase == InputActionPhase.Started)
        {
            isDrinking = true;
            Invoke("StopDrinking", 5f);
        }
    }

    void StopDrinking()
    {
        isDrinking = false;
    }
}
