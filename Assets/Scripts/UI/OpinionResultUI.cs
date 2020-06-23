using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OpinionResultUI : MonoBehaviour
{
    [Header("Fame Update")]
    public List<Image> fameStars;
    public Color fameActiveColor;
    public Color fameEmptyColor;
    public Color fameNewColor;
    public TextMeshProUGUI fameUpdateText;

    [Header("Lost Goods")]
    public GameObject lostGoods;
    public Color itemActiveColor;
    public Color itemLostColor;
    public TextMeshProUGUI lostItemsText;
    public LostItemDisplay carDisplay;
    public LostItemDisplay houseDisplay;
    public LostItemDisplay workDisplay;
    public LostItemDisplay familyDisplay;
    public LostItemDisplay friendsDisplay;

    [Header("Traits Update")]
    public GameObject traitUpdate;
    public TextMeshProUGUI mainTraitUpdateText;
    public TextMeshProUGUI firstTraitUpdateText;
    public TextMeshProUGUI secondTraitUpdateText;
    
    public void SetupInfos(NetworkPlayer player, List<LostItemData> lostItemDatas, int previousRank, int newRank, string firstNewTrait, string secondNewTrait)
    {
        lostGoods.SetActive(lostItemDatas.Count != 0);
        traitUpdate.SetActive(firstNewTrait != string.Empty);

        if(previousRank > newRank)
        {
            fameUpdateText.text = string.Format("Oh non ! Tu as perdu ta popularité, tu retombes au niveau {0}.", newRank);

            for(int i = 0; i < fameStars.Count; i++)
            {
                if(i < newRank)
                {
                    fameStars[i].color = fameActiveColor;
                }
                else
                {
                    fameStars[i].color = fameEmptyColor;
                }
            }
        }
        else if(previousRank < newRank)
        {
            fameUpdateText.text = string.Format("Bravo, tu as atteint le niveau {0} de popularité !", newRank);

            for (int i = 0; i < fameStars.Count; i++)
            {
                if (i < previousRank)
                {
                    fameStars[i].color = fameActiveColor;
                }
                else if(i < newRank)
                {
                    fameStars[i].color = fameNewColor;
                }
                else
                {
                    fameStars[i].color = fameEmptyColor;
                }
            }
        }
        
        if(lostItemDatas != null)
        {
            carDisplay.SetupInfos(player.car.level, itemActiveColor);
            houseDisplay.SetupInfos(player.house.level, itemActiveColor);
            workDisplay.SetupInfos(player.work.level, itemActiveColor);
            familyDisplay.SetupInfos(player.family.level, itemActiveColor);
            friendsDisplay.SetupInfos(player.friends.level, itemActiveColor);

            string lostItems = string.Empty;

            foreach(LostItemData lostItemData in lostItemDatas)
            {
                switch(lostItemData.itemType)
                {
                    case ItemType.Voiture:

                        lostItems += " / Ta voiture";
                        carDisplay.SetupInfos(lostItemData.previousLevel, lostItemData.newLevel, itemActiveColor, itemLostColor);
                        break;

                    case ItemType.Maison:

                        lostItems += " / Ta maison";
                        houseDisplay.SetupInfos(lostItemData.previousLevel, lostItemData.newLevel, itemActiveColor, itemLostColor);
                        break;

                    case ItemType.Travail:

                        lostItems += " / Ton travail";
                        workDisplay.SetupInfos(lostItemData.previousLevel, lostItemData.newLevel, itemActiveColor, itemLostColor);
                        break;

                    case ItemType.Famille:

                        lostItems += " / Ta famille";
                        familyDisplay.SetupInfos(lostItemData.previousLevel, lostItemData.newLevel, itemActiveColor, itemLostColor);
                        break;

                    case ItemType.Amis:

                        lostItems += " / Tes amis";
                        friendsDisplay.SetupInfos(lostItemData.previousLevel, lostItemData.newLevel, itemActiveColor, itemLostColor);
                        break;
                }
            }

            // Remove the first three char
            if(lostItems != string.Empty)
            {
                lostItemsText.text = lostItems.Remove(0, 3);
            }
        }

        if(firstNewTrait != string.Empty)
        {
            firstTraitUpdateText.text = firstNewTrait;
            secondTraitUpdateText.text = secondNewTrait;

            if(secondNewTrait != string.Empty)
            {
                mainTraitUpdateText.text = "Tu gagnes les traits de caractère suivants :";
            }
            else
            {
                mainTraitUpdateText.text = "Tu gagnes le trait de caractère suivant :";
            }
        }
    }
}
