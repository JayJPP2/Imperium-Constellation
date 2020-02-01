using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLoadShip : MonoBehaviour {

	// Use this for initialization
	void Awake () {
		GameObject ship = LoadShipManagement.LoadShipOneBlock("PrefabSave/MB/Imperium_t1");
        ship.tag = "Enemy";
        ship.transform.parent = transform;
		ship.transform.localPosition = Vector3.zero;
	}

	// Update is called once per frame
	void Update () {
		
	}
}
