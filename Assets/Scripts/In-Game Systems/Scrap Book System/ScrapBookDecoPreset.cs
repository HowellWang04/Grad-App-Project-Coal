using UnityEngine;

public enum DecoType
{
    Sticker,
    TextBox
}

[CreateAssetMenu(fileName = "NewDecoPreset", menuName = "ScrapBook/Deco Preset")]
public class ScrapBookDecoPreset : ScriptableObject
{
    public string presetName;
    public DecoType decoType;
    public Sprite icon;          // Used for sticker image and toolbar thumbnail
    public Vector2 defaultSize = new Vector2(100, 100);
}
