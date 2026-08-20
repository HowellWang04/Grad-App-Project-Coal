using UnityEngine;

public class InspectRotator : MonoBehaviour
{
    [Header("Rotation")]
    [SerializeField] private float rotateSpeed = 5f;

    [Header("Zoom")]
    [SerializeField] private float zoomSpeed = 0.1f;
    [SerializeField] private float minDistance = 0.2f;
    [SerializeField] private float maxDistance = 2f;

    private Camera cam;
    private float currentDistance;
    private Vector3 originPosition;
    private bool active = false;

    public void Activate(Camera camera, float startDistance)
    {
        cam = camera;
        currentDistance = startDistance;
        originPosition = transform.position;
        active = true;
    }

    public void Deactivate()
    {
        active = false;
    }

    private void Update()
    {
        if (!active || cam == null) return;

        HandleRotation();
        HandleZoom();
    }

    private void HandleRotation()
    {
        if (!Input.GetMouseButton(0)) return;

        float mouseX = Input.GetAxis("Mouse X") * rotateSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * rotateSpeed;

        transform.Rotate(cam.transform.up, -mouseX, Space.World);
        transform.Rotate(cam.transform.right, mouseY, Space.World);
    }

    private void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) < 0.001f) return;

        currentDistance -= scroll * zoomSpeed;
        currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);

        Vector3 newPos = cam.transform.position + cam.transform.forward * currentDistance;
        transform.position = newPos;
    }
}