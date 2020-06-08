using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LikesSlider : MonoBehaviour
{
    public TextMeshProUGUI likesText;
    public Slider valueSlider;

    public void UpdateSettings(float popularity)
    {
        if(popularity == 5)
        {
            popularity = 40;
        }
        else if(popularity > 1)
        {
            popularity = Mathf.Floor(popularity) * 5;
        }
        else
        {
            popularity = 5;
        }

        valueSlider.minValue = -popularity;
        valueSlider.maxValue = popularity;

        valueSlider.value = 0;
        likesText.text = "0";
    }

    public void OnValueUpdate(float likesValue)
    {
        likesText.text = likesValue.ToString();
    }
}
