using UnityEngine;
public enum HighlightObjectType
{
    Book,
    Helmet,
}

public class CameraHighlightObject : MonoBehaviour
{
    public HighlightObjectType objectType;

    [HideInInspector] public Renderer[] renderers;
    [HideInInspector] public Material[][] originalMaterials;

    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();
        originalMaterials = new Material[renderers.Length][];
        for (int i = 0; i < renderers.Length; i++)
            originalMaterials[i] = renderers[i].materials;
    }
}
