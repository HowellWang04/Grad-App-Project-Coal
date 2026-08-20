using UnityEngine;

namespace Controller.FinalCharacterController
{
    [DefaultExecutionOrder(-1)]
    public class PlayerMovementManager : MonoBehaviour
    {
        [Header("Game Object Refs")]
        [SerializeField] private CharacterController characterController;
        [SerializeField] private Camera playerCamera;

        [Header("Movement")]
        public float maxSpeed = 4f;
        public float acceleration = 20f;
        public float deceleration = 25f;
        public float gravity = -9.81f;

        [Header("Look")]
        public float lookSensitivity = 0.15f;
        public float lookLimit = 85f;

        [Header("State")]
        [SerializeField] private bool allowControl = true;


        private Vector3 playerFinalVelocity;            
        private float yaw;                   
        private float pitch;                 

        private PlayerLocomotionInput input;

        public bool AllowControl
        {
            get => allowControl;
            set => allowControl = value;
        }

        public bool IsGrounded => characterController.isGrounded;
        public bool IsMoving => new Vector3(playerFinalVelocity.x, 0f, playerFinalVelocity.z).sqrMagnitude > 0.01f;

        private void Awake()
        {
            input = GetComponent<PlayerLocomotionInput>();

            yaw = transform.eulerAngles.y;
        }

        private void Update()
        {
            if (!allowControl) return;

            HandleMovement();
        }

        private void LateUpdate()
        {
            if (!allowControl) return;

            HandleLook();
        }

        private void HandleMovement()
        {
            // Project camera forward/right to ground plane
            Vector3 forward = Vector3.ProjectOnPlane(playerCamera.transform.forward, Vector3.up).normalized;
            Vector3 right   = Vector3.ProjectOnPlane(playerCamera.transform.right,   Vector3.up).normalized;

            // Get input from PlayerLocomotionInput
            Vector2 moveInput = input.MovementInput;

            // Get direction by input and camera orientation
            Vector3 wishDir = right * moveInput.x + forward * moveInput.y;
            wishDir = wishDir.sqrMagnitude > 1f ? wishDir.normalized : wishDir;

            Vector3 targetVelocity = wishDir * maxSpeed;

            float accel = (wishDir.sqrMagnitude > 0.01f) ? acceleration : deceleration;

            
            Vector3 horizontalVelocity = new Vector3(playerFinalVelocity.x, 0f, playerFinalVelocity.z);
            // Smoothly move from current velocity towards target velocity
            horizontalVelocity = Vector3.MoveTowards(
                horizontalVelocity,
                targetVelocity,
                accel * Time.deltaTime
            );

            // Gravity
            if (characterController.isGrounded && playerFinalVelocity.y < 0f)
                playerFinalVelocity.y = -2f; // Stick to ground

            playerFinalVelocity.y += gravity * Time.deltaTime;

            // Combine horizontal and vertical velocity
            playerFinalVelocity.x = horizontalVelocity.x;
            playerFinalVelocity.z = horizontalVelocity.z;

            // Use playerFinalVelocity to move character
            characterController.Move(playerFinalVelocity * Time.deltaTime);
        }

        private void HandleLook()
        {
            Vector2 look = input.LookInput;

            yaw   += look.x * lookSensitivity;
            pitch -= look.y * lookSensitivity;
            pitch = Mathf.Clamp(pitch, -lookLimit, lookLimit);

            // Apply rotations to player, whole body turns Y (left/right)
            transform.rotation = Quaternion.Euler(0f, yaw, 0f);

            // Apply rotations to camera, only X (up/down)
            playerCamera.transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        }

        public void SetPlayerMovementControl(bool value)
        {
            allowControl = value;
        }
    }
}
