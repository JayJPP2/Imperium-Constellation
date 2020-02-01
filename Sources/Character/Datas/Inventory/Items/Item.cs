using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Item 
{
    static private Item[] arrayOfPrefabItem;

    public static Item[] ArrayOfPrefabItem
    {
        get
        {
            return arrayOfPrefabItem;
        }
        set
        {
            if (arrayOfPrefabItem == null)
            {
                arrayOfPrefabItem = value;
            }
            else
            {
                throw new System.InvalidOperationException("cannot reatribuate array of prefabs");
            }
        }
    }

    public enum ItemType
    {
        weapon,
        armor,
        tool,
        ressources,
        consumable,
        ammunation
    }

    [SerializeField] protected string name;
    [SerializeField] protected uint quantity;
    [SerializeField] protected float weight;
    [SerializeField] protected ItemType type;
    [SerializeField] public string systemType;
    [SerializeField] protected InventoryManager.ItemsUniqueID uniqueID;

    public Item(int uniqueID, uint quantity = 1)
    {
        try
        {
            name = arrayOfPrefabItem[uniqueID].name;
        }
        catch(Exception e)
        {
            throw e;
        }
        this.quantity = quantity;
        weight = arrayOfPrefabItem[uniqueID].weight;
        type = arrayOfPrefabItem[uniqueID].type;

    }

    protected Item(string name, uint quantity, float weight, ItemType type, InventoryManager.ItemsUniqueID uniqueID = InventoryManager.ItemsUniqueID.undefined)
    {
        this.name = name;
        this.quantity = quantity;
        this.weight = weight;
        this.type = type;
        this.UniqueID = uniqueID;
        systemType = GetType().Name;
    }

    protected Item()
    {
        systemType = GetType().Name;
    }


    public string Name
    {
        get
        {
            return name;
        }

        set
        {
            name = value;
        }
    }

    public uint Quantity
    {
        get
        {
            return quantity;
        }

        set
        {
            quantity = value;
            if(quantity < 0)
            {
                quantity = 0;
            }
        }
    }

    public float Weight
    {
        get
        {
            return weight;
        }

        set
        {
            weight = value;
        }
    }

    public ItemType Type
    {
        get
        {
            return type;
        }

        set
        {
            type = value;
        }
    }

    public InventoryManager.ItemsUniqueID UniqueID
    {
        get
        {
            return uniqueID;
        }
        set
        {
            uniqueID = value;
        }
    }
}
