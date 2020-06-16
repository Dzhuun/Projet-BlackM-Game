using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TestingScript : MonoBehaviour
{
    private List<Character> characters;
    private List<Character> charsCopy;

    private void Awake()
    {
        characters = new List<Character>(Resources.LoadAll<Character>("Characters"));
        charsCopy = new List<Character>();

        foreach(Character charac in characters)
        {
            //charsCopy.Add(charac);
            charsCopy.Add(new Character(charac));
        }
    }

    public void AddTrait()
    {
        charsCopy[0].traits.Add(new CharacterTrait(new Trait()));
    }
}
