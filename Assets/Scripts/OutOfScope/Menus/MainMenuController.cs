using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
//!Skrypt obsługujący menu główne.
public class MainMenuController : MenuController
{
    [Header("UI elements")] //Elementy UI menu głównego.
    [SerializeField] private GameObject mainCanvas;
    [SerializeField] private Button backButton;
    [SerializeField] private TMPro.TextMeshProUGUI saveLocation;
    [Header("Miscellaneous")] //Elementy dodatkowe
    [SerializeField] private DialogContainer mainTree;

    void Start()
    {
        saveDataController = SaveDataController.getInstance();
        Directory.CreateDirectory(Application.persistentDataPath + "/EndingSaves");
    }
    //!Tworzy nowy zapis gry.
    public void newGame() 
    {
        currentSave = new SaveData();
        currentSave.DialogPosition = mainTree.FirstNodeGuid;
        string filePath = Application.persistentDataPath + "/" + DateTime.Now.ToString("dd/MM/yyyy hh/mm/ss tt") + ".save";
        saveDataController.FilePath = filePath;
        saveDataController.LoadedSave = currentSave;
        saveDataController.saveToFile();
        saveDataController.load();
        SceneManager.LoadScene("MainScene");
    }
    //!Przechodzi do podmenu ładowania zapisów gry.
    public override void loadGame() 
    {
        isSave = false;
        base.loadGame();
        mainCanvas.SetActive(false);
        backButton.gameObject.SetActive(true);
        saveLocation.text = "Saves located in " + Application.persistentDataPath;
    }
    //!Wraca do menu głównego z podmenu ładowania zapisów gry.
    public override void backToMainMenu(GameObject currentCanvas) 
    {
        base.backToMainMenu(currentCanvas);
        mainCanvas.SetActive(true);
    }

    public override void loadSave(Button button)
    {
        base.loadSave(button);
        SceneManager.LoadScene("MainScene");
    }

}
