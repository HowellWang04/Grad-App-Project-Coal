using UnityEngine;
using Controller.FinalCharacterController;

public class PlayerFootstepAudio : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private PlayerMovementManager movementManager;

    [Header("Audio")]
    [SerializeField] private AudioSource footstepSource;
    [SerializeField] private AudioClip footstepClip;

    private void Awake()
    {
        if (footstepSource != null && footstepClip != null)
        {
            footstepSource.clip = footstepClip;
            footstepSource.loop = true;
            footstepSource.playOnAwake = false;
        }
    }

    private void Update()
    {
        if (movementManager == null || footstepSource == null) return;

        bool shouldPlay = movementManager.IsGrounded && movementManager.IsMoving && movementManager.AllowControl;

        if (shouldPlay && !footstepSource.isPlaying)
        {
            footstepSource.Play();
        }
        else if (!shouldPlay && footstepSource.isPlaying)
        {
            footstepSource.Pause();
        }
    }
}
