using System;
using System.Collections.Generic;
using UnityEngine;
//!Przechowuje dane na temat drzewa dialogowego.
[Serializable]
public class DialogContainer : ScriptableObject 
{
    //Pola tej klasy powodują błędy w do narzędziu edycji gdy są właściwością.
    public List<NodeConnection> Connections; //!<Dane o połączeniach.
    public List<NodeDataContainer> NodeData; //!<Dane o węzła.
    public string FirstNodeGuid; //!<ID pierwszego węzła po korzeniu.

    public DialogContainer()
    {
        Connections = new List<NodeConnection>();
        NodeData = new List<NodeDataContainer>();
    }
    //!Zwraca pierwszy węzeł.
    public NodeDataContainer getFirstNode() 
    {
        return NodeData.Find(x => x.Guid == FirstNodeGuid);
    }
    //!Zwraca dany węzeł.
    public NodeDataContainer getNode(string guid) 
    {
        return NodeData.Find(x => x.Guid == guid);
    }
}
