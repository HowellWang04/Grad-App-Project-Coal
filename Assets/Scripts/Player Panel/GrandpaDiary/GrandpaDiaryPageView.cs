using UnityEngine;

public class GrandpaDiaryPageView : MonoBehaviour
{
    [SerializeField] private string pageId;
    [SerializeField] private GameObject pageRoot;
    [SerializeField] private GrandpaDiaryTextAreaBinding[] textAreas;
    [SerializeField] private GrandpaDiaryStickyNoteSlot[] stickyNoteSlots;

    public string PageId => pageId;

    private bool initialized;

    private void Awake()
    {
        EnsureInitialized();
    }

    public void Show(GrandpaDiaryPageData pageData)
    {
        EnsureInitialized();

        if (pageRoot != null) pageRoot.SetActive(true);

        ClearTextAreas();
        HideAllStickyNoteSlots();

        if (pageData == null) return;

        ShowTextBlocks(pageData);
        ShowUnlockedStickyNotes(pageData);
    }

    public void Hide()
    {
        EnsureInitialized();

        if (pageRoot != null) pageRoot.SetActive(false);
    }

    private void EnsureInitialized()
    {
        if (initialized)
            return;

        if (pageRoot == null)
            pageRoot = gameObject;

        if (textAreas == null || textAreas.Length == 0)
            textAreas = GetComponentsInChildren<GrandpaDiaryTextAreaBinding>(true);

        if (stickyNoteSlots == null || stickyNoteSlots.Length == 0)
            stickyNoteSlots = GetComponentsInChildren<GrandpaDiaryStickyNoteSlot>(true);

        if (textAreas == null || textAreas.Length == 0)
            Debug.LogWarning($"[GrandpaDiary] Page view '{name}' has no text area bindings.");

        initialized = true;
    }

    private void ShowTextBlocks(GrandpaDiaryPageData pageData)
    {
        if (pageData.textBlocks == null) return;

        foreach (var block in pageData.textBlocks)
        {
            if (block == null || string.IsNullOrEmpty(block.markerId)) continue;

            var textArea = FindTextArea(block.markerId);
            if (textArea != null)
                textArea.SetContent(block.content);
            else
                Debug.LogWarning($"[GrandpaDiary] Page '{pageData.pageId}' cannot find text area marker '{block.markerId}'.");
        }
    }

    private void ShowUnlockedStickyNotes(GrandpaDiaryPageData pageData)
    {
        if (pageData.stickyNoteSlots == null) return;

        foreach (var slotData in pageData.stickyNoteSlots)
        {
            if (slotData == null || string.IsNullOrEmpty(slotData.slotId)) continue;
            if (!slotData.unlockedByDefault && !GrandpaDiaryStickyNoteStorage.IsUnlocked(slotData.slotId)) continue;

            var stickyNoteSlot = FindStickyNoteSlot(slotData.slotId);
            if (stickyNoteSlot != null)
                stickyNoteSlot.Show(slotData.noteContent);
            else
                Debug.LogWarning($"[GrandpaDiary] Page '{pageData.pageId}' cannot find sticky note slot '{slotData.slotId}'.");
        }
    }

    private GrandpaDiaryTextAreaBinding FindTextArea(string markerId)
    {
        if (textAreas == null) return null;

        foreach (var textArea in textAreas)
            if (textArea != null && textArea.Matches(markerId))
                return textArea;

        return null;
    }

    private GrandpaDiaryStickyNoteSlot FindStickyNoteSlot(string slotId)
    {
        if (stickyNoteSlots == null) return null;

        foreach (var stickyNoteSlot in stickyNoteSlots)
            if (stickyNoteSlot != null && stickyNoteSlot.Matches(slotId))
                return stickyNoteSlot;

        return null;
    }

    private void ClearTextAreas()
    {
        if (textAreas == null) return;

        foreach (var textArea in textAreas)
            if (textArea != null)
                textArea.SetContent("");
    }

    private void HideAllStickyNoteSlots()
    {
        if (stickyNoteSlots == null) return;

        foreach (var stickyNoteSlot in stickyNoteSlots)
            if (stickyNoteSlot != null)
                stickyNoteSlot.Hide();
    }
}
