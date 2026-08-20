using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Serialization;

// Summary:
// Maps a saved photo's fileName to the Echo scrapbook texts it captured.
// One JSON file under persistentDataPath, mirroring CameraPhotoStorage.

[Serializable]
public class EchoPhotoEntry
{
    public string fileName;
    [FormerlySerializedAs("diaryContents")]
    public List<string> scrapbookContents = new();
}

[Serializable]
public class EchoPhotoManifest
{
    public List<EchoPhotoEntry> entries = new();
}

public static class EchoPhotoStorage
{
    private static string Dir => Path.Combine(Application.persistentDataPath, "Echo");
    private static string ManifestPath => Path.Combine(Dir, "EchoPhotos.json");

    private static EchoPhotoManifest LoadManifest()
    {
        if (!File.Exists(ManifestPath)) return new EchoPhotoManifest();

        var m = JsonUtility.FromJson<EchoPhotoManifest>(File.ReadAllText(ManifestPath));
        return m ?? new EchoPhotoManifest();
    }

    private static void SaveManifest(EchoPhotoManifest manifest)
    {
        Directory.CreateDirectory(Dir);
        File.WriteAllText(ManifestPath, JsonUtility.ToJson(manifest, true));
    }

    // Record the Echo scrapbook texts a photo captured.
    public static void Save(string fileName, List<string> scrapbookContents)
    {
        if (string.IsNullOrEmpty(fileName) || scrapbookContents == null || scrapbookContents.Count == 0) return;

        var manifest = LoadManifest();

        var entry = manifest.entries.Find(e => e.fileName == fileName);
        if (entry == null)
        {
            entry = new EchoPhotoEntry { fileName = fileName };
            manifest.entries.Add(entry);
        }
        entry.scrapbookContents = new List<string>(scrapbookContents);

        SaveManifest(manifest);
    }

    // Get the Echo scrapbook texts for a photo (empty list if none).
    public static List<string> Load(string fileName)
    {
        if (string.IsNullOrEmpty(fileName)) return new List<string>();

        var entry = LoadManifest().entries.Find(e => e.fileName == fileName);
        return entry != null ? entry.scrapbookContents : new List<string>();
    }
}
