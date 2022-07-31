#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
//!Klasa reprezentująca węzeł w edytorze grafów.
public class DialogNode: Node 
{
    public string Guid; //!<ID węzła.
    public string DialogLine; //!<Kwestia dialogowa.
    public string Speaker; //!<Mówca.
    public string ExitLine; //!<Kwestia na zakończenie dialogu.
    public bool IsRoot; //!<Czy jest korzeniem?
    public bool IsChoice; //!<Czy jest zapisywalnym wyborem?
    public bool IsLeaf; //!<Czy jest liściem/ostatnim węzłem w drzewie?
    public bool IsEnding; //!<Czy prowadzi do zakończenia?
    public List<ChoiceData> ChoiceOutcomes; //!<Dane dotyczące wyborów zapisywalnych.
    private int noOfPorts = 0; //!<Liczba portów.

    private Button addOutput; //Elementy interfejsu graficznego węzła.
    private Foldout foldout;

    private TextField SpeakerLabel;
    private TextField lineLabel;

    private TextField choiceName;
    private TextField conformField;
    private TextField rebelField;
    private TextField resetField;
    private TextField preserveField;
    private TextField ExitLineLabel;

    private Toggle isLeafToggle;
    private Toggle isEndingToggle;
    private Toggle toggleChoice;
    private Toggle makesChoice;
    private List<Toggle> narrativeTypeToggles;

    public DialogNode()
    {
        Guid = System.Guid.NewGuid().ToString();
        title = "New node";
        Speaker = "DUMMY";
        DialogLine = "SAMPLE TEXT.";
        ExitLine = "";
        IsRoot = false;
        IsChoice = false;
        IsLeaf = true;
        IsEnding = false;
        ChoiceOutcomes = new List<ChoiceData>();
        controlsSetup();
    }

    public DialogNode(bool isRoot, string speaker = "DUMMY", string dialogLine = "SAMPLE TEXT.")
    {
        Guid = System.Guid.NewGuid().ToString();
        title = Speaker;
        DialogLine = dialogLine;
        Speaker = speaker;
        IsRoot = isRoot;
        ExitLine = "";
        IsChoice = false;
        IsLeaf = true;
        IsEnding = false;
        ChoiceOutcomes = new List<ChoiceData>();

        if (IsRoot)
            createPort("root", Direction.Output);
        else
            controlsSetup();
    }

