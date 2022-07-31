using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
//!Klasa zarządzająca zachowaniem postaci niezależnej w ramach dialogu.
public class SpeakerBehavior : MonoBehaviour 
{
    [Header("UI elements")] //Elementy interfejsu graficznego dialogu.
    [SerializeField] private Text popup;
    [SerializeField] private Image dialogBackground;
    [SerializeField] private Text speaker;
    [SerializeField] private Text dialogLine;
    [SerializeField] private Image icon;
    [SerializeField] private float textDelay = 0.1f; //!<Opóźnienie pomiędzy kolejnymi literami w kwestii.
    [SerializeField] private List<Button> choiceButtons;

    [Header("Dialog trees")] 
    [SerializeField] private List<DialogContainer> dialogTrees; //!<Lista przechowująca drzewa dialogowe.
    private DialogContainer currentTree; //!<Obecnie załadowane drzewo.
    private NodeDataContainer currentNode; //!<Obecnie załadowany węzeł.

    [Header("Positions")] //Zmienne pozwalające na ustalenie konkretnej pozycji gracza oraz kamery podczas dialogu.
    [SerializeField] private Vector3 cameraPosition; //!<Pozycja kamery podczas dialogu.
    [SerializeField] private Vector3 playerPosition; //!<Pozycja gracza podczas dialogu.

    //private PlayerMovementController playerControl; //!<Kontroler ruchu gracza.
    private bool isInRange = false; //?<Czy jest w zasiegu postaci niezależnej?
    private bool isDisplayingEnding = false; //!<Czy wyświetlana jest sekwencja zakończenia?
    private UnityEditor.U2D.Animation.CharacterData currentCharacter; //!<Obiekt z informacjami o obecnym mówcy.
    private SaveDataController saveDataController; //!<Kontroler stanu gry.

    void Start()
    {
        saveDataController = SaveDataController.getInstance();
        if (!isDisplayingEnding)
            disableUI();
        if(choiceButtons.Count == 3) //Wykorzystanie pętli do przypisania delegatów okazało sie w praktyce niemożliwe.
        {
            choiceButtons[0].onClick.AddListener(delegate { makeChoice(0); });
            choiceButtons[1].onClick.AddListener(delegate { makeChoice(1); });
            choiceButtons[2].onClick.AddListener(delegate { makeChoice(2); });
        }
    }

