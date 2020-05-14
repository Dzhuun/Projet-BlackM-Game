using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New scenario", menuName = "Game Assets/Scenario")]
public class Scenario : ScriptableObject
{
    public int id;

    public string title;

    public int starCost;

    public List<string> commonAnswers;

    public List<Reponse> specificAnswers;
}