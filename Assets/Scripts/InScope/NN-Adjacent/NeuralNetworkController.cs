using System.IO;
using System.Diagnostics;
using UnityEngine;
//!Klasa zarz¹dzaj¹ca komunikacj¹ z modelem sieci neuronowej.
public class NeuralNetworkController : MonoBehaviour
{
    ProcessStartInfo processInfo; //!<Zmienna przechowuj¹ca dane dotycz¹ce procesu maj¹cego na celu wykonanie skryptu.
    private string predictorPath; //!<Przechowuje œcie¿kê do skryptu w jêzyku Python umo¿liwiaj¹cego przewidzenie nastepnego wyboru za pomoc¹ modelu.
    private string modelPath; //!<Przechowuje œcie¿kê do zapisanego modelu sieci neuronowej.
    private string tokenizerPath; //!<Przechowuje œcie¿kê do zapisanego w postaci pliku JSON obiektu klasy Tokenizer, przechowuj¹cego s³ownik wszystkich GUID.
    private int lineAmount = 4; //<!Eksperymentalnie wyznaczona wartoœæ okreœlaj¹ca ile linii wczytaæ z danych wyjœciowych skryptu by otrzymaæ GUID.
    private string[] packages = {"numpy", "tensorflow==2.8.0", "protobuf==3.20.0" };

    // Start is called before the first frame update
    void Start()
    {
        predictorPath = Application.dataPath + "/Resources/NeuralNetworks/predict.py";
        modelPath = Application.dataPath + "/Resources/NeuralNetworks/NeuralNetworkModel.h5";
        tokenizerPath = Application.dataPath + "/Resources/NeuralNetworks/tokenizer.json";
    }
    //!Wywo³uje w terminalu skrypt predict.py w celu przewidzenia nastêpnego wyboru przez sieæ neuronow¹.
    public string predictNextChoice(string choiceInput)
    {
        prepareToExecuteScript(new string[]{ predictorPath, modelPath, tokenizerPath, choiceInput});
        string result = "";

        using (var process = Process.Start(processInfo))
            using (StreamReader reader = process.StandardOutput)
                for (int i = 0; i < lineAmount; i++)
                    result = reader.ReadLine();

        return result;
    }
    //!Instaluje potrzebne do funkcjonowania gry pakiety Python za pomoc¹ wywo³ania pip. Funkcja niewykorzystana, gdy¿ zamiast niej u¿yto skryptu batch.
    private void installPackages()
    {
        foreach (string package in packages)
        {
            prepareToExecuteScript(new string[] { "-m", "pip", "install", package });
            string result = "";

            using (var process = Process.Start(processInfo))
                using (StreamReader reader = process.StandardOutput)
                    result = reader.ReadToEnd();
        }
    }
    //!Przygotowuje niezbêdne dane do wykonania danej komendy w terminalu.
    private void prepareToExecuteScript(string[] arguments)
    {
        processInfo = new ProcessStartInfo();
        processInfo.FileName = "python.exe";

        processInfo.Arguments = $"\"{arguments[0]}\" \"{arguments[1]}\" \"{arguments[2]}\" \"{arguments[3]}\"";

        processInfo.UseShellExecute = false;
        processInfo.CreateNoWindow = true;
        processInfo.RedirectStandardOutput = true;
    }
}
