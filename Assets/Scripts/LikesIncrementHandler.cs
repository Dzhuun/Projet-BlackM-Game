using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LikesIncrementHandler : MonoBehaviour
{
    public LikesIncrement positiveIncrement;
    public LikesIncrement negativeIncrement;

    public static float noteValue = 0;
    public static float maxValue = 0;

    /// <summary>
    /// Initializes the increments.
    /// </summary>
    /// <param name="fame">The fame of the local player.</param>
    public void UpdateSettings(float fame)
    {
        if (fame == 5)
        {
            maxValue = 40;
        }
        else if (fame > 1)
        {
            maxValue = Mathf.Floor(fame) * 5;
        }
        else
        {
            maxValue = 5;
        }

        noteValue = 0;
    }

    public static void IncrementNote(float increment)
    {
        noteValue = Mathf.Clamp(noteValue + increment, -maxValue, maxValue);
        GameUI.Instance.UpdateOpinion(noteValue);
    }
}
