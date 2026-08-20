using System.Collections.Generic;
using UnityEngine;

public class EchoSystemManager : MonoBehaviour
{
    [SerializeField] private CameraSystemManager cameraSystemManager;
    [SerializeField] private EchoPopupUI echoPopupUI;

    private void OnEnable()
    {
        if (cameraSystemManager != null)
            cameraSystemManager.OnPhotoCapturedWithEchoes += HandleConfirmedHighlights;
    }

    private void OnDisable()
    {
        if (cameraSystemManager != null)
            cameraSystemManager.OnPhotoCapturedWithEchoes -= HandleConfirmedHighlights;
    }

    private void HandleConfirmedHighlights(IReadOnlyCollection<CameraHighlightObject> capturedObjects)
    {
        if (capturedObjects == null) return;

        foreach (var obj in capturedObjects)
        {
            if (obj == null) continue;
            if (!obj.TryGetComponent<EchoTrigger>(out var echo)) continue;

            if (echoPopupUI != null)
                echoPopupUI.Show();
            else
                Debug.LogWarning("[EchoSystem] EchoPopupUI is not assigned.");

            return;
        }
    }
}
