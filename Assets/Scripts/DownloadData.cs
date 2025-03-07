using UnityEngine;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;

// Only use SFB if NOT running in WebGL
#if !UNITY_WEBGL
using SFB;
#endif

public class DownloadData : MonoBehaviour
{
    private BehaviouralDataIO behaviouralData;
    private string dirToZip;

#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void DownloadZippedData(string fileName, string base64Zip);
#endif

    private void Awake()
    {
        behaviouralData = GetComponent<BehaviouralDataIO>();
    }

    public void DownloadZip()
    {
        dirToZip = behaviouralData.pathToParticipantDir;
        if (!Directory.Exists(dirToZip))
        {
            Debug.LogError("Directory not found: " + dirToZip);
            return;
        }
        string[] allFiles = Directory.GetFiles(dirToZip, "*", SearchOption.AllDirectories);
        if (allFiles.Length == 0)
        {
            Debug.LogWarning("No files in: " + dirToZip);
            return;
        }

        byte[] zippedRaw = MakeZip(dirToZip);
        string folderName = new DirectoryInfo(dirToZip).Name;

#if UNITY_WEBGL && !UNITY_EDITOR
        // WEBGL: encode the ZIP as base64 and call JS to trigger the browser download
        string base64Zip = System.Convert.ToBase64String(zippedRaw);
        DownloadZippedData(folderName + ".zip", base64Zip);
        Debug.Log("Triggered WebGL browser download for: " + folderName + ".zip");
#elif !UNITY_WEBGL
        // DESKTOP: use StandaloneFileBrowser to choose a save path, then write the .zip
        string chosenPath = StandaloneFileBrowser.SaveFilePanel("Save Zipped Data", "", folderName + ".zip", "");
        if (!string.IsNullOrEmpty(chosenPath))
        {
            if (!chosenPath.EndsWith(".zip"))
            {
                chosenPath += ".zip";
            }
            File.WriteAllBytes(chosenPath, zippedRaw);
            Debug.Log("Saved zip to: " + chosenPath);
        }
        else
        {
            Debug.Log("Save canceled or empty path.");
        }
#endif
    }

    private byte[] MakeZip(string pathToDir)
    {
        using (var memoryStream = new MemoryStream())
        {
            using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                string[] files = Directory.GetFiles(pathToDir, "*", SearchOption.AllDirectories);
                foreach (var filePath in files)
                {
                    string relativePath = filePath.Substring(pathToDir.Length)
                        .TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                    var entry = archive.CreateEntry(relativePath);
                    using (var entryStream = entry.Open())
                    {
                        byte[] data = File.ReadAllBytes(filePath);
                        entryStream.Write(data, 0, data.Length);
                    }
                }
            }
            return memoryStream.ToArray();
        }
    }
}
