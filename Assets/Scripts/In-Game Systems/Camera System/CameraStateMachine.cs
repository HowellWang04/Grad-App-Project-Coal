using System;

public sealed class CameraStateMachine
{
    public CameraModeState State { get; private set; } = CameraModeState.Disabled;

    public event Action<CameraModeState, CameraModeState> OnStateChanged;

    
    private bool CanEnterCameraMode() => State == CameraModeState.Disabled;

    private bool CanExitCameraMode() => State == CameraModeState.Active || State == CameraModeState.Reviewing;

    private bool CanCapture() => State == CameraModeState.Active;

    private bool CanOpenReview() => State == CameraModeState.Active;

    private bool CanCloseReview() => State == CameraModeState.Reviewing;


    // These bool methods attempt to perform a state transition and return true if successful, false if the transition was invalid in the current state

    public bool EnterCameraMode()
    {
        if (!CanEnterCameraMode()) return false;
        TransitionTo(CameraModeState.Active);
        return true;
    }

    public bool ExitCameraMode()
    {
        if (!CanExitCameraMode()) return false;
        TransitionTo(CameraModeState.Disabled);
        return true;
    }

    public bool RequestCapture()
    {
        if (!CanCapture()) return false;
        TransitionTo(CameraModeState.Capturing);
        return true;
    }

    public bool FinishCaptureToActive()
    {
        if (State != CameraModeState.Capturing) return false;
        TransitionTo(CameraModeState.Active);
        return true;
    }

    public bool FinishCaptureToReviewing()
    {
        if (State != CameraModeState.Capturing) return false;
        TransitionTo(CameraModeState.Reviewing);
        return true;
    }

    public bool OpenReview()
    {
        if (!CanOpenReview()) return false;
        TransitionTo(CameraModeState.Reviewing);
        return true;
    }

    public bool CloseReview()
    {
        if (!CanCloseReview()) return false;
        TransitionTo(CameraModeState.Active);
        return true;
    }

    private void TransitionTo(CameraModeState next)
    {
        var prev = State;
        if (prev == next) return;
        State = next;
        OnStateChanged?.Invoke(prev, next);
    }
}

