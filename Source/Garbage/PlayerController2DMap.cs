using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController2DMap : MonoBehaviour {

    Rigidbody rb;

    [SerializeField] public float speed;
    Transform parent;

    // Use this for initialization
    void Start () {

        rb = gameObject.transform.parent.GetComponent<Rigidbody>();
    }
	
	// Update is called once per frame
	void Update () {
        Vector3 velocity = rb.velocity;
        velocity += transform.forward * (Input.GetAxisRaw("Vertical") * speed);
        velocity += transform.right * (Input.GetAxisRaw("Horizontal") * speed);
        //velocity = transform.up * (Input.GetAxisRaw("Horizontal") * speed);
        rb.velocity = velocity;
    }
}
