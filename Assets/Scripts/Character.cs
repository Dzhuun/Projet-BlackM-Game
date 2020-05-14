using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Char", menuName = "Game Assets/Character")] 
public class Character : ScriptableObject
{
    public string nickname;

    public int age;

    public Sprite avatar;
    // traits, etc..
}



[System.Serializable]
public class Reponse
{
    public string character;

    public string text;
}
