﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    public int level;
    private ItemType _itemType;

    public Item(ItemType itemType)
    {
        level = 1;
        _itemType = itemType;
    }
}

public enum ItemType
{
    Maison,
    Voiture,
    Travail,
    Entourage
}
