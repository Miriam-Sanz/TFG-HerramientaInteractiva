using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Object", menuName = "Object/Create new Object")]
public class ObjectsBase : ScriptableObject
{
    [SerializeField] private string nameObject;
    [SerializeField] private Sprite ilustration;
    [System.Serializable]
    public struct ObjectEntry
    {
        public string title;        
        [TextArea]
        public string description;  
        public Sprite ilustration;  
    }

    [SerializeField] private List<ObjectEntry> objectDescriptions = new List<ObjectEntry>();
    public Vector2 Position;

    public List<ObjectEntry> ObjectDescriptions
    {
        get { return objectDescriptions; }
    }
    public string Name
    {
        get { return nameObject; }
    }
    public Sprite Ilustration
    {
        get { return ilustration; }
    }
}
