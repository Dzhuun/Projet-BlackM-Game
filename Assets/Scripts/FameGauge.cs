using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FameGauge : MonoBehaviour
{
    /// <summary>
    /// The ordered list of star images.0
    /// </summary>
    public List<Image> stars;

    /// <summary>
    /// Updates the fame UI.
    /// </summary>
    /// <param name="fame">The fame of the observed player.</param>
    public void SetGauge(float fame)
    {
        for(int i = 0; i < stars.Count; i++)
        {
            stars[i].fillAmount = Mathf.Clamp(fame - i, 0, 1);
        }
    }
}
