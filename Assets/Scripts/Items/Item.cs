using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    private ItemType _itemType;

    private int _level;

    public Item(ItemType itemType)
    {
        _itemType = itemType;
        _level = 1;
    }
}

public enum ItemType
{
    House,
    Car,
    Work,
    Entourage
}
