using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class TopDownCharacterController : MonoBehaviour, IDamageable
{
    [Header("PLayerStat")]
    public PlayerData playerData;
    private Vector2 moveInput;
    private CharacterController characterController;
    private Animator animator;
    public Transform cameraTransform;
    public float cameraSmoothSpeed = 5f;
    private float gravity = 9.81f;
    private Vector3 velocity;
    [SerializeField] private HealthUI healthUI;
    [SerializeField] private DeadUI deadUI;
    private float currentHealth;
    private float MAXHealth;
    public  event Action<float> onTakeDamage;
    [SerializeField] ParticleSystem[] myparticleSystem;

    [Header("FireStat")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 1f;
    private float nextFireTime = 0f;
    public float bulletSpeed = 10f;

    [Header("Stats")]
    public float currentDamage;
    private float currentFireRate;
    public Light mylight;

    [Header("Audio")]
    [SerializeField] private AudioSource myaudioSource;
    [SerializeField] private AudioClip gunShot;
    [SerializeField] private AudioClip pickUp;

    public float GetDamage() => currentDamage;

    private void LoadPlayerData()
    {
        if (!SaveManager.Instance.SaveExists())
        {
            Debug.LogWarning("���� ���������� �����������. ������������� �������� �� ���������.");
            currentHealth = playerData.health;
            MAXHealth = playerData.health;
            currentDamage = playerData.damage;
            currentFireRate = fireRate;
            return;
        }

        GameData data = SaveManager.Instance.Load();
        Debug.Log($"��������: HP = {data.health}, Max HP = {data.maxHealth}");

        currentHealth = data.health;
        MAXHealth = data.maxHealth;
        currentFireRate = data.fireRate;
        currentDamage = data.damage;
        healthUI.UpdateHealth(currentHealth, MAXHealth);
    }



    public void SavePlayerData()
    {
        GameData data = SaveManager.Instance.Load();
        data.health = currentHealth;
        data.maxHealth = MAXHealth;
        data.fireRate = currentFireRate;
        data.damage = currentDamage;

        SaveManager.Instance.Save(data);
        Debug.LogError("SAVE HP " + data.health);
    }


    public void DeletePlayerData()
    {
        SaveManager.Instance.DeleteSave();
    }
    void Awake()
    {
        healthUI = FindAnyObjectByType<HealthUI>();
        deadUI = FindAnyObjectByType<DeadUI>();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        
        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    private void Start()
    {
        if (SaveManager.Instance != null)
        {
            LoadPlayerData();
            Debug.Log($"����������� ������: HP = {currentHealth}, Max HP = {MAXHealth}");
        }
    }



    void Update()
    {
        RotateTowardsMouse();
        UpdateAnimations();
        MoveCamera();
        Move();
        HandleShooting();

        if (isFiring && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    public void ApplyUpgrade(Upgrade upgrade)
    {
        GameData data = SaveManager.Instance.Load();

        switch (upgrade.type)
        {
            case UpgradeType.HealthBoost:
                MAXHealth *= upgrade.value;
                //currentHealth = MAXHealth;
                break;
            case UpgradeType.DamageIncrease:
                currentDamage *= upgrade.value;
                break;
            case UpgradeType.HpPlus:
                currentHealth = Mathf.Min(currentHealth + upgrade.value, MAXHealth);
                break;
            case UpgradeType.FireRate:
                currentFireRate *= upgrade.value;
                break;
        }

        data.maxHealth = MAXHealth;
        data.health = currentHealth;
        data.damage = currentDamage;
        data.fireRate = currentFireRate;
        data.appliedUpgrades.Add(upgrade.name);
        SaveManager.Instance.Save(data);
        healthUI.UpdateHealth(currentHealth, MAXHealth);
    }


    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void Move()
    {
        Vector3 moveVector = new Vector3(moveInput.x, 0, moveInput.y) * playerData.moveSpeed;
        
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
        if (Time.timeScale == 0f) return;
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 lookPos = new Vector3(hit.point.x, transform.position.y, hit.point.z);
            transform.LookAt(lookPos);
        }
    }
    
    private void UpdateAnimations()
    {
        Vector3 localMove = transform.InverseTransformDirection(new Vector3(moveInput.x, 0, moveInput.y));

        animator.SetFloat("VelocityX", localMove.x);
        animator.SetFloat("VelocityY", localMove.z);
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

    public void IncreaseHealth(float amount)
    {
        myaudioSource.PlayOneShot(pickUp);
        PlayAllParticles();
        currentHealth = Mathf.Min(currentHealth + amount, MAXHealth);
        Debug.Log($"������� HP: {currentHealth}");
        healthUI.UpdateHealth(currentHealth, MAXHealth);
    }

    public void PlayAllParticles()
    {
        foreach (ParticleSystem ps in myparticleSystem)
        {
            if (ps != null) 
            {
                ps.Play();
            }
        }
    }

    public void TakeDamage(float damage, TrapType trapType)
    {
        currentHealth -= damage;
        onTakeDamage?.Invoke(currentHealth);

        if (currentHealth > 0)
        {
            SavePlayerData();
            healthUI.UpdateHealth(currentHealth, MAXHealth);
        }

        if (currentHealth <= 0)
        {
            DeletePlayerData();
            Die();
        }
    }

    public void Die() 
    {
        deadUI.Activate();
    }

    private bool isFiring; 

    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            isFiring = true; // �������� ��������
        }
        else if (context.canceled)
        {
            isFiring = false; // ������������� ��������
        }
    }

    private void HandleShooting()
    {
        if (Mouse.current.leftButton.isPressed && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    private void Shoot()
    {
        if (bulletPrefab != null && firePoint != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit))
            {

                Vector3 direction = new Vector3(hit.point.x - firePoint.position.x, 0, hit.point.z - firePoint.position.z).normalized;
                GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.LookRotation(direction));

                Bullet bulletScript = bullet.GetComponent<Bullet>();
                if (bulletScript != null)
                {
                    bulletScript.Initialize(direction, bulletSpeed);
                    bulletScript.SetDamage(GetDamage());
                    myaudioSource.PlayOneShot(gunShot);
                }
            }
        }
    }


    private void OnDrawGizmos()
    {
        if (firePoint == null || Camera.main == null) return;

        Gizmos.color = Color.red;

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        
    }
}
