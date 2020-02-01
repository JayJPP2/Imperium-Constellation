using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{

	[SerializeField] GameObject mainCamera;

	public bool usedFixedUpdate = true;

	private Transform target;

	private Ship ship;

	private void Start()
	{
		target = GameObject.FindGameObjectWithTag("CameraTarget").transform;
		ship = GameObject.FindGameObjectWithTag("Player").GetComponent<Ship>();
		endPos = target.transform.position;
		endRotation = target.transform.rotation;
		lastState = ship.GetInputState();

		float[] values = new float[32];

		for (int i = 0; i < 32; i++)
		{
			values[i] = 2500f;
		}
		values[10] = 0f;

		Camera.main.layerCullDistances = values;
	}

	/*private void Update()
	{
		if (!usedFixedUpdate)
			UpdateCamera();
	}*/

	/*private void FixedUpdate()
	{
		if (usedFixedUpdate)
			UpdateCamera();
	}*/

	private float lerpTimer = 5f;
	private const float LERP_TIME = 2f;

	private Vector3 endPos;
	private Quaternion endRotation;

	private Ship.State lastState;

	private Vector3 turnPos;

	private Vector3 up;
	private Vector3 forward;
	private Vector3 right;

	public void UpdateCamera()
	{
		lerpTimer += Time.deltaTime;
		if (ship.GetInputState() != lastState)
		{
			lastState = ship.GetInputState();
			lerpTimer = 0f;
			endPos = target.transform.position;
			endRotation = target.transform.rotation;
			switch (lastState)
			{
				case Ship.State.playerInputs:
					break;
				case Ship.State.playerRollLeft:
					break;
				case Ship.State.playerRollRight:
					break;
				case Ship.State.playerTurnDown:
					up = ship.transform.up;
					forward = ship.transform.forward;
					right = ship.transform.right;
					break;
				case Ship.State.playerTurnUp:
					up = ship.transform.up;
					forward = ship.transform.forward;
					right = ship.transform.right;
					break;
				default:
					break;
			}
		}

		if (target != null && ship != null)
		{
			switch (lastState)
			{
				case Ship.State.playerInputs:
					target.transform.position = ship.transform.position + ship.transform.up * 4.25f/* * ship.Throttle*/;
					target.transform.rotation = Quaternion.Lerp(endRotation, ship.transform.rotation, 4f * (lerpTimer / LERP_TIME) * (lerpTimer / LERP_TIME));
					break;
				case Ship.State.playerRollLeft:
					target.transform.position = Vector3.Lerp(endPos, ship.transform.position + ship.transform.up * 4.25f/* * ship.Throttle*/, 4f * lerpTimer / LERP_TIME);
					break;
				case Ship.State.playerRollRight:
					target.transform.position = Vector3.Lerp(endPos, ship.transform.position + ship.transform.up * 4.25f/* * ship.Throttle*/, 4f * lerpTimer / LERP_TIME);
					break;
				case Ship.State.playerTurnDown:
					target.transform.position = ship.transform.position + up * 4.25f/* * ship.Throttle*/;
					target.transform.forward = forward * Mathf.Cos(Mathf.PI * lerpTimer / 0.58f) + right * Mathf.Sin(Mathf.PI * lerpTimer / 0.58f);
					break;
				case Ship.State.playerTurnUp:
					target.transform.position = ship.transform.position + up * 4.25f/* * ship.Throttle*/;
					target.transform.forward = forward * Mathf.Cos(Mathf.PI * lerpTimer / 0.58f) - right * Mathf.Sin(Mathf.PI * lerpTimer / 0.58f);
					break;
				default:
					break;
			}
			if (mainCamera != null)
			{
				if (ship.GetInputState() == Ship.State.playerInputs)
				{
					if (Input.GetButton("PushRight"))
					{
						mainCamera.GetComponent<Cinemachine.CinemachineVirtualCamera>().enabled = false;
					}
					else
					{
						mainCamera.GetComponent<Cinemachine.CinemachineVirtualCamera>().enabled = true;
					}
				}
				else
				{
					mainCamera.GetComponent<Cinemachine.CinemachineVirtualCamera>().enabled = true;
				}
			}
		}
	}

}