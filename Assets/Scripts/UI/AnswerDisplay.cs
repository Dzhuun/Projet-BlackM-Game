using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnswerDisplay : MonoBehaviour
{
    public int answerID;

    public Text answerText;

    public void SetupInfos(string answer)
    {
        gameObject.SetActive(true);

        answerText.text = answer;
    }
}
