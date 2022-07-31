using System;
//!Przechowuje dane o połączeniach pomiędzy węzłami do zapisu w edytorze i do odczytu przez grę.
[Serializable]
public class NodeConnection 
{
    //Pola tej klasy powodują błędy w do narzędziu edycji gdy są właściwością.
    public string NodeGuid; //!<ID węzła wyjściowego.
    public string PortName; //!<Nazwa portu.
    public string TargetGuid; //!<ID węzła wejściowego.

    public NodeConnection(string NodeGuid, string PortName, string TargetGuid)
    {
        this.NodeGuid = NodeGuid;
        this.PortName = PortName;
        this.TargetGuid = TargetGuid;
    }
}
