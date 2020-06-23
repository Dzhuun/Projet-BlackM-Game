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
}


[System.Serializable]
public class Answer
{
    public string text;
    public AnswerResultType resultType;
    public List<AnswerTrait> traits;

    public int GetLikesValue(int fameRank)
    {
        return SettingsManager.Instance.answerResultHandler.GetLikesValue(fameRank, resultType);
    }
}

[System.Serializable]
public class SpecificAnswer : Answer
{
    public Character character;
}

public enum AnswerResultType
{
    Bon,
    Commun,
    Insuffisant,
    Mauvais
}