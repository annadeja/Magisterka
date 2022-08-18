using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
//!Skrypt bazowy dla kontrolerów obs³uguj¹cych menu.
public class MenuController : MonoBehaviour
{
    [Header("UI elements")] //Elementu UI menu g³ównego.
    [SerializeField] protected GameObject loadGameCanvas;
    [SerializeField] protected List<Button> saveButtons;
    [SerializeField] protected Button leftArrow;
    [SerializeField] protected Button rightArrow;

    protected SaveDataController saveDataController; //!<Kontroler stanu gry.
    protected SaveData currentSave; //!<Obecny zapis gry.
    protected List<string> saveFileNames; //!<Nazwy plików zapisu.
    protected int page = 0; //!<Numer strony zapisów w podmenu ³adowania gry.
    protected bool isSave = false; //!<Czy jest to menu, w którym mo¿na zapisaæ grê?

    void Start()
    {
        saveDataController = SaveDataController.getInstance();
    }
    //!Przechodzi do podmenu ³adowania zapisów gry.
    public virtual void loadGame()
    {
        loadGameCanvas.SetActive(true);
        saveFileNames = Directory.EnumerateFiles(Application.persistentDataPath, "*.save").ToList();
        showSaves();
    }
    //!Wychodzi z gry.
    public void exitGame()
    {
        Application.Quit(1);
    }
    //!Przechodzi na poprzedni¹ stronê.
    public void previousPage()
    {
        page--;
        showSaves();
    }
    //!Przechodzi na nastêpn¹ stronê.
    public void nextPage()
    {
        page++;
        showSaves();
    }
    //!Pokazuje zapisy gry.
    protected void showSaves()
    {
        if (saveFileNames.Count < saveButtons.Count + 1)
        {
            leftArrow.gameObject.SetActive(false);
            rightArrow.gameObject.SetActive(false);
        }
        else if (page <= 0)
        {
            leftArrow.gameObject.SetActive(false);
            rightArrow.gameObject.SetActive(true);
        }
        else if (page >= (saveFileNames.Count - 1) / saveButtons.Count)
        {
            leftArrow.gameObject.SetActive(true);
            rightArrow.gameObject.SetActive(false);
        }
        else
        {
            leftArrow.gameObject.SetActive(true);
            rightArrow.gameObject.SetActive(true);
        }
        int start = 0;
        if (isSave)
        {
            start++;
            saveButtons[0].gameObject.SetActive(true);
            saveButtons[0].GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "New save";
        }
        for (int i = start; i < saveButtons.Count; i++)
        {
            if (page * saveButtons.Count + i - start >= saveFileNames.Count)
                saveButtons[i].gameObject.SetActive(false);
            else
            {
                saveButtons[i].gameObject.SetActive(true);
                string buttonText = saveFileNames[page * saveButtons.Count + i - start].Replace(Application.persistentDataPath + "\\", "");
                buttonText = buttonText.Replace(".save", "");
                saveButtons[i].GetComponentInChildren<TMPro.TextMeshProUGUI>().text = buttonText;
            }
        }
    }
    //!£aduje zapis gry.
    public virtual void loadSave(Button button)
    {
        if(!isSave)
        {
            saveDataController.FilePath = Application.persistentDataPath + "/" + button.GetComponentInChildren<TMPro.TextMeshProUGUI>().text + ".save";
            saveDataController.loadSaveFile();
            saveDataController.load();
        }
    }
    //!Wraca do menu g³ównego z podmenu ³adowania zapisów gry.
    public virtual void backToMainMenu(GameObject currentCanvas)
    {
        currentCanvas.SetActive(false);
    }
}