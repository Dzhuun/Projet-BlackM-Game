using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Char", menuName = "Game Assets/Character")] 
public class Character : ScriptableObject
{
    public string nickname;

    public string description;
    
    public Sprite avatar;

	public List<CharacterTrait> traits;
}

