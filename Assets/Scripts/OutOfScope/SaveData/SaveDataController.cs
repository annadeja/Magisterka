using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;
//!Klasa o strukturze pseudo-singletona, która przechowuje i obsługuje dane nt. stanu gry.
public class SaveDataController : MonoBehaviour 
{
    private static SaveDataController instance; //!<Instancja klasy.
    public SaveData LoadedSave { get; set; } //!<Obecnie wczytany zapis gry.
    public string FilePath { get; set; } //!<Ścieżka do zapisu lub wczytania pliku stanu gry.
    public string DialogPosition { get; set; } //!<Pozycja gracza na drzewie dialogowym.
    private GameObject dialog; //!<Obiekt gracza.
    private bool justLoaded = false; //!<Określa czy właśnie wczytano nową scenę.

    void Update()
    {
        if (justLoaded) //Jeżeli dopiero co wczytano zapis gry, to skrypt odnajduje obiekt gracza zawarty w scenie i dopasowuje jego pozycję do tej zapisanej.
        {
            dialog = GameObject.Find("DialogController");
            if (dialog)
            {
                DialogController dialogController = dialog.GetComponentInChildren<DialogController>();
                dialogController.loadNode(LoadedSave.DialogPosition);
                justLoaded = false;
            }
        }
    }
    //!Tworzy instancję przy uruchomieniu gry.
    void Awake() 
    {
        if (instance == null)
            instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
    //!Zwraca instację.
    public static SaveDataController getInstance() 
    {
        return instance;
    }
    //!Aktualizuje dane zapisu gry.
    public void updateSaveData() 
    {
        LoadedSave.LastLocation = SceneManager.GetActiveScene().name;
        LoadedSave.DialogPosition = DialogPosition;
    }
    //!Wczytuje zapis gry z pliku.
    public void loadSaveFile() 
    {
        FileStream saveFile;
        if (File.Exists(FilePath))
            saveFile = File.OpenRead(FilePath);
        else
            return;
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        LoadedSave = (SaveData)binaryFormatter.Deserialize(saveFile);
        saveFile.Close();
    }
    //!Zapisuje stan gry do pliku.
    public void saveToFile() 
    {
        FileStream saveFile;
        saveFile = File.OpenWrite(FilePath);
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        binaryFormatter.Serialize(saveFile, LoadedSave);
        saveFile.Close();
    }
    //!Wczytuje zapis gry.
    public void load() 
    {
        DialogPosition = LoadedSave.DialogPosition;
        justLoaded = true;
    }
    //!Sprawdza czy wybór gracza pokrywa się z wyborem przewidzianym przez sieć neuronową i przyznaje graczowi odpowiednie punkty.
    public void compareToNeuralNetworkChoice(bool agreedWithNeuralNetwork, bool predictedAvailableChoice)
    {
        if (!predictedAvailableChoice)
            return;
        if (agreedWithNeuralNetwork)
            LoadedSave.ConformChoices++;
        else
            LoadedSave.RebelChoices++;
    }
    //!Zapisuje pojedynczy wybór gracza.
    public bool saveChoice(NodeDataContainer currentNode, int i) 
    {
        string portName = currentNode.OutputPorts[i].PortName;
        ChoiceData choiceData = currentNode.ChoiceOutcomes.Find(x => x.PortName == portName);
        skillCheck(choiceData);
        if (!choiceData.WasFailed && choiceData.ChoiceTitle == "Ending")
            return true;
        foreach (ChoiceData choice in LoadedSave.PastChoices)
            if (choiceData.ChoiceTitle == choice.ChoiceTitle)
                return false;
        LoadedSave.PastChoices.Add(choiceData);
        identifyChoicePath(choiceData);
        return false;
    }
    //!Sprawdza do jakiej ścieżki przynależy dany wybór i inkrementuje odpowiadający mu licznik.
    private void identifyChoicePath(ChoiceData choiceData)
    {
        if (choiceData.Path == NarrativePath.Conform)
            LoadedSave.ConformChoices+= choiceData.ChoicePoints;
        else if (choiceData.Path == NarrativePath.Rebel)
            LoadedSave.RebelChoices+= choiceData.ChoicePoints;
        else if (choiceData.Path == NarrativePath.Reset)
            LoadedSave.ResetChoices+= choiceData.ChoicePoints;
        else if (choiceData.Path == NarrativePath.Preserve)
            LoadedSave.PreserveChoices+= choiceData.ChoicePoints;
    }
    //!Sprawdza czy gracz posiada wystarczająco wysoki poziom umiejętności by dokonać danego wyboru.
    public void skillCheck(ChoiceData choiceData)
    {
        if ((LoadedSave.ConformChoices >= choiceData.RequiredConform || LoadedSave.RebelChoices >= choiceData.RequiredRebel)
            && (LoadedSave.ResetChoices >= choiceData.RequiredReset || LoadedSave.PreserveChoices >= choiceData.RequiredPreserve))
            choiceData.WasFailed = false;
        else
            choiceData.WasFailed = true;
    }
    //!Wczytuje sekwencję zakończenia gry.
    public void loadEnding()
    {
        updateSaveData();
        FilePath = Application.persistentDataPath + "/EndingSaves/" + DateTime.Now.ToString("dd/MM/yyyy hh/mm/ss tt") + " ENDING.save";
        saveToFile();
        SceneManager.LoadScene("EndingScene");
    }
}
