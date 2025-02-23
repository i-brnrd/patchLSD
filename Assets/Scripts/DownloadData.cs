using UnityEngine;
using System.IO;
using System.Runtime.InteropServices;
using System.IO.Compression;

public class DownloadData : MonoBehaviour
{
    private GameManager gameManager;

    private void Awake()
    {
        gameManager = GetComponent<GameManager>();
    }

    // 1. Declare the external JS function from our .jslib
    [DllImport("__Internal")]
    private static extern void DownloadFile(string fullPath, string fileContent);
    [DllImport("__Internal")]
    private static extern void DownloadZippedData(string fileName, string base64Zip);


    public void DownloadMyTextFile()
    {
        // 
        string fullPath= gameManager.pathToBehaviouralFolder + "/Choice.txt";
     

        // 3. Read it into a string
        if (File.Exists(fullPath))
        {
            string fileContent = File.ReadAllText(fullPath);

#if UNITY_WEBGL && !UNITY_EDITOR
            // 4. Only call the JS function in a real WebGL build (not in the Editor).
            DownloadFile(fullPath, fileContent);
#else
            // For debugging in the Editor, just show the content
            Debug.Log($"[EditorMode] Would download {fullPath}:\n{fileContent}");
#endif
        }
        else
        {
            Debug.LogError("File not found: " + fullPath);
        }
    }


    public void DownloadAllAsZip()
    {
        // 2. List all files in persistentDataPath
        string folder = gameManager.pathToBehaviouralFolder;
        if (!Directory.Exists(folder))
        {
            Debug.LogError("No directory found at: " + folder);
            return;
        }

        string[] allFiles = Directory.GetFiles(folder);
        if (allFiles.Length == 0)
        {
            Debug.LogWarning("No files found in: " + folder);
            return;
        }

        // 3. Create an in-memory ZIP
        byte[] zipBytes = CreateZipInMemory(allFiles);

        // 4. Convert to base64 so we can pass it to JS easily
        string base64Zip = System.Convert.ToBase64String(zipBytes);

#if UNITY_WEBGL && !UNITY_EDITOR
        // 5. Call the JS plugin function to trigger download
        DownloadZippedData("myData.zip", base64Zip);
#else
        // In Editor, just show a log
        Debug.Log("[Editor] Would download myData.zip, length: " + zipBytes.Length);
#endif
    }

    private byte[] CreateZipInMemory(string[] filePaths)
    {
        using (var memoryStream = new MemoryStream())
        {
            using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                foreach (string filePath in filePaths)
                {
                    // For each file, add an entry in the ZIP
                    string entryName = Path.GetFileName(filePath); // name inside the zip
                    var entry = archive.CreateEntry(entryName);

                    // Write the file contents into the zip entry
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


