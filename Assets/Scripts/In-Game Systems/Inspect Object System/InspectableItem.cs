using UnityEngine;

public class InspectableItem : MonoBehaviour
{
    [Header("Info")]
    public string itemName = "Unknown Item";
    [TextArea(2, 4)]
    public string itemDescription = "";

    [Header("Inspect Settings")]
    public float inspectDistance = 0.5f;   // 物品在相机前多远
    public float inspectScale = 1f;        // 展示时的缩放倍数
    public Vector3 inspectRotationOffset;  // 初始旋转偏移
}