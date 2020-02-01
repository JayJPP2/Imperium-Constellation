using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WeaponState : MonoBehaviour
{
	public static GameObject ShootPoolParent = null;

	public bool canFire;
	public float fireRate;
	public float speedShoot;
	public float timerSinceLastShot;
	public GameObject projectilePrefab;
	public GameObject projectileImpactPrefab;
	public Transform spawn;
	public GameObject parent;
	public uint Range;
	public uint damage;
	public WeaponStat.WeaponType type;
	AudioSource source;
	public WeaponShootManager parentWSM;
    public GameObject TorpedoTypePlaceHolder;
    public GameObject FXFiring;

    public bool isProcessing { get; private set; }

    public List<GameObject> poolProjectilePrefab;

	public void FixedUpdate()
	{
		if (!canFire)
		{
			timerSinceLastShot += Time.deltaTime;
			if (timerSinceLastShot >= fireRate)
			{
				canFire = true;
				timerSinceLastShot -= fireRate;
                if (TorpedoTypePlaceHolder != null)
                {
                    TorpedoTypePlaceHolder.SetActive(true);
                }
            }
		}
	}

	// the weapon's default rotation
	private Quaternion defaultRotation;

	public void SetDirection(Vector3 direction)
	{
		transform.localRotation = defaultRotation;
		transform.Rotate(transform.up, Mathf.Acos(-Vector3.Dot(direction, transform.right)) * Mathf.Rad2Deg - 90f, Space.World);
		transform.Rotate(transform.right, Mathf.Acos(Vector3.Dot(direction, transform.up)) * Mathf.Rad2Deg - 90f, Space.World);
	}

	public void SetUpPool()
	{
		if (ShootPoolParent == null)
		{
			GameObject.Find("ShootsPool");
		}

		if (source == null)
		{
			source = GetComponent<AudioSource>();
			if (source != null)
			{
				SettingsControllersMenu.GetSingleton().RefferenceAudioSource(source);
			}
		}

		if (projectilePrefab != null)
		{
			poolProjectilePrefab = new List<GameObject>();
			int amountOfPrefab = Mathf.CeilToInt((Range / speedShoot) / fireRate) + 1;
			for (int i = 0; i < amountOfPrefab; i++)
			{
				GameObject newPoolMember = Instantiate(projectilePrefab);
				Shoot refShoot = newPoolMember.AddComponent<Shoot>();
				if (projectileImpactPrefab != null)
				{
					refShoot.SetImpactFx(projectileImpactPrefab);
				}
				refShoot.SetShooter(parent);
				refShoot.speed = speedShoot;
				refShoot.origin = parentWSM.typeOfShip;
				refShoot.range = Range;
				refShoot.damage = damage;
				poolProjectilePrefab.Add(newPoolMember);
				if (ShootPoolParent != null)
				{
					newPoolMember.transform.SetParent(ShootPoolParent.transform, false);
				}

                if (type == WeaponStat.WeaponType.Torpedo)
                {
                    newPoolMember.GetComponent<WeaponParticles>().CustomInit();
                }

                newPoolMember.SetActive(false);
			}
		}

        if(type == WeaponStat.WeaponType.Torpedo)
        {
            TorpedoTypePlaceHolder = transform.GetChild(0).gameObject;
        }

        if(type == WeaponStat.WeaponType.Turret)
        {
            FXFiring = transform.GetChild(0).GetChild(0).gameObject;
            spawn = FXFiring.transform;
        }

        isProcessing = false;
        defaultRotation = transform.localRotation;
	}

	static float spread = 5f; // shoot spread in degrees, same for all allies and enemies;


    IEnumerator CoroutineDeactiveShotFx()
    {
        if(isProcessing)
        {
            yield break;
        }
        else
        {
            isProcessing = true;
        }
        FXFiring.SetActive(true);
        FXFiring.transform.parent.GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(2.0f);
        FXFiring.SetActive(false);
        isProcessing = false;
    }


	public void FireAI(float initialSpeed, Transform target)
	{
		if (canFire)
		{
			GameObject temp = poolProjectilePrefab.First(x => x.activeSelf == false);
			temp.transform.forward = spawn.forward;
			temp.transform.position = spawn.position;
			temp.transform.rotation = spawn.rotation;
			temp.transform.Rotate(temp.transform.right, Random.Range(-spread, spread));
			temp.transform.Rotate(temp.transform.up, Random.Range(-spread, spread));
			temp.GetComponent<Shoot>().SetSpeed(speedShoot + initialSpeed);
			temp.GetComponent<Shoot>().ResetTouch();
			temp.SetActive(true);
            if(TorpedoTypePlaceHolder != null)
            {
                TorpedoTypePlaceHolder.SetActive(false);
                temp.GetComponent<WeaponParticles>().Fire();
            }

            if(FXFiring != null)
            {
                StartCoroutine(CoroutineDeactiveShotFx());
            }
            canFire = false;
			temp.GetComponent<Shoot>().rotationSave = temp.transform.rotation;
			if (target != null && Random.Range(0f, 1f) < 0.03f)
			{
				temp.GetComponent<Shoot>().target = target;
				temp.GetComponent<Shoot>().distanceToTarget = Vector3.Distance(spawn.position, target.transform.position);
			}
			else
			{
				temp.GetComponent<Shoot>().target = null;
			}
			if (source != null && !source.isPlaying)
			{
				source.volume = 0.5f;
				source.Play();
			}
		}
	}

    public Coroutine FireTurretT2()
    {
        if(canFire)
        {
            return (StartCoroutine(CoroutineDeactiveShotFx()));
        }
        return null;
    }


	public void Fire(float initialSpeed, Transform target = null)
	{
		if (canFire)
		{
			GameObject temp = poolProjectilePrefab.First(x => x.activeSelf == false);
			temp.transform.rotation = spawn.rotation;
			temp.transform.forward = spawn.forward;
			temp.transform.position = spawn.position;
			temp.GetComponent<Shoot>().SetSpeed(speedShoot + initialSpeed);
			temp.GetComponent<Shoot>().target = target;
			if (target != null)
			{
				temp.GetComponent<Shoot>().distanceToTarget = Vector3.Distance(spawn.position, target.transform.position);
			}
			temp.GetComponent<Shoot>().rotationSave = temp.transform.rotation;
			temp.GetComponent<Shoot>().ResetTouch();
			temp.SetActive(true);
            if (TorpedoTypePlaceHolder != null)
            {
                TorpedoTypePlaceHolder.SetActive(false);
                temp.GetComponent<WeaponParticles>().Fire();
            }
            canFire = false;
			if (source != null && !source.isPlaying && IsOkToShoot(this))
			{
				source.Play();
			}
		}
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
