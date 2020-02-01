using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponShootManager : MonoBehaviour
{
	public List<WeaponState> listWeaponState { get; private set; }


	private Ship shipREF;
	bool isIA;

	public Shoot.Origin typeOfShip;
	List<WeaponStat.WeaponType> availableTypeMissile;
	List<WeaponStat.WeaponType> availableTypeTurret;
	WeaponStat.WeaponType selectedTypeMissile;
	WeaponStat.WeaponType selectedTypeTurret;

	public WeaponStat.WeaponType GetSelectedTypeTurret()
	{
		return selectedTypeTurret;
	}

	int indexListMissile;
	int indexListTurret;
	// Use this for initialization
	void Awake()
	{
		shipREF = null;
		listWeaponState = new List<WeaponState>();
		typeOfShip = Shoot.Origin.player;
	}

	public void lateStart()
	{
		availableTypeMissile = new List<WeaponStat.WeaponType>();
		availableTypeTurret = new List<WeaponStat.WeaponType>();
		foreach (var item in listWeaponState)
		{
			if ((item.type == WeaponStat.WeaponType.Missile ||
			   item.type == WeaponStat.WeaponType.Torpedo)
				&& !availableTypeMissile.Contains(item.type))
			{
				availableTypeMissile.Add(item.type);
			}

			if ((item.type == WeaponStat.WeaponType.MachineGun ||
			  item.type == WeaponStat.WeaponType.Turret)
			   && !availableTypeTurret.Contains(item.type))
			{
				availableTypeTurret.Add(item.type);
			}
		}

		indexListMissile = indexListTurret = 0;

		if (availableTypeMissile.Count > 0)
		{
			selectedTypeMissile = availableTypeMissile[indexListMissile];

		}
		else
		{
			selectedTypeMissile = WeaponStat.WeaponType.Undefined;
		}

		if (availableTypeTurret.Count > 0)
		{
			selectedTypeTurret = availableTypeTurret[indexListTurret];
		}
		else
		{
			selectedTypeTurret = WeaponStat.WeaponType.Undefined;
		}
	}

	private GameObject turret = null;
	private GameObject canon = null;

	private Quaternion defaultTurretRotation;
	private Quaternion defaultCanonRotation;

	public void AddTurret(GameObject _turret)
	{
		turret = _turret;
		canon = turret.transform.Find("canon").gameObject;
		defaultTurretRotation = turret.transform.localRotation;
		defaultCanonRotation = canon.transform.localRotation;
	}

	public void SetIA()
	{
		isIA = true;
	}

	public void GetShipReference(Ship shipREF)
	{
		this.shipREF = shipREF;
	}

	public void SetOrigin(Shoot.Origin origin)
	{
		typeOfShip = origin;
	}

	// Update is called once per frame
	void Update()
	{
		if (SceneController.IDScene != SceneController.SceneID.MainGameScene)
		{
			return;
		}

		if (isIA)
		{
			return;
		}

		listWeaponState.ForEach(x => x.SetDirection(x.type == WeaponStat.WeaponType.MachineGun && selectedTypeTurret == WeaponStat.WeaponType.MachineGun ? GetFireDirection(transform.root) : transform.forward));

		if (Input.GetButtonDown("ChangeWeaponMode") && selectedTypeMissile != WeaponStat.WeaponType.Undefined)
		{
			indexListMissile = (indexListMissile + 1) % availableTypeMissile.Count;
			selectedTypeMissile = availableTypeMissile[indexListMissile];

			if (selectedTypeMissile == WeaponStat.WeaponType.Missile)
			{
				GameObject.FindGameObjectWithTag("UI2D").GetComponent<UI2D>().ChangeMissile("Missile");
			}
			else if (selectedTypeMissile == WeaponStat.WeaponType.Torpedo)
			{
				GameObject.FindGameObjectWithTag("UI2D").GetComponent<UI2D>().ChangeMissile("Torpedo");
			}
		}

		if (Input.GetButtonDown("ChangeWeaponModeTurret") && selectedTypeTurret != WeaponStat.WeaponType.Undefined)
		{
			indexListTurret = (indexListTurret + 1) % availableTypeTurret.Count;
			selectedTypeTurret = availableTypeTurret[indexListTurret];
		}

		if (turret != null && selectedTypeTurret == WeaponStat.WeaponType.Turret)
		{
			Vector3 direction = GetFireDirection(transform.root);
			turret.transform.localRotation = defaultTurretRotation;
			turret.transform.Rotate(turret.transform.up, Mathf.Acos(-Vector3.Dot(direction, turret.transform.right)) * Mathf.Rad2Deg - 90f, Space.World);
			if (Vector3.Dot(direction, transform.up) > 0f)
			{
				canon.transform.localRotation = defaultCanonRotation;
				canon.transform.Rotate(canon.transform.right, Mathf.Acos(Vector3.Dot(direction, canon.transform.up)) * Mathf.Rad2Deg - 90f, Space.World);
			}
		}

		if (Input.GetButton("Fire1") && !Input.GetButton("PushRight"))
		{
			ProceedFire(selectedTypeTurret, shipREF.GetSelectedTarget() != null ? shipREF.GetSelectedTarget().transform : null);
		}
		else
		{
			foreach (WeaponState ws in listWeaponState)
			{
				if (ws.type == WeaponStat.WeaponType.MachineGun)
				{
					ws.GetComponent<AudioSource>().Stop();
				}
			}
		}

		if (
			!Input.GetButton("PushRight") &&
			((Input.GetButtonDown("Fire2") && selectedTypeMissile == WeaponStat.WeaponType.Torpedo) ||
			(Input.GetButton("Fire2") && selectedTypeMissile == WeaponStat.WeaponType.Missile)))
		{
			ProceedFire(selectedTypeMissile, shipREF.GetSelectedTarget() == null || shipREF.getLockTimerNormalized() < 1f ? null : shipREF.GetSelectedTarget().transform);
		}
	}

	public void ProceedFireAI(float initialSpeed, Transform target)
	{
		listWeaponState.ForEach(x => x.FireAI(initialSpeed, target));
	}

	private static float limitCanon = Mathf.Cos(Mathf.PI / 72f);

	public static float GetLimitCanon()
	{
		return limitCanon;
	}

    IEnumerator FireTurretSequenceRobot(Transform target, WeaponState ws)
    {
        if(ws.isProcessing)
        {
            yield break;
        }

        Coroutine analyze = ws.FireTurretT2();

        while (ws.FXFiring.activeSelf)
        {
            yield return new WaitForEndOfFrame();
        }

        if (target == null)
        {
            ws.Fire(shipREF.Velocity.magnitude, target);
        }
        else
        {
            if (GameObject.Find("UIGame").GetComponent<UIGame>().IsBigCursorAligned())
            {
                ws.Fire(shipREF.Velocity.magnitude, target);
            }
            else
            {
                ws.Fire(shipREF.Velocity.magnitude, null);
            }
        }
    }

	public void ProceedFire(WeaponStat.WeaponType type, Transform target = null)
	{
		foreach (WeaponState ws in listWeaponState)
		{
			if (ws.type == type)
			{
				if (type == WeaponStat.WeaponType.MachineGun)
				{
					if (target == null)
					{
						ws.Fire(shipREF.Velocity.magnitude, target);
					}
					else
					{
						float targetDistance = Vector3.Distance(target.position, ws.spawn.position);
						Vector3 toSphere = target.position + target.GetComponent<Unit>().GetSpeed() * target.forward / ((ws.speedShoot + transform.parent.GetComponent<Ship>().Velocity.magnitude) / targetDistance) - ws.spawn.position;
						Vector3 temp, temp2;
						temp = Vector3.Cross(ws.spawn.forward, toSphere);
						temp2 = Vector3.Cross(ws.spawn.forward, temp);

						float distance = Mathf.Abs(Vector3.Dot(temp2.normalized, toSphere));
						float limit;

						if (targetDistance > 400f)
						{
							limit = 8f;
						}
						else if (targetDistance > 200f)
						{
							limit = 4f;
						}
						else
						{
							limit = 2f;
						}

						if (distance < limit)
						{
							ws.Fire(shipREF.Velocity.magnitude, target);
						}
						else
						{
							ws.Fire(shipREF.Velocity.magnitude, null);
						}
					}
				}
				else if (type == WeaponStat.WeaponType.Torpedo)
				{
					if (ws.canFire)
					{
						ws.Fire(shipREF.Velocity.magnitude, target);
						return;
					}
				}
				else if (type == WeaponStat.WeaponType.Turret)
				{
                    StartCoroutine(FireTurretSequenceRobot(target, ws));
                }
				else
				{
					ws.Fire(shipREF.Velocity.magnitude, target);
				}
			}
		}
	}

	public void AddWeaponState(WeaponState newWS)
	{
		newWS.parentWSM = this;
		listWeaponState.Add(newWS);
	}

	private static float fireZoneRadius = 0.2f;

	public static Vector3 GetFireDirection(Transform playerTransform)
	{
		Ray ray;
		string[] temp = Input.GetJoystickNames();

		if (Input.GetJoystickNames().Length > 0)
		{
			Vector3 mousePos;
			Vector3 joystick;
			joystick.x = Input.GetAxis("CursorHorizontal");
			joystick.y = -Input.GetAxis("CursorVertical");
			joystick.z = 0f;
			if (joystick.magnitude > 1f)
			{
				joystick.Normalize();
			}
			mousePos.x = Screen.width / 2f + joystick.x * (float)Screen.width * fireZoneRadius;
			mousePos.y = Screen.height / 2f + joystick.y * (float)Screen.height * fireZoneRadius;
			mousePos.z = 0f;
			ray = Camera.main.ScreenPointToRay(mousePos);
		}
		else
		{
			Vector3 mousePos = Input.mousePosition;
			mousePos.x -= Screen.width / 2f;
			mousePos.y -= Screen.height / 2f;
			mousePos.x /= Screen.width;
			mousePos.y /= Screen.height;
			mousePos.z = 0f;
			if (mousePos.magnitude > fireZoneRadius)
			{
				mousePos.Normalize();
				mousePos *= fireZoneRadius;
			}
			mousePos.x *= Screen.width;
			mousePos.y *= Screen.height;
			mousePos.x += Screen.width / 2f;
			mousePos.y += Screen.height / 2f;
			ray = Camera.main.ScreenPointToRay(mousePos);
		}
		Vector3 playerToRay = ray.origin - playerTransform.position;
		ray.origin = playerTransform.position;
		ray.origin += Vector3.Dot(playerTransform.right, playerToRay) * playerTransform.right;
		ray.origin += Vector3.Dot(playerTransform.up, playerToRay) * playerTransform.up;

		Vector3 destination = ray.origin + ray.direction * Mathf.Sqrt(UIGame.GetCursorDistance() * UIGame.GetCursorDistance() - (ray.origin - playerTransform.position).sqrMagnitude);

		return (destination - playerTransform.position).normalized;
	}

	public bool IsOkToShoot(WeaponState newWS)
	{
		float nextFire = 0.0f;
		bool isOk = false;

		if (Time.time > nextFire)
		{
			nextFire = Time.time + newWS.fireRate;
			isOk = true;
		}
		return isOk;
	}
}