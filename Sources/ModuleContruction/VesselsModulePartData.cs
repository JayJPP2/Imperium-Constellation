using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;


public class VesselsModulePartData
{
    public enum TypeOfModule
    {
        CentralHub,
        Reactor,
        Generator,
        Weapon,
        ModuleCount
    }

    public enum TierPart
    {
        Tier_0,
        Tier_1,
        Tier_2,
        TierCount
    }

    public string UniqueID { get; private set; }

    public string PrefabPath { get; private set; }

    public string DisplayName { get; private set; }

    public string DescriptionParts { get; private set; }

    public VesselsPartStat PartStat { get; private set; }

    public TypeOfModule Type { get; private set; }

    public List<AnchorPointData> ConnectionPoints { get; private set; }

    public AnchorPoint.AnchorType typeOfAnchor { get; private set; }

    public Texture2D miniatureImageTexture { get; private set; }

    public Sprite miniaturePreviewTexture { get; private set; }

    public Vector3? fireCamPosition { get; private set; }
    public Quaternion? fireCamRotation { get; private set; }

    public TierPart Tier { get; private set; }

    public VesselsModulePartData()
    {
        this.UniqueID = "Undefined";
        PartStat = null;
        ConnectionPoints = null;
        miniatureImageTexture = null;
        fireCamPosition = null;
        fireCamRotation = null;
    }

    public void LoadFromJSON(JObject json, TypeOfModule type, string path)
    {
        try
        {
            this.UniqueID = json["uniqueID"].Value<string>();
            this.PrefabPath = path;
            this.DisplayName = json["displayName"].Value<string>();
            this.DescriptionParts = json["descriptionParts"].Value<string>();
            Tier = (TierPart)json["tier"].Value<int>();
            this.Type = type;
            typeOfAnchor = json["connecticType"] == null ? AnchorPoint.AnchorType.undefined : (AnchorPoint.AnchorType)json["connecticType"].Value<int>();

            if (json["AnchorsPoint"] != null)
            { 
                ConnectionPoints = new List<AnchorPointData>();
                JArray AnchorsPointsData = json["AnchorsPoint"].Value<JArray>();
                if(AnchorsPointsData.Count > 1)
                {
                    for (int i = 0; i < AnchorsPointsData.Count; i++)
                    {
                        AnchorPointData pointData = new AnchorPointData();
                        pointData.LoadFromJSON(AnchorsPointsData[i].Value<JObject>());
                        ConnectionPoints.Add(pointData);
                    }
                }

                miniatureImageTexture = Resources.Load<Texture2D>(json["miniaturePreviewPath"].Value<string>());
            }

            if(json["miniaturelittlePreviewPath"] != null)
            {
                miniaturePreviewTexture = Resources.Load<Sprite>(json["miniaturelittlePreviewPath"].Value<string>());
            }

            if(json["firecamera"] != null)
            {
                fireCamPosition = new Vector3(
                    json["firecamera"]["position"]["x"].Value<float>(),
                    json["firecamera"]["position"]["y"].Value<float>(),
                    json["firecamera"]["position"]["z"].Value<float>()
                    );

                fireCamRotation = new Quaternion(
                    json["firecamera"]["rotation"]["x"].Value<float>(),
                    json["firecamera"]["rotation"]["y"].Value<float>(),
                    json["firecamera"]["rotation"]["z"].Value<float>(),
                    json["firecamera"]["rotation"]["w"].Value<float>()
                    );
            }
        }
        catch(Exception e)
        {
            throw (e);
        }
    }


    public void LinkPartStat(VesselsPartStat toBeLinked)
    {
        PartStat = toBeLinked;
    }

    public override string ToString()
    {
        return ("\nUniqueID : " + UniqueID + "\nprefabPath : " + PrefabPath + "\ndisplayName : " + DisplayName + "\ndescriptionParts : " + DescriptionParts);
    }

}