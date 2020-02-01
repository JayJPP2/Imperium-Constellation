using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour {

	private Vector3 rotation;

    // Use this for initialization
    void Start()
    {
		Vector3 direction = new Vector3(Random.Range(-1f,1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
		GetComponent<Rigidbody>().velocity = direction * Random.Range(10f, 100f) / GetComponent<Rigidbody>().mass;
		rotation = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * Random.Range(5f, 60f) / GetComponent<Rigidbody>().mass;
		GetComponent<Rigidbody>().mass *= transform.localScale.x * transform.localScale.y * transform.localScale.z;
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(rotation * Time.deltaTime);
	}
}
