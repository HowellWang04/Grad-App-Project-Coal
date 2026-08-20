using UnityEngine;
using UnityEngine.InputSystem;
using Controller.FinalCharacterController;

public sealed class PlayerPanelInputRouter : MonoBehaviour
{
    [Header("Object Refs")]
    [SerializeField] private PlayerLocomotionInput locomotionInput;
    [SerializeField] private CameraSystemManager cameraSystemManager;
    [SerializeField] private PlayerPanelUIManager playerPanel;

    private InputAction playerPanelToggle;
    private InputAction back;

    private void OnEnable()
    {
        if (locomotionInput == null) return;
        if (playerPanel == null) return;
        if (cameraSystemManager == null) return;
        
        var locomotionMap = locomotionInput.PlayerControls.PlayerLocomotionMap;

        playerPanelToggle = locomotionMap.PlayerPanelToggle;
        back = locomotionMap.UIBack;

        playerPanelToggle.performed += OnPanelToggle;
        back.performed += OnBack;
    }

    private void OnDisable()
    {
        if (playerPanelToggle != null) playerPanelToggle.performed -= OnPanelToggle;
        if (back != null) back.performed -= OnBack;
    }

    private void OnPanelToggle(InputAction.CallbackContext ctx)
    {
        if (playerPanel == null) return;
        if (!CanTogglePlayerPanel()) return;

        playerPanel.Toggle();
    }

    private void OnBack(InputAction.CallbackContext ctx)
    {
        if (playerPanel == null) return;

        if (playerPanel.IsOpen)
            playerPanel.Close();
    }

    // The PlayerPanel can only be toggled when not in the middle of capturing or reviewing photos.
    private bool CanTogglePlayerPanel()
    {
        if (cameraSystemManager == null) return true;

        return cameraSystemManager.CurrentState == CameraModeState.Disabled;
    }
}
