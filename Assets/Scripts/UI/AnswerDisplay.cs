using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AnswerDisplay : MonoBehaviour
{
    public int answerID;
    public TextMeshProUGUI answerText;

    public void SetupInfos(Answer answer)
    {
        gameObject.SetActive(true);

        answerText.text = answer.text;
    }
}
