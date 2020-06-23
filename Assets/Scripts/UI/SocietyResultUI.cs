using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SocietyResultUI : MonoBehaviour
{
    public TextMeshProUGUI textResult;

    [Header("Rank 0 and 1")]
    public SocietySpecificResult result_0_1;

    [Header("Rank 2")]
    public SocietySpecificResult result_2;

    [Header("Rank 3")]
    public SocietySpecificResult result_3;

    [Header("Rank 4")]
    public SocietySpecificResult result_4;

    [Header("Rank 5")]
    public SocietySpecificResult result_5;

    /// <summary>
    /// Show the opinion of the society depending on the value of likes given by the society.
    /// </summary>
    /// <param name="opinion">The likes value given by the society.</param>
    public void ShowResult(int opinionValue, float playerRank)
    {
        int rankLevel = Mathf.FloorToInt(playerRank);

        switch(rankLevel)
        {
            case 0:
            case 1:

                textResult.text = result_0_1.GetComment(opinionValue);
                break;

            case 2:

                textResult.text = result_2.GetComment(opinionValue);
                break;

            case 3:

                textResult.text = result_3.GetComment(opinionValue);
                break;

            case 4:

                textResult.text = result_4.GetComment(opinionValue);
                break;

            case 5:

                textResult.text = result_5.GetComment(opinionValue);
                break;
        }
    }
}

[System.Serializable]
public class SocietySpecificResult
{
    public List<SocietyResult> societyResults = new List<SocietyResult>(4);

    public string GetComment(int value)
    {
        foreach(SocietyResult result in societyResults)
        {
            if(result.value == value)
            {
                return result.comment;
            }
        }

        Debug.LogError("No text has been linked with the society likes value given at this rank.");

        return string.Empty;
    }
}

[System.Serializable]
public class SocietyResult
{
    public int value;
    [TextArea(1, 2)] public string comment;
}
