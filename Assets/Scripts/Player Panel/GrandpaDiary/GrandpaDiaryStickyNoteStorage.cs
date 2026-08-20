using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class GrandpaDiaryUnlockedStickyNoteManifest
{
    public List<string> unlockedSlotIds = new();
}

public static class GrandpaDiaryStickyNoteStorage
{
    private static string Dir => Path.Combine(Application.persistentDataPath, "GrandpaDiary");
    private static string ManifestPath => Path.Combine(Dir, "UnlockedStickyNotes.json");

    private static GrandpaDiaryUnlockedStickyNoteManifest LoadManifest()
    {
        if (!File.Exists(ManifestPath))
            return new GrandpaDiaryUnlockedStickyNoteManifest();

        var manifest = JsonUtility.FromJson<GrandpaDiaryUnlockedStickyNoteManifest>(File.ReadAllText(ManifestPath));
        return manifest ?? new GrandpaDiaryUnlockedStickyNoteManifest();
    }

    private static void SaveManifest(GrandpaDiaryUnlockedStickyNoteManifest manifest)
    {
        Directory.CreateDirectory(Dir);
        File.WriteAllText(ManifestPath, JsonUtility.ToJson(manifest, true));
    }

    public static List<string> LoadUnlockedSlotIds()
    {
        return new List<string>(LoadManifest().unlockedSlotIds);
    }

    public static bool IsUnlocked(string slotId)
    {
        if (string.IsNullOrEmpty(slotId))
            return false;

        return LoadManifest().unlockedSlotIds.Contains(slotId);
    }

    public static void Unlock(string slotId)
    {
        if (string.IsNullOrEmpty(slotId))
            return;

        var manifest = LoadManifest();
        if (manifest.unlockedSlotIds.Contains(slotId))
            return;

        manifest.unlockedSlotIds.Add(slotId);
        SaveManifest(manifest);
    }
}
