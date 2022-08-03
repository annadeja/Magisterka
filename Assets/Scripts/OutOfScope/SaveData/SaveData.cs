using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//!Klasa przechowująca stan gry.
[Serializable]
public class SaveData 
{
    //public CharacterStats PlayerStats { set; get; } //!<Statystyki gracza.
    public List<ChoiceData> PastChoices { set; get; } //!<Lista wyborów gracza.
    public string LastLocation { set; get; } //!<Scena w jakiej ostatnio przebywał gracz.
    public string DialogPosition { set; get; } //!<Klasa Vector3 nie jest serializowalna, stąd pozycję gracza przechowuje się w postaci tablicy.
    public int ConformChoices { set; get; } //!<Pole określające liczbę wyborów w danej ścieżce.
    public int RebelChoices { set; get; } //!<Pole określające liczbę wyborów w danej ścieżce.
    public int ResetChoices { set; get; } //!<Pole określające liczbę wyborów w danej ścieżce.
    public int PreserveChoices { set; get; } //!<Pole określające liczbę wyborów w danej ścieżce.

    public SaveData()
    {
        //PlayerStats = new CharacterStats();
        PastChoices = new List<ChoiceData>();
        LastLocation = "MainScene";
        DialogPosition = "0";
        ConformChoices = 0;
        RebelChoices = 0;
        ResetChoices = 0;
        PreserveChoices = 0;
    }

    public SaveData(string lastLocation, string dialogPosition)
    {
        //PlayerStats = new CharacterStats();
        PastChoices = new List<ChoiceData>();
        LastLocation = lastLocation;
        DialogPosition = dialogPosition;
        ConformChoices = 0;
        RebelChoices = 0;
        ResetChoices = 0;
        PreserveChoices = 0;
    }
}
