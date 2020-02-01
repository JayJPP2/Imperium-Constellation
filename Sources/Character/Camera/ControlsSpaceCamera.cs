using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsSpaceCamera : MonoBehaviour {

    float forwardSpeed = 100f;
    float sideSpeed = 75f;
    float upSpeed = 75f;

    float sensitivity = 360f;

    float rotateSpeed = 7.5f;

    float stopLimit = 50f;

    // Use this for initialization
    void Start () {
        GetComponent<Rigidbody>().drag = 0f;
        GetComponent<Rigidbody>().angularDrag = 5f;
        GetComponent<Rigidbody>().useGravity = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update () {
        transform.Rotate(Vector3.up, Input.GetAxis("Mouse X") * Time.deltaTime * sensitivity);
        transform.Rotate(Vector3.right, -Input.GetAxis("Mouse Y") * Time.deltaTime * sensitivity);

        if (Input.GetKey(KeyCode.Space))
        {
            GetComponent<Rigidbody>().AddForce(transform.up * upSpeed);
        }
        if (Input.GetKey(KeyCode.C))
        {
            GetComponent<Rigidbody>().AddForce(transform.up * -upSpeed);
        }
        if (Input.GetKey(KeyCode.Z))
        {
            GetComponent<Rigidbody>().AddForce(transform.forward * forwardSpeed);
        }
        if (Input.GetKey(KeyCode.S))
        {
            GetComponent<Rigidbody>().AddForce(transform.forward * -forwardSpeed);
        }
        if (Input.GetKey(KeyCode.Q))
        {
            GetComponent<Rigidbody>().AddForce(transform.right * -sideSpeed);
        }
        if (Input.GetKey(KeyCode.D))
        {
            GetComponent<Rigidbody>().AddForce(transform.right * sideSpeed);
        }
        if (Input.GetKey(KeyCode.A))
        {
            GetComponent<Rigidbody>().AddTorque(transform.forward * rotateSpeed);
        }
        if (Input.GetKey(KeyCode.E))
        {
            GetComponent<Rigidbody>().AddTorque(transform.forward * -rotateSpeed);
        }
        if (Input.GetKey(KeyCode.X))
        {
            if (GetComponent<Rigidbody>().velocity.magnitude < 1f)
            {
                GetComponent<Rigidbody>().velocity = Vector3.zero;
            }
            else if (GetComponent<Rigidbody>().velocity.magnitude < stopLimit)
            {
                GetComponent<Rigidbody>().velocity *= 0.9f;
            }
        }
        Debug.Log(GetComponent<Rigidbody>().velocity.magnitude);
    }
}
