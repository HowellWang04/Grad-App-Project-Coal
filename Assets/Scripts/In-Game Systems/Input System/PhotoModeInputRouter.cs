using UnityEngine;
using UnityEngine.InputSystem;
using Controller.FinalCharacterController;

public sealed class PhotoModeInputRouter : MonoBehaviour
{
    [Header("Object Refs")]
    [SerializeField] private PlayerLocomotionInput locomotionInput;
    [SerializeField] private CameraSystemManager cameraSystemManager;
    [SerializeField] private PlayerPanelUIManager playerPanel;
    [SerializeField] private PlayerMovementManager playerMovementManager;
    [SerializeField] private CameraRaiseAnimationHandler cameraRaiseAnimationHandler;
    
    private InputAction cameraToggle;   
    private InputAction shutter;
    private InputAction cursorHold;

    private bool cursorMode = false;
    private float enterTime;

    private void OnEnable()
    {
        if (locomotionInput == null) return;
        if (cameraSystemManager == null) return;
        if (playerPanel == null) return;

        var map = locomotionInput.PlayerControls.PlayerLocomotionMap;

        cameraToggle = map.CameraModeToggle;
        shutter = map.PhotoShutter;
        cursorHold = map.CursorHold;
        cameraToggle.performed += OnCameraTogglePerformed;
        shutter.performed += OnShutterPerformed;
        cursorHold.started += OnCursorToggle;
        shutter.Disable();
    }

    private void OnDisable()
    {
        if (cameraToggle != null) cameraToggle.performed -= OnCameraTogglePerformed;
        if (shutter != null) shutter.performed -= OnShutterPerformed;
        if (cursorHold != null)
            cursorHold.started -= OnCursorToggle;
    }

    private void OnCameraTogglePerformed(InputAction.CallbackContext ctx)
    {
        if (cameraSystemManager == null) return;
        if (playerPanel.IsOpen) return;

        if (cameraSystemManager.CurrentState == CameraModeState.Reviewing) return;

        if (cameraSystemManager.CurrentState == CameraModeState.Disabled)
        {
            if (cameraRaiseAnimationHandler != null)
            {
                cameraRaiseAnimationHandler.PlayRaise(() =>
                {
                    cameraSystemManager.EnterCameraMode();
                    enterTime = Time.time;
                    shutter.Enable();
                });
            }
            else
            {
                cameraSystemManager.EnterCameraMode();
                enterTime = Time.time;
                shutter.Enable();
            }
        }
        else
        {
            if (cursorMode) ExitCursorMode();
            shutter.Disable();
            cameraSystemManager.ExitCameraMode();
            if (cameraRaiseAnimationHandler != null)
                cameraRaiseAnimationHandler.PlayLower(null);
        }
    }

    private void OnShutterPerformed(InputAction.CallbackContext ctx)
    {
        if (cameraSystemManager == null) return;
        if (playerPanel.IsOpen) return;
        if (cameraSystemManager.CurrentState == CameraModeState.Disabled) return; 
        if (cursorMode) return;

        var currentState = cameraSystemManager.CurrentState;

        // In REVIEWING, shutter button exits review (back to ACTIVE) instead of attempting another capture
        if (currentState == CameraModeState.Reviewing)
        {
            cameraSystemManager.CloseReview();
            return;
        }

        // In ACTIVE, shutter button initiates capture
        if (currentState == CameraModeState.Active)
        {
            cameraSystemManager.Capture();
        }
    }

    private void OnCursorToggle(InputAction.CallbackContext ctx)
    {
        if (cameraSystemManager.CurrentState != CameraModeState.Active) return;

        if (!cursorMode)
        {
            cursorMode = true;
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            if (playerMovementManager != null) playerMovementManager.SetPlayerMovementControl(false);
        }
        else
        {
            ExitCursorMode();
        }
    }

    private void ExitCursorMode()
    {
        if (!cursorMode) return;

        cursorMode = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (playerMovementManager != null) playerMovementManager.SetPlayerMovementControl(true);
    }
}
