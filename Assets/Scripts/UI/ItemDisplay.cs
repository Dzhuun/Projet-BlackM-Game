using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemDisplay : MonoBehaviour
{
    [Header("Settings")]
    public ItemType itemType;
    public List<GameObject> stars;

    protected int currentItemLevel = 0;

    /// <summary>
    /// Updates the informations of the item.
    /// </summary>
    public virtual void SetupInfos()
    {
        currentItemLevel = 0;

        switch(itemType)
        {
            case ItemType.Voiture:

                currentItemLevel = GameManager.currentPlayer.car.level;
                break;

            case ItemType.Maison:

                currentItemLevel = GameManager.currentPlayer.house.level;
                break;

            case ItemType.Travail:

                currentItemLevel = GameManager.currentPlayer.work.level;
                break;

            case ItemType.Amis:

                currentItemLevel = GameManager.currentPlayer.friends.level;
                break;

            case ItemType.Famille:

                currentItemLevel = GameManager.currentPlayer.family.level;
                break;

            default:
                
                return;
        }

        for (int i = 0; i < stars.Count; i++)
        {
            stars[i].SetActive(i < currentItemLevel);
        }
    }

}
