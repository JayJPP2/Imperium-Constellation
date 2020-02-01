using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamTrailer : MonoBehaviour {

    [SerializeField] float sensHori;
    [SerializeField] float sensVerti;

    private float yaw, pitch;

    Vector3 screenPos;

	// Use this for initialization
	void Start () {
        sensHori = sensVerti = 8.0f;
        yaw = pitch = 0.0f;
}
	
	// Update is called once per frame
	void Update ()
    {
        transform.position += Input.GetAxis("Horizontal") * sensHori * transform.right;
        transform.position += Input.GetAxis("Vertical") * sensVerti * transform.forward;

        if(Input.GetMouseButton(2))
        {
            yaw += sensHori * Input.GetAxis("Mouse X");
            pitch -= sensVerti * Input.GetAxis("Mouse Y");
     
            transform.rotation = Quaternion.Euler(pitch, yaw, 0.0f);
        }
    }
}
