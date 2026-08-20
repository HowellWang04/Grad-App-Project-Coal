using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Controller.FinalCharacterController;

public sealed class PlayerPanelUIManager : MonoBehaviour
{
    [Header("Root UI Ref")]
    [SerializeField] private GameObject playerPanelUI;
    
    [Header("Manager Refs")]
    [SerializeField] private PlayerMovementManager playerMovementManager;
    [SerializeField] private CameraPhotoAlbumUIManager cameraPhotoAlbumUIManager;

    [Header("Tabs")]
    [SerializeField] private GameObject Albumtab;
    [SerializeField] private GameObject ScrapBooktab;
    [FormerlySerializedAs("GrandpaNotesTab")]
    [SerializeField] private GameObject GrandpaDiaryTab;
    [SerializeField] private GameObject BackpackTab;

    [Header("Grandpa's Diary")]
    [FormerlySerializedAs("grandpaNotesText")]
    [SerializeField] private TMP_Text grandpaDiaryText;
    [FormerlySerializedAs("grandpaDiaryMarkdown")]
    [SerializeField] private TextAsset grandpaDiaryJson;
    [SerializeField] private GrandpaDiaryPageData[] grandpaDiaryPages;
    [SerializeField] private GrandpaDiaryPageView[] grandpaDiaryPageViews;
    [FormerlySerializedAs("grandpaNotesPrevPageButton")]
    [SerializeField] private Button grandpaDiaryPrevPageButton;
    [FormerlySerializedAs("grandpaNotesNextPageButton")]
    [SerializeField] private Button grandpaDiaryNextPageButton;
    [FormerlySerializedAs("grandpaNotesPageText")]
    [SerializeField] private TMP_Text grandpaDiaryPageText;

    [Header("Buttons")]
    [SerializeField] private Button albumButton;
    [SerializeField] private Button scrapBookButton;
    [FormerlySerializedAs("grandpaNotesButton")]
    [SerializeField] private Button grandpaDiaryButton;
    [SerializeField] private Button backpackButton;
    

    public bool IsOpen { get; private set; }

    private CursorLockMode prevLockState;
    private bool prevCursorVisible;
    private bool cachedCursor;
    private int currentGrandpaDiaryPageIndex;

    private void Awake()
    {
        LoadGrandpaDiaryJson();
        IsOpen = false;
        ApplyOpenState();
    }

    private void Start()
    {
        InitBtnListeners();
        ShowAlbum();
    }


    public void Open()  => SetOpen(true);
    public void Close() => SetOpen(false);
    public void Toggle() => SetOpen(!IsOpen);

    public void SetOpen(bool open)
    {
        if (open == IsOpen) return;
        IsOpen = open;
        ApplyOpenState();
    }

    private void InitBtnListeners()
    {
        if (albumButton != null) albumButton.onClick.AddListener(ShowAlbum);
        if (scrapBookButton != null) scrapBookButton.onClick.AddListener(ShowScrapBook);
        if (grandpaDiaryButton != null) grandpaDiaryButton.onClick.AddListener(ShowGrandpaDiary);
        if (backpackButton != null) backpackButton.onClick.AddListener(ShowBackpack);
        if (grandpaDiaryPrevPageButton != null) grandpaDiaryPrevPageButton.onClick.AddListener(ShowPreviousGrandpaDiaryPage);
        if (grandpaDiaryNextPageButton != null) grandpaDiaryNextPageButton.onClick.AddListener(ShowNextGrandpaDiaryPage);
    }


