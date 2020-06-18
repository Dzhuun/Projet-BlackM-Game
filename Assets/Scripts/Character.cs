using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Char", menuName = "Game Assets/Character")] 
public class Character : ScriptableObject
{
    public string nickname;
    [TextArea] public string description;
    public bool male;
    public Sprite avatar;
	public List<CharacterTrait> traits;

    public Character(Character charToCopy)
    {
        nickname = charToCopy.nickname;
        description = charToCopy.description;
        avatar = charToCopy.avatar;
        male = charToCopy.male; 
        traits = new List<CharacterTrait>(charToCopy.traits);
    }
}

