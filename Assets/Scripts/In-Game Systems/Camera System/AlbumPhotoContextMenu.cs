using UnityEngine;
using UnityEngine.UI;

public class AlbumPhotoContextMenu : MonoBehaviour
{
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private Button addToScrapBookButton;

    private string currentFileName;

    private void Awake()
    {
        Hide();
        if (addToScrapBookButton != null)
            addToScrapBookButton.onClick.AddListener(OnAddToScrapBook);
    }

    public void Show(string fileName, Vector2 screenPosition)
    {
        currentFileName = fileName;

        if (menuPanel != null)
        {
            menuPanel.SetActive(true);
            menuPanel.GetComponent<RectTransform>().position = screenPosition;
        }
    }

    public void Hide()
    {
        if (menuPanel != null) menuPanel.SetActive(false);
        currentFileName = null;
    }

    private void OnAddToScrapBook()
    {
        if (string.IsNullOrEmpty(currentFileName)) return;

        ScrapBookDataManager.Instance.RegisterPhoto(currentFileName);
        Hide();
    }

    private void Update()
    {
        // Click anywhere else to close the menu
        if (menuPanel != null && menuPanel.activeSelf)
        {
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
            {
                // Check if click is outside the menu
                RectTransform rt = menuPanel.GetComponent<RectTransform>();
                if (!RectTransformUtility.RectangleContainsScreenPoint(rt, Input.mousePosition))
                    Hide();
            }
        }
    }
}
