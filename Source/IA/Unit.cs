using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
	private enum State
	{
		FollowLeader,
		Chase,
		TakeAdvantage,
		Flee,
		Dead
	}

	private WeaponShootManager WSMRef;
	private State state;

	private bool outOfZone;

	private List<MeshRenderer> meshOfShipOriginalForm;
	private GameObject ExplosionGOFX;
	private ParticleSystem[] particleSystems;
	private GameObject WreckParent;
	private AudioSource ExplosionSound;
	private float timer;

	public void InitState()
	{
		state = State.FollowLeader;
	}

	public void FightState()
	{
		state = State.TakeAdvantage;
	}

	private float speed = 80f;
	public float GetSpeed()
	{
		return speed;
	}

	private GameObject target;
	private Vector3 targetPos;
	private Rigidbody rb;

	private float targetTimer;
	private float targetMaxTimer;

	private const float MIN_TIME = 2f;
	private const float MAX_TIME = 5f;

	private bool targetPlayer;

	public void SetTarget(GameObject _target, bool _targetPlayer = false)
	{
		target = _target;
		targetTimer = 0f;
		targetPlayer = _targetPlayer;
		targetMaxTimer = Random.Range(MIN_TIME, MAX_TIME);
	}

	public GameObject GetTarget()
	{
		return target;
	}

	public void SetTargetPos(Vector3 _targetPos)
	{
		targetPos = _targetPos;
	}

	private UnitType type;

	public void SetUnitType(UnitType newType)
	{
		type = newType;
	}

	public UnitType GetUnitType()
	{
		return type;
	}

	private Squad squad;

	public void SetSquad(Squad newSquad)
	{
		squad = newSquad;
	}

	public Squad GetSquad()
	{
		return squad;
	}

	public float health { get; private set; }

	private void Awake()
	{
		rb = GetComponent<Rigidbody>();
	}

	float shootProtection = 2f;
	float collisionProtection = 10f;

	private bool isAlive = true;

	public bool IsAlive()
	{
		return isAlive;
	}

	public void Init()
	{
		health = 100f;
		type = UnitType.UnitT1;
		squad = null;
		state = State.FollowLeader;
		outOfZone = false;
		targetPos = transform.position;
		target = null;
		StartCoroutine(ChangeRandom());
		WSMRef = gameObject.transform.GetChild(0).gameObject.GetComponentInChildren<WeaponShootManager>();
		if (WSMRef != null)
		{
			WSMRef.SetIA();
			WSMRef.GetShipReference(null);
		}

        meshOfShipOriginalForm = new List<MeshRenderer>();

        WreckParent = transform.GetChild(0).GetChild(1).gameObject;

		if (WreckParent.name.Contains("Connection"))
		{
			WreckParent = transform.GetChild(0).GetChild(0).GetChild(0).GetChild(1).gameObject;
			ExplosionGOFX = transform.GetChild(0).GetChild(0).GetChild(0).GetChild(2).gameObject;
			meshOfShipOriginalForm.Add(transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponentInChildren<MeshRenderer>());
            for (int i = 1; i < transform.GetChild(0).childCount; i++)
            {
                for (int j = 0; j < transform.GetChild(0).GetChild(i).childCount; j++)
                {
                    meshOfShipOriginalForm.Add(transform.GetChild(0).GetChild(i).GetChild(j).GetComponent<MeshRenderer>());
                }
            }
        }
		else
		{
			ExplosionGOFX = transform.GetChild(0).GetChild(2).gameObject;
			meshOfShipOriginalForm.Add(transform.GetChild(0).GetChild(0).GetComponentInChildren<MeshRenderer>());
		}


		ExplosionGOFX.SetActive(true);
		particleSystems = ExplosionGOFX.GetComponentsInChildren<ParticleSystem>();
		timer = 0.0f;
		foreach (var item in particleSystems)
		{
			if (item.main.duration > timer)
			{
				timer = item.main.duration;
			}
			item.Stop();
		}
		StartCoroutine(ManageMovementCoroutine());

		ExplosionSound = ExplosionGOFX.GetComponent<AudioSource>();
		SettingsControllersMenu.GetSingleton().RefferenceAudioSource(ExplosionSound);

		averageSpeed = 0f;
		foreach (WeaponState ws in WSMRef.listWeaponState)
		{
			averageSpeed += ws.speedShoot;
		}
		averageSpeed /= WSMRef.listWeaponState.Count;
	}

	private float averageSpeed;

	IEnumerator ChangeRandom()
	{
		while (true)
		{
			movementRand = /*Random.Range(0f, 1f) < 0.1f ? 0f :*/ Random.Range(-0.5f, 1.5f);
			yield return new WaitForSeconds(0.5f);
		}
	}

	private const float ROTATION_ACCURACY = 0.1f;
	private const float ROTATION_SPEED = 20f;

	private float movementRand;

	private void ManageMovement()
	{
		rb.velocity = transform.forward * speed;

		float realRand = target != null && Vector3.Distance(target.transform.position, transform.position) < 30f ? 4f * movementRand : movementRand;

		Vector3 toTarget = targetPos - transform.position;
		RaycastHit hit;
		if (Physics.Raycast(transform.position, transform.forward, out hit))
		{
			if (hit.collider.gameObject.tag == "Asteroid")
			{
				toTarget = Vector3.Lerp(toTarget, hit.point + hit.normal * 30f - transform.position, Mathf.Pow(1f - (Vector3.Distance(hit.point, transform.position) - 30f) / 200f, 8f));
				realRand = 4f;
			}
		}

		toTarget.Normalize();
		if (Vector3.Dot(toTarget, transform.forward) < 0f)
		{
			if (Vector3.Dot(toTarget, transform.up) < 0f)
			{
				rb.AddTorque(transform.right * ROTATION_SPEED * realRand);
			}
			else
			{
				rb.AddTorque(transform.right * -ROTATION_SPEED * realRand);
			}
		}
		else
		{
			float upDot = Vector3.Dot(toTarget, transform.up);
			float rightDot = Vector3.Dot(toTarget, transform.right);
			if (upDot < -ROTATION_ACCURACY)
			{
				rb.AddTorque(transform.right * ROTATION_SPEED * realRand);
			}
			else if (upDot > ROTATION_ACCURACY)
			{
				rb.AddTorque(transform.right * -ROTATION_SPEED * realRand);
			}
			if (rightDot < -ROTATION_ACCURACY)
			{
				rb.AddTorque(transform.forward * ROTATION_SPEED * realRand);
			}
			else if (rightDot > ROTATION_ACCURACY)
			{
				rb.AddTorque(transform.forward * -ROTATION_SPEED * realRand);
			}
		}
	}

	private const float limitZMin = 50f;
	private const float limitZMax = 1000f;
	//	private const float limitSide = 100f;
	private float limitAngle = Mathf.Cos(Mathf.PI / 12f);

	IEnumerator ManageMovementCoroutine()
	{
		while (isAlive)
		{
			switch (state)
			{
				case State.FollowLeader:
					ManageMovement();
					break;
				case State.Chase:
					targetTimer += Time.deltaTime;
					if (targetTimer > targetMaxTimer || target == null)
					{
						squad.AssignTarget(gameObject);
					}
					if (target == null)
					{
						SetTargetPos(transform.position + transform.forward * speed * 2f);
					}
					else
					{
						if (target != null && Vector3.Distance(target.transform.position, transform.position) < 30f)
						{
							if (Vector3.Dot(target.transform.position - transform.position, transform.up) < 0f)
							{
								SetTargetPos(target.transform.position + transform.up * 20f);
							}
							else
							{
								SetTargetPos(target.transform.position - transform.up * 20f);
							}
						}
						else
						{
							SetTargetPos(GetPositionInFuture());
						}
					}
					ManageMovement();
					break;
				case State.TakeAdvantage:
					targetTimer += Time.deltaTime;
					if (targetTimer > targetMaxTimer || target == null)
					{
						squad.AssignTarget(gameObject);
					}
					if (target == null)
					{
						SetTargetPos(transform.position + transform.forward * speed * 2f);
					}
					else
					{
						if (target != null && Vector3.Distance(target.transform.position, transform.position) < 30f)
						{
							if (Vector3.Dot(target.transform.position - transform.position, transform.up) < 0f)
							{
								SetTargetPos(target.transform.position + transform.up * 20f);
							}
							else
							{
								SetTargetPos(target.transform.position - transform.up * 20f);
							}
						}
						else
						{
							SetTargetPos(GetPositionInFuture());
						}
					}
					ManageMovement();
					break;
				case State.Flee:
					targetTimer += Time.deltaTime;
					if (targetTimer > targetMaxTimer || target == null)
					{
						squad.AssignTarget(gameObject);
					}
					if (target == null)
					{
						SetTargetPos(transform.position + transform.forward * speed * 2f);
					}
					else
					{
						if (target != null && Vector3.Distance(target.transform.position, transform.position) < 30f)
						{
							if (Vector3.Dot(target.transform.position - transform.position, transform.up) < 0f)
							{
								SetTargetPos(target.transform.position + transform.up * 20f);
							}
							else
							{
								SetTargetPos(target.transform.position - transform.up * 20f);
							}
						}
						else
						{
							SetTargetPos(GetPositionInFuture());
						}
					}
					ManageMovement();
					break;
				default:
					break;
			}
			if (state != State.FollowLeader && target != null)
			{
				Vector3 toTarget = GetPositionInFuture() - transform.position;
				if (Vector3.Dot(transform.forward, toTarget) > limitZMin
					&& Vector3.Dot(transform.forward, toTarget) < limitZMax
					&& Vector3.Dot(transform.forward, toTarget.normalized) > limitAngle)
				{
					if (state != State.Chase)
					{
						state = State.Chase;
						StartCoroutineShoot();
					}
				}
				else
				{
					state = State.TakeAdvantage;
				}
			}
			yield return new WaitForEndOfFrame();
		}
	}

	private Vector3 GetPositionInFuture()
	{
		float speed;
		if (targetPlayer)
		{
			speed = target.GetComponent<Rigidbody>().velocity.magnitude;
		}
		else
		{
			speed = target.GetComponent<Unit>().speed;
		}
		return target.transform.position + target.transform.forward * speed / (averageSpeed / Vector3.Distance(target.transform.position, transform.position));
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		if (!isAlive)
		{
			return;
		}

		/*if (Input.GetKey(KeyCode.K) && squad.GetAIType() == AIType.AIAlly)
		{
			Die();
		}*/
	}

	void StartCoroutineShoot()
	{
		float timer = Mathf.Infinity;
		for (int i = 0; i < WSMRef.listWeaponState.Count; i++)
		{
			if (timer > WSMRef.listWeaponState[i].fireRate)
			{
				timer = WSMRef.listWeaponState[i].fireRate;
			}
		}
		StartCoroutine(Shoot(timer));
	}

	IEnumerator Shoot(float minTimer)
	{
		while (isAlive && state == State.Chase)
		{
			WSMRef.ProceedFireAI(speed, target.transform);
			yield return new WaitForSeconds(minTimer);
		}
	}

	public void TakeShootDamage(float damage, bool touchedByPlayer = false)
	{
		health -= damage / shootProtection;
		if (health <= 0f)
		{
			Die(touchedByPlayer);
		}
		else if (touchedByPlayer)
		{
			// INDICATION HIT
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.tag == "Ammunition")
		{
			return;
		}

		Vector3 velocity = GetComponent<Rigidbody>().velocity;
		if (collision.gameObject.GetComponent<Rigidbody>())
		{
			velocity -= collision.gameObject.GetComponent<Rigidbody>().velocity;
		}
		health -= velocity.magnitude / collisionProtection;
		if (health <= 0f)
		{
			Die();
		}
		collisionTimer = 0f;
	}

	private float collisionTimer;

	private void OnCollisionStay(Collision collision)
	{
		collisionTimer += Time.deltaTime;

		if (collisionTimer > 3f)
		{
			if (collision.gameObject.tag == "Asteroid")
			{
				Die();
			}
			else
			{
				transform.position += transform.up * (Random.Range(0f, 1f) < 0.5f ? -2f : 2f) + transform.right * (Random.Range(0f, 1f) < 0.5f ? -2f : 2f) + transform.forward * (Random.Range(0f, 1f) < 0.5f ? -2f : 2f);
			}
		}
	}

	IEnumerator WaitBeforeHiddingExplosionGO(float timer, bool show)
	{
		if (show)
		{

			ExplosionSound.Play();

			foreach (var item in particleSystems)
			{
				item.Play();

			}
			float value = timer + 0.01f;
			yield return new WaitForSeconds(0.2f);
			meshOfShipOriginalForm.ForEach(x => x.gameObject.SetActive(false));
			WreckParent.SetActive(true);
			WreckParent.GetComponent<WreckMovement>().InitPart(10.0f);
			yield return new WaitForSeconds(value - 0.02f);
		}
		else
		{
            meshOfShipOriginalForm.ForEach(x => x.gameObject.SetActive(false));
            WreckParent.SetActive(true);
			WreckParent.GetComponent<WreckMovement>().InitPart(10.0f);
		}

		ExplosionGOFX.SetActive(false);
	}

	void Die(bool touchedByPlayer = false)
	{
		if (isAlive)
		{
			if (squad != null)
			{
				squad.RemoveUnit(gameObject);
			}

			state = State.Dead;
			GameObject shapeHolder = transform.GetChild(0).GetChild(0).gameObject;

			rb.velocity = Vector3.zero;
			StartCoroutine(WaitBeforeHiddingExplosionGO(timer, meshOfShipOriginalForm[0].isVisible));

			isAlive = false;
			if (touchedByPlayer)
			{
				UI2D.ResetKillTimer();
				GameObject.FindGameObjectWithTag("UI2D").GetComponent<UI2D>().AddKill();
			}
		}
		GameObject.FindGameObjectWithTag("Player").GetComponent<Ship>().CheckEnemyDeath(gameObject);
		gameObject.tag = "Wreck";
	}
}

public enum UnitType
{
	UnitT1 = 10,
	UnitT2 = 100
}