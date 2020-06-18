using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Trait", menuName = "Game Assets/Trait")]
public class Trait : ScriptableObject
{
    public string maleName;
    public string femaleName;
    public bool isNegative;

    /// <summary>
    /// Returns the trait name according to the sex of the character.
    /// </summary>
    /// <param name="character">The character concerned by the trait.</param>
    /// <returns>The male or female trait.</returns>
    public string GetTraitName(Character character)
    {
        return character.male ? maleName : femaleName; 
    }

    public static bool operator ==(Trait v1, Trait v2)
    {
        return (v1.maleName == v2.maleName);
    }

    public static bool operator !=(Trait v1, Trait v2)
    {
        return (v1.maleName != v2.maleName);
    }
}

[System.Serializable]
public class CharacterTrait
{
    public Trait trait;

    [HideInInspector] public bool isActive = true;

    public CharacterTrait(Trait _trait)
    {
        trait = _trait;
        isActive = true;
    }
}

[System.Serializable]
public class AnswerTrait
{
    public Trait trait;
    public int traitRespected = 0;
    public int traitNotRespected = 0;
}


