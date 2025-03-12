using UnityEngine;
using UnityEngine.InputSystem;

public class TopDownCharacterController : MonoBehaviour
{
    public float moveSpeed = 5f; // Скорость передвижения
    public Camera cameraTransform; // Камера, следящая за персонажем
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
            cameraHeight = cameraTransform.transform.position.y; // Запоминаем высоту камеры
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
            // Нормализуем вектор, чтобы скорость была постоянной
            moveDirection.Normalize();
            characterController.Move(moveDirection * moveSpeed * Time.deltaTime);

            // Поворачиваем персонажа в сторону движения
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
