using UnityEngine;
using UnityEngine.Serialization;

public class EchoTrigger : MonoBehaviour
{
    [TextArea(3, 6)]
    [FormerlySerializedAs("diaryContent")]
    public string scrapbookContent;

    public string stickyNoteSlotId;
}
