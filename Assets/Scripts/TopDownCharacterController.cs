using DG.Tweening;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class TopDownCharacterController : MonoBehaviour, IDamageable
{
    [Header("PLayerStat")]
    public PlayerData playerData;
    public Transform player;
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

    [Header("FireStat")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    private float fireRate = 1f;
    private float nextFireTime = 0f;
    public float bulletSpeed = 10f;
    public int currentmRicochets;
    private DamageType currentDamageTypes = DamageType.Normal;

    [Header("Stats")]
    private float currentDamage;
    private float currentFireRate;
    public Light mylight;

    [Header("Audio")]
    [SerializeField] private AudioSource myaudioSource;
    [SerializeField] private AudioClip gunShot;
    [SerializeField] private AudioClip pickUp;
    [SerializeField] private AudioClip dash;

    [Header("Coin")]
    public static Action<int> OnCoinsChanged;
    private int coinCount = 1;
    public int currentCoins;

    [Header("Shield")]
    [SerializeField] private float currentShieldValue;
    [SerializeField] private float MAXShieldValue = 50;
    private bool hasShieldAttribute = false;
    private bool canActivateShield = true;
    [SerializeField] private float shieldCooldownTime = 15f;
    private float shieldCooldownTimer = 0f;
    [SerializeField] private GameObject ShieldObject;

    [Header("Animation Tracking")]
    [SerializeField] private string attackAnimationName = "Death";
    [SerializeField][Range(0, 1)] private float deathpplicationPoint = 0.8f;
    private AnimatorStateInfo currentStateInfo;
    private bool isDeathAnimation = false;
    private bool isDead = false;

    [Header("Input")]
    public InputActionAsset inputActions;

    [Header("Dashing")]
    public float dashForce = 10f;
    public float dashUpwardForce = 0f;
    public float dashDuration = 0.25f;
    public float dashCooldown = 2.0f;
    public float dashFov = 60;
    public bool allowAllDirections = true;
    public bool resetVelocityBeforeDash = true;

    private bool isDashing = false;
    private float dashTimer = 0f;
    private float dashCooldownTimer = 0f;
    private Vector3 dashDirection;
    private float originalFov;
    private Camera mainCam;
    private int defaultLayer;
    [SerializeField] private string dashLayerName = "PlayerDuringDash";
    private int dashLayer;

    private bool isLevelCompleted = false;

    public float GetDamage() => currentDamage;

    private void LoadPlayerData()
    {
        /*if (!SaveManager.Instance.SaveExists())
        {
            Debug.LogWarning("Файл сохранения отсутствует. Устанавливаем значения по умолчанию.");
            currentHealth = playerData.health;
            MAXHealth = playerData.health;
            currentDamage = playerData.damage;
            currentFireRate = fireRate;
            currentmRicochets = 
            return;
        }*/

        GameData data = SaveManager.Instance.Load();
        

        currentHealth = data.health;
        MAXHealth = data.maxHealth;
        currentFireRate = data.fireRate;
        currentDamage = data.damage;
        currentmRicochets = data.ricochets;
        currentCoins = data.coins;
        MAXShieldValue = data.shieldValue;
        hasShieldAttribute = data.Shield;
        currentDamageTypes = (DamageType)data.damageType;
        OnCoinsChanged?.Invoke(currentCoins);
        healthUI.UpdateHealth(currentHealth, MAXHealth);
        Debug.LogError(currentDamageTypes);
    }



    public void SavePlayerData()
    {
        GameData data = SaveManager.Instance.Load();
        data.health = currentHealth;
        data.maxHealth = MAXHealth;
        data.fireRate = currentFireRate;
        data.damage = currentDamage;
        data.coins = currentCoins;
        data.ricochets = currentmRicochets;
        data.Shield = hasShieldAttribute;
        data.shieldValue = MAXShieldValue;
        data.damageType = (int)currentDamageTypes;
        SaveManager.Instance.Save(data);
        Debug.LogError("The data o player was saved");
    }


    public void DeletePlayerData()
    {
        SaveManager.Instance.DeleteSave();
    }

    public void OnLevelCompleted()
    {
        isLevelCompleted = true;
    }

    private void OnEnable()
    {
        inputActions.FindActionMap("Player").Enable();
        inputActions.FindActionMap("Player").FindAction("Dash").performed += OnDash;
        EndLabirint.OnLevelCompleted += OnLevelCompleted;
    }

    private void OnDisable()
    {
        inputActions.FindActionMap("Player").FindAction("Dash").performed -= OnDash;
        inputActions.FindActionMap("Player").Disable();
        EndLabirint.OnLevelCompleted -= OnLevelCompleted;

        if (!isDead && isLevelCompleted)
        {
            SavePlayerData();
        }
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
        mainCam = Camera.main;
        defaultLayer = gameObject.layer;
        dashLayer = LayerMask.NameToLayer(dashLayerName);
        if (mainCam != null)
            originalFov = mainCam.fieldOfView;
    }

    private void Start()
    {
        if (SaveManager.Instance != null)
        {
            LoadPlayerData();
        }
        Debug.LogError("Current Fire Rate " + currentFireRate);
        Debug.LogError("Current Damage Type " + currentDamageTypes);
    }

    private float fireCooldownTimer = 0f;

    void Update()
    {
        float dt = Time.deltaTime * PauseGameState.LocalTimeScale;
        UpdateDash(Time.deltaTime * PauseGameState.LocalTimeScale);


        if (PauseGameState.IsPaused)
        {
            inputActions.FindActionMap("Player").Disable();
        }
        else
        {
            inputActions.FindActionMap("Player").Enable();
        }

        RotateTowardsMouse();
        UpdateAnimations();
        MoveCamera();
        Move();
        HandleShooting();
        OnShieldActivate();

        if (!PauseGameState.IsPaused)
        {
            fireCooldownTimer += dt;

            if (isFiring && fireCooldownTimer >= 1f / currentFireRate)
            {
                Shoot();
                fireCooldownTimer = 0f;
            }
        }

        if (!canActivateShield)
        {
            shieldCooldownTimer += dt;
            if (shieldCooldownTimer >= shieldCooldownTime)
            {
                canActivateShield = true;
                Debug.Log("Щит готов к повторной активации");
            }
        }
/*
        if (isDying)
        {
            currentStateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (currentStateInfo.IsName("Death"))
            {
                float normalizedTime = currentStateInfo.normalizedTime % 1;
                if (normalizedTime >= deathpplicationPoint)
                {
                    
                }
            }
        }*/


    }

    void OnShieldActivate() 
    {
        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            if (canActivateShield)
            {
                ActivateShield();
            }
            else
            {
                Debug.Log("Щит недоступен. Получите соответствующий атрибут.");
            }
        }
    }


    public void ApplyUpgrade(Upgrade upgrade)
    {
        GameData data = SaveManager.Instance.Load();

        if (currentCoins < upgrade.cost)
        {
            Debug.LogWarning($"Недостаточно монет. Требуется: {upgrade.cost}, доступно: {currentCoins}");
            return;
        }

        currentCoins -= upgrade.cost;
        OnCoinsChanged?.Invoke(currentCoins);
        data.coins = currentCoins;

        switch (upgrade.type)
        {
            case UpgradeType.HealthBoost:
                MAXHealth *= upgrade.value;
                currentHealth = MAXHealth;
                break;
            case UpgradeType.DamageIncrease:
                currentDamage *= upgrade.value;
                break;
            case UpgradeType.HpPlus:
                currentHealth = Mathf.Min(currentHealth + upgrade.value, MAXHealth);
                break;
            case UpgradeType.FireRate:
                currentFireRate *= (1f - upgrade.value);
                Debug.LogError("Current Fire Rate " + currentFireRate);
                break;
            case UpgradeType.Ricoshet:
                currentmRicochets += Mathf.RoundToInt(upgrade.value);
                break;
        }

        data.maxHealth = MAXHealth;
        data.health = currentHealth;
        data.damage = currentDamage;
        data.fireRate = currentFireRate;
        data.ricochets = currentmRicochets;
        data.appliedUpgrades.Add(upgrade.name);
        SaveManager.Instance.Save(data);
        healthUI.UpdateHealth(currentHealth, MAXHealth);
    }

    public void ApplyAtributes(Atribute atribute) 
    {
        GameData data = SaveManager.Instance.Load();
        if(currentCoins < atribute.cost) 
        {
            return;
        }
        currentCoins -= atribute.cost;
        OnCoinsChanged(currentCoins);
        data.coins = currentCoins;

        switch (atribute.type) 
        {
            case AtributeType.IceBullet:
                AddDamageType(DamageType.Ice);
                data.damageType = (int)currentDamageTypes;
                break;
            case AtributeType.FireBullet:
                AddDamageType(DamageType.Fire);
                data.damageType = (int)currentDamageTypes;
                break;
            case AtributeType.PoisonBullets:
                AddDamageType(DamageType.Poison);
                data.damageType = (int)currentDamageTypes;
                break;
            case AtributeType.Shield:
                hasShieldAttribute = true;
                data.Shield = true;
                break;

        }
        data.maxHealth = MAXHealth;
        data.health = currentHealth;
        data.damage = currentDamage;
        data.fireRate = currentFireRate;
        data.ricochets = currentmRicochets;
        data.appliedAttributes.Add(atribute.name);
        data.damageType = (int)currentDamageTypes;
        SaveManager.Instance.Save(data);
        healthUI.UpdateHealth(currentHealth, MAXHealth);
        Debug.Log($"Атрибут {atribute.name} добавлен в состояние игрока.");
    }

    public void RemoveAtribute(Atribute atribute)
    {
        if (atribute == null)
        {
            Debug.LogWarning("Попытка удалить null-атрибут.");
            return;
        }

        GameData data = SaveManager.Instance.Load();

        if (data.appliedAttributes.Contains(atribute.name))
        {
            data.appliedAttributes.Remove(atribute.name);
        }
        switch (atribute.type)
        {
            case AtributeType.IceBullet:
                currentDamageTypes &= ~DamageType.Ice;
                break;
            case AtributeType.FireBullet:
                currentDamageTypes &= ~DamageType.Fire;
                break;
            case AtributeType.PoisonBullets:
                currentDamageTypes &= ~DamageType.Poison;
                break;
            case AtributeType.Shield:
                hasShieldAttribute = false;
                canActivateShield = false;
                data.Shield = false;
                break;
        }

        data.damageType = (int)currentDamageTypes;
        SaveManager.Instance.Save(data);

        Debug.Log($"Атрибут {atribute.name} удалён из состояния игрока и сохранения.");
    }

    private void ActivateShield()
    {
        if (!hasShieldAttribute)
        {
            Debug.Log("Щит недоступен. Получите атрибут 'Shield' для активации.");
            return;
        }

        if (!canActivateShield)
        {
            Debug.Log("Щит находится на перезарядке.");
            return;
        }

        currentShieldValue = MAXShieldValue;
        hasShieldAttribute = true;
        canActivateShield = false;
        shieldCooldownTimer = 0f;
        healthUI.UpdateShield(currentShieldValue, MAXShieldValue);
        Debug.Log("Щит активирован");
        ShieldObject.SetActive(true);
    }





    public void AddDamageType(DamageType damageType) 
    {
        currentDamageTypes |= damageType;
    }

    public void RemoveDamageType(DamageType damageType)
    {
        currentDamageTypes &= ~damageType;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.started && dashCooldownTimer <= 0f && !isDashing)
        {
            StartDash();
        }
    }

    private void StartDash()
    {
        myaudioSource.PlayOneShot(dash);
        isDashing = true;
        dashTimer = dashDuration;
        dashCooldownTimer = dashCooldown;
        healthUI.UpdateDash(dashCooldown);
        Vector3 inputDirection = new Vector3(moveInput.x, 0, moveInput.y);
        dashDirection = allowAllDirections && inputDirection != Vector3.zero
            ? inputDirection.normalized
            : transform.forward;

        if (resetVelocityBeforeDash)
            velocity = Vector3.zero;

        if (mainCam != null)
        {
            DOTween.Kill(mainCam);
            mainCam.DOFieldOfView(dashFov, 0.15f).SetEase(Ease.OutQuad);
        }

        gameObject.layer = dashLayer;
    }



    private void UpdateDash(float dt)
    {
        if (dashCooldownTimer > 0f)
            dashCooldownTimer -= dt;

        if (!isDashing)
            return;

        characterController.Move(dashDirection * dashForce * dt);

        dashTimer -= dt;
        if (dashTimer <= 0f)
        {
            EndDash();
        }
    }

    private void EndDash()
    {
        isDashing = false;
        gameObject.layer = defaultLayer;

        if (mainCam != null)
        {
            DOTween.Kill(mainCam);
            mainCam.DOFieldOfView(originalFov, 0.25f).SetEase(Ease.OutQuad);
        }
    }




    private void Move()
    {
        float DT = Time.deltaTime * PauseGameState.LocalTimeScale;
        Vector3 moveVector = new Vector3(moveInput.x, 0, moveInput.y) * playerData.moveSpeed;
        
        if (!characterController.isGrounded)
        {
            velocity.y -= gravity * DT;
        }
        else
        {
            velocity.y = -0.1f; 
        }
        
        characterController.Move((moveVector + velocity) * DT);
    }
    
    private void RotateTowardsMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Time.timeScale == 0f) return;
        if (isDead == true) return;
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
        float DT = Time.deltaTime * PauseGameState.LocalTimeScale;
        if (cameraTransform != null)
        {
            Vector3 targetPosition = new Vector3(transform.position.x, cameraTransform.position.y, transform.position.z);
            cameraTransform.position = Vector3.Lerp(cameraTransform.position, targetPosition, cameraSmoothSpeed * DT);
        }
    }

    public void IncreaseHealth(float amount)
    {
        myaudioSource.PlayOneShot(pickUp);
        currentHealth = Mathf.Min(currentHealth + amount, MAXHealth);
        Debug.Log($"Текущее HP: {currentHealth}");
        healthUI.UpdateHealth(currentHealth, MAXHealth);
    }


    public void TakeDamage(float damage, TrapType trapType, DamageType damageType)
    {
        if (hasShieldAttribute && currentShieldValue > 0)
        {
            float shieldAbsorbed = Mathf.Min(currentShieldValue, damage);
            currentShieldValue -= shieldAbsorbed;
            damage -= shieldAbsorbed;
            healthUI.UpdateShield(currentShieldValue, MAXShieldValue);
            Debug.Log("Щит атакуют.");
            if (currentShieldValue <= 0)
            {
                canActivateShield = false;
                Debug.Log("Щит разрушен.");
                ShieldObject.SetActive(false);
            }
        }

        if (damage > 0f)
        {
            currentHealth -= damage;
            onTakeDamage?.Invoke(currentHealth);

            if (currentHealth > 0)
            {
                //SavePlayerData();
                healthUI.UpdateHealth(currentHealth, MAXHealth);
            }

            if (currentHealth <= 0)
            {
                Die();
            }
        }
    }

    public async void Die()
    {
        if (isDead) return;

        isDead = true;
        animator.SetTrigger("Death");
        healthUI.gameObject.SetActive(false);
        characterController.enabled = false;

        Transform cameraTransform = Camera.main.transform;
        float duration = 3f;
        float targetY = transform.position.y + 2f;
        cameraTransform.DOMoveY(targetY, duration).SetEase(Ease.InOutSine);

        await Task.Delay(TimeSpan.FromSeconds(duration));

        deadUI.Activate();
        PauseGameState.Pause();
        DeletePlayerData();
        Debug.LogError("DIE");
    }



    private bool isFiring; 

    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            isFiring = true;
        }
        else if (context.canceled)
        {
            isFiring = false;
        }
    }

    private void HandleShooting()
    {
        if (PauseGameState.IsPaused)
            return;

        float dt = Time.deltaTime * PauseGameState.LocalTimeScale;

        fireCooldownTimer += dt;

        if (Mouse.current.leftButton.isPressed && fireCooldownTimer >= currentFireRate)
        {
            Shoot();
            fireCooldownTimer = 0f;
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
                    bulletScript.Initialize(direction, bulletSpeed, currentDamage, currentmRicochets, currentDamageTypes);
                    bulletScript.SetDamage(GetDamage());
                    myaudioSource.PlayOneShot(gunShot);
                }
            }
        }
    }

    private void AddCoin() 
    {
            myaudioSource.PlayOneShot(pickUp);
            currentCoins += coinCount;
            OnCoinsChanged?.Invoke(currentCoins);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Coin"))
        {
            AddCoin();
            Destroy(other.gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        if (firePoint == null || Camera.main == null) return;

        Gizmos.color = Color.red;

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        
    }
}
