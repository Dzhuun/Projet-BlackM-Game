using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LikesSlider : MonoBehaviour
{
    public Text likesText;
    public Slider valueSlider;

    public void UpdateSettings(float popularity)
    {
        if(popularity > 1)
        {
            popularity = Mathf.Floor(popularity) * 10;
        }
        else
        {
            popularity = 10;
        }

        valueSlider.minValue = -popularity;
        valueSlider.maxValue = popularity;

        valueSlider.value = 0;
        likesText.text = "0";
    }

    public void OnValueUpdate(float likesValue)
    {
        Debug.Log("OnValueUpdate");
        likesText.text = likesValue.ToString();
    }
}
