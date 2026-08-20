using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class EchoSimpleShimmer : MonoBehaviour
{
    [Header("Glow")]
    [SerializeField] private Color glowColor = new Color(1f, 0.82f, 0.25f, 1f);
    [SerializeField] private float minAlpha = 0.25f;
    [SerializeField] private float maxAlpha = 0.75f;
    [SerializeField] private float glowThickness = 4f;
    [SerializeField] private float pulseSpeed = 0.7f;

    private Outline outline;
    private float phaseOffset;

    private void Awake()
    {
        phaseOffset = Random.Range(0f, Mathf.PI * 2f);
        SetupOutline();
    }

    private void OnEnable()
    {
        SetupOutline();
    }

    private void SetupOutline()
    {
        if (outline != null) return;

        Graphic targetGraphic = GetComponent<Graphic>();

        if (targetGraphic == null)
            targetGraphic = GetComponentInChildren<Graphic>();

        if (targetGraphic == null)
            return;

        outline = targetGraphic.GetComponent<Outline>();

        if (outline == null)
            outline = targetGraphic.gameObject.AddComponent<Outline>();

        outline.useGraphicAlpha = false;
        outline.effectDistance = new Vector2(glowThickness, glowThickness);
    }

    private void Update()
    {
        if (outline == null)
        {
            SetupOutline();
            return;
        }

        // 0 到 1 之间平滑循环
        float t = (Mathf.Sin(Time.unscaledTime * pulseSpeed + phaseOffset) + 1f) * 0.5f;

        // 再平滑一次，让亮暗切换更软
        t = t * t * (3f - 2f * t);

        Color c = glowColor;
        c.a = Mathf.Lerp(minAlpha, maxAlpha, t);

        outline.effectColor = c;

        // 不要每帧改厚度，厚度固定会更顺滑
        outline.effectDistance = new Vector2(glowThickness, glowThickness);
    }

    public void SetStyle(Color newGlowColor, float newGlowThickness)
    {
        glowColor = newGlowColor;
        glowThickness = newGlowThickness;

        if (outline != null)
            outline.effectDistance = new Vector2(glowThickness, glowThickness);
    }
}