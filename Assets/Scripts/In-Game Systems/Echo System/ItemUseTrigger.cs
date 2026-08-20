using UnityEngine;
using UnityEngine.Events;

public class ItemUseTrigger : MonoBehaviour
{
    [SerializeField] private string requiredItemName;
    [SerializeField] private GameObject promptUI;
    [SerializeField] private UnityEvent onUse;

    private bool playerInRange;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = false;
        if (promptUI != null) promptUI.SetActive(false);
    }

    private void Update()
    {
        if (!playerInRange) return;

        bool hasItem = HasRequiredItem();
        if (promptUI != null) promptUI.SetActive(hasItem);

        if (hasItem && Input.GetKeyDown(KeyCode.F))
        {
            if (promptUI != null) promptUI.SetActive(false);
            onUse.Invoke();
        }
    }

    private bool HasRequiredItem()
    {
        foreach (var entry in ItemBackpackManager.Instance.Items)
        {
            if (entry.itemName == requiredItemName) return true;
        }
        return false;
    }
}
