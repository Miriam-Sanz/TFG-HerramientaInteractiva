using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Room", menuName = "Room/Create new Room")]

public class RoomBase : ScriptableObject
{
    [SerializeField] string nameRoom;

    [SerializeField] List<ScriptableObject> objects;

    [SerializeField] List<ScriptableObject> character;

    [SerializeField] Sprite background;


    //[SerializeField] int order;

    public string Name
    {
        get { return nameRoom; }
    }

    public List<ScriptableObject> Objects
    {
        get { return objects; }
    }

    public List<ScriptableObject> Characters
    {
        get { return character; }
    }

    public Sprite Backgrounds
    {
        get { return background; }
    }
}
