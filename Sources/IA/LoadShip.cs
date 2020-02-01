using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class LoadShip : MonoBehaviour
{
    private static string[] arrayOfID = null;
    private static string LoadPath = null;
    private static int allyNumberCount = 0;
	// Use this for initialization
	void Awake()
	{
        if(LoadPath == null)
        {
            LoadPath = "PrefabSave/NoneMB/";
        }

        if(arrayOfID == null)
        {
            TextAsset asset = Resources.Load<TextAsset>(LoadPath + "ID");
            JObject data = JObject.Parse(asset.text);
            JArray IDs = data["uniqueID"].Value<JArray>();
            arrayOfID = new string[IDs.Count];
            for (int i = 0; i < arrayOfID.Length; i++)
            {
                arrayOfID[i] = IDs[i].Value<string>();
            }
        }

        int randValue = Random.Range(0, arrayOfID.Length);
        GameObject ship = LoadShipManagement.LoadShip("Ally_" + allyNumberCount, 1, LoadPath + arrayOfID[randValue]);
        allyNumberCount++;
        ship.tag = "Ally";
		ship.transform.parent = transform;
		ship.transform.localPosition = Vector3.zero;
		MeshCollider[] colliders = GetComponentsInChildren<MeshCollider>(true);
		foreach (MeshCollider mc in colliders)
		{
			mc.enabled = true;
		}
		/*MeshFilter[] filters = GetComponentsInChildren<MeshFilter>();
		foreach (MeshFilter mf in filters)
		{
			MeshCollider mesh = mf.gameObject.AddComponent<MeshCollider>();
			mesh.sharedMesh = mf.sharedMesh;
			mesh.inflateMesh = true;
			mesh.convex = true;
		}
		// UNTIL WE HAVE TEXTURES
		MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
		foreach (MeshRenderer mr in renderers)
		{
			mr.material.color = GetComponent<Enemy>() != null ? new Color(1f, 0.25f, 0f) : Color.blue;
		}*/
	}

	// Update is called once per frame
	void Update()
	{

	}
}
