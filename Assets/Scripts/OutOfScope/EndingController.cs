using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
//!Skrypt kontrolujący sekwencję zakończenia.
public class EndingController: MonoBehaviour
{
    [SerializeField] private GameObject revolutionCanvas; //!<Tło zakonczenia nr 1.
    [SerializeField] private GameObject reformCanvas; //!<Tło zakonczenia nr 2.
    [SerializeField] private GameObject conquestCanvas; //!<Tło zakonczenia nr 3.

    [SerializeField] private GameObject endingSpeaker; //!<Obiekt odpowiedzialny za prezentację treści zakończenia.
    [SerializeField] private DialogContainer revolutionDialog; //!<Treść zakończenia nr 1.
    [SerializeField] private DialogContainer reformDialog; //!<Treść zakończenia nr 2.
    [SerializeField] private DialogContainer conquestDialog; //!<Treść zakończenia nr 3.

    private SaveDataController saveDataController;
    private bool revolutionEnding = false; //!<Flaga wskazująca zaprezentowanie zakończenia nr 1.
    private bool reformEnding = false; //!<Flaga wskazująca zaprezentowanie zakończenia nr 2.
    private bool conquestEnding = false; //!<Flaga wskazująca zaprezentowanie zakończenia nr 3.

    void Start()
    {
        saveDataController = SaveDataController.getInstance(); //Pobiera dane stanu gry.
        //determineEndingType(saveDataController.LoadedSave);
        displayEnding();
    }
    //!Sprawdza jakie wybory były wykonywane najczęściej i dobiera odpowiednie zakonczenie.
    //public void determineEndingType(SaveData loadedSave) 
    //{
    //    if (loadedSave.RevolutionChoices > loadedSave.ReformChoices && loadedSave.RevolutionChoices > loadedSave.ConquestChoices)
    //        revolutionEnding = true;
    //    else if (loadedSave.ReformChoices > loadedSave.RevolutionChoices && loadedSave.ReformChoices > loadedSave.ConquestChoices)
    //        reformEnding = true;
    //    else
    //        conquestEnding = true; //Domyślne zakończenie, nawet gdy nie da się wyłonić dominującego typu wyborów.
    //}
    //!Wyświetla zakończenie.
    public void displayEnding() 
    {
        DialogController endingBehavior = endingSpeaker.GetComponent<DialogController>();
        if (revolutionEnding)
        {
            revolutionCanvas.SetActive(true);
            endingBehavior.setEndingDialog(revolutionDialog);
        }
        else if (reformEnding)
        {
            reformCanvas.SetActive(true);
            endingBehavior.setEndingDialog(reformDialog);
        }
        else
        {
            conquestCanvas.SetActive(true);
            endingBehavior.setEndingDialog(conquestDialog);
        }
    }

}
