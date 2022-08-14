using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Character Data", menuName = "CharacterData")]
[Serializable]
//!Przechowuje dane dotyczące formatowania okna dialogowego na podstawie danej postaci.
public class CharacterData : ScriptableObject 
{
    public string Name; //!<Imię mówcy.
    public Sprite Icon; //!<Portret mówcy.
    public Color TextColor; //!<Kolor tekstu mówcy.
}
