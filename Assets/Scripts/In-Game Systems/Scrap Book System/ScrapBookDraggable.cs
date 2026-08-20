using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Attach to elements in EditTopBar scroll area.
/// Drag to create a copy on the ScrapBookCanvas.
/// </summary>
public class ScrapBookDraggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private string photoFileName;
    private Texture2D photoThumb;
    private ScrapBookDecoPreset decoPreset;

    private GameObject dragGhost;
    private RectTransform canvasRect;
    private Canvas rootCanvas;
    private ScrapBookCanvas scrapBookCanvas;

    // Setup for photo elements: thumb for ghost preview, fileName for loading full res on drop
    public void SetupPhoto(string fileName, Texture2D thumb, ScrapBookCanvas canvas)
    {
        photoFileName = fileName;
        photoThumb = thumb;
        decoPreset = null;
        scrapBookCanvas = canvas;
        CacheRefs();
    }

    // Setup for decoration elements (sticker or textbox)
    public void Setup(ScrapBookDecoPreset preset, ScrapBookCanvas canvas)
    {
        decoPreset = preset;
        photoFileName = null;
        photoThumb = null;
        scrapBookCanvas = canvas;
        CacheRefs();
    }

    private void Start()
    {
        CacheRefs();
    }

    private void CacheRefs()
    {
        if (rootCanvas == null)
        {
            var parentCanvas = GetComponentInParent<Canvas>();
            if (parentCanvas != null) rootCanvas = parentCanvas.rootCanvas;
        }
        if (canvasRect == null && scrapBookCanvas != null)
            canvasRect = scrapBookCanvas.GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        CacheRefs();
        if (rootCanvas == null) return;

        dragGhost = new GameObject("DragGhost");
        dragGhost.transform.SetParent(rootCanvas.transform, false);

        var rt = dragGhost.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(80, 80);

        // Build ghost visual
        if (photoThumb != null)
        {
            var raw = dragGhost.AddComponent<RawImage>();
            raw.texture = photoThumb;
            raw.raycastTarget = false;
        }
        else if (decoPreset != null && decoPreset.icon != null)
        {
            var img = dragGhost.AddComponent<Image>();
            img.sprite = decoPreset.icon;
            img.preserveAspect = true;
            img.raycastTarget = false;
        }

        var cg = dragGhost.AddComponent<CanvasGroup>();
        cg.alpha = 0.7f;
        cg.blocksRaycasts = false;

        UpdateGhostPosition(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        UpdateGhostPosition(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (dragGhost != null)
        {
            Destroy(dragGhost);
            dragGhost = null;
        }

        if (scrapBookCanvas == null || canvasRect == null) return;

        // Check if drop position is inside ScrapBookCanvas
        if (!RectTransformUtility.RectangleContainsScreenPoint(canvasRect, eventData.position, eventData.pressEventCamera))
            return;

        // Convert screen position to local position inside ScrapBookCanvas
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect, eventData.position, eventData.pressEventCamera, out Vector2 localPos);

        // Place element on canvas: load full resolution photo
        if (!string.IsNullOrEmpty(photoFileName))
        {
            Texture2D fullTex = CameraPhotoStorage.LoadTexture(photoFileName);
            if (fullTex != null)
                scrapBookCanvas.AddPhotoElement(fullTex, localPos, photoFileName);
            return;
        }
        if (decoPreset != null)
            scrapBookCanvas.AddDecoElement(decoPreset, localPos);
    }

    private void UpdateGhostPosition(PointerEventData eventData)
    {
        if (dragGhost == null || rootCanvas == null) return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rootCanvas.transform as RectTransform, eventData.position, eventData.pressEventCamera, out Vector2 pos);

        dragGhost.GetComponent<RectTransform>().anchoredPosition = pos;
    }
}
