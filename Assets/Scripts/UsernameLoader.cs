using System.IO;
using TMPro;
using UnityEngine;

public class UsernameLoader : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text usernameInfoText;

    [Header("Text Settings")]
    public string fileName = "username.txt";
    public string defaultName = "Unknown human";

    private string filePath;

    void OnEnable()
    {
        string projectRootPath = Directory.GetParent(Application.dataPath).FullName;
        filePath = Path.Combine(projectRootPath, fileName);
        Debug.Log(filePath);
        LoadAndApplyUsername();
    }

    void LoadAndApplyUsername()
    {
        string username = defaultName;

        if (File.Exists(filePath))
        {
            string fileContent = File.ReadAllText(filePath).Trim();
            if (!string.IsNullOrEmpty(fileContent))
                username = fileContent;
        }

        usernameInfoText.text = username;
    }
}
