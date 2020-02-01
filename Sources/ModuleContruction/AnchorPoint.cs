using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class AnchorPoint
{
    public enum AnchorType
    {
        undefined = -1,
        mono,
        duo,
        trio,
        quadrio,
        quinqueo
    }


    public VesselsModulePartData.TypeOfModule[] RestrictedConnection { get; private set; }

    public AnchorType TypeAnchor { get; private set; }

    public Vector3[] AnchorPoints { get; private set; }
    public Vector3[] AnchorPointsRotation { get; private set; }

    public VesselsModulePartData.TierPart? SpecificTier { get; private set; }

    public bool isUsed;

    public AnchorPoint()
    {
        this.TypeAnchor = AnchorType.undefined;
    }

    public void LoadFromJSON(JObject json)
    {
        if(json["specificTypeForAnchPoint"] == null || json["specificTypeForAnchPoint"].Value<string>() == "")
        {
             this.RestrictedConnection = null;
        }
        else
        {
            RestrictedConnection = new VesselsModulePartData.TypeOfModule[1];
            RestrictedConnection[0] = (VesselsModulePartData.TypeOfModule)json["specificTypeForAnchPoint"].Value<int>();
        }

        this.TypeAnchor = (AnchorType)json["AnchorType"].Value<int>();

        if(json["specificTier"] != null)
        {
            SpecificTier = (VesselsModulePartData.TierPart)json["specificTier"].Value<int>();
        }
        else
        {
            SpecificTier = null;
        }

        JArray pointsArray = json["points"].Value<JArray>();

        AnchorPoints = new Vector3[pointsArray.Count];
        AnchorPointsRotation = new Vector3[pointsArray.Count];

        for (int i = 0; i < pointsArray.Count; i++)
        {
            AnchorPoints[i].x = pointsArray[i]["transfromPosition"]["x"].Value<float>();
            AnchorPoints[i].y = pointsArray[i]["transfromPosition"]["y"].Value<float>();
            AnchorPoints[i].z = pointsArray[i]["transfromPosition"]["z"].Value<float>();

            if (pointsArray[i]["transfromRotation"] == null)
            {
                AnchorPointsRotation[i] = Vector3.zero;
            }
            else
            {
                AnchorPointsRotation[i].x = pointsArray[i]["transfromRotation"]["x"].Value<float>();
                AnchorPointsRotation[i].y = pointsArray[i]["transfromRotation"]["y"].Value<float>();
                AnchorPointsRotation[i].z = pointsArray[i]["transfromRotation"]["z"].Value<float>();
            }
        }

        this.isUsed = false;
    }
}
