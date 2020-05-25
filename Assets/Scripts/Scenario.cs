using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New scenario", menuName = "Game Assets/Scenario")]
public class Scenario : ScriptableObject
{
    public int id;

    public string description;

    public int starCost;

    public List<Answer> commonAnswers;

    public List<SpecificAnswer> specificAnswers;

    //public void GetAnswer()
}


[System.Serializable]
public class Answer
{
    public string text;

    public int likesValue;
}

[System.Serializable]
public class SpecificAnswer : Answer
{
    public Character character;
}