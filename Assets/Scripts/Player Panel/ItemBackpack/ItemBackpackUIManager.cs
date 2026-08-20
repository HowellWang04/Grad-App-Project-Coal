using UnityEngine;
using UnityEngine.UI;

public class ItemBackpackUIManager : MonoBehaviour
{
    [Header("Slot Refs")]
    [SerializeField] private ItemBackpackSlot slotPrefab;
    [SerializeField] private Transform slotContainer;

    [Header("Inspect Refs")]
    [SerializeField] private InspectUIManager inspectUI;
    [SerializeField] private Camera playerCamera;

    [Header("Scroll Refs")]
    [SerializeField] private ScrollRect scrollRect;

    private void OnEnable()
    {
        if (ItemBackpackManager.Instance != null)
            ItemBackpackManager.Instance.OnBackpackChanged += RefreshUI;

        if (inspectUI != null)
            inspectUI.OnInspectStopped += EnableScroll;

        RefreshUI();
    }

    private void OnDisable()
    {
        if (ItemBackpackManager.Instance != null)
            ItemBackpackManager.Instance.OnBackpackChanged -= RefreshUI;

        if (inspectUI != null)
            inspectUI.OnInspectStopped -= EnableScroll;
    }

    public void DisableScroll()
    {
        if (scrollRect != null) scrollRect.enabled = false;
    }

    private void EnableScroll()
    {
        if (scrollRect != null) scrollRect.enabled = true;
    }

    private void RefreshUI()
    {
        if (slotContainer == null || slotPrefab == null) return;

        
        foreach (Transform child in slotContainer)
            Destroy(child.gameObject);

        
        if (ItemBackpackManager.Instance == null) return;

        foreach (var entry in ItemBackpackManager.Instance.Items)
        {
            var slot = Instantiate(slotPrefab, slotContainer);
            slot.Setup(entry, inspectUI, playerCamera, DisableScroll);
        }
    }
}
