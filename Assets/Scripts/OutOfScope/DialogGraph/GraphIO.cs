#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
//!Klasa typu singleton zapewniająca wczytywanie i zapis grafów.
public class GraphIO 
{
    private static GraphIO instance; //!<Instancja.
    private DialogNodeView view; //!<Widok narzędzia edycji.
    private List<Edge> edges; //!<Połączenia grafu.
    private List<DialogNode> nodes; //!<Węzły grafu.

    private GraphIO()
    {}
    //!Zwraca instancję.
    public static GraphIO getInstance() 
    {
        if (instance == null)
            instance = new GraphIO();
        return instance;
    }
    //!Ładuje nowe dane.
    public void refreshData(DialogNodeView newView)
    {
        view = newView;
        edges = view.edges.ToList();
        nodes = view.nodes.ToList().Cast<DialogNode>().ToList();
    }
    //!Zapisuje graf do pliku.
    public void save(string fileName)
    {
        if (edges.Count == 0)
            return;

        DialogContainer dialogContainer = ScriptableObject.CreateInstance<DialogContainer>(); //Tworzy plik zapisu.
        List<Edge> connectedPorts = new List<Edge>();
        foreach (Edge edge in edges)
            if (edge.input.node != null)
                connectedPorts.Add(edge);

        foreach (DialogNode node in nodes) //Tworzy obiekty reprezentujące węzły do zapisu.
        {
            if (node.IsRoot)
                continue;
            NodeDataContainer data = new NodeDataContainer(node.Guid, node.DialogLine, node.Speaker, node.ExitLine, node.IsChoice, node.IsLeaf, node.IsEnding, node.ChoiceOutcomes, node.GetPosition().position);
            dialogContainer.NodeData.Add(data);
        }

        foreach (Edge edge in connectedPorts) //Tworzy obiekty reprezentujące połączenia i przypisuje je do odpowiednich węzłów.
        {
            DialogNode outputNode = (DialogNode)edge.output.node;
            DialogNode inputNode = (DialogNode)edge.input.node;

            NodeConnection connection = new NodeConnection(outputNode.Guid, edge.output.portName, inputNode.Guid);
            dialogContainer.Connections.Add(connection);

            if (edge.output.portName == "root")
                dialogContainer.FirstNodeGuid = inputNode.Guid;
            else
                dialogContainer.NodeData.Find(x => x.Guid == outputNode.Guid).OutputPorts.Add(connection);
        }

        AssetDatabase.CreateAsset(dialogContainer, "Assets/Resources/Dialogs/" + fileName + ".asset"); //Zapisuje plik jako asset.
        AssetDatabase.SaveAssets();
    }
    //!Ładuje graf do edytora.
    public void load(string fileName)
    {
        DialogContainer dialogContainer = Resources.Load<DialogContainer>("Dialogs/" + fileName); //Ładuje plik.
        if (dialogContainer == null)
            return;

        deleteNodes(dialogContainer);

        foreach (NodeDataContainer data in dialogContainer.NodeData) //Odtwarza węzły na podstawie pliku.
        {
            DialogNode node = new DialogNode(data);
            foreach (NodeConnection portConnection in dialogContainer.Connections)
            {
                if (portConnection.NodeGuid == data.Guid)
                    node.createPort(portConnection.PortName, Direction.Output);
            }
            view.AddElement(node);
            node.SetPosition(new Rect(data.Position.x, data.Position.y, 200, 200));
        }
        nodes = view.nodes.ToList().Cast<DialogNode>().ToList();

        foreach (NodeConnection portConnection in dialogContainer.Connections) //Odtwarza połączenia.
        {
            DialogNode output = nodes.Find(x => x.Guid == portConnection.NodeGuid);
            DialogNode input = nodes.Find(x => x.Guid == portConnection.TargetGuid);
            List<Port> ports = output.getOutputPorts();
            Port outputPort = ports.Find(x => x.portName == portConnection.PortName);
            if (input != null)
            {
                output.IsLeaf = false;
                view.Add(outputPort.ConnectTo((Port)input.inputContainer[0]));
            }
            else
                output.removePort(outputPort);
        }
    }
    //!Usuwa dotychczasowe węzły z edytora.
    private void deleteNodes(DialogContainer dialogContainer)
    {
        foreach (DialogNode node in nodes)
        {
            if (node.IsRoot)
            {
                node.Guid = dialogContainer.Connections.Find(x => x.PortName == "root").NodeGuid;
            }
            else
            {
                foreach (Edge edge in edges)
                    if (edge.input.node == node)
                        view.RemoveElement(edge);
                view.RemoveElement(node);
            }
        }
    }
}
#endif