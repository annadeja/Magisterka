using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
//!Klasa zarządzająca przebiegiem dialogu.
public class DialogController : MonoBehaviour 
{
    [Header("UI elements")] //Elementy interfejsu graficznego dialogu.
    [SerializeField] private TMPro.TextMeshProUGUI dialogBox;
    [SerializeField] private float textDelay = 0.1f; //!<Opóźnienie pomiędzy kolejnymi literami w kwestii.
    [SerializeField] private List<Button> choiceButtons;
    [SerializeField] private Material defaultMaterial; //!<Materiał określający domyślny wygląd tekstu odpowiedzi.
    [SerializeField] private Material highlightedChoiceMaterial; //!<Materiał określający wygląd tekstu odpowiedzi wybranej przez sieć neuronową.

    [Header("Dialog trees")] 
    [SerializeField] private List<DialogContainer> dialogTrees; //!<Lista przechowująca drzewa dialogowe.
    private DialogContainer currentTree; //!<Obecnie załadowane drzewo.
    private NodeDataContainer currentNode; //!<Obecnie załadowany węzeł.
    private string predictedGuid = ""; //!GUID najnowszego przewidzianego przez AI węzła dialogowego.
    private bool predictedAvailableChoice = false; //!Czy sieć neuronowa przewidziała jeden z dostępnych obecnie dla gracza wyborów?

    private bool justLoaded = true; //!<Czy scena została dopiero co załadowana?
    private bool isDisplayingEnding = false; //!<Czy wyświetlana jest sekwencja zakończenia?
    private SaveDataController saveDataController; //!<Kontroler stanu gry.
    [SerializeField] private NeuralNetworkController neuralNetworkController; //!<Skrypt obsługujący komunikację z siecią neuronową.

    void Start()
    {
        saveDataController = SaveDataController.getInstance();
        if (choiceButtons.Count == 3) //Wykorzystanie pętli do przypisania delegatów okazało sie w praktyce niemożliwe.
        {
            choiceButtons[0].onClick.AddListener(delegate { makeChoice(0); });
            choiceButtons[1].onClick.AddListener(delegate { makeChoice(1); });
            choiceButtons[2].onClick.AddListener(delegate { makeChoice(2); });
        }
    }

    void Update()
    {
        if(justLoaded && isDisplayingEnding) //Sprawdzanie czy wyświetlane jest zakończenie.
        {
            justLoaded = false;
            displayNextDialog();
        }
    }
   
    //!Przygotowuje do wyświetlania obecnej kwestii dialogowej.
    private void displayNextDialog() 
    {
        if (currentNode.IsEnding) //Wszczyna sekwencję zakończenia jeżeli dialog ma ustawioną odpowiednią flagę.
        {
            saveDataController.loadEnding();
            return;
        }
        if (currentNode == null)
            return;
        dialogBox.text = "";
        saveDataController.DialogPosition = currentNode.Guid;
        saveDataController.LoadedSave.NodeSequence+=currentNode.Guid + " ";
        StartCoroutine("typeText");

        if (currentNode.IsLeaf) //Przygotowuje do zakończenia dialogu.
        {
            choiceButtons[0].gameObject.SetActive(true);
            choiceButtons[0].GetComponentInChildren<TMPro.TextMeshProUGUI>().text = currentNode.ExitLine;
            choiceButtons[0].onClick.RemoveAllListeners();
            choiceButtons[0].onClick.AddListener(endConversation);
            return;
        }
        else //Domyślny przypadek, zakłada dalszą kontynuację dialogu.
            prepareChoiceButtons();
    }
    //!Przygotowuje przyciski odpowiedzi w wypadku kontynuowania dialogu.
    private void prepareChoiceButtons()
    {
        predictedGuid = "";
        if (currentNode.OutputPorts.Count > 1 && !isDisplayingEnding) //Wywołanie sieci neuronowej w celu przewidzenia następnego wyboru.
            predictedGuid = neuralNetworkController.predictNextChoice(saveDataController.LoadedSave.NodeSequence);
        predictedAvailableChoice = false;
        for (int i = 0; i < currentNode.OutputPorts.Count; i++)
        {
            choiceButtons[i].gameObject.SetActive(true);
            choiceButtons[i].GetComponentInChildren<TMPro.TextMeshProUGUI>().text = currentNode.OutputPorts[i].PortName;
            if (currentNode.OutputPorts[i].TargetGuid == predictedGuid)
            {
                choiceButtons[i].GetComponentInChildren<TMPro.TextMeshProUGUI>().fontMaterial = highlightedChoiceMaterial;
                predictedAvailableChoice = true;
            }
            else
                choiceButtons[i].GetComponentInChildren<TMPro.TextMeshProUGUI>().fontMaterial = defaultMaterial;
        }
    }
    //!Wywoływana po naciśnięciu przycisku z odpowiedzią. Zapisuje wybór jeżeli takowy istnieje oraz przygotowuje kolejny węzeł dialogu.
    public void makeChoice(int i) 
    {
        bool isEnding = false;
        bool agreedWithNeuralNetwork = false;
        if (predictedGuid == currentNode.OutputPorts[i].TargetGuid)
            agreedWithNeuralNetwork = true;
        if (currentNode.IsChoice)
            isEnding = saveDataController.saveChoice(currentNode, i);
        saveDataController.compareToNeuralNetworkChoice(agreedWithNeuralNetwork, predictedAvailableChoice);
        currentNode = currentTree.getNode(currentNode.OutputPorts[i].TargetGuid);
        currentNode.IsEnding = isEnding;
        displayNextDialog();
    }
    //!Kończy dialog.
    private void endConversation() 
    {
        if (isDisplayingEnding)
        {
            SceneManager.LoadScene("MainMenuScene");
            return;
        }
    }
    //!Ładuje dialog od zapisanej pozycji węzła.
    public void loadNode(string dialogPosition)
    {
        getNextTree();
        if (dialogPosition != "0")
        {
            currentNode = currentTree.getNode(dialogPosition);
        }
        displayNextDialog();
    }
    //!Przygotowuje kolejne drzewo dialogowe.
    private void getNextTree() 
    {
        if (dialogTrees.Count != 0)
        {
            currentTree = dialogTrees[0];
            currentNode = currentTree.getFirstNode();
            choiceButtons[0].onClick.RemoveAllListeners();
            choiceButtons[0].onClick.AddListener(delegate { makeChoice(0); });
        }
        else
        {
            currentTree = null;
            currentNode = null;
        }
    }
    //!Ustala treść zakończenia poprzez wybór odpowiedniego drzewa.
    public void setEndingDialog(DialogContainer endingDialog) 
    {
        dialogTrees.Add(endingDialog);
        getNextTree();
        isDisplayingEnding = true;
    }
    //!Wyświetla tekst dialogu litera po literze.
    private IEnumerator typeText() 
    {
        foreach (Button button in choiceButtons)
            button.gameObject.SetActive(false);
        foreach (char character in currentNode.DialogLine.ToCharArray())
        {
            if ((Input.GetMouseButton(0) || Input.GetButtonDown("Submit")))
            {
                dialogBox.text = currentNode.DialogLine;
                yield break;
            }
            dialogBox.text += character;
            yield return new WaitForSeconds(textDelay);
        }
    }
}
