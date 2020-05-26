using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Database : MonoBehaviour
{
    public static List<Character> characters;

    private List<Scenario> scenarios;

    private static List<Scenario> scenarios_1;
    private static List<Scenario> scenarios_2;
    private static List<Scenario> scenarios_3;
    private static List<Scenario> scenarios_4;

    private void Awake()
    {
        characters = Resources.LoadAll<Character>("Characters").ToList();

        scenarios = Resources.LoadAll<Scenario>("Scenarios").ToList();
        
        scenarios_1 = scenarios.FindAll(x => x.starCost == 1);
        scenarios_2 = scenarios.FindAll(x => x.starCost == 2);
        scenarios_3 = scenarios.FindAll(x => x.starCost == 3);
        scenarios_4 = scenarios.FindAll(x => x.starCost == 4);
    }

    public static Scenario GetScenario(float popularity, int scenarioID)
    {
        float starCost = Mathf.Floor(popularity);

        switch(starCost)
        {
            case 1:

                return scenarios_1.Find(x => x.id == scenarioID);

            case 2:

                return scenarios_2.Find(x => x.id == scenarioID);

            case 3:

                return scenarios_3.Find(x => x.id == scenarioID);

            case 4:

                return scenarios_4.Find(x => x.id == scenarioID);

            default:

                Debug.LogError(string.Format("Incorrect Star cost ({0}) or scenario ID ({1}).", starCost, scenarioID));
                return null;
        }
    }
}
