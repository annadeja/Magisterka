using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

public class SaveDataExtractor : MonoBehaviour
{
    SaveDataController saveDataController;
    // Start is called before the first frame update
    void Start()
    {
        saveDataController = SaveDataController.getInstance();
    }

    public void extractSaveData()
    {
        StringBuilder stringBuilder = new StringBuilder();
        string line;
        foreach(string filePath in Directory.GetFiles(@Application.persistentDataPath, "*ENDING.save"))
        {
            saveDataController.FilePath = filePath;
            saveDataController.loadSaveFile();
            line = string.Join(", ", saveDataController.LoadedSave.NodeSequence);
            stringBuilder.AppendLine(line);
        }
        File.WriteAllText("F:\\Magisterka\\SaveData.csv", stringBuilder.ToString());
    }
}
