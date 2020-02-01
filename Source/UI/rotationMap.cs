using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotationMap : MonoBehaviour {


    [SerializeField] float localSpeedRotation = 0.05f;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Rotation();

    }

    void Rotation()
    {
        transform.Rotate(Vector3.forward, (360.0f * Time.deltaTime) * localSpeedRotation);
    }
}
