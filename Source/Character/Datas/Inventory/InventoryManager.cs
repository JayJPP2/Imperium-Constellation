using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public enum ItemsUniqueID
    {
        undefined = -1,
        littleRock,
        bow,
        arrow,
        axe,
        pickaxe,
        fist,
        spear,
        cudgel,
        blowpipe,
        thorn,
        wood,
        stone,
        iron,
        coal,
        silex,
        blood
    }

    static private int inventoryWidth = 5;
    static List<Item>[] Inventory;

    void Awake()
    {
        Item.ArrayOfPrefabItem = Save.LoadArray<Item>("itemProfiles.json");
        Ressource.ArrayOfPrefabResources = Save.LoadArray<Ressource>("ressourcesProfiles.json");
        Weapon.ArrayOfPrefab = Save.LoadArray<Weapon>("weaponProfiles.json");

        Inventory = new List<Item>[inventoryWidth];
        for (int i = 0; i < inventoryWidth; i++)
        {
            Inventory[i] = new List<Item>();
        }
    }

    void Update()
    {
       
    }

    //permet de trouver la liste la plus courte 
    //dans les X listes qui composent la largeur de
    //l'inventaire 
    //x: position dans le tabealu de listes
    //y: position dans la liste
    static private Vector2Int FindFirstFreeIndex()
    {
        Vector2Int listNb = new Vector2Int(0, -1);
        int lowerCount = Inventory[0].Count;
        for (int i = 1; i < inventoryWidth; i++)
        {
            if (Inventory[i].Count < lowerCount)
            {
                lowerCount = Inventory[i].Count;
                listNb.x = i;
            }

        }
        //get pos of first free index in least filled list
        listNb.y = Inventory[listNb.x].Count;
        return listNb;
    }

    //x: position dans le tabealu de listes
    //y: position dans la liste
    static public Vector2Int FindIndexOfRessource(Item _item)
    {
        Debug.Log(_item);
        Vector2Int indexOfRessource = new Vector2Int(-1, -1);
        for (int i = 0; i < inventoryWidth; i++)
        {
            indexOfRessource.y = Inventory[i].FindIndex(x => x.UniqueID == _item.UniqueID);

            if (indexOfRessource.y != -1)
            {
                indexOfRessource.x = i;
                i = inventoryWidth;
            }
        }

        return indexOfRessource;
    }

    static public Item GetItemInInventory(Item _item)
    {
        Vector2Int pos = FindIndexOfRessource(_item);
        if (pos.x != -1 && pos.y != -1)
        {
            return Inventory[pos.x][pos.y];
        }
        else
        {
            return null;
        }
    }

    //return false if no item found
    static public bool ItemIsInInventory(Item _item)
    {
        bool sortie = true;
        Vector2Int indexOfRessource = new Vector2Int(-1, -1);
        for (int i = 0; i < inventoryWidth; i++)
        {
            indexOfRessource.y = Inventory[i].FindIndex(x => string.Equals(x.Name, _item.Name));

            if (indexOfRessource.y != -1)
            {
                indexOfRessource.x = i;
                i = inventoryWidth;
            }
        }
        if (indexOfRessource.x == -1 && indexOfRessource.y == -1)
        {
            sortie = false;
        }
        return sortie;
    }


    static public bool InventoryHasItem(ItemsUniqueID uniqueID, out Item item)
    {
        Vector2Int indexOfRessource = new Vector2Int(-1, -1);
        for (int i = 0; i < inventoryWidth; i++)
        {
            indexOfRessource.y = Inventory[i].FindIndex(x => x.UniqueID == uniqueID);

            if (indexOfRessource.y != -1)
            {
                indexOfRessource.x = i;
                i = inventoryWidth;
            }
        }
        if (indexOfRessource.x == -1 && indexOfRessource.y == -1)
        {
            item = null;
            return false;
        }
        else
        {
            item = Inventory[indexOfRessource.x][indexOfRessource.y];
            return true;
        }
    }

    static public void AddItemToInventory(ItemsUniqueID uniqueID, uint quantity = 1)
    {
        Item newItem;
        switch (uniqueID)
        {
            case ItemsUniqueID.littleRock:
                newItem = new Weapon(0);
                break;
            case ItemsUniqueID.bow:
                newItem = new Weapon(1);
                break;
            case ItemsUniqueID.arrow:
                newItem = new Item(0, quantity);
                break;
            case ItemsUniqueID.axe:
                newItem = new Weapon(2);
                break;
            case ItemsUniqueID.pickaxe:
                newItem = new Weapon(3);
                break;
            case ItemsUniqueID.fist:
                newItem = new Weapon(4);
                break;
            case ItemsUniqueID.spear:
                newItem = new Weapon(5);
                break;
            case ItemsUniqueID.cudgel:
                newItem = new Weapon(6);
                break;
            case ItemsUniqueID.blowpipe:
                newItem = new Weapon(7);
                break;
            case ItemsUniqueID.thorn:
                newItem = new Item(1, quantity);
                break;
            case ItemsUniqueID.wood:
            case ItemsUniqueID.stone:
            case ItemsUniqueID.iron:
            case ItemsUniqueID.coal:
            case ItemsUniqueID.silex:
            case ItemsUniqueID.blood:
                newItem = new Ressource((Ressource.RessourceType)((int)uniqueID - 10), quantity);
                break;
            default:
                return;
        }

        newItem.UniqueID = uniqueID;
        AddItemInInventory(newItem);
    }


    static public void AddItemInInventory(Item _item)
    {
        Vector2Int AddRessourceInList = FindIndexOfRessource(_item);
        if ((_item.Type == Item.ItemType.ressources || _item.Type == Item.ItemType.ammunation) && AddRessourceInList.x != -1)
        {
            Inventory[AddRessourceInList.x][AddRessourceInList.y].Quantity += _item.Quantity;
        }
        else if (AddRessourceInList.x == -1)
        {
            AddRessourceInList = FindFirstFreeIndex();
            Inventory[AddRessourceInList.x].Add(_item);
        }
        else
        {
            Inventory[AddRessourceInList.x].Add(_item);
        }
    }

    static public bool RemoveItemInInventory(Item _item, uint _qtity = 1)
    {
        Vector2Int RemoveRessourceInList = FindIndexOfRessource(_item);
        if ((_item.Type == Item.ItemType.ressources || _item.Type == Item.ItemType.ammunation) && RemoveRessourceInList.x != -1)
        {
            Inventory[RemoveRessourceInList.x][RemoveRessourceInList.y].Quantity -= _qtity;
            if (Inventory[RemoveRessourceInList.x][RemoveRessourceInList.y].Quantity <= 0)
            {
                Inventory[RemoveRessourceInList.x].Remove(_item);
            }
        }
        else if (RemoveRessourceInList.x == -1)
        {
            return false;
        }
        else
        {
            Inventory[RemoveRessourceInList.x].Remove(_item);
        }
        return true;
    }
}
