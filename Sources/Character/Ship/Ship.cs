using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class Ship : MonoBehaviour
{
	public bool isPlayer = false;

	public enum State
	{
		playerInputs,
		playerRollLeft,
		playerRollRight,
		playerTurnUp,
		playerTurnDown
	}

	private State inputState;
	private float manoeuverTimer;

	private Vector3 rightDirection;
	private Vector3 upDirection;
	private Vector3 forwardDirection;

	private Quaternion endRotation;
	private Quaternion goalRotation;

	public State GetInputState()
	{
		return inputState;
	}

	private State lastState;

	public void SetInputState(State newState)
	{
		manoeuverTimer = 0f;
		lastState = inputState;
		inputState = newState;
		endRotation = transform.rotation;
		if (inputState != State.playerInputs)
		{
			rightDirection = transform.right;
			upDirection = transform.up;
			forwardDirection = transform.forward;
			if (inputState == State.playerRollLeft || inputState == State.playerRollRight)
			{
				goalRotation = transform.rotation;
			}
			else
			{
				goalRotation.SetLookRotation(-forwardDirection, upDirection);
			}
		}
	}

	private ShipInput input;
	private ShipPhysics physics;

	public static Ship PlayerShip { get { return playerShip; } }
	private static Ship playerShip;

	public bool UsingMouseInput { get { return input.useKeyboard; } }
	public Vector3 Velocity { get { return physics.Rigidbody.velocity; } }
	public Vector3 VelocityNormalized { get { return Velocity.normalized; } }
	public float Throttle { get { return input.throttle; } }
	public void TakeOutZoneDamage() { health -= 5.0f; if (health <= 0f) Death(); }

	float health = 200f;
	float maxHealth = 200f;
	float collisionProtection = 10f;
	float shootProtection = 2f;


	private bool alive;

	public bool IsAlive()
	{
		return alive;
	}

	public float GetHealth()
	{
		return health;
	}

	public float GetMaxHealth()
	{
		return maxHealth;
	}

	public float GetRelativeHealth()
	{
		return health / maxHealth;
	}

	public void InitColliders()
	{
		MeshCollider[] colliders = GetComponentsInChildren<MeshCollider>(true);
		foreach (MeshCollider mc in colliders)
		{
			mc.enabled = true;
		}
	}

	private const float targetLimitZMin = 50f;
	private const float targetLimitZMax = 1000f;
	private const float targetLimitSide = 100f;

	private List<GameObject> possibleTargets;
	private GameObject selectedTarget;

	public GameObject GetSelectedTarget()
	{
		return selectedTarget;
	}

	private float lockTimer;
	private const float LOCK_TIME = 2f;
	public float getLockTimerNormalized()
	{
		return lockTimer / LOCK_TIME;
	}

	public void CheckEnemyDeath(GameObject enemy)
	{
		if (possibleTargets.Contains(enemy))
		{
			if (selectedTarget == enemy)
			{
				selectedTarget = possibleTargets[(possibleTargets.IndexOf(selectedTarget) + 1) % possibleTargets.Count];
				lockTimer = 0f;
			}
			possibleTargets.Remove(enemy);
		}
	}

	public IEnumerator GetPossibleTargets(float timer)
	{
		while (true)
		{
			possibleTargets.Clear();
			GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
			foreach (GameObject enemy in enemies)
			{
				if (enemy.transform.parent == null)
				{
					Vector3 vector = enemy.transform.position - transform.position;
					float X = Vector3.Dot(vector, transform.right);
					float Y = Vector3.Dot(vector, transform.up);
					float Z = Vector3.Dot(vector, transform.forward);
					if (Z > targetLimitZMin && Z < targetLimitZMax
						&& X > -targetLimitSide && X < targetLimitSide
						&& Y > -targetLimitSide && Y < targetLimitSide)
					{
						bool added = false;
						float shift = X * X + Y * Y;
						Vector3 temp;
						float tempX;
						float tempY;
						for (int i = 0; i < possibleTargets.Count && !added; i++)
						{
							temp = possibleTargets[i].transform.position - transform.position;
							tempX = Vector3.Dot(temp, transform.right);
							tempY = Vector3.Dot(temp, transform.up);
							if (shift < tempX * tempX + tempY * tempY)
							{
								possibleTargets.Insert(i, enemy);
								added = true;
							}
						}
						if (!added)
						{
							possibleTargets.Add(enemy);
						}
					}
				}
			}
			if (possibleTargets.Count > 0)
			{
				if (selectedTarget == null || !possibleTargets.Contains(selectedTarget))
				{
					selectedTarget = possibleTargets[0];
					lockTimer = 0f;
				}
			}
			else
			{
				selectedTarget = null;
			}
			yield return new WaitForSeconds(timer);
		}
	}

	private GameObject turret = null;

	public GameObject GetTurret()
	{
		return turret;
	}

	CameraBehaviour mainCamera = null;

	private void Start()
	{
		input = GetComponent<ShipInput>();
		physics = GetComponent<ShipPhysics>();

		mainCamera = Camera.main.GetComponent<CameraBehaviour>();

		alive = true;

		InitColliders();

		inputState = State.playerInputs;
		lastState = State.playerInputs;

		deathUI.SetActive(false);
		transform.position = GameObject.Find("worldCenter").transform.position - new Vector3(0f, 0f, GameObject.Find("worldCenter").GetComponent<outZone>().GetRadius() * 0.95f);
		possibleTargets = new List<GameObject>();
		selectedTarget = null;
		lockTimer = 0f;

		GameObject[] allTurrets = GameObject.FindGameObjectsWithTag("TurretT2");
		foreach (GameObject go in allTurrets)
		{
			if (go.transform.root.gameObject == gameObject)
			{
				turret = go;
				GetComponentInChildren<WeaponShootManager>().AddTurret(turret);
				maxHealth = 2000f;
				health = maxHealth;
				break;
			}
		}
	}

	private const float ROLL_VALUE = 540f;
	private const float ROLL_LIMIT = 360f;
	private const float ROLL_SHIFT = 40f;

	private const float TURN_VALUE = 300f;
	private const float TURN_LIMIT = 180f;

	public Vector3 GetRollDistance()
	{
		return rightDirection * ROLL_SHIFT * manoeuverTimer;
	}

	private float targetTimer = 0.5f;

	private void FixedUpdate()
	{
		manoeuverTimer += Time.deltaTime;
		if (alive)
		{
			switch (inputState)
			{
				case State.playerInputs:
					if (lastState != State.playerInputs && manoeuverTimer < 0.5f)
					{
						if (lastState == State.playerRollLeft || lastState == State.playerRollRight)
						{
							transform.rotation = Quaternion.Lerp(endRotation, goalRotation, manoeuverTimer * 2f);
						}
						else
						{
							transform.rotation = Quaternion.Lerp(endRotation, goalRotation, manoeuverTimer * 2f);
						}
					}
					else
					{
						physics.SetPhysicsInput(new Vector3(input.strafe, 0.0f, input.throttle), new Vector3(input.pitch, input.yaw, input.roll));
					}
					break;
				case State.playerRollLeft:
					transform.Rotate(transform.forward * ROLL_VALUE * Time.deltaTime, Space.World);
					if (manoeuverTimer * ROLL_VALUE > ROLL_LIMIT)
					{
						transform.Rotate(transform.forward * (manoeuverTimer * ROLL_VALUE - ROLL_LIMIT), Space.World);
						SetInputState(State.playerInputs);
					}
					else
					{
						transform.position -= rightDirection * ROLL_SHIFT * Time.deltaTime;
					}
					break;
				case State.playerRollRight:
					transform.Rotate(transform.forward * -ROLL_VALUE * Time.deltaTime, Space.World);
					if (manoeuverTimer * ROLL_VALUE > ROLL_LIMIT)
					{
						transform.Rotate(transform.forward * (ROLL_LIMIT - manoeuverTimer * ROLL_VALUE), Space.World);
						SetInputState(State.playerInputs);
					}
					else
					{
						transform.position += rightDirection * ROLL_SHIFT * Time.deltaTime;
					}
					break;
				case State.playerTurnUp:
					transform.Rotate(rightDirection * -TURN_VALUE * Time.deltaTime, Space.World);
					transform.Rotate(transform.forward * TURN_VALUE * Time.deltaTime, Space.World);
					if (manoeuverTimer * TURN_VALUE > TURN_LIMIT)
					{
						transform.Rotate(rightDirection * (TURN_LIMIT - manoeuverTimer * TURN_VALUE), Space.World);
						transform.Rotate(transform.forward * (manoeuverTimer * TURN_VALUE - TURN_LIMIT), Space.World);
						SetInputState(State.playerInputs);
					}
					break;
				case State.playerTurnDown:
					transform.Rotate(rightDirection * TURN_VALUE * Time.deltaTime, Space.World);
					transform.Rotate(transform.forward * TURN_VALUE * Time.deltaTime, Space.World);
					if (manoeuverTimer * TURN_VALUE > TURN_LIMIT)
					{
						transform.Rotate(rightDirection * (TURN_LIMIT - manoeuverTimer * TURN_VALUE), Space.World);
						transform.Rotate(transform.forward * (TURN_LIMIT - manoeuverTimer * TURN_VALUE), Space.World);
						SetInputState(State.playerInputs);
					}
					break;
			}

			if (isPlayer)
				playerShip = this;

			lockTimer += Time.deltaTime;
			targetTimer += Time.deltaTime;
			if (Input.GetAxisRaw("ChangeTarget") > 0f && selectedTarget != null && targetTimer > 0.5f)
			{
				selectedTarget = possibleTargets[(possibleTargets.IndexOf(selectedTarget) + 1) % possibleTargets.Count];
				if (possibleTargets.Count > 1)
				{
					lockTimer = 0f;
				}
				targetTimer = 0f;
			}
			if (Input.GetAxisRaw("ChangeTarget") < 0f && selectedTarget != null && targetTimer > 0.5f)
			{
				selectedTarget = possibleTargets[(possibleTargets.IndexOf(selectedTarget) > 0 ? possibleTargets.IndexOf(selectedTarget) - 1 : possibleTargets.Count - 1) % possibleTargets.Count];
				if (possibleTargets.Count > 1)
				{
					lockTimer = 0f;
				}
				targetTimer = 0f;
			}
		}
		mainCamera.UpdateCamera();
	}

	private void OnCollisionEnter(Collision collision)
	{
		Vector3 velocity = GetComponent<Rigidbody>().velocity;
		if (collision.gameObject.GetComponent<Rigidbody>())
		{
			velocity -= collision.gameObject.GetComponent<Rigidbody>().velocity;
		}
		health -= velocity.magnitude / collisionProtection;
		if (health <= 0f)
		{
			Death();
		}
	}

	public void TakeShootDamage(float damage)
	{
		health -= damage / shootProtection;
		if (health <= 0f)
		{
			Death();
		}
	}

	IEnumerator ApplyDeath(float time)
	{
		yield return new WaitForSeconds(time);
		SceneManager.LoadScene("ModuleCreation");
	}

	[SerializeField] GameObject deathUI;
	[SerializeField] GameObject deathUI2;

	private void Death()
	{
		StartCoroutine(ApplyDeath(3f));
		alive = false;
		GetComponent<ShipInput>().enabled = false;
		GetComponent<ShipPhysics>().enabled = false;
		GetComponent<Rigidbody>().Sleep();
		foreach (MeshRenderer mr in GetComponentsInChildren<MeshRenderer>())
		{
			mr.enabled = false;
		}
		foreach (ParticleSystem ps in GetComponentsInChildren<ParticleSystem>())
		{
			ParticleSystem.EmissionModule em = ps.emission;
			em.enabled = false;
		}
		foreach (TrailRenderer tr in GetComponentsInChildren<TrailRenderer>())
		{
			tr.enabled = false;
		}
		foreach (WeaponShootManager wsm in GetComponentsInChildren<WeaponShootManager>())
		{
			wsm.enabled = false;
		}
		deathUI.SetActive(true);
		deathUI2.SetActive(true);
	}
}