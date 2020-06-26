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
    private static List<Scenario> scenarios_5;

    public static List<Trait> positiveTraits;

    private void Awake()
    {
        // Load the character resources and copy them in order to modify the copy and not the resources
        List<Character> tempCharList = Resources.LoadAll<Character>("Characters").ToList();
        characters = new List<Character>();
        foreach(Character charac in tempCharList)
        {
            characters.Add(new Character(charac));
        }

        scenarios = Resources.LoadAll<Scenario>("Scenarios").ToList();
        
        scenarios_1 = scenarios.FindAll(x => x.starCost == 1);
        scenarios_2 = scenarios.FindAll(x => x.starCost == 2);
        scenarios_3 = scenarios.FindAll(x => x.starCost == 3);
        scenarios_4 = scenarios.FindAll(x => x.starCost == 4);
        scenarios_5 = scenarios.FindAll(x => x.starCost == 5);

        positiveTraits = Resources.LoadAll<Trait>("Traits/Positifs").ToList();
    }

    public static Scenario GetScenario(float popularity, int scenarioID)
    {
        float starCost = Mathf.Floor(popularity);

        switch(starCost)
        {
            case 0:
            case 1:

                return scenarios_1.Find(x => x.id == scenarioID);

            case 2:

                return scenarios_2.Find(x => x.id == scenarioID);

            case 3:

                return scenarios_3.Find(x => x.id == scenarioID);

            case 4:

                return scenarios_4.Find(x => x.id == scenarioID);

            case 5:

                return scenarios_5.Find(x => x.id == scenarioID);

            default:

                Debug.LogError(string.Format("Incorrect Star cost ({0}) or scenario ID ({1}).", starCost, scenarioID));
                return null;
        }
    }
}
