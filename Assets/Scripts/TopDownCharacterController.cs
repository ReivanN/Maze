using UnityEngine;
using UnityEngine.InputSystem;

public class TopDownCharacterController : MonoBehaviour
{
    [SerializeField]public float moveSpeed = 5f;
    [SerializeField][HideInInspector]private Vector2 moveInput;
    [SerializeField][HideInInspector]private CharacterController characterController;
    [SerializeField][HideInInspector]private Animator animator;
    [SerializeField][HideInInspector]public Transform cameraTransform;
    [SerializeField]public float cameraSmoothSpeed = 5f;
    [SerializeField][HideInInspector]public float gravity = 9.81f;
    [SerializeField][HideInInspector]private Vector3 velocity;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    void Update()
    {
        RotateTowardsMouse();
        UpdateAnimations();
        MoveCamera();
        Move();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void Move()
    {
        Vector3 moveVector = new Vector3(moveInput.x, 0, moveInput.y) * moveSpeed;

        if (!characterController.isGrounded)
        {
            velocity.y -= gravity * Time.deltaTime;
        }
        else
        {
            velocity.y = -0.1f;
        }

        characterController.Move((moveVector + velocity) * Time.deltaTime);
    }

    private void RotateTowardsMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 lookPos = new Vector3(hit.point.x, transform.position.y, hit.point.z);
            transform.LookAt(lookPos);
        }
    }

    private void UpdateAnimations()
    {
        animator.SetFloat("VelocityX", moveInput.x);
        animator.SetFloat("VelocityY", moveInput.y);
        animator.SetFloat("Speed", moveInput.sqrMagnitude);
    }

    private void MoveCamera()
    {
        if (cameraTransform != null)
        {
            Vector3 targetPosition = new Vector3(transform.position.x, cameraTransform.position.y, transform.position.z);
            cameraTransform.position = Vector3.Lerp(cameraTransform.position, targetPosition, cameraSmoothSpeed * Time.deltaTime);
        }
    }
}
