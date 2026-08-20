using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class CameraSystemManager : MonoBehaviour
{


    [Header("UI Root Ref")]
    [SerializeField] private GameObject CameraUIRoot;

    [Header("UI Manager Refs")]
    [SerializeField] private CameraTweakUIManager cameraTweakingUIManager;
    [SerializeField] private CameraPhotoPreviewUIManager cameraPhotoPreviewUIManager;
    [SerializeField] private CameraPhotoAlbumUIManager cameraPhotoAlbumUIManager;

    [Header("Photo Capture Function Provider")]
    [SerializeField] private CameraPhotoCapture cameraPhotoCapture;

    [Header("Echo System")]
    [SerializeField] private CameraHighlightObjectDetector highlightObjectDetector;

    public event Action<IReadOnlyCollection<CameraHighlightObject>> OnPhotoCapturedWithEchoes;


    // Initialize the state machine to manage photo mode states
    private CameraStateMachine stateMachine;
    // Expose the current state for external reference
    public CameraModeState CurrentState => stateMachine.State;

    private Coroutine captureRoutine;
    private List<CameraHighlightObject> captureSnapshot;


    private void Awake()
    {
        stateMachine = new CameraStateMachine();
        stateMachine.OnStateChanged += HandleStateChanged;

        if (CameraUIRoot != null) CameraUIRoot.SetActive(false);
        if (cameraPhotoPreviewUIManager != null) cameraPhotoPreviewUIManager.HidePreviewImmediate();
    }

    private void Start()
    {
        if (cameraPhotoAlbumUIManager != null)
            StartCoroutine(cameraPhotoAlbumUIManager.PrewarmThumbnails());
    }

    public void EnterCameraMode()
    {
        if (!stateMachine.EnterCameraMode()) return;
    }

    public void ExitCameraMode()
    {
        if (!stateMachine.ExitCameraMode()) return;
    }

    private void HandleStateChanged(CameraModeState prev, CameraModeState next)
    {
        switch (next)
        {
            case CameraModeState.Disabled:
                if (CameraUIRoot != null) CameraUIRoot.SetActive(false);
                if (cameraPhotoPreviewUIManager != null) cameraPhotoPreviewUIManager.HidePreviewImmediate();
                if (cameraTweakingUIManager != null) cameraTweakingUIManager.ResetCameraTweaks();
                break;

            case CameraModeState.Active:
                if (CameraUIRoot != null) CameraUIRoot.SetActive(true);
                if (cameraPhotoPreviewUIManager != null) cameraPhotoPreviewUIManager.HidePreviewImmediate();
                break;

            case CameraModeState.Capturing:
                if (CameraUIRoot != null) CameraUIRoot.SetActive(true);
                break;

            case CameraModeState.Reviewing:
                if (CameraUIRoot != null) CameraUIRoot.SetActive(true);
                break;
        }
    }

    public void Capture()
    {
        if (!stateMachine.RequestCapture()) return;

        if (highlightObjectDetector != null)
        {
            var raw = new List<CameraHighlightObject>(highlightObjectDetector.HighlightedObjects);
            captureSnapshot = highlightObjectDetector.FilterConfirmedObjects(raw);
        }
        else
            captureSnapshot = null;

        if (cameraPhotoCapture == null) { stateMachine.FinishCaptureToActive(); return; }

        if (captureRoutine != null) StopCoroutine(captureRoutine);
        captureRoutine = StartCoroutine(CaptureFlow());
    }

    private IEnumerator CaptureFlow()
    {
        if (cameraTweakingUIManager != null) CameraUIRoot.SetActive(false);

        Texture2D captured = null;
        yield return cameraPhotoCapture.Capture(tex => captured = tex);

        if (cameraTweakingUIManager != null) CameraUIRoot.SetActive(true);

        if (captured == null)
        {
            stateMachine.FinishCaptureToActive();
            yield break;
        }

        if (cameraPhotoPreviewUIManager != null) cameraPhotoPreviewUIManager.ShowCapturedPreview(captured);

        string fileName = null;
        if (cameraPhotoAlbumUIManager != null) fileName = cameraPhotoAlbumUIManager.AddPhotoFromTexture(captured);

        // Bind the captured Echo texts to this photo's fileName for later scrapbook display.
        if (!string.IsNullOrEmpty(fileName) && captureSnapshot != null)
        {
            var scrapbookContents = new List<string>();
            foreach (var obj in captureSnapshot)
                if (obj.TryGetComponent<EchoTrigger>(out var echo))
                {
                    scrapbookContents.Add(echo.scrapbookContent);

                    if (!string.IsNullOrEmpty(echo.stickyNoteSlotId))
                        GrandpaDiaryStickyNoteStorage.Unlock(echo.stickyNoteSlotId);
                }

            EchoPhotoStorage.Save(fileName, scrapbookContents);
        }

        if (cameraPhotoAlbumUIManager != null) cameraPhotoAlbumUIManager.Refresh();

        OnPhotoCapturedWithEchoes?.Invoke(captureSnapshot);

        stateMachine.FinishCaptureToReviewing();
        captureRoutine = null;
    }

    public void CloseReview()
    {
        if (!stateMachine.CloseReview()) return;
        if (cameraPhotoPreviewUIManager != null) cameraPhotoPreviewUIManager.ClosePreview();
    }
}
