using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemBackpackSlot : MonoBehaviour
{
    [Header("UI Refs")]
    [SerializeField] private RawImage previewImage;
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private Button slotButton;

    [Header("Preview Settings")]
    [SerializeField] private Vector3 previewCameraOffset = new Vector3(0f, 0f, -1f);
    [SerializeField] private int renderTextureSize = 256;
    [SerializeField] private int previewLayer = 7;

    private ItemBackpackManager.BackpackEntry entry;
    private GameObject previewClone;
    private Camera previewCamera;
    private RenderTexture renderTexture;
    private InspectUIManager inspectUI;
    private Camera playerCamera;
    private Action onInspectStarted;

    public void Setup(ItemBackpackManager.BackpackEntry data, InspectUIManager inspect, Camera cam, Action onInspectStartedCallback = null)
    {
        entry = data;
        inspectUI = inspect;
        playerCamera = cam;
        onInspectStarted = onInspectStartedCallback;

        if (itemNameText != null) itemNameText.text = data.itemName;
        if (slotButton != null) slotButton.onClick.AddListener(OnSlotClicked);

        BuildPreview();
    }

    private void BuildPreview()
    {
        if (entry.prefab == null || previewImage == null) return;

        // 创建 RenderTexture
        renderTexture = new RenderTexture(renderTextureSize, renderTextureSize, 16);
        previewImage.texture = renderTexture;

        // 在场景外某处生成预览克隆
        Vector3 spawnPos = new Vector3(0f, -1000f, 0f);
        previewClone = Instantiate(entry.prefab, spawnPos, Quaternion.Euler(entry.inspectRotationOffset));
        previewClone.transform.localScale = entry.prefab.transform.lossyScale * entry.inspectScale;
        previewClone.SetActive(true);
        SetLayerRecursive(previewClone, previewLayer);

        // 移除不需要的组件
        foreach (var col in previewClone.GetComponentsInChildren<Collider>()) Destroy(col);
        foreach (var rb in previewClone.GetComponentsInChildren<Rigidbody>()) Destroy(rb);
        var inspectable = previewClone.GetComponent<InspectableItem>();
        if (inspectable != null) Destroy(inspectable);

        // 创建预览相机
        var camGO = new GameObject("PreviewCam_" + entry.itemName);
        camGO.transform.SetParent(previewClone.transform.parent);
        camGO.transform.position = spawnPos + previewCameraOffset * entry.inspectDistance;
        camGO.transform.LookAt(previewClone.transform.position);

        previewCamera = camGO.AddComponent<Camera>();
        previewCamera.cullingMask = 1 << previewLayer;
        previewCamera.clearFlags = CameraClearFlags.SolidColor;
        previewCamera.backgroundColor = Color.clear;
        previewCamera.targetTexture = renderTexture;
        previewCamera.nearClipPlane = 0.01f;
    }

    private void OnSlotClicked()
    {
        if (inspectUI == null || playerCamera == null) return;
        if (inspectUI.IsInspecting) return;

        var inspectable = entry.prefab.GetComponent<InspectableItem>();
        if (inspectable == null) return;

        onInspectStarted?.Invoke();
        inspectUI.StartInspect(inspectable, playerCamera);
    }

    private void SetLayerRecursive(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
            SetLayerRecursive(child.gameObject, layer);
    }

    private void OnDestroy()
    {
        if (previewClone != null) Destroy(previewClone);
        if (previewCamera != null) Destroy(previewCamera.gameObject);
        if (renderTexture != null) renderTexture.Release();
    }
}
