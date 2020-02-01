using UnityEngine;
using Newtonsoft.Json.Linq;


public abstract class VesselsPartStat
{
    public enum Faction
    {
        Human,
        Robot,
        Alien,
        Imperium
    }

    public string UniqueID { get; protected set;}

    public int Cost { get; protected set; }

    public Faction PartFaction { get; protected set; }

    public uint HullLife { get; protected set; }

    public VesselsModulePartData ModulePartData { get; protected set;}

    public VesselsPartStat()
    {
        this.Cost = 0;
    }

    public VesselsPartStat(JObject JSONObject, VesselsModulePartData assigned)
    {
        UniqueID = JSONObject["uniqueID"].Value<string>();
        Cost = JSONObject["cost"].Value<int>();
        PartFaction = (Faction)JSONObject["Faction"].Value<int>();
        HullLife = JSONObject["HullLife"].Value<uint>();
        ModulePartData = assigned;
    }

    public override string ToString()
    {
        return ("Tier: " + ModulePartData.Tier.ToString() + "\nType: " + ModulePartData.Type.ToString() +"\nCost: " + Cost + "\nFaction: " + PartFaction.ToString() + "\nHull Life: " + HullLife);
    }
}

public class CentralHubStat : VesselsPartStat
{
    public int ModuleMaxSlots { get; private set; }
    public int BatteryMaxSlots { get; private set; }

    public CentralHubStat(JObject JSONObject, VesselsModulePartData assigned) : base(JSONObject, assigned)
    {
        ModuleMaxSlots = assigned.ConnectionPoints.Count;
        BatteryMaxSlots = JSONObject["BatteryMaxSlots"].Value<int>();
    }

    public override string ToString()
    {
        return (base.ToString() + "\nModule Max Slots: " + ModuleMaxSlots + "\nBattery Max Slots: " + BatteryMaxSlots);
    }
}

public class ReactorStat : VesselsPartStat
{
    public uint Vmax { get; private set; }
    public uint TimeToVMax { get; private set; }
    public float Manouevrability { get; private set; }

    public ReactorStat(JObject JSONObject, VesselsModulePartData assigned) : base(JSONObject, assigned)
    {
        Vmax = JSONObject["Vmax"].Value<uint>();
        TimeToVMax = JSONObject["TimeToVMax"].Value<uint>();
        Manouevrability = JSONObject["Manouevrability"].Value<float>();
    }

    public override string ToString()
    {
        return (base.ToString() + "\nVmax: " + Vmax + "\nTime to Vmax: " + TimeToVMax + "\nManoeuvrability: " + Manouevrability);
    }
}

public class GeneratorStat : VesselsPartStat
{
    public uint ShieldStrengh { get; private set; }
    public float ShieldSize { get; private set; }

    public GeneratorStat(JObject JSONObject, VesselsModulePartData assigned) : base(JSONObject, assigned)
    {
        ShieldSize = JSONObject["ShieldSize"].Value<float>();
        ShieldStrengh = JSONObject["ShieldStrengh"].Value<uint>();
    }

    public override string ToString()
    {
        return (base.ToString() + "\nShieldSize: " + ShieldSize + "\nShieldStrengh: " + ShieldStrengh);
    }
}

public class WeaponStat : VesselsPartStat
{
    public enum WeaponType
    {
        Turret,
        Missile,
        MachineGun,
        Torpedo,
        Undefined
    }

    public uint FireBurst { get; private set; }
    public float FireRate { get; private set; }
    public float speedShot { get; private set; }
    public string projectilPrefabPath { get; private set; }
    public string projectilImpactPrefabPath { get; private set; }
    public uint range { get; private set; }
    public uint damage { get; private set; }
    public WeaponType weaponType { get; private set; }

    public WeaponStat(JObject JSONObject, VesselsModulePartData assigned) : base(JSONObject, assigned)
    {
        FireRate = JSONObject["FireRate"].Value<float>();
        FireBurst = JSONObject["FireBurst"].Value<uint>();
        speedShot = JSONObject["speedShot"].Value<float>();
        projectilPrefabPath = JSONObject["ProjectilePrefabPath"].Value<string>();
        projectilImpactPrefabPath = JSONObject["ProjectileImpactFX"].Value<string>();

        range = JSONObject["range"].Value<uint>();
        damage = JSONObject["damage"].Value<uint>();
        weaponType = (WeaponType)JSONObject["type"].Value<int>();
    }

    public override string ToString()
    {
        return (base.ToString() + "\nFireRate: " + FireRate + "\nFireBurst: " + FireBurst + "\nShot Speed: " + speedShot + "\nRange: " + range + "\nDamage: " + damage);
    }
}