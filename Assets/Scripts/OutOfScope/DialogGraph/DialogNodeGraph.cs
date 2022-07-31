#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
//!Klasa okna edytora węzłów.
public class DialogNodeGraph : GraphViewEditorWindow 
{
    private DialogNodeView view; //!<Widok edytora.
    private string fileName = "New tree"; //!<Nazwa pliku.
    private TextField fileNameField; //!<Pole na nazwę pliku.

    [MenuItem("Dialog Tree/Dialog Node Graph")]
    //!Otwiera okno edytora i dodaje tą opcje do paska edytora Unity.
    public static void openWindow()
    {
        DialogNodeGraph window = GetWindow<DialogNodeGraph>();
        window.titleContent = new GUIContent("Dialog Node Graph.");
    }
    //!Definiuje pasek edytora grafów.
    private void createMenubar() 
    {
        Toolbar menuBar = new Toolbar();
        Button newNodeBtn = new Button(view.createNode);
        newNodeBtn.text = "Add node";
        menuBar.Add(newNodeBtn);

        fileNameField = new TextField();
        fileNameField.value = fileName;
        fileNameField.RegisterCallback<InputEvent>(setFileName);
        menuBar.Add(fileNameField);

        Button saveBtn = new Button(save);
        saveBtn.text = "Save";
        menuBar.Add(saveBtn);
        Button loadBtn = new Button(load);
        loadBtn.text = "Load";
        menuBar.Add(loadBtn);

        rootVisualElement.Add(menuBar);
    }
    //!Definiuję minimapę grafu.
    private void createMinimap() 
    {
        MiniMap minimap = new MiniMap();
        minimap.anchored = true;
        minimap.SetPosition(new Rect(10, 30, 200, 150));
        view.Add(minimap);
    }
    //!Ustawia nazwę pliku.
    private void setFileName(InputEvent e) 
    {
        fileName = fileNameField.value;
    }
    //!Zapisuje plik.
    private void save() 
    {
        if (fileName == "" || fileName == null)
            return;
        GraphIO graphIO = GraphIO.getInstance();
        graphIO.refreshData(view);
        graphIO.save(fileName);
    }
    //!Wczytuje plik.
    private void load() 
    {
        if (fileName == "" || fileName == null)
            return;
        GraphIO graphIO = GraphIO.getInstance();
        graphIO.refreshData(view);
        graphIO.load(fileName);
    }
    //!Ustawia parametry okna po jego aktywacji.
    void OnEnable() 
    {
        view = new DialogNodeView();
        VisualElementExtensions.StretchToParentSize(view);
        rootVisualElement.Add(view);
        createMenubar();
        createMinimap();
    }
    //!Czyści widok po deaktywacji okna.
    void OnDisable() 
    {
        rootVisualElement.Remove(view);
    }
}
#endif