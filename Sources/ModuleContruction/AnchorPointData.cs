using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public class AnchorPointData
{
    public VesselsModulePartData.TypeOfModule[] typeAllowed { get; private set; }
    public List<AnchorPoint> points { get; private set; }

    public Vector3 CameraForAnchorViewPosition { get; private set; }
    public Quaternion CameraForAnchorViewQuaternion { get; private set; }
    public Vector3 MiniatureLocalPosition { get; private set; }
    public Vector3 MiniatureLocalPosition_2 { get; private set; }

    public void LoadFromJSON(JObject AnchorData)
    {
        VesselsModulePartData.TypeOfModule[] typeAllowed = AnchorData["typesOfPart"].Value<JArray>().ToObject<VesselsModulePartData.TypeOfModule[]>();

        JObject TransformCamera = AnchorData["preview_CameraTransform"].Value<JObject>();

        if (TransformCamera != null)
        {
            CameraForAnchorViewPosition = new Vector3(
                TransformCamera["position"]["x"].Value<float>(),
                TransformCamera["position"]["y"].Value<float>(),
                TransformCamera["position"]["z"].Value<float>());
            CameraForAnchorViewQuaternion = new Quaternion(
                TransformCamera["rotation"]["x"].Value<float>(),
                TransformCamera["rotation"]["y"].Value<float>(),
                TransformCamera["rotation"]["z"].Value<float>(),
                TransformCamera["rotation"]["w"].Value<float>());
        }

        JArray points = AnchorData["points"].Value<JArray>();

        this.typeAllowed = typeAllowed;
        this.points = new List<AnchorPoint>();
        for (int j = 0; j < points.Count; j++)
        {
            AnchorPoint newPoints = new AnchorPoint();
            newPoints.LoadFromJSON(points[j].Value<JObject>());
            this.points.Add(newPoints);
        }

        if(AnchorData["miniaturePreview_Position"] != null)
        {
            MiniatureLocalPosition = new Vector3(
                AnchorData["miniaturePreview_Position"]["x"].Value<float>(),
                AnchorData["miniaturePreview_Position"]["y"].Value<float>(),
                AnchorData["miniaturePreview_Position"]["z"].Value<float>());
        }

        MiniatureLocalPosition_2 = Vector3.zero;

        if (AnchorData["miniaturePreview_Position_2"] != null)
        {
            MiniatureLocalPosition_2 = new Vector3(
                AnchorData["miniaturePreview_Position_2"]["x"].Value<float>(),
                AnchorData["miniaturePreview_Position_2"]["y"].Value<float>(),
                AnchorData["miniaturePreview_Position_2"]["z"].Value<float>());
        }
    }
}