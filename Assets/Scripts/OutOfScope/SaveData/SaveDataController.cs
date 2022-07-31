using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;
//!Klasa o strukturze pseudo-singletona, która przechowuje i obsługuje dane nt. stanu gry.
public class SaveDataController : MonoBehaviour 
{
    private static SaveDataController instance; //!<Instancja klasy.
    public SaveData LoadedSave { get; set; } //!<Obecnie wczytany zapis gry.
    public string FilePath { get; set; } //!<Ścieżka do zapisu lub wczytania pliku stanu gry.
    private string dialogPosition; //!<Pozycja gracza na drzewie dialogowym.
    private GameObject player; //!<Obiekt gracza.
    private bool justLoaded = false; //!<Określa czy właśnie wczytano nową scenę.

    void Update()
    {
        if (justLoaded) //Jeżeli dopiero co wczytano zapis gry, to skrypt odnajduje obiekt gracza zawarty w scenie i dopasowuje jego pozycję do tej zapisanej.
        {
            player = GameObject.Find("Player");
            if (player)
            {
                Transform transform = player.GetComponentInChildren<Transform>();
                player.SetActive(false);
                //transform.position = playerPosition;
                player.SetActive(true);
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
        //LoadedSave.PlayerPosition[0] = player.transform.position.x;
        //LoadedSave.PlayerPosition[1] = player.transform.position.y;
        //LoadedSave.PlayerPosition[2] = player.transform.position.z;
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
        SceneManager.LoadScene(LoadedSave.LastLocation);
        //playerPosition = new Vector3(LoadedSave.PlayerPosition[0], LoadedSave.PlayerPosition[1], LoadedSave.PlayerPosition[2]);
        justLoaded = true;
    }
    //!Zapisuje pojedynczy wybór gracza.
    public void saveChoice(NodeDataContainer currentNode, int i) 
    {
        string portName = currentNode.OutputPorts[i].PortName;
        ChoiceData choiceData = currentNode.ChoiceOutcomes.Find(x => x.PortName == portName);
        //choiceData.skillCheck(LoadedSave.PlayerStats);
        LoadedSave.PastChoices.Add(choiceData);
        identifyChoicePath(choiceData);
    }
    //!Sprawdza do jakiej ścieżki przynależy dany wybór i inkrementuje odpowiadający mu licznik.
    private void identifyChoicePath(ChoiceData choiceData)
    {
        if (choiceData.Path == NarrativePath.Conform)
            LoadedSave.ConformChoices++;
        else if (choiceData.Path == NarrativePath.Rebel)
            LoadedSave.RebelChoices++;
        else if (choiceData.Path == NarrativePath.Reset)
            LoadedSave.ResetChoices++;
        else if (choiceData.Path == NarrativePath.Preserve)
            LoadedSave.PreserveChoices++;
    }

    //!Wczytuje sekwencję zakończenia gry.
    public void loadEnding()
    {
        SceneManager.LoadScene("Ending");
    }
}
