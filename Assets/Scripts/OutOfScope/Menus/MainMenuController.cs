using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
//!Skrypt obsługujący menu główne.
public class MainMenuController : MenuController
{
    [Header("UI elements")] //Elementu UI menu głównego.
    [SerializeField] private GameObject mainCanvas;
    [SerializeField] private Button backButton;

    void Start()
    {
        saveDataController = SaveDataController.getInstance();
    }
    //!Tworzy nowy zapis gry.
    public void newGame() 
    {
        currentSave = new SaveData();
        string filePath = Application.persistentDataPath + "/" + " " + DateTime.Now.ToString("dd/MM/yyyy hh/mm/ss tt") + ".save";
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