    private void ApplyOpenState()
    {
        bool open = IsOpen;

        if (playerPanelUI != null) playerPanelUI.SetActive(open);
        if (playerMovementManager != null) playerMovementManager.SetPlayerMovementControl(!open);
        if (!open && cameraPhotoAlbumUIManager != null) cameraPhotoAlbumUIManager.OnPanelClosed();

        if (open && !cachedCursor)
        {
            prevLockState = Cursor.lockState;
            prevCursorVisible = Cursor.visible;
            cachedCursor = true;
        }

        if (open)
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
        else
        {
            if (cachedCursor)
            {
                Cursor.lockState = prevLockState;
                Cursor.visible = prevCursorVisible;
                cachedCursor = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }

    public void ShowAlbum()
    {
        HideAllTabs();
        if (Albumtab != null) Albumtab.SetActive(true);
    }

    public void ShowScrapBook()
    {
        HideAllTabs();
        if (ScrapBooktab != null) ScrapBooktab.SetActive(true);
    }

    public void ShowGrandpaDiary()
    {
        HideAllTabs();
        if (GrandpaDiaryTab != null) GrandpaDiaryTab.SetActive(true);
        ShowGrandpaDiaryPage(currentGrandpaDiaryPageIndex);
    }

    public void ShowGrandpaDiary(string content)
    {
        if (grandpaDiaryText != null) grandpaDiaryText.text = content;
        ShowGrandpaDiary();
    }

    public void ShowGrandpaDiaryPage(int pageIndex)
    {
        int pageCount = GetGrandpaDiaryPageCount();
        if (pageCount <= 0)
        {
            if (grandpaDiaryText != null) grandpaDiaryText.text = "";
            if (grandpaDiaryPageText != null) grandpaDiaryPageText.text = "0 / 0";
            SetGrandpaDiaryPageButtons(false, false);
            HideAllGrandpaDiaryPageViews();
            return;
        }

        currentGrandpaDiaryPageIndex = Mathf.Clamp(pageIndex, 0, pageCount - 1);
        var pageData = grandpaDiaryPages[currentGrandpaDiaryPageIndex];

        if (!ShowGrandpaDiaryPageView(pageData, currentGrandpaDiaryPageIndex))
            ShowGrandpaDiaryFallbackText(pageData);

        if (grandpaDiaryPageText != null)
            grandpaDiaryPageText.text = $"{currentGrandpaDiaryPageIndex + 1} / {pageCount}";

        SetGrandpaDiaryPageButtons(currentGrandpaDiaryPageIndex > 0, currentGrandpaDiaryPageIndex < pageCount - 1);
    }

    private void ShowPreviousGrandpaDiaryPage()
    {
        ShowGrandpaDiaryPage(currentGrandpaDiaryPageIndex - 1);
    }

    private void ShowNextGrandpaDiaryPage()
    {
        ShowGrandpaDiaryPage(currentGrandpaDiaryPageIndex + 1);
    }

    public void ShowBackpack()
    {
        HideAllTabs();
        if (BackpackTab != null) BackpackTab.SetActive(true);
    }

    private void HideAllTabs()
    {
        if (Albumtab != null) Albumtab.SetActive(false);
        if (ScrapBooktab != null) ScrapBooktab.SetActive(false);
        if (GrandpaDiaryTab != null) GrandpaDiaryTab.SetActive(false);
        if (BackpackTab != null) BackpackTab.SetActive(false);
    }

    private int GetGrandpaDiaryPageCount()
    {
        return grandpaDiaryPages != null ? grandpaDiaryPages.Length : 0;
    }

    private void LoadGrandpaDiaryJson()
    {
        if (grandpaDiaryJson == null)
        {
            Debug.LogWarning("[GrandpaDiary] Grandpa Diary Json is not assigned.");
            return;
        }

        var parsedPages = GrandpaDiaryJsonParser.Parse(grandpaDiaryJson.text);
        if (parsedPages.Length > 0)
        {
            grandpaDiaryPages = parsedPages;
            Debug.Log($"[GrandpaDiary] Loaded {grandpaDiaryPages.Length} diary pages from JSON.");
        }
        else
        {
            Debug.LogWarning("[GrandpaDiary] Diary JSON parsed 0 pages.");
        }
    }

    private bool ShowGrandpaDiaryPageView(GrandpaDiaryPageData pageData, int pageIndex)
    {
        HideAllGrandpaDiaryPageViews();

        var pageView = FindGrandpaDiaryPageView(pageData, pageIndex);
        if (pageView == null)
        {
            Debug.LogWarning($"[GrandpaDiary] Cannot find page view for page '{pageData?.pageId}' at index {pageIndex}.");
            return false;
        }

        pageView.Show(pageData);
        return true;
    }

    private GrandpaDiaryPageView FindGrandpaDiaryPageView(GrandpaDiaryPageData pageData, int pageIndex)
    {
        if (grandpaDiaryPageViews == null || grandpaDiaryPageViews.Length == 0)
            return null;

        if (pageData != null && !string.IsNullOrEmpty(pageData.pageId))
        {
            foreach (var pageView in grandpaDiaryPageViews)
                if (pageView != null && pageView.PageId == pageData.pageId)
                    return pageView;
        }

        if (pageIndex >= 0 && pageIndex < grandpaDiaryPageViews.Length)
            return grandpaDiaryPageViews[pageIndex];

        return null;
    }

    private void HideAllGrandpaDiaryPageViews()
    {
        if (grandpaDiaryPageViews == null) return;

        foreach (var pageView in grandpaDiaryPageViews)
            if (pageView != null)
                pageView.Hide();
    }

    private void ShowGrandpaDiaryFallbackText(GrandpaDiaryPageData pageData)
    {
        if (grandpaDiaryText == null) return;

        if (pageData == null || pageData.textBlocks == null || pageData.textBlocks.Count == 0)
        {
            grandpaDiaryText.text = "";
            return;
        }

        var content = "";
        foreach (var block in pageData.textBlocks)
        {
            if (block == null || string.IsNullOrEmpty(block.content)) continue;
            if (!string.IsNullOrEmpty(content)) content += "\n\n";
            content += block.content;
        }

        grandpaDiaryText.text = content;
    }

    private void SetGrandpaDiaryPageButtons(bool canGoPrevious, bool canGoNext)
    {
        if (grandpaDiaryPrevPageButton != null) grandpaDiaryPrevPageButton.interactable = canGoPrevious;
        if (grandpaDiaryNextPageButton != null) grandpaDiaryNextPageButton.interactable = canGoNext;
    }
}
