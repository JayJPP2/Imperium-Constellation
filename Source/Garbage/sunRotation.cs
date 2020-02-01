using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sunRotation : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        this.gameObject.transform.Rotate(Vector3.up, 15.0f*Time.deltaTime);
	}
}
