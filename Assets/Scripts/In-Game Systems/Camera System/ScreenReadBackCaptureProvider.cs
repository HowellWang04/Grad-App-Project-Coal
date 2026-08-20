using System;
using System.Collections;
using UnityEngine;

namespace PhotoSystem.Runtime.Capture
{
    public sealed class ScreenReadbackCaptureProvider : MonoBehaviour
    {
        [Header("Capture Options")]
        [SerializeField] private bool reuseBuffer = true;

        private Texture2D buffer;

        public IEnumerator Capture(Action<Texture2D> onCaptured)
        {
            yield return new WaitForEndOfFrame();

            int w = Screen.width;
            int h = Screen.height;

            if (!reuseBuffer || buffer == null || buffer.width != w || buffer.height != h)
                buffer = new Texture2D(w, h, TextureFormat.RGB24, false);

            var region = new Rect(0, 0, w, h);
            buffer.ReadPixels(region, 0, 0, false);
            buffer.Apply(false);

            
            var copy = new Texture2D(buffer.width, buffer.height, buffer.format, false);
            copy.SetPixels(buffer.GetPixels());
            copy.Apply(false);

            onCaptured?.Invoke(copy);
        }
    }
}
