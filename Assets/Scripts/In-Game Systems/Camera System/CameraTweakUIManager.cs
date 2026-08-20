using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CameraTweakUIManager : MonoBehaviour
{
    [Header("Camera Ref")]
    [SerializeField] private Camera photoCamera;

    [Header("UI Components Refs")]
    [SerializeField] private GameObject cameraTab;
    [SerializeField] private GameObject postTab;
    [SerializeField] private Button cameraTabButton;
    [SerializeField] private Button postTabButton;
    [SerializeField] private Button resetButton;
    [SerializeField] private Toggle gridToggle;
    [SerializeField] private GameObject gridOverlay;

    [Header("Focal Length")]
    [SerializeField] private Slider focalLengthSlider;
    [SerializeField] private TMP_Text focalLengthText;

    [Header("Range (mm)")]
    [SerializeField] private float minFocalLength = 7f;
    [SerializeField] private float maxFocalLength = 275f;

    [Header("Depth of Field")]
    [SerializeField] private Volume globalVolume;
    [SerializeField] private Slider apertureSlider;
    [SerializeField] private TMP_Text apertureText;
    [SerializeField] private float minAperture = 1.4f;
    [SerializeField] private float maxAperture = 16f;

    [Header("Vignette")]
    [SerializeField] private Slider vignetteSlider;
    [SerializeField] private TMP_Text vignetteText;
    [SerializeField] private float minVignette = 0f;
    [SerializeField] private float maxVignette = 1f;

    [Header("Exposure")]
    [SerializeField] private Slider exposureSlider;
    [SerializeField] private TMP_Text exposureText;
    [SerializeField] private float minExposure = -3f;
    [SerializeField] private float maxExposure = 3f;

    [Header("Bloom")]
    [SerializeField] private Slider bloomSlider;
    [SerializeField] private TMP_Text bloomText;
    [SerializeField] private float minBloom = 0f;
    [SerializeField] private float maxBloom = 10f;

    [Header("Saturation")]
    [SerializeField] private Slider saturationSlider;
    [SerializeField] private TMP_Text saturationText;
    [SerializeField] private float minSaturation = -100f;
    [SerializeField] private float maxSaturation = 100f;

    private DepthOfField dof;
    private Vignette vignette;
    private ColorAdjustments colorAdjustments;
    private Bloom bloom;

    private float defaultFocalLength;
    private float defaultAperture;
    private float defaultVignetteIntensity;
    private float defaultExposure;
    private float defaultBloomIntensity;
    private float defaultSaturation;

    private bool defaultsSaved = false;

    // 35mm full-frame sensor height (24mm)
    private const float SensorHalfHeight = 12f;

    private void Awake()
    {
        if (globalVolume != null)
        {
            globalVolume.profile.TryGet(out dof);
            globalVolume.profile.TryGet(out vignette);
            globalVolume.profile.TryGet(out colorAdjustments);
            globalVolume.profile.TryGet(out bloom);
        }
        SaveDefaults();
    }

    private void OnEnable()
    {
        if (focalLengthSlider != null)
        {
            focalLengthSlider.minValue = minFocalLength;
            focalLengthSlider.maxValue = maxFocalLength;
            focalLengthSlider.onValueChanged.AddListener(OnFocalLengthChanged);
            SyncFocalLengthSlider();
        }

        if (apertureSlider != null)
        {
            apertureSlider.minValue = minAperture;
            apertureSlider.maxValue = maxAperture;
            apertureSlider.onValueChanged.AddListener(OnApertureChanged);
            SyncApertureSlider();
        }

        if (vignetteSlider != null)
        {
            vignetteSlider.minValue = minVignette;
            vignetteSlider.maxValue = maxVignette;
            vignetteSlider.onValueChanged.AddListener(OnVignetteChanged);
            SyncVignetteSlider();
        }

        if (exposureSlider != null)
        {
            exposureSlider.minValue = minExposure;
            exposureSlider.maxValue = maxExposure;
            exposureSlider.onValueChanged.AddListener(OnExposureChanged);
            SyncExposureSlider();
        }

        if (bloomSlider != null)
        {
            bloomSlider.minValue = minBloom;
            bloomSlider.maxValue = maxBloom;
            bloomSlider.onValueChanged.AddListener(OnBloomChanged);
            SyncBloomSlider();
        }

        if (saturationSlider != null)
        {
            saturationSlider.minValue = minSaturation;
            saturationSlider.maxValue = maxSaturation;
            saturationSlider.onValueChanged.AddListener(OnSaturationChanged);
            SyncSaturationSlider();
        }

        if (cameraTabButton != null) cameraTabButton.onClick.AddListener(ShowCameraTab);
        if (postTabButton != null)   postTabButton.onClick.AddListener(ShowPostTab);
        if (resetButton != null)     resetButton.onClick.AddListener(ResetCameraTweaks);
        if (gridToggle != null)      gridToggle.onValueChanged.AddListener(OnGridToggleChanged);

        ShowCameraTab();
    }

    private void OnDisable()
    {
        if (focalLengthSlider != null) focalLengthSlider.onValueChanged.RemoveListener(OnFocalLengthChanged);
        if (apertureSlider != null)    apertureSlider.onValueChanged.RemoveListener(OnApertureChanged);
        if (vignetteSlider != null)    vignetteSlider.onValueChanged.RemoveListener(OnVignetteChanged);
        if (exposureSlider != null)    exposureSlider.onValueChanged.RemoveListener(OnExposureChanged);
        if (bloomSlider != null)       bloomSlider.onValueChanged.RemoveListener(OnBloomChanged);
        if (saturationSlider != null)  saturationSlider.onValueChanged.RemoveListener(OnSaturationChanged);
        if (cameraTabButton != null)   cameraTabButton.onClick.RemoveListener(ShowCameraTab);
        if (postTabButton != null)     postTabButton.onClick.RemoveListener(ShowPostTab);
        if (resetButton != null)       resetButton.onClick.RemoveListener(ResetCameraTweaks);
        if (gridToggle != null)        gridToggle.onValueChanged.RemoveListener(OnGridToggleChanged);
    }

    // ── Focal Length ────────────────────────────────────────────

    private float FovFromFocalLength(float mm) =>
        2f * Mathf.Atan(SensorHalfHeight / mm) * Mathf.Rad2Deg;

    private float FocalLengthFromFov(float fov) =>
        SensorHalfHeight / Mathf.Tan(fov * 0.5f * Mathf.Deg2Rad);

    private void OnFocalLengthChanged(float mm)
    {
        if (photoCamera == null) return;
        photoCamera.fieldOfView = FovFromFocalLength(mm);
        if (focalLengthText != null) focalLengthText.text = $"{mm:F0}mm";
    }

    private void SyncFocalLengthSlider()
    {
        if (photoCamera == null || focalLengthSlider == null) return;
        float mm = Mathf.Clamp(FocalLengthFromFov(photoCamera.fieldOfView), minFocalLength, maxFocalLength);
        focalLengthSlider.SetValueWithoutNotify(mm);
        if (focalLengthText != null) focalLengthText.text = $"{mm:F0}mm";
    }

    // ── Aperture ─────────────────────────────────────────────────

    private void OnApertureChanged(float value)
    {
        if (dof == null) return;
        dof.aperture.Override(value);
        if (apertureText != null) apertureText.text = $"{value:F1}";
    }

    private void SyncApertureSlider()
    {
        if (dof == null || apertureSlider == null) return;
        apertureSlider.SetValueWithoutNotify(dof.aperture.value);
        if (apertureText != null) apertureText.text = $"{dof.aperture.value:F1}";
    }

    // ── Vignette ──────────────────────────────────────────────────

    private void OnVignetteChanged(float value)
    {
        if (vignette == null) return;
        vignette.intensity.Override(value);
        if (vignetteText != null) vignetteText.text = $"{value * 100f:F0}%";
    }

    private void SyncVignetteSlider()
    {
        if (vignette == null || vignetteSlider == null) return;
        vignetteSlider.SetValueWithoutNotify(vignette.intensity.value);
        if (vignetteText != null) vignetteText.text = $"{vignette.intensity.value * 100f:F0}%";
    }

    // ── Exposure ──────────────────────────────────────────────────

    private void OnExposureChanged(float value)
    {
        if (colorAdjustments == null) return;
        colorAdjustments.postExposure.Override(value);
        if (exposureText != null) exposureText.text = $"{value:F1}";
    }

    private void SyncExposureSlider()
    {
        if (colorAdjustments == null || exposureSlider == null) return;
        exposureSlider.SetValueWithoutNotify(colorAdjustments.postExposure.value);
        if (exposureText != null) exposureText.text = $"{colorAdjustments.postExposure.value:F1}";
    }

    // ── Bloom ─────────────────────────────────────────────────────

    private void OnBloomChanged(float value)
    {
        if (bloom == null) return;
        bloom.intensity.Override(value);
        if (bloomText != null) bloomText.text = $"{value:F1}";
    }

    private void SyncBloomSlider()
    {
        if (bloom == null || bloomSlider == null) return;
        bloomSlider.SetValueWithoutNotify(bloom.intensity.value);
        if (bloomText != null) bloomText.text = $"{bloom.intensity.value:F1}";
    }

    // ── Saturation ────────────────────────────────────────────────

    private void OnSaturationChanged(float value)
    {
        if (colorAdjustments == null) return;
        colorAdjustments.saturation.Override(value);
        if (saturationText != null) saturationText.text = $"{value:F0}%";
    }

    private void SyncSaturationSlider()
    {
        if (colorAdjustments == null || saturationSlider == null) return;
        saturationSlider.SetValueWithoutNotify(colorAdjustments.saturation.value);
        if (saturationText != null) saturationText.text = $"{colorAdjustments.saturation.value:F0}%";
    }

    // ── Tabs & Grid ───────────────────────────────────────────────

    private void ShowCameraTab()
    {
        if (cameraTab != null) cameraTab.SetActive(true);
        if (postTab != null)   postTab.SetActive(false);
    }

    private void ShowPostTab()
    {
        if (cameraTab != null) cameraTab.SetActive(false);
        if (postTab != null)   postTab.SetActive(true);
    }

    private void OnGridToggleChanged(bool isOn)
    {
        if (gridOverlay != null) gridOverlay.SetActive(isOn);
    }

    // ── Defaults ──────────────────────────────────────────────────

    private void SaveDefaults()
    {
        if (defaultsSaved) return;
        defaultsSaved = true;

        if (photoCamera != null)
            defaultFocalLength = FocalLengthFromFov(photoCamera.fieldOfView);
        if (dof != null)
            defaultAperture = dof.aperture.value;
        if (vignette != null)
            defaultVignetteIntensity = vignette.intensity.value;
        if (colorAdjustments != null)
        {
            defaultExposure    = colorAdjustments.postExposure.value;
            defaultSaturation  = colorAdjustments.saturation.value;
        }
        if (bloom != null)
            defaultBloomIntensity = bloom.intensity.value;
    }

    public void ResetCameraTweaks()
    {
        if (photoCamera != null)
            photoCamera.fieldOfView = FovFromFocalLength(defaultFocalLength);
        if (dof != null)
            dof.aperture.Override(defaultAperture);
        if (vignette != null)
            vignette.intensity.Override(defaultVignetteIntensity);
        if (colorAdjustments != null)
        {
            colorAdjustments.postExposure.Override(defaultExposure);
            colorAdjustments.saturation.Override(defaultSaturation);
        }
        if (bloom != null)
            bloom.intensity.Override(defaultBloomIntensity);

        SyncFocalLengthSlider();
        SyncApertureSlider();
        SyncVignetteSlider();
        SyncExposureSlider();
        SyncBloomSlider();
        SyncSaturationSlider();
    }
}
