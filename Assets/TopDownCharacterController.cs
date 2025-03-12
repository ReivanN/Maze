using UnityEngine;
using UnityEngine.InputSystem;

public class TopDownCharacterController : MonoBehaviour
{
    public float moveSpeed = 5f; // �������� ������������
    public Camera cameraTransform; // ������, �������� �� ����������
    private CharacterController characterController;
    private Vector2 moveInput;
    private float cameraHeight;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        if (cameraTransform == null) 
        {
            cameraTransform = FindAnyObjectByType<Camera>();
        }
        if (cameraTransform != null)
        {
            cameraHeight = cameraTransform.transform.position.y; // ���������� ������ ������
        }
    }

    void Update()
    {
        Move();
        FollowCamera();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    void Move()
    {
        Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y);
        if (moveDirection.magnitude > 0.1f)
        {
            // ����������� ������, ����� �������� ���� ����������
            moveDirection.Normalize();
            characterController.Move(moveDirection * moveSpeed * Time.deltaTime);

            // ������������ ��������� � ������� ��������
            transform.forward = moveDirection;
        }
    }

    void FollowCamera()
    {
        if (cameraTransform != null)
        {
            cameraTransform.transform.position = new Vector3(transform.position.x, cameraHeight, transform.position.z);
        }
    }
}
