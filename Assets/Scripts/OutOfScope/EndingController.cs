using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
//!Skrypt kontrolujący sekwencję zakończenia.
public class EndingController: MenuController
{
    [SerializeField] private DialogController endingSpeaker; //!<Obiekt odpowiedzialny za prezentację treści zakończenia.
    [SerializeField] private DialogContainer conformResetDialog; //!<Treść zakończenia nr 1.
    [SerializeField] private DialogContainer rebelResetDialog; //!<Treść zakończenia nr 2.
    [SerializeField] private DialogContainer conformPreserveDialog; //!<Treść zakończenia nr 3.
    [SerializeField] private DialogContainer rebelPreserveDialog; //!<Treść zakończenia nr 4.

    //private SaveDataController saveDataController;
    private bool conformResetEnding = false; //!<Flaga wskazująca zaprezentowanie zakończenia nr 1.
    private bool rebelResetEnding = false; //!<Flaga wskazująca zaprezentowanie zakończenia nr 2.
    private bool conformPreserveEnding = false; //!<Flaga wskazująca zaprezentowanie zakończenia nr 3.
    private bool rebelPreserveEnding = false; //!<Flaga wskazująca zaprezentowanie zakończenia nr 4.
    private bool justLoaded = true;

    void Start()
    {
        saveDataController = SaveDataController.getInstance(); //Pobiera dane stanu gry.
        determineEndingType(saveDataController.LoadedSave);
        displayEnding();
    }
    void Update()
    {
        if (justLoaded)
        {
            justLoaded = false;
            displayEnding();
        }
    }
    //!Sprawdza jakie wybory były wykonywane najczęściej i dobiera odpowiednie zakonczenie.
    public void determineEndingType(SaveData loadedSave)
    {
        if (loadedSave.ConformChoices > loadedSave.RebelChoices && loadedSave.ResetChoices > loadedSave.PreserveChoices)
            conformResetEnding = true;
        else if (loadedSave.RebelChoices > loadedSave.ConformChoices && loadedSave.ResetChoices > loadedSave.PreserveChoices)
            rebelResetEnding = true;
        else if (loadedSave.ConformChoices > loadedSave.RebelChoices && loadedSave.PreserveChoices > loadedSave.ResetChoices)
            conformPreserveEnding = true;
        else
            rebelPreserveEnding = true; //Domyślne zakończenie, nawet gdy nie da się wyłonić dominującego typu wyborów.
    }
    //!Wyświetla zakończenie.
    public void displayEnding() 
    {
        if (conformResetEnding)
            endingSpeaker.setEndingDialog(conformResetDialog);
        else if (rebelResetEnding)
            endingSpeaker.setEndingDialog(rebelResetDialog);
        else if (conformPreserveEnding)
            endingSpeaker.setEndingDialog(conformPreserveDialog);
        else
            endingSpeaker.setEndingDialog(rebelPreserveDialog);
    }

    public override void backToMainMenu(GameObject currentCanvas)
    {
        SceneManager.LoadScene("MainMenuScene");
    }
}
