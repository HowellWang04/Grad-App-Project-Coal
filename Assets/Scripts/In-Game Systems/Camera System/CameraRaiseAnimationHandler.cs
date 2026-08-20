using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public sealed class CameraRaiseAnimationHandler : MonoBehaviour
{
    [Header("Camera Model Ref")]
    [SerializeField] private GameObject cameraModel;

    [Header("Post Processing Volume Ref")]
    [SerializeField] private Volume globalVolume;

    [Header("Start State (below screen)")]
    [SerializeField] private Vector3 startLocalPosition = new Vector3(0f, -0.6f, 1.2f);
    [SerializeField] private Vector3 startLocalRotation = new Vector3(0f, 0f, 0f);
    [SerializeField] private Vector3 startLocalScale    = new Vector3(0.3f, 0.3f, 0.3f);

    [Header("End State (close to eye)")]
    [SerializeField] private Vector3 endLocalPosition = new Vector3(0f, 0f, 0.4f);
    [SerializeField] private Vector3 endLocalRotation = new Vector3(0f, 0f, 0f);
    [SerializeField] private Vector3 endLocalScale    = new Vector3(2f, 2f, 2f);

    [Header("Timing")]
    [SerializeField] private float duration = 0.6f;
    [SerializeField] private AnimationCurve curve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private Coroutine current;
    private DepthOfField dof;

    private void Awake()
    {
        if (cameraModel != null) cameraModel.SetActive(false);
        if (globalVolume != null) globalVolume.profile.TryGet(out dof);
    }

    public void PlayRaise(Action onComplete)
    {
        if (current != null) StopCoroutine(current);
        cameraModel.SetActive(true);
        current = StartCoroutine(AnimateRoutine(startLocalPosition, startLocalRotation, startLocalScale,
                                                endLocalPosition,   endLocalRotation,   endLocalScale,
                                                hideOnComplete: true, onComplete));
    }

    public void PlayLower(Action onComplete)
    {
        if (current != null) StopCoroutine(current);
        cameraModel.SetActive(true);
        hideCameraModelShadows();
        cameraModel.transform.localPosition    = endLocalPosition;
        cameraModel.transform.localEulerAngles = endLocalRotation;
        cameraModel.transform.localScale       = endLocalScale;
        current = StartCoroutine(AnimateRoutine(endLocalPosition, endLocalRotation, endLocalScale,
                                                startLocalPosition, startLocalRotation, startLocalScale,
                                                hideOnComplete: true, onComplete));
    }

    private IEnumerator AnimateRoutine(
        Vector3 fromPos, Vector3 fromRot, Vector3 fromScale,
        Vector3 toPos,   Vector3 toRot,   Vector3 toScale,
        bool hideOnComplete, Action onComplete)
    {
        if (dof != null) dof.active = false;
        

        cameraModel.transform.localPosition    = fromPos;
        cameraModel.transform.localEulerAngles = fromRot;
        cameraModel.transform.localScale       = fromScale;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = curve.Evaluate(Mathf.Clamp01(elapsed / duration));

            cameraModel.transform.localPosition    = Vector3.Lerp(fromPos,   toPos,   t);
            cameraModel.transform.localEulerAngles = Vector3.Lerp(fromRot,   toRot,   t);
            cameraModel.transform.localScale       = Vector3.Lerp(fromScale, toScale, t);

            yield return null;
        }

        if (hideOnComplete) cameraModel.SetActive(false);
        if (dof != null) dof.active = true;
        current = null;
        onComplete?.Invoke();
    }

    private void hideCameraModelShadows()
    {
        var renderers = cameraModel.GetComponentsInChildren<Renderer>();
        foreach (var r in renderers)
        {
            r.shadowCastingMode = ShadowCastingMode.Off;
            r.receiveShadows = false;
        }
    }
}
