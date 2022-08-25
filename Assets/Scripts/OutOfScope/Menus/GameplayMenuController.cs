using System;
using UnityEngine;
using UnityEngine.UI;
//!Skrypt obs�uguj�cy menu dost�pne podczas rozgrywki.
public class GameplayMenuController : MenuController
{
    void Start()
    {
        saveDataController = SaveDataController.getInstance();
    }
    //!Zapisuje gr�.
    public void save(Button button)
    {
        if (!isSave)
            return;
        if (button.GetComponentInChildren<TMPro.TextMeshProUGUI>().text.Equals("New save")) //Sprawdza czy gracz tworzy nowy zapis gry.
        {
            saveDataController.FilePath = Application.persistentDataPath + "/" + DateTime.Now.ToString("dd.MM.yyyy hh.mm.ss tt") + ".save";
        }
        else
            saveDataController.FilePath = Application.persistentDataPath + "/" + button.GetComponentInChildren<TMPro.TextMeshProUGUI>().text + ".save";

        saveDataController.updateSaveData();
        saveDataController.saveToFile();
        backToMainMenu(loadGameCanvas);
    }
    //!W��cza lub wy��cza menu zapisu gry.
    public void saveGame()
    {
        if (loadGameCanvas.activeSelf && isSave)
        {
            isSave = false;
            backToMainMenu(loadGameCanvas);
        }
        else
        {
            isSave = true;
            base.loadGame();
        }
    }
    //!Przechodzi do podmenu �adowania zapis�w gry.
    public override void loadGame()
    {
        if (loadGameCanvas.activeSelf && !isSave)
            backToMainMenu(loadGameCanvas);
        else
        {
            isSave = false;
            base.loadGame();
        }
    }
    //!Wraca do menu g��wnego z podmenu �adowania zapis�w gry.
    public override void backToMainMenu(GameObject currentCanvas)
    {
        base.backToMainMenu(currentCanvas);
    }
    //!�aduje zapis gry.
    public override void loadSave(Button button)
    {
        base.loadSave(button);
        backToMainMenu(loadGameCanvas);
    }
}
