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

    public float runSpeedMultiplier;    // ???곫묾?????猷???얜즲 獄쏄퀣??
    public float runStamina;            // ???걟??롫뮉 ???곫묾???쎈믦첋紐껉돌
    private float originMoveSpeed;      // ?λ뜃由???猷???얜즲 (癰귣벀???

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

    // ??筌띾뜆?녷묾?
    public bool isInHydrateLocation = false;
    public bool isDrinking = false;


    public Action Inventory;            // ?紐껉뭣?醫듼봺 ??용┛ ??源??
    private PlayerCondition playerCondition; // PlayerCondition ?뚮똾猷??곕뱜 (??쎄묶沃섎챶援????怨밴묶 ?온??


    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        playerCondition = GetComponent<PlayerCondition>();
        originMoveSpeed = moveSpeed;                // ?λ뜃由???猷???얜즲 ????
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
        // 燁삳?李?????읈 筌ｌ꼶?곭몴???묐뻬 (筌띾뜆?????낆젾 獄쏆꼷??
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


    // ???곫묾???낆젾 筌ｌ꼶??(??쎄묶沃섎챶援????걟 獄???猷???얜즲 筌앹빓?)
    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            Debug.Log("a");
            // ???곫묾???뽰삂: ??쎄묶沃섎챶援밭몴????걟??랁???猷???얜즲???誘れ뿫
            if (playerCondition.UseStamina(runStamina))
            {
                moveSpeed *= runSpeedMultiplier;
                StartCoroutine(RunStaminaDrain());
            }
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            // ???곫묾?餓λ쵐? ???λ뜃由???猷???얜즲嚥?癰귣벀??
            moveSpeed = originMoveSpeed;
        }
    }

    // ???곫묾???筌왖??우읅??곗쨮 ??쎄묶沃섎챶援????걟??롫뮉 ?꾨뗀竊??
    private IEnumerator RunStaminaDrain()
    {
        while (moveSpeed > originMoveSpeed)
        {
            if (!playerCondition.UseStamina(runStamina * Time.deltaTime))
            {
                // ??쎄묶沃섎챶援??봔鈺?????猷???얜즲???λ뜃由??
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

    // ?紐껉뭣?醫듼봺 ?紐꾪뀱 ??낆젾 筌ｌ꼶??(?紐껉뭣?醫듼봺 UI ??뽯뻻)
    public void OnInventory(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            Inventory?.Invoke();
            ToggleCursur(); // ?紐껉뭣?醫듼봺 ??쎈탞 ???뚣끉苑뚨몴???뽯뻻
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
