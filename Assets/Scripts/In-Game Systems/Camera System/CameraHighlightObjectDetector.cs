using System.Collections.Generic;
using UnityEngine;

public class CameraHighlightObjectDetector : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Camera photoCamera;
    [SerializeField] private CameraSystemManager cameraSystemManager;

    [Header("Detection")]
    [SerializeField] private float maxDistance = 50f;
    [SerializeField] private LayerMask occlusionMask = ~0;

    [Header("Highlight Material Ref")]
    [SerializeField] private Material highlightMaterial;

    private readonly HashSet<CameraHighlightObject> highlightedSubjects = new();
    public IReadOnlyCollection<CameraHighlightObject> HighlightedObjects => highlightedSubjects;

    public List<CameraHighlightObject> FilterConfirmedObjects(IEnumerable<CameraHighlightObject> candidates)
    {
        var result = new List<CameraHighlightObject>();
        foreach (var obj in candidates)
        {
            if (IsFullyInViewport(obj) && IsNotOccluded(obj))
                result.Add(obj);
        }
        return result;
    }

    private bool IsFullyInViewport(CameraHighlightObject subject)
    {
        foreach (var renderer in subject.renderers)
        {
            Bounds b = renderer.bounds;
            Vector3[] corners = {
                b.center,
                b.min,
                b.max,
                new Vector3(b.min.x, b.min.y, b.max.z),
                new Vector3(b.min.x, b.max.y, b.min.z),
                new Vector3(b.max.x, b.min.y, b.min.z),
                new Vector3(b.min.x, b.max.y, b.max.z),
                new Vector3(b.max.x, b.min.y, b.max.z),
                new Vector3(b.max.x, b.max.y, b.min.z),
            };

            foreach (var corner in corners)
            {
                Vector3 vp = photoCamera.WorldToViewportPoint(corner);
                if (vp.z < 0 || vp.x < 0 || vp.x > 1 || vp.y < 0 || vp.y > 1)
                    return false;
            }
        }
        return true;
    }

    private bool IsNotOccluded(CameraHighlightObject subject)
    {
        Vector3 camPos = photoCamera.transform.position;
        foreach (var renderer in subject.renderers)
        {
            Vector3 center = renderer.bounds.center;
            Vector3 dir = center - camPos;
            float dist = dir.magnitude;
            if (Physics.Raycast(camPos, dir.normalized, out RaycastHit hit, dist, occlusionMask))
            {
                if (hit.transform.IsChildOf(subject.transform) || hit.transform == subject.transform)
                    continue;
                return false;
            }
        }
        return true;
    }

    private void Update()
    {
        if (cameraSystemManager == null || photoCamera == null) return;

        
        /*
            Only detect and highlight when photo mode is active, otherwise clear all highlights;
            When in Active state, taking a photo will transition the state machine to Capturing state, which is also non-Active, thus also clearing highlights and preventing leftover highlights after photo is taken.
        */
        if (cameraSystemManager.CurrentState == CameraModeState.Active)
            DetectAndHighlight();
        else
            ClearAllHighlights();
    }

    private void DetectAndHighlight()
    {
        var allSubjects = FindObjectsByType<CameraHighlightObject>();
        var visibleThisFrame = new HashSet<CameraHighlightObject>();

        foreach (var subject in allSubjects)
        {
            if (IsVisibleToCamera(subject))
            {
                visibleThisFrame.Add(subject);

                if (!highlightedSubjects.Contains(subject))
                    ApplyHighlight(subject);
            }
        }

        var toRemove = new List<CameraHighlightObject>();
        foreach (var subject in highlightedSubjects)
        {
            if (!visibleThisFrame.Contains(subject))
                toRemove.Add(subject);
        }
        foreach (var subject in toRemove)
            RemoveHighlight(subject);
    }

    private bool IsVisibleToCamera(CameraHighlightObject subject)
    {

        // Use multiple points of the bounding box to check, if any of them is within the viewport, it's considered visible
        foreach (var renderer in subject.renderers)
        {
            if (!renderer.isVisible) continue;

            Bounds bounds = renderer.bounds;

            // Check the 8 corners + center of the bounding box = 9 points
            Vector3[] checkPoints = new Vector3[]
            {
                bounds.center,
                bounds.min,
                bounds.max,
                new Vector3(bounds.min.x, bounds.min.y, bounds.max.z),
                new Vector3(bounds.min.x, bounds.max.y, bounds.min.z),
                new Vector3(bounds.max.x, bounds.min.y, bounds.min.z),
                new Vector3(bounds.min.x, bounds.max.y, bounds.max.z),
                new Vector3(bounds.max.x, bounds.min.y, bounds.max.z),
                new Vector3(bounds.max.x, bounds.max.y, bounds.min.z),
            };

            foreach (var point in checkPoints)
            {
                Vector3 viewportPos = photoCamera.WorldToViewportPoint(point);

                if (viewportPos.z < 0) continue;
                if (viewportPos.x < 0 || viewportPos.x > 1) continue;
                if (viewportPos.y < 0 || viewportPos.y > 1) continue;

                float distance = Vector3.Distance(photoCamera.transform.position, point);
                if (distance > maxDistance) continue;
                return true;
            }
        }

        return false;
    }

    private void ApplyHighlight(CameraHighlightObject subject)
    {
        highlightedSubjects.Add(subject);

        foreach (var renderer in subject.renderers)
        {
            var mats = new List<Material>(renderer.materials);
            mats.Add(highlightMaterial);
            renderer.materials = mats.ToArray();
        }
    }

    private void RemoveHighlight(CameraHighlightObject subject)
    {
        highlightedSubjects.Remove(subject);

        for (int i = 0; i < subject.renderers.Length; i++)
            subject.renderers[i].materials = subject.originalMaterials[i];
    }

    private void ClearAllHighlights()
    {
        foreach (var subject in highlightedSubjects)
        {
            for (int i = 0; i < subject.renderers.Length; i++)
                subject.renderers[i].materials = subject.originalMaterials[i];
        }
        highlightedSubjects.Clear();
    }
}