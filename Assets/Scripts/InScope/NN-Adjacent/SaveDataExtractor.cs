using UnityEngine;
using System.IO;
using System.Text;
//!Klasa odpowiedzialna za ekstrakcj� danych z zapis�w gry na potrzeb� trenowania sieci neuronowej oraz analizy statystycznej.
public class SaveDataExtractor : MonoBehaviour
{
    SaveDataController saveDataController; //!<Kontroler stanu gry.
    // Start is called before the first frame update
    void Start()
    {
        saveDataController = SaveDataController.getInstance();
    }
    //!Wydobywa dane niezb�dne do trenowania sieci neuronowej.
    public void extractNeuralNetworkData()
    {
        StringBuilder stringBuilder = new StringBuilder();
        foreach(string filePath in Directory.GetFiles(Application.persistentDataPath + "/EndingSaves/", "*ENDING.save"))
        {
            saveDataController.FilePath = filePath;
            saveDataController.loadSaveFile();
            stringBuilder.AppendLine(saveDataController.LoadedSave.NodeSequence);
        }
        File.WriteAllText("F:\\Magisterka\\Magisterka\\NeuralNetwork\\SaveData.csv", stringBuilder.ToString());
    }
}
