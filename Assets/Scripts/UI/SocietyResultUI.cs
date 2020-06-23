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
    public void ShowResult(AnswerResultType resultType, float playerRank)
    {
        int rankLevel = Mathf.FloorToInt(playerRank);

        switch(rankLevel)
        {
            case 0:
            case 1:
                textResult.text = result_0_1.GetComment(resultType);
                break;

            case 2:
                textResult.text = result_2.GetComment(resultType);
                break;

            case 3:
                textResult.text = result_3.GetComment(resultType);
                break;

            case 4:
                textResult.text = result_4.GetComment(resultType);
                break;

            case 5:
                textResult.text = result_5.GetComment(resultType);
                break;
        }
    }
}

[System.Serializable]
public class SocietySpecificResult
{
    public List<SocietyResult> societyResults = new List<SocietyResult>(4);

    public string GetComment(AnswerResultType resultType)
    {
        foreach(SocietyResult result in societyResults)
        {
            if(result.resultType == resultType)
            {
                return result.comment;
            }
        }

        Debug.LogError(string.Format("No text has been linked with the society for the resultType ({0}).", resultType.ToString()));

        return string.Empty;
    }
}

[System.Serializable]
public class SocietyResult
{
    public AnswerResultType resultType;
    [TextArea(1, 2)] public string comment;
}
