using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Ressource : Item
{
    static private Ressource[] arrayOfPrefabResources;

    public static Ressource[] ArrayOfPrefabResources
    {
        get
        {
            return arrayOfPrefabResources;
        }
        set
        {
            if (arrayOfPrefabResources == null)
            {
                arrayOfPrefabResources = value;
            }
            else
            {
                throw new System.InvalidOperationException("cannot reatribuate array of prefabs");
            }
        }
    }


    public enum RessourceType
    {
        wood,
        stone,
        iron,
        coal,
        silex,
        blood
    }


    [SerializeField]
    protected RessourceType typeRessources;

    public Ressource(RessourceType type, uint quantity) : 
        base(ArrayOfPrefabResources[(int)type].Name,
        quantity,
        ArrayOfPrefabResources[(int)type].Weight,
        ArrayOfPrefabResources[(int)type].Type,
        (InventoryManager.ItemsUniqueID)type + 10)
    {
        typeRessources = type;
    }


    public RessourceType TypeRessources
    {
        get
        {
            return typeRessources;
        }

        set
        {
            typeRessources = value;
        }
    }
}
