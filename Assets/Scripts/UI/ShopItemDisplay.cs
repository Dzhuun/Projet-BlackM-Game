using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemDisplay : ItemDisplay
{
    [Header("Item Display")]
    public GameObject itemDisplay;
    public Button buyButton;
    public TextMeshProUGUI upgradeCostValue;
    public Image likesIcon;

    [Header("Validate Display")]
    public GameObject validateDisplay;
    public TextMeshProUGUI validateUpgradeCostValue;


    private int _upgradeCost = 0;

    private void Awake()
    {
        buyButton.interactable = false;
    }

    public override void SetupInfos()
    {
        base.SetupInfos();
        
        if (currentItemLevel + 1 > Mathf.FloorToInt(GameManager.currentPlayer.fame))
        {
            buyButton.interactable = false;

            if(currentItemLevel == 5)
            {
                likesIcon.gameObject.SetActive(false);
                upgradeCostValue.text = string.Empty;
                return;
            }
        }

        // Show the level of the next upgrade
        stars[currentItemLevel].SetActive(true);

        // The cost is equal to the base cost (10) plus the current level * 10
        _upgradeCost = 10 * (currentItemLevel + 1);

        // Activate the upgrade button only if the player has enough likes
        if (_upgradeCost <= GameManager.currentPlayer.likes)
        {
            buyButton.interactable = true;
        }
        else
        {
            buyButton.interactable = false;
        }

        // Display the likes cost
        upgradeCostValue.text = _upgradeCost.ToString();
        likesIcon.gameObject.SetActive(true);
        
    }


    public void ShowItemDisplay()
    {
        validateDisplay.SetActive(false);
        itemDisplay.SetActive(true);
    }

    public void ShowValidateDisplay()
    {
        ShopManager.Instance.ResetShopDisplays();

        itemDisplay.SetActive(false);
        validateDisplay.SetActive(true);

        validateUpgradeCostValue.text = _upgradeCost.ToString();
    }

    /// <summary>
    /// Upgrades an item.
    /// </summary>
    public void BuyUpgrade()
    {
        GameManager.BuyUpgrade(itemType, _upgradeCost);

        ShopManager.Instance.UpdateDisplays();

        GameUI.Instance.RefreshPlayerInfos();

        ShowItemDisplay();
    }
}
