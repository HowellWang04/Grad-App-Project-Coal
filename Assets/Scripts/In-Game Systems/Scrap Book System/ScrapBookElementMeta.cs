// Summary: 
// Attached to every element placed on the ScrapBookCanvas.
// Stores type and source id for save/load.
public class ScrapBookElementMeta : UnityEngine.MonoBehaviour
{
    public string type; // "photo" / "sticker" / "textbox"
    public string id;   // photo: fileName, sticker/textbox: DecoPreset.presetName
}
