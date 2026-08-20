using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class CameraPhotoAlbumUIManager : MonoBehaviour
{
    [Header("UI Component Refs")]
    [SerializeField] private Transform gridParent;
    [SerializeField] private GameObject thumbPrefab;
    [SerializeField] private Button prevButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private TMP_Text pageText;
    [SerializeField] private CameraPhotoPreviewUIManager cameraPhotoPreviewUIManager;
    [SerializeField] private Toggle deleteModeToggle;
    [SerializeField] private Button deleteButton;
    [SerializeField] private Button deleteAllButton;

    [Header("Context Menu")]
    [SerializeField] private AlbumPhotoContextMenu contextMenu;

    [Header("Paging")]
    [SerializeField] private int pageSize = 32;

    [Header("Thumbnail Settings")]
    [SerializeField] private int thumbMaxSize = 256;

    [Header("Echo Indicator")]
    [SerializeField] private Color echoGlowColor = new Color(1f, 0.82f, 0.25f, 1f);
    [SerializeField] private float echoGlowThickness = 4f;


    private List<string> fileNames;
    private readonly List<GameObject> spawned = new();
    private int pageIndex = 0;
    private int currentPreviewIndex = -1;
    private readonly Dictionary<string, Texture2D> thumbCache = new();

    private bool isDeleteMode = false;
    private readonly HashSet<string> selectedForDeletion = new();

    private bool initialized = false;

    private void EnsureInitialized()
    {
        if (initialized) return;
        initialized = true;

        fileNames = CameraPhotoStorage.LoadManifest();

        if (prevButton != null) prevButton.onClick.AddListener(PrevPage);
        if (nextButton != null) nextButton.onClick.AddListener(NextPage);

        if (deleteModeToggle != null)
        {
            deleteModeToggle.isOn = false;
            deleteModeToggle.onValueChanged.AddListener(OnDeleteModeChanged);
        }
        if (deleteButton != null)
        {
            deleteButton.gameObject.SetActive(false);
            deleteButton.onClick.AddListener(OnDeleteConfirmed);
        }
        if (deleteAllButton != null)
{
        deleteAllButton.gameObject.SetActive(false);
        deleteAllButton.onClick.AddListener(OnDeleteAllConfirmed);
}
    }

    private void Awake()
    {
        EnsureInitialized();
        Refresh();
    }

    private void Start()
    {
        StartCoroutine(PrewarmThumbnails());
    }

    public IEnumerator PrewarmThumbnails()
    {
        EnsureInitialized();
        foreach (var fileName in fileNames)
        {
            if (!thumbCache.ContainsKey(fileName))
                GetOrCreateThumbnail(fileName);
            yield return null;
        }
    }

    private void OnEnable()
    {
        EnsureInitialized();
        Refresh();
    }

    public void OnPanelClosed()
    {
        if (currentPreviewIndex != -1)
        {
            if (cameraPhotoPreviewUIManager != null) cameraPhotoPreviewUIManager.ClosePreview();
            currentPreviewIndex = -1;
        }
    }

    public string AddPhotoFromTexture(Texture2D fullResCopy)
    {
        EnsureInitialized();
        string fileName = CameraPhotoStorage.SavePhotoPng(fullResCopy);

        fileNames.Insert(0, fileName);
        CameraPhotoStorage.SaveManifest(fileNames);

        pageIndex = 0;
        Refresh();

        return fileName;
    }

    private int TotalPages => Mathf.Max(1, Mathf.CeilToInt(fileNames.Count / (float)pageSize));

    private void PrevPage()
    {
        pageIndex = Mathf.Max(0, pageIndex - 1);
        Refresh();
    }

    private void NextPage()
    {
        pageIndex = Mathf.Min(TotalPages - 1, pageIndex + 1);
        Refresh();
    }

    public void Refresh()
    {
        foreach (var go in spawned) Destroy(go);
        spawned.Clear();

        int start = pageIndex * pageSize;
        int end = Mathf.Min(start + pageSize, fileNames.Count);

        for (int i = start; i < end; i++)
        {
            int capturedIndex = i;
            string fileName = fileNames[i];

            Texture2D thumbTex = GetOrCreateThumbnail(fileName);
            if (thumbTex == null) continue;

            GameObject go = Instantiate(thumbPrefab, gridParent);
            spawned.Add(go);

            ApplyEchoIndicator(go, fileName);

            var item = go.GetComponent<PhotoThumbItem>();
            if (item != null)
            {
                int index = capturedIndex;
                string file = fileName;

                if (isDeleteMode)
                {
                    item.Bind(thumbTex, fileName: file, contextMenu: contextMenu);
                    item.SetDeleteMode(true, selected =>
                    {
                        if (selected)
                            selectedForDeletion.Add(file);
                        else
                            selectedForDeletion.Remove(file);
                    });
                    item.SetSelected(selectedForDeletion.Contains(file));
                }
                else
                {
                    item.Bind(thumbTex, () => OpenPhotoFromThumbnail(index), file, contextMenu);
                    item.SetDeleteMode(false);
                }
            }
        }

        pageText.text = $"{pageIndex + 1} / {TotalPages}";
    }

    private void ApplyEchoIndicator(GameObject thumbGo, string fileName)
    {
        if (thumbGo == null) return;
        if (EchoPhotoStorage.Load(fileName).Count == 0) return;

        var shimmer = thumbGo.GetComponent<EchoSimpleShimmer>();
        if (shimmer == null)
            shimmer = thumbGo.AddComponent<EchoSimpleShimmer>();

        shimmer.SetStyle(echoGlowColor, echoGlowThickness);
    }

    private void OpenPhotoFromThumbnail(int index)
    {
        if (currentPreviewIndex == index)
        {
            if (cameraPhotoPreviewUIManager != null) cameraPhotoPreviewUIManager.ClosePreview();
            currentPreviewIndex = -1;
            return;
        }

        string fileName = fileNames[index];
        Texture2D tex = CameraPhotoStorage.LoadTexture(fileName);
        if (tex == null) return;

        if (cameraPhotoPreviewUIManager != null) cameraPhotoPreviewUIManager.ShowPreviewFromThumbnail(tex);
        currentPreviewIndex = index;
    }

    private Texture2D GetOrCreateThumbnail(string fileName)
    {
        if (thumbCache.TryGetValue(fileName, out var cached))
            return cached;

        Texture2D tex = CameraPhotoStorage.LoadTexture(fileName);
        if (tex == null) return null;

        Texture2D thumb = MakeThumbnail(tex, thumbMaxSize);
        Object.Destroy(tex);
        thumbCache[fileName] = thumb;
        return thumb;
    }

    private static Texture2D MakeThumbnail(Texture2D src, int maxSize)
    {
        int w = src.width, h = src.height;
        float scale = Mathf.Min(maxSize / (float)w, maxSize / (float)h, 1f);
        int tw = Mathf.Max(1, Mathf.RoundToInt(w * scale));
        int th = Mathf.Max(1, Mathf.RoundToInt(h * scale));

        RenderTexture rt = RenderTexture.GetTemporary(tw, th, 0);
        Graphics.Blit(src, rt);

        RenderTexture prev = RenderTexture.active;
        RenderTexture.active = rt;

        Texture2D dst = new Texture2D(tw, th, TextureFormat.RGB24, false);
        dst.ReadPixels(new Rect(0, 0, tw, th), 0, 0, false);
        dst.Apply(false);

        RenderTexture.active = prev;
        RenderTexture.ReleaseTemporary(rt);

        return dst;
    }
    private void OnDeleteModeChanged(bool on)
    {
        isDeleteMode = on;
        if (deleteButton != null) deleteButton.gameObject.SetActive(on);
        if (deleteAllButton != null) deleteAllButton.gameObject.SetActive(on); 
        if (!on) selectedForDeletion.Clear();
        Refresh();
    }

    private void OnDeleteConfirmed()
    {
        if (selectedForDeletion.Count == 0) return;

        foreach (string fileName in selectedForDeletion)
        {
            fileNames.Remove(fileName);

            if (thumbCache.TryGetValue(fileName, out var tex))
            {
                Object.Destroy(tex);
                thumbCache.Remove(fileName);
            }

            string path = Path.Combine(CameraPhotoStorage.PhotosDir, fileName);
            if (File.Exists(path)) File.Delete(path);
        }

        CameraPhotoStorage.SaveManifest(fileNames);
        selectedForDeletion.Clear();

        int totalPages = Mathf.Max(1, Mathf.CeilToInt(fileNames.Count / (float)pageSize));
        if (pageIndex >= totalPages) pageIndex = totalPages - 1;

        Refresh();
    }

    private void OnDeleteAllConfirmed()
    {
        if (fileNames.Count == 0) return;

        foreach (string fileName in fileNames)
        {
            if (thumbCache.TryGetValue(fileName, out var tex))
            {
                Object.Destroy(tex);
                thumbCache.Remove(fileName);
            }

            string path = Path.Combine(CameraPhotoStorage.PhotosDir, fileName);
            if (File.Exists(path)) File.Delete(path);
        }

        fileNames.Clear();
        selectedForDeletion.Clear();
        CameraPhotoStorage.SaveManifest(fileNames);

        pageIndex = 0;
        Refresh();
    }
}
