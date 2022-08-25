using System.IO;
using System.Diagnostics;
using UnityEngine;
//!Klasa zarz�dzaj�ca komunikacj� z modelem sieci neuronowej.
public class NeuralNetworkController : MonoBehaviour
{
    ProcessStartInfo processInfo; //!<Zmienna przechowuj�ca dane dotycz�ce procesu maj�cego na celu wykonanie skryptu.
    private string predictorPath; //!<Przechowuje �cie�k� do skryptu w j�zyku Python umo�liwiaj�cego przewidzenie nastepnego wyboru za pomoc� modelu.
    private string modelPath; //!<Przechowuje �cie�k� do zapisanego modelu sieci neuronowej.
    private string tokenizerPath; //!<Przechowuje �cie�k� do zapisanego w postaci pliku JSON obiektu klasy Tokenizer, przechowuj�cego s�ownik wszystkich GUID.
    private int lineAmount = 4; //<!Eksperymentalnie wyznaczona warto�� okre�laj�ca ile linii wczyta� z danych wyj�ciowych skryptu by otrzyma� GUID.
    private string[] packages = {"numpy", "tensorflow==2.8.0", "protobuf==3.20.0" };

    // Start is called before the first frame update
    void Start()
    {
        predictorPath = Application.dataPath + "/Resources/NeuralNetworks/predict.py";
        modelPath = Application.dataPath + "/Resources/NeuralNetworks/NeuralNetworkModel.h5";
        tokenizerPath = Application.dataPath + "/Resources/NeuralNetworks/tokenizer.json";
    }
    //!Wywo�uje w terminalu skrypt predict.py w celu przewidzenia nast�pnego wyboru przez sie� neuronow�.
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
    //!Instaluje potrzebne do funkcjonowania gry pakiety Python za pomoc� wywo�ania pip. Funkcja niewykorzystana, gdy� zamiast niej u�yto skryptu batch.
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
    //!Przygotowuje niezb�dne dane do wykonania danej komendy w terminalu.
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
