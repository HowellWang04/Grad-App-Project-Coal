using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Controller.FinalCharacterController
{
    [DefaultExecutionOrder(-2)]
    public class PlayerLocomotionInput : MonoBehaviour, PlayerControls.IPlayerLocomotionMapActions
    {
        public PlayerControls PlayerControls { get; private set; }
        public Vector2 MovementInput { get; private set; }
        public Vector2 LookInput { get; private set; }

        private void Awake()
        {
            PlayerControls = new PlayerControls();
        }

        private void OnEnable()
        {
            PlayerControls.Enable();
            PlayerControls.PlayerLocomotionMap.Enable();
            PlayerControls.PlayerLocomotionMap.SetCallbacks(this);
        }

        private void OnDisable()
        {
            PlayerControls.PlayerLocomotionMap.RemoveCallbacks(this);
            PlayerControls.PlayerLocomotionMap.Disable();
            PlayerControls.Disable();
        }

        public void OnMovement(InputAction.CallbackContext context)
        {
            MovementInput = context.ReadValue<Vector2>();
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            LookInput = context.ReadValue<Vector2>();
        }

        public void OnCameraZoom(InputAction.CallbackContext context)
        {
            // handled in CameraZoom.cs
        }

        public void OnCancel(InputAction.CallbackContext context)
        {
            // not used
        }

        public void OnCameraModeToggle(InputAction.CallbackContext context)
        {
            // handled in PhotoSystemController.cs
        }

        public void OnPhotoShutter(InputAction.CallbackContext context)
        {
            // handled in PhotoSystemController.cs
        }

        public void OnPlayerPanelToggle(InputAction.CallbackContext context)
        {
            // handled in PlayerPanelUIManager.cs
        }

        public void OnUIBack(InputAction.CallbackContext context)
        {
            // handled in PlayerPanelController.cs
        }

        public void OnCursorHold(InputAction.CallbackContext context)
        {
            // handled in PhotoModeInputRouter.cs
        }

    }
}