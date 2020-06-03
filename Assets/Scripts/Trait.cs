using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Trait", menuName = "Game Assets/Trait")]
public class Trait : ScriptableObject
{
    [UnityEngine.Serialization.FormerlySerializedAs("trait")] public string traitName;
    public bool isNegative;

    public static bool operator ==(Trait v1, Trait v2)
    {
        return (v1.traitName == v2.traitName);
    }

    public static bool operator !=(Trait v1, Trait v2)
    {
        return (v1.traitName != v2.traitName);
    }
}

[System.Serializable]
public class CharacterTrait
{
    public Trait trait;

    [HideInInspector] public bool isActive = true;
}

[System.Serializable]
public class AnswerTrait
{
    public Trait trait;
    public int traitRespected = 0;
    public int traitNotRespected = 0;
}


