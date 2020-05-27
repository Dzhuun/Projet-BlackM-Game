using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemDisplay : MonoBehaviour
{
    public Text itemLevelValue;
    public Text upgradeCostValue;
    public Text upgradeCostText;
    public Text buttonText;
    public Button buyButton;
    public ItemType itemType;

    private int _upgradeCost = 0;

    private void Awake()
    {
        buyButton.interactable = false;
    }

    public void SetupInfos()
    {
        int currentItemLevel = 0;

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

            case ItemType.Entourage:

                currentItemLevel = GameManager.currentPlayer.entourage.level;
                break;

            default:
                
                return;
        }

        if(currentItemLevel == 5)
        {
            buyButton.interactable = false;
            buttonText.text = "Niveau Max";
            upgradeCostText.gameObject.SetActive(false);
            upgradeCostValue.gameObject.SetActive(false);
            itemLevelValue.text = currentItemLevel.ToString();
            
            return;
        }

        // The cost is equal to the base cost (10) plus the next level upgrade * 10
        _upgradeCost = 10 * (currentItemLevel + 2);

        // Activate the upgrade button only if the player has enough likes
        if(_upgradeCost <= GameManager.currentPlayer.likes)
        {
            buyButton.interactable = true;
        }
        else
        {
            buyButton.interactable = false;
        }

        buttonText.text = string.Format("Acheter Niv.{0}", currentItemLevel + 1);

        upgradeCostText.gameObject.SetActive(true);
        upgradeCostValue.gameObject.SetActive(true);
        itemLevelValue.text = currentItemLevel.ToString();
        upgradeCostValue.text = _upgradeCost.ToString();
    }

    /// <summary>
    /// Upgrade an item.
    /// </summary>
    public void BuyUpgrade()
    {
        GameManager.BuyUpgrade(itemType, _upgradeCost);
    }
}
