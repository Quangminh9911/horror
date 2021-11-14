using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FpsController : MonoBehaviour
{




    [Header("Functional Options")]
    [SerializeField] private bool canSprint = true;
    [SerializeField] private bool canJump = true;
    [SerializeField] private bool canCrouch = true;


    [Header("Controls")]
    [SerializeField] private KeyCode sprintkey = KeyCode.LeftShift;
    [SerializeField] private KeyCode jumpkey = KeyCode.Space;
    [SerializeField] private KeyCode crouchkey = KeyCode.LeftControl;
    public bool CanMove { get; private set; } = true;


    private bool IsSprinting => canSprint && Input.GetKey(sprintkey);
    private bool ShouldJump => Input.GetKeyDown(jumpkey) && characterController.isGrounded;
    private bool ShouldCrouch => Input.GetKeyDown(crouchkey) && !duringCrouchAnimation && characterController.isGrounded;



    [Header("Moverment Parameters")]
    [SerializeField] private float walkSpeed = 3.0f;
    [SerializeField] private float sprintSpeed = 6.0f;
    [SerializeField] private float crouchSpeed = 1.5f;


    [Header("Look Parameters")]
    [SerializeField, Range(1, 10)] private float lookspeedX = 2.0f;
    [SerializeField, Range(1, 10)] private float lookspeedY = 2.0f;
    [SerializeField, Range(1, 100)] private float upperlooklimit = 80f;
    [SerializeField, Range(1, 100)] private float lowerlooklimit = 80f;

    [Header("Jumping parameters")]
    [SerializeField] private float jumpForce = 8.0f;
    [SerializeField] private float gravity = 30f;

    [Header("Crouch Parameters")]
    [SerializeField] private float crouchHeight = 0.5f;
    [SerializeField] private float standingHeight = 2f;
    [SerializeField] private float timetoCrouch = 0.25f;
    [SerializeField] private Vector3 crouchingCenter = new Vector3(0, 0.5f, 0);
    [SerializeField] private Vector3 standingCenter = new Vector3(0, 0, 0);
    private bool isCrouching;
    private bool duringCrouchAnimation;

    private Camera playerCamera;
    private CharacterController characterController;
    private Vector3 moveDirection;
    private Vector2 currentInput;

    private float rotationX = 0;

    void Awake()
    {
        playerCamera = GetComponentInChildren<Camera>();
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    void Update()
    {
        if (CanMove)
        {
            HandleMovementInput();
            HandleMouselook();

            if (canJump) 
                HandleJump();

            if (canCrouch) 
                HandleCrouch();

            ApplyFinalMovements();

        }
    }

    private void HandleMovementInput()
    {
        currentInput = new Vector2((isCrouching ? crouchSpeed : IsSprinting ? sprintSpeed : walkSpeed) * Input.GetAxis("Vertical"), (isCrouching ? crouchSpeed : IsSprinting ? sprintSpeed : walkSpeed) * Input.GetAxis("Horizontal"));

        float moveDirectionY = moveDirection.y;
        moveDirection = (transform.TransformDirection(Vector3.forward) * currentInput.x) + (transform.TransformDirection(Vector3.right) * currentInput.y);
        moveDirection.y = moveDirectionY;
    }
    private void HandleMouselook()
    {
        rotationX -= Input.GetAxis("Mouse Y") * lookspeedY;
        rotationX = Mathf.Clamp(rotationX, -upperlooklimit, lowerlooklimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookspeedX, 0);
    }

    private void HandleJump()
    {
        if (ShouldJump)
        {
            moveDirection.y = jumpForce;
        }
    }
    private void HandleCrouch()
    {
        if (ShouldCrouch)
            StartCoroutine(CrouchStand());
    }
    private void ApplyFinalMovements()
    {
        if (!characterController.isGrounded)
            moveDirection.y -= gravity * Time.deltaTime;

        characterController.Move(moveDirection * Time.deltaTime);
    }
    private IEnumerator CrouchStand()
    {
        duringCrouchAnimation = true;


        Vector3 targerCenter = isCrouching ? standingCenter : crouchingCenter;
        Vector3 currentCenter = characterController.center;
        float timeElapsed = 0f;
        float targetHeight = isCrouching ? standingHeight : crouchHeight;
        float currentHeight = characterController.height;
        

        while (timeElapsed < timetoCrouch)
        {
            characterController.center = Vector3.Lerp(currentCenter, targerCenter, timeElapsed/timetoCrouch);
            characterController.height = Mathf.Lerp(currentHeight, targetHeight, timeElapsed/timetoCrouch);
            
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        characterController.height = targetHeight;
        characterController.center = targerCenter;

        isCrouching = !isCrouching;

        duringCrouchAnimation = false;
    }
}
