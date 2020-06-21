﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AnswerDisplay : MonoBehaviour
{
    public int answerID;
    public TextMeshProUGUI answerText;
    public Button button;
    public Image border;

    /// <summary>
    /// Initializes the answer informations.
    /// </summary>
    /// <param name="answer">The answer that will be displayed.</param>
    public void SetupInfos(Answer answer)
    {
        gameObject.SetActive(true);

        if(border != null)
        {
            border.gameObject.SetActive(false);
        }

        answerText.text = answer.text;
    }

    /// <summary>
    /// Selects an answer.
    /// </summary>
    public void SelectAnswer()
    {
        border.gameObject.SetActive(true);
        button.interactable = false;

        GameManager.Instance.SelectAnswer(this);
    }

    /// <summary>
    /// Deselects an answer.
    /// </summary>
    public void Deselect()
    {
        border.gameObject.SetActive(false);
        button.interactable = true;
    }
}
