using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MentalHealthUI : MonoBehaviour
{
    [Header("Settings")]
    public Color greenColor = Color.green;
    public Color yellowColor = Color.yellow;
    public Color redColor = Color.red;

    [Header("Images")]
    public Image gauge;
    public Image plusImage;

    public void SetMentalHealth(float mentalHealth)
    {
        // The visual graphic of the mental health is a slider between 5% and 95% of the fill amount of the image
        // Let's linearly interpolate the mentalHealth value (between 0 and 100) to the interval [0.05;0.95]
        float gaugeValue = 0.90f * mentalHealth / 100 + 0.05f;
        gauge.fillAmount = gaugeValue;

        if(mentalHealth > 66)
        {
            gauge.color = greenColor;
        }
        else if(mentalHealth > 33)
        {
            gauge.color = yellowColor;
        }
        else
        {
            gauge.color = redColor;
        }
    }
}