    public DialogNode(NodeDataContainer data)
    {
        Guid = data.Guid;
        title = data.Speaker;
        DialogLine = data.DialogLine;
        Speaker = data.Speaker;
        IsChoice = data.IsChoice;
        IsLeaf = true;
        IsEnding = data.IsEnding;
        ExitLine = data.ExitLine;
        if (data.ChoiceOutcomes != null)
            ChoiceOutcomes = new List<ChoiceData>(data.ChoiceOutcomes);
        else
            ChoiceOutcomes = new List<ChoiceData>();
        controlsSetup();
    }
    //!Tworzy nowy port.
    public void createPort(string name, Direction direction, Port.Capacity capacity = Port.Capacity.Single) 
    {
        if (noOfPorts > 3) //Limit odpowiedzi do trzech.
            return;
        Port port = InstantiatePort(Orientation.Horizontal, direction, capacity, typeof(bool));
        port.portName = name;
        if (direction == Direction.Input)
                inputContainer.Add(port);
        else
        {
            if(!IsRoot)
            {
                portControlsSetup(port);
                IsLeaf = false;
                isLeafToggle.value = false;
                //isLeafToggle.SetEnabled(false);
                IsEnding = false;
                isEndingToggle.value = false;
                isEndingToggle.SetEnabled(false);
                ExitLineLabel.SetEnabled(false);
            }
            outputContainer.Add(port);
        }
        refreshNode();
    }
    //!Definiuje UI portu.
    private void portControlsSetup(Port port) 
    {
        ChoiceData choiceData = ChoiceOutcomes.Find(x => x.PortName == port.portName);
        if (choiceData == null)
        {
            choiceData = new ChoiceData(port.portName);
            ChoiceOutcomes.Add(choiceData);
        }

        Button removeBtn = new Button(delegate { removePort(port); });
        removeBtn.text = "x";
        Button editBtn = new Button(delegate { editResponse(choiceData); });
        editBtn.text = "Edit";

        TextField portNameField = new TextField();
        portNameField.value = port.portName;
        portNameField.RegisterCallback<InputEvent, Port>(setPortName, port);

        port.contentContainer.Add(portNameField);
        port.contentContainer.Add(editBtn);
        port.contentContainer.Add(removeBtn);
    }
    //!Usuwa port.
    public void removePort(Port port) 
    {
        noOfPorts--;
        if (noOfPorts <= 0)
        {
            noOfPorts = 0;
            IsLeaf = true;
            isLeafToggle.value = true;
            isEndingToggle.SetEnabled(true);
            ExitLineLabel.SetEnabled(true);
        }

        foreach(Edge edge in port.connections)
        {
            edge.input.Disconnect(edge);
            edge.RemoveFromHierarchy();
        }
        outputContainer.Remove(port);
        ChoiceData choiceData = ChoiceOutcomes.Find(x => x.PortName == port.portName); //Nie można do fukcji przekazać po prostu choiceData, gdyż wywoływana jest także poza tą klasą.
        ChoiceOutcomes.Remove(choiceData);
        refreshNode();
    }
    //!Umożliwia edycję właściwości portu/odpowiedzi.
    private void editResponse(ChoiceData choiceData) 
    {
        foldout.SetEnabled(true);
        foldout.value = true;
        choiceName.value = choiceData.ChoiceTitle;
        choiceName.RegisterCallback<InputEvent, ChoiceData>(setChoiceName, choiceData);
        makesChoice.value = choiceData.WasMade;
        makesChoice.RegisterCallback<MouseUpEvent, ChoiceData>(setChoiceOutcome, choiceData);

        foreach(Toggle narrativeType in narrativeTypeToggles)
        {
            narrativeType.RegisterCallback<MouseUpEvent, ChoiceData>(setNarrativePath, choiceData);
            if (choiceData.Path.ToString() == narrativeType.text)
                narrativeType.value = true;
            else
                narrativeType.value = false;
        }

        conformField.value = choiceData.RequiredConform.ToString();
        conformField.RegisterCallback<InputEvent, ChoiceData>(setConformRequirement, choiceData);
        rebelField.value = choiceData.RequiredRebel.ToString();
        rebelField.RegisterCallback<InputEvent, ChoiceData>(setRebelRequirement, choiceData);
        resetField.value = choiceData.RequiredReset.ToString();
        resetField.RegisterCallback<InputEvent, ChoiceData>(setResetRequirement, choiceData);
        preserveField.value = choiceData.RequiredPreserve.ToString();
        preserveField.RegisterCallback<InputEvent, ChoiceData>(setPreserveRequirement, choiceData);
    }
    //!Tworzy domyślną odpowiedź.
    private void createDefaultOutput() 
    {
        createPort("New response " + ++noOfPorts, Direction.Output);
    }
    //!Odświeża węzeł.
    private void refreshNode() 
    {
        RefreshExpandedState();
        RefreshPorts();
    }
    //!Definiuje UI węzła.
    private void controlsSetup() 
    {
        createPort("input", Direction.Input, Port.Capacity.Multi);
        
        labelsSetup();
        togglesSetup();
        foldoutSetup();

        addOutput = new Button(createDefaultOutput);
        addOutput.text = "Add response";

        extensionContainer.Add(foldout);
        extensionContainer.Add(addOutput);
        refreshNode();
    }
    //!Definiuje UI kwestii i mówcy.
    private void labelsSetup() 
    {
        lineLabel = new TextField();
        lineLabel.label = "Line:";
        lineLabel.value = DialogLine;
        lineLabel.RegisterCallback<InputEvent>(setDialogLine);

        SpeakerLabel = new TextField();
        SpeakerLabel.label = "Speaker:";
        SpeakerLabel.value = Speaker;
        SpeakerLabel.RegisterCallback<InputEvent>(setSpeaker);

        extensionContainer.Add(SpeakerLabel);
        extensionContainer.Add(lineLabel);
    }
    //!Definiuje UI checkboxów.
    private void togglesSetup() 
    {
        isLeafToggle = new Toggle();
        isLeafToggle.text = "Is this a leaf node?";
        isLeafToggle.RegisterCallback<MouseUpEvent>(setAsLeaf);
        isEndingToggle = new Toggle();
        isEndingToggle.text = "Is this the end of the game?";
        if (IsEnding)
            isEndingToggle.value = true;
        isEndingToggle.RegisterCallback<MouseUpEvent>(setAsEnding);
        ExitLineLabel = new TextField();
        ExitLineLabel.label = "Exit line:";
        ExitLineLabel.value = ExitLine;
        ExitLineLabel.RegisterCallback<InputEvent>(setExitLine);
        if (IsLeaf)
            isLeafToggle.value = true;
        else
            ExitLineLabel.SetEnabled(false);
        isLeafToggle.SetEnabled(false);

        toggleChoice = new Toggle();
        toggleChoice.text = "Is this a narrative choice?";
        if (IsChoice)
            toggleChoice.value = true;
        toggleChoice.RegisterCallback<MouseUpEvent>(setAsChoice);

        extensionContainer.Add(isLeafToggle);
        extensionContainer.Add(isEndingToggle);
        extensionContainer.Add(ExitLineLabel);
        extensionContainer.Add(toggleChoice);
    }
    //!Definiuje UI części zwijanej.
    private void foldoutSetup() 
    {
        foldout = new Foldout();
        foldout.value = false;
        choiceName = new TextField();
        choiceName.label = "Choice title:";
        makesChoice = new Toggle();
        makesChoice.text = "Makes a choice?";
        makesChoice.value = true;

        conformField = new TextField();
        rebelField = new TextField();
        conformField.label = "Conform point requirement:";
        rebelField.label = "Rebel point requirement:";

        foldout.contentContainer.Add(makesChoice);
        foldout.contentContainer.Add(choiceName);
        narrativePathCheckboxesSetup();
        foldout.contentContainer.Add(conformField);
        foldout.contentContainer.Add(rebelField);

        foldout.SetEnabled(false);
    }
    //!Definiuje UI checkboxów opisujących ścieżki fabularne.
    private void narrativePathCheckboxesSetup() 
    {
        narrativeTypeToggles = new List<Toggle>();
        foreach(NarrativePath narrativePath in Enum.GetValues(typeof(NarrativePath)))
        {
            Toggle narrativeType = new Toggle();
            narrativeType.text = narrativePath.ToString();
            narrativeTypeToggles.Add(narrativeType);
            foldout.contentContainer.Add(narrativeType);
        }
    }
    //!Ustawia kwestię.
    private void setDialogLine(InputEvent e) 
    {
        this.DialogLine = lineLabel.value;
    }
    //!Ustawia mówcę.
    private void setSpeaker(InputEvent e) 
    {
        this.Speaker = SpeakerLabel.value;
        this.title = SpeakerLabel.value;
    }
    //!Ustawia nazwę portu.
    private void setPortName(InputEvent e, Port port) 
    {
        ChoiceData choiceData = ChoiceOutcomes.Find(x => x.PortName == e.previousData);
        if (choiceData != null)
            choiceData.PortName = e.newData;
        port.portName = e.newData;
    }
    //!Ustawia flagę IsLeaf.
    private void setAsLeaf(MouseUpEvent e) 
    {
        IsLeaf = isLeafToggle.value;
        ExitLineLabel.SetEnabled(isLeafToggle.value);
    }
    //!Ustawia flagę IsEnding.
    private void setAsEnding(MouseUpEvent e) 
    {
        IsEnding = isEndingToggle.value;
    }
    //!Ustawia kwestię wyjściową.
    private void setExitLine(InputEvent e) 
    {
        this.ExitLine = ExitLineLabel.value;
    }
    //!Ustawia flagę IsChoice.
    private void setAsChoice(MouseUpEvent e) 
    {
        IsChoice = toggleChoice.value;
    }
    //!Ustawia nazwę wyboru.
    private void setChoiceName(InputEvent e, ChoiceData choiceData) 
    {
        choiceData.ChoiceTitle = e.newData;
    }
    //!Ustawia flagę WasMade wyboru.
    private void setChoiceOutcome(MouseUpEvent e, ChoiceData choiceData) 
    {
        choiceData.WasMade = makesChoice.value;
    }
    //!Ustawia wymagania wyboru co do sprzyjania.
    private void setConformRequirement(InputEvent e, ChoiceData choiceData)
    {
        choiceData.RequiredConform = int.Parse(conformField.value);
    }
    //!Ustawia wymagania wyboru co do zaprzeczenia.
    private void setRebelRequirement(InputEvent e, ChoiceData choiceData)
    {
        choiceData.RequiredRebel = int.Parse(rebelField.value);
    }
    //!Ustawia wymagania wyboru co do resetu.
    private void setResetRequirement(InputEvent e, ChoiceData choiceData)
    {
        choiceData.RequiredReset = int.Parse(resetField.value);
    }
    //!Ustawia wymagania wyboru co do utrzymania.
    private void setPreserveRequirement(InputEvent e, ChoiceData choiceData)
    {
        choiceData.RequiredPreserve = int.Parse(preserveField.value);
    }
    //!Ustawia typ ścieżki fabularnej wyboru.
    private void setNarrativePath(MouseUpEvent e, ChoiceData choiceData) 
    {
        Toggle narrativeType = (Toggle) e.target;
        if (narrativeType.value)
        {
            choiceData.Path = (NarrativePath) Enum.Parse(typeof(NarrativePath), narrativeType.text);
            foreach (Toggle toggle in narrativeTypeToggles)
                if (toggle != narrativeType)
                    toggle.value = false;
        }
        else
            narrativeType.value = true;
    }
    //!Zwraca listę odpowiedzi.
    public List<Port> getOutputPorts() 
    {
        List<Port> ports = outputContainer.Children().ToList().Cast<Port>().ToList();
        return ports;
    }
}
#endif