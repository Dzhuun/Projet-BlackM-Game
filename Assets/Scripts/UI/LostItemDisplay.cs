using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LostItemDisplay : MonoBehaviour
{
    public ItemType itemType;
    public List<Image> stars;

    /// <summary>
    /// Shows the current level of the good and all the level it has lost.
    /// </summary>
    /// <param name="previousRank"></param>
    /// <param name="newRank"></param>
    /// <param name="activatedColor"></param>
    /// <param name="lostColor"></param>
    public void SetupInfos(int previousRank, int newRank, Color activatedColor, Color lostColor)
    {
        for(int i = 0; i < stars.Count; i++)
        {
            if(i < newRank)
            {
                stars[i].gameObject.SetActive(true);
                stars[i].color = activatedColor;
            }
            else if(i < previousRank)
            {
                stars[i].gameObject.SetActive(true);
                stars[i].color = lostColor;
            }
            else
            {
                stars[i].gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Shows the current level of the item.
    /// </summary>
    /// <param name="currentRank"></param>
    /// <param name="activatedColor"></param>
    public void SetupInfos(int currentRank, Color activatedColor)
    {
        for(int i = 0; i < stars.Count; i++)
        {
            if(i < currentRank)
            {
                stars[i].gameObject.SetActive(true);
                stars[i].color = activatedColor;
            }
            else
            {
                stars[i].gameObject.SetActive(false);
            }
        }
    }
}
