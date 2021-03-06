﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MentalHealthUI : MonoBehaviour
{
    [Header("Settings")]
    public Color greenColor = Color.green;
    public Color yellowColor = Color.yellow;
    public Color redColor = Color.red;

    [Header("Button")]
    [UnityEngine.Serialization.FormerlySerializedAs("gauge")] public Image buttonGauge;
    public Image plusImage;

    [Header("Panel")]
    public GameObject panel;
    public TextMeshProUGUI mentalHealthValue;
    public Color activatedColor;
    public Color deactivatedColor;
    public Image panelGauge;
    public List<TextMeshProUGUI> sideEffects;

    private void Awake()
    {
        panel.SetActive(false);
    }

    /// <summary>
    /// Updates the mental Health UI.
    /// </summary>
    /// <param name="player">The player concerned.</param>
    public void SetMentalHealth(NetworkPlayer player)
    {
        gameObject.SetActive(true);

        float mentalHealth = player.mentalHealth;

        // The visual graphic of the mental health is a slider between 5% and 95% of the fill amount of the image
        // Let's linearly interpolate the mentalHealth value (between 0 and 100) to the interval [0.05;0.95]
        buttonGauge.fillAmount = 0.90f * mentalHealth / 100 + 0.05f;

        if (mentalHealth > 66)
        {
            buttonGauge.color = greenColor;
        }
        else if(mentalHealth > 33)
        {
            buttonGauge.color = yellowColor;
        }
        else
        {
            buttonGauge.color = redColor;
        }

        mentalHealthValue.text = string.Format("{0}%", mentalHealth);
        panelGauge.fillAmount = mentalHealth / 100;

        int mentalLevel = player.GetMentalHealthLevel();

        for(int i = 0; i < sideEffects.Count; i++)
        {
            sideEffects[i].color = i < mentalLevel ? activatedColor : deactivatedColor;
        }
    }

    /// <summary>
    /// Toggles the active state of the mental health panel.
    /// </summary>
    public void TogglePanel()
    {
        panel.SetActive(!panel.activeInHierarchy);
    }

    /// <summary>
    /// Hides the complete mental Health UI.
    /// </summary>
    public void SetActive(bool active)
    {
        panel.SetActive(active);
        gameObject.SetActive(active);
    }
}
