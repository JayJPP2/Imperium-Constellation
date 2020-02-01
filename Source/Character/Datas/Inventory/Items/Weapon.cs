using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Weapon : Item
{
    public enum WeaponType
    {
        range,
        melee
    };

    static private Weapon[] arrayOfPrefab;

    public static Weapon[] ArrayOfPrefab
    {
        get
        {
            return arrayOfPrefab;
        }
        set
        {
            if(arrayOfPrefab == null)
            {
                arrayOfPrefab = value;
            }
            else
            {
                throw new System.InvalidOperationException("cannot reatribuate array of prefabs");
            }
        }
    }

    public Weapon(int weaponID) : base(ArrayOfPrefab[weaponID].name, 1, ArrayOfPrefab[weaponID].weight, ArrayOfPrefab[weaponID].type, InventoryManager.ItemsUniqueID.undefined)
    {
        durability = ArrayOfPrefab[weaponID].durability;
        damage = ArrayOfPrefab[weaponID].damage;
        staminaCost = ArrayOfPrefab[weaponID].staminaCost;
        typeWeapon = ArrayOfPrefab[weaponID].typeWeapon;
    }

    [SerializeField] protected float durability;
    [SerializeField] protected float damage;
    [SerializeField] protected float staminaCost;
    [SerializeField] protected WeaponType typeWeapon;

    public float Durability
    {
        get
        {
            return durability;
        }

        set
        {
            durability = value;
        }
    }

    public float Damage
    {
        get
        {
            return damage;
        }

        set
        {
            damage = value;
        }
    }

    public float StaminaCost
    {
        get
        {
            return staminaCost;
        }

        set
        {
            staminaCost = value;
        }
    }

    public WeaponType TypeWeapon
    {
        get
        {
            return typeWeapon;
        }

        set
        {
            typeWeapon = value;
        }
    }
}