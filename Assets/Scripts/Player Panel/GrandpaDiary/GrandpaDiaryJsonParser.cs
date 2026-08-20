using System;
using UnityEngine;

public static class GrandpaDiaryJsonParser
{
    [Serializable]
    public class GrandpaDiaryJsonRoot
    {
        public GrandpaDiaryPageData[] pages;
    }

    public static GrandpaDiaryPageData[] Parse(string json)
    {
        if (string.IsNullOrEmpty(json))
            return new GrandpaDiaryPageData[0];

        try
        {
            var root = JsonUtility.FromJson<GrandpaDiaryJsonRoot>(json);
            if (root == null || root.pages == null)
            {
                Debug.LogWarning("[GrandpaDiary] Diary JSON has no pages array.");
                return new GrandpaDiaryPageData[0];
            }

            Array.Sort(root.pages, (a, b) => a.pageIndex.CompareTo(b.pageIndex));
            return root.pages;
        }
        catch (Exception exception)
        {
            Debug.LogWarning($"[GrandpaDiary] Failed to parse diary JSON: {exception.Message}");
            return new GrandpaDiaryPageData[0];
        }
    }
}
