﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//!Klasa przechowująca dane dotyczące danego wyboru.
[Serializable]
public class ChoiceData 
{
    public string PortName; //!<Nazwa portu/odpowiedzi.
    public string ChoiceTitle; //!<Tytuł wyboru.
    public bool WasMade; //!<Czy wyboru dokonano?
    public bool WasFailed; //!<Czy wybór był nieudany?
    public NarrativePath Path; //!<Ścieżka fabularna do jakiej przynależy wybór.
    public int RequiredConform; //!<Wymagana liczba punktów sprzyjania.
    public int RequiredRebel; //!<Wymagana liczba punktów zaprzeczenia.
    public int RequiredReset; //!<Wymagana liczba punktów resetu.
    public int RequiredPreserve; //!<Wymagana liczba punktów utrzymania.

    public ChoiceData(string portName, string choiceTitle = "Sample choice", bool wasMade = true)
    {
        this.PortName = portName;
        this.ChoiceTitle = choiceTitle;
        this.WasMade = wasMade;
        this.Path = NarrativePath.None;
        WasFailed = false;
        RequiredConform = 0;
        RequiredRebel = 0;
        RequiredReset = 0;
        RequiredPreserve = 0;
    }
    //!Sprawdza czy gracz posiada wystarczająco wysoki poziom umiejętności by dokonać danego wyboru.
    //public void skillCheck(CharacterStats playerStats)
    //{
    //    if (!(playerStats.Charisma >= RequiredConform && playerStats.Deception >= RequiredRebel))
    //        WasFailed = true;
    //}
}
