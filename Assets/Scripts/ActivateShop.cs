using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class ActivateShop : MonoBehaviour
{
    [SerializeField] private OpenShowText interactionText;
    [SerializeField] private InputActionReference activateShopAction;
    private UpgradeUI shopUI;

    [SerializeField] private UnityEvent onShopActivate;

    private bool isPlayerInRange = false;

    private void Awake()
    {
        interactionText = FindObjectOfType<OpenShowText>(true);
        shopUI = FindObjectOfType<UpgradeUI>(true);
    }

    private void OnEnable()
    {
        activateShopAction.action.Enable();
        activateShopAction.action.performed += OnActivateShop;
    }

    private void OnDisable()
    {
        activateShopAction.action.performed -= OnActivateShop;
        activateShopAction.action.Disable();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            if (interactionText != null)
                interactionText.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            if (interactionText != null)
                interactionText.gameObject.SetActive(false);
        }
    }

    private void OnActivateShop(InputAction.CallbackContext context)
    {
        if (!isPlayerInRange) return;

        interactionText?.gameObject.SetActive(false);
        shopUI?.gameObject.SetActive(true);
        shopUI.StartShop();
        onShopActivate?.Invoke();
    }
}
