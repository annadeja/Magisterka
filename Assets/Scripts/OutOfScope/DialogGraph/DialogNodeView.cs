#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
//!Klasa widoku edytora grafów.
public class DialogNodeView : GraphView 
{
    public DialogNodeView()
    {
        VisualElementExtensions.AddManipulator(this, new ContentDragger());
        VisualElementExtensions.AddManipulator(this, new ContentZoomer());
        VisualElementExtensions.AddManipulator(this, new SelectionDragger());
        VisualElementExtensions.AddManipulator(this, new RectangleSelector());
        VisualElementExtensions.AddManipulator(this, new EdgeManipulator());

        createRootNode();
    }
    //!Tworzy węzeł-korzeń.
    private void createRootNode() 
    {
        DialogNode root = new DialogNode(true, "root");
        root.SetPosition(new Rect(200, 200, 200, 200));
        root.capabilities &= ~Capabilities.Movable;
        root.capabilities &= ~Capabilities.Deletable;
        AddElement(root);
    }
    //!Tworzy nowy węzeł.
    public void createNode() 
    {
        DialogNode node = new DialogNode();
        node.SetPosition(new Rect(200, 200, 200, 200));
        AddElement(node);
    }
    //!Umożliwia podłączanie dowolnych węzłów ze sobą.
    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter) 
    {
        List<Port> compatiblePorts = new List<Port>();
        ports.ForEach(port =>
            {
                if (port != startPort && port.node != startPort.node && port.direction != startPort.direction)
                    compatiblePorts.Add(port);
            });
        return compatiblePorts;
    }
}
#endif