using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public GameObject itemDisplaysHandler;
    public GameObject shopItemDisplaysHandler;
    private ItemDisplay[] itemDisplays;
    private ShopItemDisplay[] shopItemDisplays;

    public static ShopManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void UpdateDisplays()
    {
        if(itemDisplays == null)
        {
            itemDisplays = itemDisplaysHandler.GetComponentsInChildren<ItemDisplay>();
        }

        foreach(ItemDisplay itemDisplay in itemDisplays)
        {
            itemDisplay.SetupInfos();
        }

        if(shopItemDisplays == null)
        {
            shopItemDisplays = shopItemDisplaysHandler.GetComponentsInChildren<ShopItemDisplay>();
        }

        foreach (ShopItemDisplay shopItemDisplay in shopItemDisplays)
        {
            shopItemDisplay.SetupInfos();
        }
    }

    public void ResetShopDisplays()
    {
        if (shopItemDisplays == null)
        {
            shopItemDisplays = shopItemDisplaysHandler.GetComponentsInChildren<ShopItemDisplay>();
        }

        foreach (ShopItemDisplay shopItemDisplay in shopItemDisplays)
        {
            shopItemDisplay.ShowItemDisplay();
        }
    }
}
