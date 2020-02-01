using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipInput : MonoBehaviour
{
	public bool useKeyboard = true;

	[Space]
	[Range(-1, 1)]
	public float pitch;
	[Range(-1, 1)]
	public float yaw;
	[Range(-1, 1)]
	public float roll;
	[Range(-1, 1)]
	public float strafe;
	[Range(0, 1)]
	public float throttle;

	// How quickly the throttle reacts to input.
	private const float THROTTLE_SPEED = 0.5f;

	// Keep a reference to the ship this is attached to just in case.
	private Ship ship;

	private bool isT2;

	private void Awake()
	{
		ship = GetComponent<Ship>();
		isT2 = false;
		StartCoroutine(DetectTurret());
	}

	private IEnumerator DetectTurret()
	{
		yield return new WaitForEndOfFrame();
		isT2 = ship.GetTurret() != null;
	}

	private float manoeuverTimer = 0f;
	private const float MANOEUVER_DELAY = 2f;

	private void Update()
	{
		if (useKeyboard)
		{
			if (ship.GetInputState() == Ship.State.playerInputs)
			{
				manoeuverTimer += Time.deltaTime;

				pitch = Input.GetAxis("Vertical"); // Z and S to rotate up and down
				if (pitch < 0f && Input.GetButton("Manoeuvre") && manoeuverTimer > MANOEUVER_DELAY && !isT2)
				{
					ship.SetInputState(Ship.State.playerTurnUp);
					pitch = 0f;
					manoeuverTimer = 0f;
				}
				else if (pitch > 0f && Input.GetButton("Manoeuvre") && manoeuverTimer > MANOEUVER_DELAY && !isT2)
				{
					ship.SetInputState(Ship.State.playerTurnDown);
					pitch = 0f;
					manoeuverTimer = 0f;
				}
				roll = -Input.GetAxis("Horizontal") * 0.1f; // Q and D to roll
				if (roll < 0f && Input.GetButton("Manoeuvre") && manoeuverTimer > MANOEUVER_DELAY && !isT2)
				{
					ship.SetInputState(Ship.State.playerRollRight);
					roll = 0f;
					manoeuverTimer = 0f;
				}
				else if (roll > 0f && Input.GetButton("Manoeuvre") && manoeuverTimer > MANOEUVER_DELAY && !isT2)
				{
					ship.SetInputState(Ship.State.playerRollLeft);
					roll = 0f;
					manoeuverTimer = 0f;
				}
				yaw = Input.GetAxis("Yaw") * 0.5f;
				if (Input.GetAxisRaw("Forward") != 0f)
				{
					throttle = Mathf.MoveTowards(throttle, 1f, Time.deltaTime * THROTTLE_SPEED);
				}
				else
				{
					if (Input.GetAxisRaw("Backward") != 0f)
					{
						throttle = Mathf.MoveTowards(throttle, 0f, Time.deltaTime * THROTTLE_SPEED);
					}
					else
					{
						if (throttle > 0.25f)
						{
							throttle = Mathf.MoveTowards(throttle, 0.25f, Time.deltaTime * THROTTLE_SPEED / 5f);
						}
					}
				}
				if (throttle < 0.25f)
				{
					throttle = 0.25f;
				}
			}
		}
		else
		{
			pitch = Input.GetAxis("Vertical");
			yaw = Input.GetAxis("Horizontal");

			strafe = 0.0f;
		}
	}
}