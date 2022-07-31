using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//!Przechowuje dane o węzle grafu dialogowego w formie do zapisu przez edytor oraz do odczytu przez grę.
[Serializable]
public class NodeDataContainer 
{
    //Pola tej klasy powodują błędy w do narzędziu edycji gdy są właściwością.
    public string Guid; //!<Id węzła.
    public string DialogLine; //!<Kwestia dialogowa.
    public string Speaker; //!<Mówca.
    public string ExitLine; //!<Kwestia na zakończenie dialogu.
    public Vector2 Position; //!<Pozycja na grafie w edytorze.
    public List<NodeConnection> OutputPorts; //!<Wyjścia węzła.
    public bool IsChoice; //!<Czy jest zapisywalnym wyborem?
    public bool IsLeaf; //!<Czy jest liściem/ostatnim węzłem w drzewie?
    public bool IsEnding; //!<Czy prowadzi do zakończenia?
    public List<ChoiceData> ChoiceOutcomes; //!<Dane dotyczące wyborów zapisywalnych.
    //Ten konstruktor posiada taką formę danych wejściowych z racji chęci oddzielenia GraphView od tej klasy. Inaczej użycie jej byłoby niemożliwe wewnątrz gry.
    public NodeDataContainer(string Guid, string DialogLine, string Speaker, string ExitLine, bool IsChoice, bool IsLeaf, bool IsEnding, List<ChoiceData> ChoiceOutcomes, Vector2 Position)
    {
        this.Guid = Guid;
        this.DialogLine = DialogLine;
        this.Speaker = Speaker;
        this.ExitLine = ExitLine;
        this.Position = Position;
        this.OutputPorts = new List<NodeConnection>();
        this.IsChoice = IsChoice;
        this.IsLeaf = IsLeaf;
        this.IsEnding = IsEnding;
        if (IsChoice)
            this.ChoiceOutcomes = new List<ChoiceData>(ChoiceOutcomes);
        else
            this.ChoiceOutcomes = null;
    }
}
