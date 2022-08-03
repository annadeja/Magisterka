using System;
using UnityEngine;
using UnityEngine.UI;

public class GameplayMenuController : MenuController
{
    void Start()
    {
        saveDataController = SaveDataController.getInstance();
    }
    //!Zapisuje grê.
    public void save(Button button)
    {
        if (!isSave)
            return;
        if (button.GetComponentInChildren<TMPro.TextMeshProUGUI>().text.Equals("New save"))
        {
            saveDataController.FilePath = Application.persistentDataPath + "/" + " " + DateTime.Now.ToString("dd/MM/yyyy hh/mm/ss tt") + ".save";
        }
        else
            saveDataController.FilePath = Application.persistentDataPath + "/" + button.GetComponentInChildren<TMPro.TextMeshProUGUI>().text + ".save";

        saveDataController.updateSaveData();
        saveDataController.saveToFile();
        backToMainMenu(loadGameCanvas);
    }
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
    //!Przechodzi do podmenu ³adowania zapisów gry.
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
    //!Wraca do menu g³ównego z podmenu ³adowania zapisów gry.
    public override void backToMainMenu(GameObject currentCanvas)
    {
        base.backToMainMenu(currentCanvas);
    }

    public override void loadSave(Button button)
    {
        base.loadSave(button);
        backToMainMenu(loadGameCanvas);
    }
}
