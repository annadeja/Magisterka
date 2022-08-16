using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;
using UnityEngine;

public class NeuralNetworkController : MonoBehaviour
{
    ProcessStartInfo processInfo;
    string predictorPath;
    string modelPath;
    string tokenizerPath;
    int lineAmount = 4;
    string[] packages = {"numpy", "tensorflow==2.8.0" };

    // Start is called before the first frame update
    void Start()
    {
        //string results = predictNextChoice("6ff9ed28-41ec-4fd2-b342-5b87f1ae0c5b 55c865ee-c650-4a5a-85e0-c64dd8a53247");
        //UnityEngine.Debug.Log(results);
        predictorPath = Application.dataPath + "/Resources/NeuralNetworks/predict.py";
        modelPath = Application.dataPath + "/Resources/NeuralNetworks/NeuralNetworkModel.h5";
        tokenizerPath = Application.dataPath + "/Resources/NeuralNetworks/tokenizer.json";
        //UnityEngine.Debug.Log(Application.dataPath);
        installPackages();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public string predictNextChoice(string choiceInput)
    {
        prepareToExecuteScript(new string[]{ predictorPath, modelPath, tokenizerPath, choiceInput});
        string result = "";

        using (var process = Process.Start(processInfo))
        {
            using (StreamReader reader = process.StandardOutput)
            {
                for (int i = 0; i < lineAmount; i++)
                    result = reader.ReadLine();
            }
        }
        return result;
    }

    private void installPackages()
    {
        foreach (string package in packages)
        {
            prepareToExecuteScript(new string[] { "-m", "pip", "install", package });
            string result = "";

            using (var process = Process.Start(processInfo))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    result = reader.ReadToEnd();
                    //UnityEngine.Debug.Log(result);
                }
            }
        }

    }

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
