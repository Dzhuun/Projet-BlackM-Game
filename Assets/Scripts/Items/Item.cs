using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    public int level;
    private ItemType _itemType;

    public Item(ItemType itemType, int currentLevel)
    {
        level = currentLevel;
        _itemType = itemType;
    }
}

public enum ItemType
{
    Maison,
    Voiture,
    Travail,
    Amis,
    Famille
}
