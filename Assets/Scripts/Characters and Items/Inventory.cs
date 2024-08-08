using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.ObjectModel;
using Unity.VisualScripting;

public class Inventory : MonoBehaviour
{
    public ItemEntity equippedItem;
    public ProfessionEntity equippedProfession;

    protected ItemEntity[] inventoryItems;
    public readonly ItemEntity[] InventoryItems;

    protected List<ItemEntity> itemsInConvoy;
    public readonly ReadOnlyCollection<ItemEntity> ItemsInConvoy;

    protected List<ProfessionEntity> equipableProfessions;
    public ReadOnlyCollection<ProfessionEntity> EquipablProfessions;

    public Inventory()
    {
        itemsInConvoy = new List<ItemEntity>();
        ItemsInConvoy = itemsInConvoy.AsReadOnly();

        equipableProfessions = new List<ProfessionEntity>();
        EquipablProfessions = equipableProfessions.AsReadOnly();

        inventoryItems = new ItemEntity[7];
        InventoryItems = inventoryItems.AsReadOnlyCollection() as ItemEntity[];
    }

    public void AddToConvoy(ItemEntity itemToAdd)
    { 
        itemsInConvoy.Add(itemToAdd);
    }

    public void RemoveFromConvoy(ItemEntity itemToRemove)
    {
        itemsInConvoy.Remove(itemToRemove);
    }
    
    //This is really messy and won't work, but is a placeholder for rn.
    public void TradeWithConvoy(ItemEntity itemToTakeFromConvoy, ItemEntity itemToRemoveFromInventory)
    {
        for (int i = InventoryItems.Length - 1; i >= 0; i--)
        {
            if (InventoryItems[i] == itemToRemoveFromInventory)
            {
                for (int j = ItemsInConvoy.Count - 1; i >= 0; i--)
                {
                    if (ItemsInConvoy[j] == itemToTakeFromConvoy)
                    {
                        itemsInConvoy[j] = itemToRemoveFromInventory;
                        inventoryItems[i] = itemToTakeFromConvoy;

                        return;
                    }
                }
            }
        }
    }

}
