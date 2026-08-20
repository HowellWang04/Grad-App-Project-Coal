using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class GrandpaDiaryStickyNoteSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private string slotId;
    [SerializeField] private GameObject noteRoot;
    [SerializeField] private TMP_Text noteText;
    [SerializeField, Range(0f, 1f)] private float normalAlpha = 1f;
    [SerializeField, Range(0f, 1f)] private float hoverAlpha = 0.35f;

    public string SlotId => slotId;

    private bool initialized;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        EnsureInitialized();
        Hide();
    }

    public bool Matches(string id)
    {
        return !string.IsNullOrEmpty(slotId) && slotId == id;
    }

    public void Show(string content)
    {
        EnsureInitialized();

        if (noteRoot != null) noteRoot.SetActive(true);
        if (noteText != null) noteText.text = content ?? "";
        SetAlpha(normalAlpha);
    }

    public void Hide()
    {
        EnsureInitialized();

        if (noteText != null) noteText.text = "";
        SetAlpha(normalAlpha);
        if (noteRoot != null) noteRoot.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SetAlpha(hoverAlpha);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetAlpha(normalAlpha);
    }

    private void EnsureInitialized()
    {
        if (initialized)
            return;

        if (noteRoot == null)
            noteRoot = gameObject;

        if (noteText == null)
            noteText = GetComponent<TMP_Text>();

        if (noteText == null)
            noteText = GetComponentInChildren<TMP_Text>(true);

        canvasGroup = noteRoot.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = noteRoot.AddComponent<CanvasGroup>();

        if (noteText == null)
            Debug.LogWarning($"[GrandpaDiary] Sticky note slot '{name}' has no TMP_Text target.");

        initialized = true;
    }

    private void SetAlpha(float alpha)
    {
        EnsureInitialized();

        if (canvasGroup != null)
            canvasGroup.alpha = alpha;
    }
}
