using UnityEngine;
using System.IO;
using System.Text;
//!Klasa odpowiedzialna za ekstrakcjê danych z zapisów gry na potrzebê trenowania sieci neuronowej oraz analizy statystycznej.
public class SaveDataExtractor : MonoBehaviour
{
    SaveDataController saveDataController; //!<Kontroler stanu gry.
    // Start is called before the first frame update
    void Start()
    {
        saveDataController = SaveDataController.getInstance();
    }
    //!Wydobywa dane niezbêdne do trenowania sieci neuronowej.
    public void extractNeuralNetworkData()
    {
        StringBuilder stringBuilderTraining = new StringBuilder();
        StringBuilder stringBuilderAnalysis = new StringBuilder();
        stringBuilderAnalysis.AppendLine("Reset;Preserve;Conform;Rebel");
        foreach (string filePath in Directory.GetFiles(Application.persistentDataPath + "/EndingSaves/", "*ENDING.save"))
        {
            saveDataController.FilePath = filePath;
            saveDataController.loadSaveFile();
            stringBuilderTraining.AppendLine(saveDataController.LoadedSave.NodeSequence);
            stringBuilderAnalysis.AppendLine(string.Format("{0};{1};{2};{3}", saveDataController.LoadedSave.ResetChoices,
                saveDataController.LoadedSave.PreserveChoices, saveDataController.LoadedSave.ConformChoices, saveDataController.LoadedSave.RebelChoices));
        }
        File.WriteAllText("F:\\Magisterka\\Magisterka\\NeuralNetwork\\SaveData.csv", stringBuilderTraining.ToString());
        File.WriteAllText("F:\\Magisterka\\Magisterka\\NeuralNetwork\\AnalysisData.csv", stringBuilderAnalysis.ToString());
    }
}
