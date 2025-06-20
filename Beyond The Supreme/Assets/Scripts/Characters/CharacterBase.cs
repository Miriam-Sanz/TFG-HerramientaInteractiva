using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Character", menuName = "Character/Create new Character")]
public class CharacterBase : ScriptableObject
{
    [SerializeField] private string nameChac;

    [System.Serializable]
    public struct DialogEntry
    {
        public string text;       // L�nea de di�logo
        public Sprite expression; // Expresi�n del personaje en ese di�logo
        public Sprite dialogIlustration; //Ilustraci�n que acompa�a a los di�logos
        public string speakerName; // Nombre del personaje que habla
    }

    [SerializeField] private List<DialogEntry> dialogs = new List<DialogEntry>();
    [SerializeField] private List<Sprite> ilustrations;
    public Vector2 Position;

    public string Name
    {
        get { return nameChac; }
    }

    public List<DialogEntry> Dialogs
    {
        get { return dialogs; }
    }

    public List<Sprite> Ilustrations
    {
        get { return ilustrations; }
    }
}
