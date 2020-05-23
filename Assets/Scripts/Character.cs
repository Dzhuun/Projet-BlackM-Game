using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Char", menuName = "Game Assets/Character")] 
public class Character : ScriptableObject
{
    public string nickname;

    public string description;
    
    public Sprite avatar;

	public string personality_1;

	public string personality_2;
	
	public string personality_3;

  	// public int age;
	// Etoile (popularité) de base du personnage : 2 Etoile / 5
	// Possiblité de gagner ou perdre des "traits de personnality" en fonction du classement, max 5 (?)
}



[System.Serializable]
public class Reponse
{
    public Character character;

    public string text;
}
