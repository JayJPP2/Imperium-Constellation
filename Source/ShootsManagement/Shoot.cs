using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Shoot : MonoBehaviour
{

	public float damage;
	public float speed;
	public uint range;
	private float travelledDistance;

	public float distanceToTarget;

	private ParticleSystem[] ps;
	private Renderer rendererReference;

	private Rigidbody refRigidbody;

	public enum Origin
	{
		player,
		enemy,
		ally
	}

	public Origin origin;

	bool hasTouched;

	public void ResetTouch()
	{
		hasTouched = false;
	}

	public Transform target = null;

	private GameObject shooter;
	public GameObject impactFX_GO;
	private ParticleSystem[] listParticles;

	public void SetShooter(GameObject _shooter)
	{
		shooter = _shooter;
		ps = transform.GetComponentsInChildren<ParticleSystem>();
		if (impactFX_GO != null)
		{
			impactFX_GO.SetActive(false);
		}
	}

	public void SetImpactFx(GameObject prefabToInstantiate)
	{
		impactFX_GO = Instantiate(prefabToInstantiate);
		listParticles = impactFX_GO.GetComponentsInChildren<ParticleSystem>();
		impactFX_GO.SetActive(false);
	}

	public void ChangeParent()
	{
		if (GameObject.Find("Shoots"))
		{
			transform.parent = GameObject.Find("Shoots").transform;
		}
		else
		{
			transform.parent = new GameObject("Shoots").transform;
		}
	}

	private void OnBecameInvisible()
	{
		for (int i = 0; i < ps.Length; i++)
		{
			ps[i].Stop();
		}
	}

	private void OnBecameVisible()
	{
		for (int i = 0; i < ps.Length; i++)
		{
			ps[i].Play();
		}
	}

	void Awake()
	{
		listParticles = null;
		impactFX_GO = null;
		rendererReference = GetComponentInChildren<Renderer>();
		refRigidbody = GetComponent<Rigidbody>();
	}

	public void SetSpeed(float speed)
	{
		this.speed = speed;
		refRigidbody.velocity = transform.forward * speed;
	}

	// Use this for initialization
	void Start()
	{
		hasTouched = false;
		travelledDistance = 0.0f;
		distanceToTarget = 0f;
	}

	public Quaternion rotationSave;

	private void FixedUpdate()
	{
		if (target != null && ((target.gameObject.GetComponent<Unit>() != null && target.gameObject.GetComponent<Unit>().IsAlive()) || (target.gameObject.GetComponent<Ship>() != null && target.gameObject.GetComponent<Ship>().IsAlive())))
		{
			Quaternion temp = Quaternion.LookRotation(target.position - transform.position);
			if (distanceToTarget > 1f)
			{
				transform.rotation = Quaternion.Lerp(rotationSave, temp, travelledDistance / distanceToTarget);
			}
			else
			{
				transform.rotation = temp;
			}
			if (Vector3.Distance(transform.position, target.transform.position) < speed * Time.deltaTime)
			{
				refRigidbody.velocity = transform.forward * Vector3.Distance(transform.position, target.transform.position) / Time.deltaTime;
			}
			else
			{
				refRigidbody.velocity = transform.forward * speed;
			}
		}

		travelledDistance += speed * Time.deltaTime;

		if (travelledDistance >= range)
		{
			travelledDistance = 0.0f;
			distanceToTarget = 0f;
			refRigidbody.velocity = Vector3.zero;
			gameObject.SetActive(false);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Ammunition" && other.transform.root.GetComponent<Shoot>().shooter == shooter) // if 2 shots from the same ship are colliding, don't trigger effect
		{
			return;
		}
		if (other.gameObject != shooter && other.transform.root.gameObject.tag != shooter.tag)
		{
			if (!hasTouched)
			{
				if (impactFX_GO != null && rendererReference.isVisible)
				{
					impactFX_GO.transform.position = this.transform.position;
					impactFX_GO.SetActive(true);
					for (int i = 0; i < listParticles.Length; i++)
					{
						listParticles[i].Play();
					}
				}

				hasTouched = true;
				if (origin == Origin.enemy)
				{
					if (other.transform.root.tag == "Player")
					{
						other.transform.root.GetComponent<Ship>().TakeShootDamage(damage);
					}
					else if (other.transform.root.tag == "Ally")
					{
						other.transform.root.GetComponent<Unit>().TakeShootDamage(damage);
					}
				}
				else if (origin == Origin.player)
				{
					if (other.transform.root.tag == "Enemy")
					{
						other.transform.root.GetComponent<Unit>().TakeShootDamage(damage, true);
					}
					else if (other.transform.root.tag == "Ally")
					{
						// FIRENDLY FIRE
					}
				}
				else
				{
					if (other.transform.root.tag == "Enemy")
					{
						other.transform.root.GetComponent<Unit>().TakeShootDamage(damage);
					}
				}
			}

			travelledDistance = 0.0f;
			distanceToTarget = 0f;
			refRigidbody.velocity = Vector3.zero;
			gameObject.SetActive(false);
		}
	}
}
