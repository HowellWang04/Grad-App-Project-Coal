using TMPro;
using UnityEngine;

public class GrandpaDiaryTextAreaBinding : MonoBehaviour
{
    [SerializeField] private string markerId;
    [SerializeField] private TMP_Text targetText;

    public string MarkerId => markerId;

    private bool initialized;

    private void Awake()
    {
        EnsureInitialized();
    }

    public bool Matches(string id)
    {
        return !string.IsNullOrEmpty(markerId) && markerId == id;
    }

    public void SetContent(string content)
    {
        EnsureInitialized();

        if (targetText != null)
            targetText.text = content ?? "";
    }

    private void EnsureInitialized()
    {
        if (initialized)
            return;

        if (targetText == null)
            targetText = GetComponent<TMP_Text>();

        if (targetText == null)
            targetText = GetComponentInChildren<TMP_Text>(true);

        if (targetText == null)
            Debug.LogWarning($"[GrandpaDiary] Text area '{name}' has no TMP_Text target.");

        initialized = true;
    }
}
