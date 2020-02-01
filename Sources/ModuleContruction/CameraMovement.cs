using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraMovement : MonoBehaviour {

    [SerializeField] float sensitivyHorizontal;
    [SerializeField] float sensitivyVertical;
    [SerializeField] float sensitivyZoom;
    [SerializeField] float minFov;
    [SerializeField] float maxFov;
    Transform centerOfWorld;
    private float yaw, pitch;
    CinemachineVirtualCamera Editor_Camera;
    private ConstructionModuleMain refEditor;

    // Use this for initialization
    void Awake ()
	{
        centerOfWorld = GameObject.Find("CenterOfWorld").transform;
        yaw = pitch = 0.0f;
        Editor_Camera = gameObject.GetComponent<CinemachineVirtualCamera>();
        CameraZoomUpdate(0.8f);
        yaw = 120.0f;
        pitch = 0.0f;
        centerOfWorld.rotation = Quaternion.Euler(pitch, yaw, 0.0f);
    }

    private void Start()
    {
        refEditor = ConstructionModuleMain.GetSingleton();
    }

    void CameraRotationUpdateMouse()
    {
        yaw += sensitivyHorizontal * Input.GetAxis("Mouse X");
        pitch -= sensitivyVertical * Input.GetAxis("Mouse Y");
        pitch = Mathf.Clamp(pitch, -45.0f, 60.0f);

        centerOfWorld.rotation = Quaternion.Euler(pitch, yaw, 0.0f);
    }

    void CameraRotationUpdateJoystick()
    {
        yaw += sensitivyHorizontal * Input.GetAxis("View X");
        pitch -= sensitivyVertical * Input.GetAxis("View Y");
        pitch = Mathf.Clamp(pitch, -45.0f, 60.0f);

        centerOfWorld.rotation = Quaternion.Euler(pitch, yaw, 0.0f);
    }


    void CameraZoomUpdate(float value = 0.0f)
    {
        Vector3 toBeAdded = Vector3.zero;
        if (value == 0.0f)
        {
            toBeAdded = Editor_Camera.gameObject.transform.forward * Input.GetAxis("Mouse ScrollWheel") * sensitivyZoom;
        }
        else
        {
            toBeAdded -= Editor_Camera.gameObject.transform.forward * value * sensitivyZoom;
        }
        float distance = Vector3.Distance(toBeAdded + Editor_Camera.gameObject.transform.position, centerOfWorld.position);

        if (distance > 5.0f && distance < 25.0f)
        {
            Editor_Camera.gameObject.transform.position += toBeAdded;
        }
    }
	
	// Update is called once per frame
	void Update ()
	{
        if (refEditor.camState != ConstructionModuleMain.EditorCameraSTATE.FREE || refEditor.camState == ConstructionModuleMain.EditorCameraSTATE.SAVING)
        {
            return;
        }

        if (Input.GetKey(KeyCode.LeftAlt) && Input.GetMouseButton(0) || Input.GetMouseButton(2))
        { 
           CameraRotationUpdateMouse(); 
		}
        else if(Input.GetJoystickNames().Length > 0)
        {
            CameraRotationUpdateJoystick();
        }
        CameraZoomUpdate();
    }
}
