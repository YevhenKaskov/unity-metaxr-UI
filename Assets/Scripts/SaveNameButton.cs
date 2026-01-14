using System.IO;
using TMPro;
using UnityEngine;

public class SaveNameButton : MonoBehaviour
{
    [Header("UI")]
    public TMP_InputField nameInputField;

    [Header("File Settings")]
    public string fileName = "username.txt";

    public void SaveName()
    {
        // Get text or default
        string nameToSave = nameInputField.text.Trim();
        if (string.IsNullOrEmpty(nameToSave))
            nameToSave = "Unknown human";

        // Project root = parent of Assets
        string projectRootPath = Directory.GetParent(Application.dataPath).FullName;
        string filePath = Path.Combine(projectRootPath, fileName);

        // Write file
        File.WriteAllText(filePath, nameToSave);

        Debug.Log($"Name saved: '{nameToSave}' at:\n{filePath}");
    }
}