    void Update()
    {
        if (Input.GetButtonDown("Submit") || isDisplayingEnding) //Sprawdzanie czy gracz wszczyna dialog.
            startDialog();
    }
    //!Zaczyna dialog tylko wtedy gdy gracz jest w zasiegu postaci niezależnej lub gdy odbywa się sekwencja zakończenia gry.
    public void startDialog()
    {
        if (isInRange || isDisplayingEnding) 
        {
            getNextTree();
            if (currentTree == null) //Przerywa dialog jeżeli obecne drzewo jest puste.
                return;
            enableUI();
            //disablePlayerControls();
            displayNextDialog();
        }
    }
    //!Wyłącza interfejs dialogu.
    private void disableUI() 
    {
        popup.gameObject.SetActive(false);
        foreach(Button button in choiceButtons)
            button.gameObject.SetActive(false);
        dialogBackground.gameObject.SetActive(false);
        icon.gameObject.SetActive(false);
        speaker.gameObject.SetActive(false);
        dialogLine.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    //!Włącza interfejs dialogu.
    private void enableUI() 
    {
        popup.gameObject.SetActive(false);
        dialogBackground.gameObject.SetActive(true);
        icon.gameObject.SetActive(true);
        speaker.gameObject.SetActive(true);
        dialogLine.gameObject.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    ////!Odbiera graczowi kontrolę nad jego postacią oraz przygotowuje do zmiany sterowania odpowiedniej dla interfejsu dialogu.
    //private void disablePlayerControls() 
    //{
    //    if (playerControl == null)
    //        return;
    //    playerControl.animator.Rebind();
    //    playerControl.animator.Update(0f);
    //    playerControl.animator.enabled = false;
    //    playerControl.gameObject.SetActive(false);
    //    playerControl.transform.position = playerPosition;
    //    playerControl.gameObject.SetActive(true);
    //    playerControl.CanMove = false;
    //}
    ////!Przywraca graczowi kontrolę nad bohaterem.
    //private void enablePlayerControls() 
    //{
    //    if (playerControl == null)
    //        return;
    //    playerControl.animator.enabled = true;
    //    playerControl.CanMove = true;
    //}
    ////!Przygotowuje do wyświetlania obecnej kwestii dialogowej.
    private void displayNextDialog() 
    {
        if (currentNode == null)
            return;
        dialogLine.text = "";
        //currentCharacter = Resources.Load<UnityEditor.U2D.Animation.CharacterData>("Dialogs/Character_data/" + currentNode.Speaker);
        //if (currentCharacter)
        //{
        //    speaker.text = currentCharacter.Name;
        //    icon.sprite = currentCharacter.Icon;
        //    dialogLine.color = currentCharacter.TextColor;
        //}
        StartCoroutine("typeText");

        if (currentNode.IsEnding) //Wszczyna sekwencję zakończenia jeżeli dialog ma ustawiona odpowiednią flagę.
            saveDataController.loadEnding();
        else if (currentNode.IsLeaf) //Przygotowuje do zakończenia dialogu.
        {
            choiceButtons[0].gameObject.SetActive(true);
            choiceButtons[0].GetComponentInChildren<Text>().text = currentNode.ExitLine;
            choiceButtons[0].onClick.RemoveAllListeners();
            choiceButtons[0].onClick.AddListener(endConversation);
            return;
        }
        else //Domyślny przypadek, zakłada dalszą kontynuację dialogu.
            for (int i = 0; i < currentNode.OutputPorts.Count; i++)
            {
                choiceButtons[i].gameObject.SetActive(true);
                choiceButtons[i].GetComponentInChildren<Text>().text = currentNode.OutputPorts[i].PortName;
            }
    }
    //!Wywoływana po naciśnięciu przycisku z odpowiedzią. Zapisuje wybór jeżeli takowy istnieje oraz przygotowuje kolejny węzeł dialogu.
    public void makeChoice(int i) 
    {
        if (currentNode.IsChoice)
            saveDataController.saveChoice(currentNode, i);
        currentNode = currentTree.getNode(currentNode.OutputPorts[i].TargetGuid);
        displayNextDialog();
    }
    //!Kończy dialog.
    private void endConversation() 
    {
        if (isDisplayingEnding)
        {
            SceneManager.LoadScene("MainMenu");
            return;
        }
        disableUI();
        //enablePlayerControls();
    }
    //!Przygotowuje kolkejne drzewo dialogowe.
    private void getNextTree() 
    {
        if (dialogTrees.Count != 0)
        {
            currentTree = dialogTrees[0];
            dialogTrees.Remove(currentTree);
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
        isDisplayingEnding = true;
    }
    //!Wyświetla tekst dialogu litera po literze.
    private IEnumerator typeText() 
    {
        foreach (Button button in choiceButtons)
            button.gameObject.SetActive(false);
        foreach (char character in currentNode.DialogLine.ToCharArray())
        {
            if ((Input.GetMouseButton(0) || Input.GetButtonDown("Submit")) && !isDisplayingEnding)
            {
                dialogLine.text = currentNode.DialogLine;
                yield break;
            }
            dialogLine.text += character;
            yield return new WaitForSeconds(textDelay);
        }
    }
    //!Wykrywa gdy gracz jest wystarczająco blisko postaci niezależnej by podjąć dialog.
    private void OnTriggerEnter(Collider other) 
    {
        if (dialogTrees.Count != 0)
            popup.gameObject.SetActive(true);
        isInRange = true;
        //playerControl = other.gameObject.GetComponentInChildren<PlayerMovementController>();
    }
    //!Resetuje odpowiednie flagi gdy gracz się oddala.
    private void OnTriggerExit(Collider other) 
    {
        popup.gameObject.SetActive(false);
        isInRange = false;
    }
}
