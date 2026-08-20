using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public sealed class PhotoThumbItem : MonoBehaviour, IPointerClickHandler
{
    [Header("UI")]
    [SerializeField] private RawImage thumbImage;
    [SerializeField] private Button button;
    [SerializeField] private Toggle selectToggle;

    private Texture2D sourceTexture;
    private string fileName;
    private Action onClick;
    private AlbumPhotoContextMenu contextMenu;

    private void OnDestroy()
    {
        if (button != null) button.onClick.RemoveAllListeners();
        if (selectToggle != null) selectToggle.onValueChanged.RemoveAllListeners();
    }

    public void Bind(Texture2D tex, Action onClick = null, string fileName = null, AlbumPhotoContextMenu contextMenu = null)
    {
        sourceTexture = tex;
        this.onClick = onClick;
        this.fileName = fileName;
        this.contextMenu = contextMenu;
        if (thumbImage != null) thumbImage.texture = tex;

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            if (onClick != null) button.onClick.AddListener(() => onClick());
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (contextMenu != null && !string.IsNullOrEmpty(fileName))
                contextMenu.Show(fileName, eventData.position);
        }
    }

    public void SetDeleteMode(bool on, Action<bool> onToggleChanged = null)
    {
        if (selectToggle != null)
        {
            selectToggle.gameObject.SetActive(on);
            selectToggle.isOn = false;
            selectToggle.onValueChanged.RemoveAllListeners();
            if (on && onToggleChanged != null)
                selectToggle.onValueChanged.AddListener(isOn => onToggleChanged(isOn));
        }
        if (button != null) button.interactable = !on;
    }

    public void SetSelected(bool selected)
    {
        if (selectToggle != null) selectToggle.isOn = selected;
    }
}
