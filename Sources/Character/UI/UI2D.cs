using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class UI2D : MonoBehaviour
{
	[SerializeField] GameObject player;
	[SerializeField] GameObject health;
	[SerializeField] GameObject speed;
	[SerializeField] Image panelOutOfZone;
	[SerializeField] Text outOfZone;
	float timer;

	private float outZoneTimer;
	private bool isOutZone;

	public void OutOfZone(bool _isOutZone)
	{
		isOutZone = _isOutZone;
		if (!isOutZone)
		{
			outZoneTimer = 0f;
		}
	}

	// map
	[SerializeField] GameObject map;
	[SerializeField] GameObject enemyImage;
	[SerializeField] GameObject allyImage;

	GameObject[] enemies;
	GameObject[] enemiesUI;

	GameObject[] allies;
	GameObject[] alliesUI;

	private float UIScale;

	[SerializeField] GameObject targetImage;
	private GameObject target;

	public Vector3 GetTargetImagePos()
	{
		return targetImage.transform.position;
	}

	public bool GetTargetImageActive()
	{
		return targetImage.activeSelf;
	}

	[SerializeField] GameObject findEnemy;
	[SerializeField] GameObject findEnemy2;

	[SerializeField] GameObject fireHelp;

	[SerializeField] GameObject remainingEnemies;
	[SerializeField] GameObject remainingAllies;

	[SerializeField] GameObject PauseUI;

	Ship shipRef;

	[SerializeField] GameObject killSign;
	[SerializeField] Text killCountText;

	[SerializeField] Text changeMissile;

	private float changeMissileTimer;

	private static float killTimer = 10f;

	public static void ResetKillTimer()
	{
		killTimer = 0f;
	}

	private int killCount = 0;

	public void AddKill()
	{
		killCount++;
		killCountText.text = killCount.ToString();
	}

	[SerializeField] GameObject loadTimer;

	void Start()
	{
		enemiesUI = null;
		alliesUI = null;
		target = null;
		shipRef = player.GetComponent<Ship>();
		StartCoroutine(InitUI());
		StartCoroutine(RefreshMap(0.02f));
		StartCoroutine(DetectEnemies(0.02f));
		lastCountAlly = lastCountEnemy = 0;
		changeMissileTimer = 1f;
        saveLastHealth = 0.0f;
    }

	public void ChangeMissile(string missile)
	{
		changeMissileTimer = 0f;
		changeMissile.text = missile;
	}

	public void SetPauseUI()
	{
		PauseUI.SetActive(!PauseUI.activeSelf);

		if (PauseUI.activeSelf)
		{
			EventSystem.current.SetSelectedGameObject(null);
			EventSystem.current.SetSelectedGameObject(PauseUI.transform.GetChild(1).gameObject);
		}
	}

	public void MainMenu()
	{
		SceneController.Instance.MainMenu();
	}

	public void Exit()
	{
		SceneController.Instance.QuitGame();
	}

	private Transform UITransform;

	IEnumerator InitUI()
	{
		while (GameObject.FindGameObjectsWithTag("Enemy").Length == 0)
		{
			yield return new WaitForEndOfFrame();
		}
		enemies = GameObject.FindGameObjectsWithTag("Enemy");
		int nbEnemies = 0;
		for (int i = 0; i < enemies.Length; i++)
		{
			if (enemies[i].transform.parent == null)
			{
				nbEnemies++;
			}
		}

		enemiesUI = new GameObject[nbEnemies];

		for (int i = 0; i < nbEnemies; i++)
		{
			enemiesUI[i] = Instantiate(enemyImage);
			enemiesUI[i].transform.SetParent(map.transform, false);
		}

		while (GameObject.FindGameObjectsWithTag("Ally").Length == 0)
		{
			yield return new WaitForEndOfFrame();
		}
		allies = GameObject.FindGameObjectsWithTag("Ally");
		int nbAllies = 0;
		for (int i = 0; i < allies.Length; i++)
		{
			if (allies[i].transform.parent == null)
			{
				nbAllies++;
			}
		}

		alliesUI = new GameObject[nbAllies];

		for (int i = 0; i < nbAllies; i++)
		{
			alliesUI[i] = Instantiate(allyImage);
			alliesUI[i].transform.SetParent(map.transform, false);
		}
		UIScale = map.transform.root.localScale.y;

		WSMRef = shipRef.GetComponentInChildren<WeaponShootManager>();

		canon = shipRef.GetTurret() != null ? shipRef.GetTurret().transform.Find("canon") : null;

		UITransform = transform.root;
	}

	private Transform canon;

	private WeaponShootManager WSMRef = null;

	private const float RADAR_SCALE = 50f;
	private const float RADAR_RADIUS = 130f;

	private int lastCountAlly;
	private int lastCountEnemy;

	IEnumerator FadeBack(float time, Text text)
	{
		float cptr = 0.0f;
		while (cptr < time)
		{
			text.color = Color.red;
			cptr += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
		text.color = new Color(0.08627451f, 0.6431373f, 0.9372549f, 1.0f);
		yield return null;
	}

	IEnumerator RefreshMap(float timer)
	{
		while (!player.GetComponent<Ship>().IsAlive()) // Security : assure that player has started before to check if he is alive
		{
			yield return new WaitForEndOfFrame();
		}
		while (player.GetComponent<Ship>().IsAlive())
		{
			enemies = GameObject.FindGameObjectsWithTag("Enemy");

			if (enemiesUI != null)
			{
				int UIindex = 0;
				for (int i = 0; i < enemies.Length; i++)
				{
					if (enemies[i].transform.parent == null)
					{
						Vector3 vector = enemies[i].transform.position - player.transform.position;
						Vector3 radarPos = new Vector3(Vector3.Dot(player.transform.right, vector) / RADAR_SCALE, Vector3.Dot(player.transform.forward, vector) / RADAR_SCALE, -Vector3.Dot(player.transform.up, vector) / RADAR_SCALE);
						radarPos /= UIScale;
						if (radarPos.magnitude < RADAR_RADIUS)
						{
							enemiesUI[UIindex].transform.localPosition = radarPos;
							enemiesUI[UIindex].SetActive(true);
							if (radarPos.z > 0f)
							{
								enemiesUI[UIindex].transform.localScale = new Vector3(enemiesUI[UIindex].transform.localScale.x, -Mathf.Abs(enemiesUI[UIindex].transform.localScale.y), enemiesUI[UIindex].transform.localScale.z);
							}
							else
							{
								enemiesUI[UIindex].transform.localScale = new Vector3(enemiesUI[UIindex].transform.localScale.x, Mathf.Abs(enemiesUI[UIindex].transform.localScale.y), enemiesUI[UIindex].transform.localScale.z);
							}
						}
						else
						{
							enemiesUI[UIindex].SetActive(false);
						}
						UIindex++;
					}
				}

				for (int i = UIindex; i < enemiesUI.Length; i++)
				{
					enemiesUI[i].SetActive(false);
				}

				if (lastCountEnemy != UIindex)
				{
					StartCoroutine(FadeBack(1.0f, remainingEnemies.GetComponent<Text>()));
				}

				remainingEnemies.GetComponent<Text>().text = UIindex.ToString();
				lastCountEnemy = UIindex;
			}

			allies = GameObject.FindGameObjectsWithTag("Ally");

			if (alliesUI != null)
			{
				int UIindex = 0;
				for (int i = 0; i < allies.Length; i++)
				{
					if (allies[i].transform.parent == null)
					{
						Vector3 vector = allies[i].transform.position - player.transform.position;
						Vector3 radarPos = new Vector3(Vector3.Dot(player.transform.right, vector) / RADAR_SCALE, Vector3.Dot(player.transform.forward, vector) / RADAR_SCALE, -Vector3.Dot(player.transform.up, vector) / RADAR_SCALE);
						radarPos /= UIScale;
						if (radarPos.magnitude < RADAR_RADIUS)
						{
							alliesUI[UIindex].transform.localPosition = radarPos;
							alliesUI[UIindex].SetActive(true);
							if (radarPos.z > 0f)
							{
								alliesUI[UIindex].transform.localScale = new Vector3(alliesUI[UIindex].transform.localScale.x, -Mathf.Abs(alliesUI[UIindex].transform.localScale.y), alliesUI[UIindex].transform.localScale.z);
							}
							else
							{
								alliesUI[UIindex].transform.localScale = new Vector3(alliesUI[UIindex].transform.localScale.x, Mathf.Abs(alliesUI[UIindex].transform.localScale.y), alliesUI[UIindex].transform.localScale.z);
							}
						}
						else
						{
							alliesUI[UIindex].SetActive(false);
						}
						UIindex++;
					}
				}

				for (int i = UIindex; i < alliesUI.Length; i++)
				{
					alliesUI[i].SetActive(false);
				}

				if (lastCountAlly != UIindex)
				{
					StartCoroutine(FadeBack(1.0f, remainingAllies.GetComponent<Text>()));
				}

				remainingAllies.GetComponent<Text>().text = UIindex.ToString();
				lastCountAlly = UIindex;

			}

			yield return new WaitForSeconds(timer);
		}
	}

	private float detectTimer = 0f;

	IEnumerator DetectEnemies(float timer)
	{
		while (!player.GetComponent<Ship>().IsAlive()) // Security : assure that player has started before to check if he is alive
		{
			yield return new WaitForEndOfFrame();
		}
		while (player.GetComponent<Ship>().IsAlive())
		{
			enemies = GameObject.FindGameObjectsWithTag("Enemy");

			if (enemies.Length > 0 && shipRef.GetSelectedTarget() == null)
			{
				GameObject closestEnemy = null;
				float distance = 100000f;
				float tempDistance;
				for (int i = 0; i < enemies.Length; i++)
				{
					if (enemies[i].transform.parent == null)
					{
						tempDistance = Vector3.Distance(player.transform.position, enemies[i].transform.position);
						if (tempDistance < distance)
						{
							distance = tempDistance;
							closestEnemy = enemies[i];
						}
					}
				}
				if (closestEnemy == null)
				{
					yield return new WaitForSeconds(timer);
				}
				Vector3 screenPos = Camera.main.WorldToScreenPoint(closestEnemy.transform.position);
				if (screenPos.z > 0f
					&& screenPos.x > 100f && screenPos.x < Screen.width - 100f
					&& screenPos.y > 100f && screenPos.y < Screen.height - 100f)
				{
					findEnemy.SetActive(true);
					findEnemy2.SetActive(false);
					screenPos.z = 0f;
					findEnemy.transform.position = screenPos;
				}
				else if (distance > 1000f || detectTimer > 5f)
				{
					findEnemy.SetActive(true);
					findEnemy2.SetActive(true);
					screenPos.z = 0f;
					Vector3 toEnemy = closestEnemy.transform.position - player.transform.position;
					screenPos.x = Vector3.Dot(player.transform.right, toEnemy);
					screenPos.y = Vector3.Dot(player.transform.up, toEnemy);
					findEnemy2.transform.up = screenPos.normalized;
					findEnemy.transform.position = new Vector3(Screen.width / 2f + findEnemy2.transform.up.x * Screen.width * 0.3f, Screen.height / 2f + findEnemy2.transform.up.y * Screen.height * 0.3f, 0f);
					findEnemy2.transform.position = findEnemy.transform.position + findEnemy2.transform.up * 20f;
				}
				else
				{
					findEnemy.SetActive(false);
					findEnemy2.SetActive(false);
				}
			}
			else
			{
				findEnemy.SetActive(false);
				findEnemy2.SetActive(false);
			}
			yield return new WaitForSeconds(timer);
		}
	}

    private float saveLastHealth;

    IEnumerator FlashImpact()
    {
        float timer = 0.0f;

        while(timer < 0.8f)
        {
            health.GetComponent<Image>().color = new Color(0.509434f, 0f, 0f, 1f);
            health.GetComponent<Image>().material.SetFloat("_IsActive", 1.0f);
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        health.GetComponent<Image>().material.SetFloat("_IsActive", 0.0f);
        health.GetComponent<Image>().color = Color.white;
    }

	void Update()
	{
		if (player.GetComponent<Ship>().IsAlive())
		{
			if (shipRef.GetSelectedTarget() == null)
			{
				detectTimer += Time.deltaTime;
			}
			else
			{
				detectTimer = 0f;
			}

            if(saveLastHealth != player.GetComponent<Ship>().GetRelativeHealth())
            {
                StartCoroutine(FlashImpact());
            }
            saveLastHealth = player.GetComponent<Ship>().GetRelativeHealth();

            health.GetComponent<Image>().fillAmount = player.GetComponent<Ship>().GetRelativeHealth();
			speed.GetComponent<Image>().fillAmount = player.GetComponent<Ship>().Throttle;
			target = player.GetComponent<Ship>().GetSelectedTarget();
			if (target == null || Input.GetButton("PushRight"))
			{
				targetImage.SetActive(false);
				fireHelp.SetActive(false);
			}
			else
			{
				targetImage.SetActive(true);
				targetImage.transform.position = Camera.main.WorldToScreenPoint(target.transform.position);
				targetImage.transform.position -= UITransform.forward * Vector3.Dot(UITransform.forward, targetImage.transform.position - UITransform.position);
				List<WeaponState> wss = player.GetComponentInChildren<WeaponShootManager>().listWeaponState;
				float averageSpeed = 0f;
				Vector3 averagePosition = Vector3.zero;
				int nbWeapons = 0;
				bool missile = false;
				foreach (WeaponState ws in wss)
				{
					if (ws.type == WeaponStat.WeaponType.MachineGun)
					{
						averageSpeed += ws.speedShoot;
						averagePosition += ws.transform.position;
						nbWeapons++;
					}
					else
					{
						missile = true;
					}
				}
				if (nbWeapons > 0 && WSMRef.GetSelectedTypeTurret() == WeaponStat.WeaponType.MachineGun)
				{
					fireHelp.SetActive(true);
					averageSpeed /= nbWeapons;
					averagePosition /= nbWeapons;
					Vector3 futurePos = target.transform.position + target.GetComponent<Unit>().GetSpeed() * target.transform.forward / ((averageSpeed + player.GetComponent<Ship>().Velocity.magnitude) / Vector3.Distance(target.transform.position, averagePosition));
					fireHelp.transform.position = Camera.main.WorldToScreenPoint(averagePosition + (futurePos - averagePosition).normalized * UIGame.GetCursorDistance());
				}
				else
				{
					fireHelp.SetActive(false);
				}
				if (missile)
				{
					targetImage.GetComponent<Image>().color = player.GetComponent<Ship>().getLockTimerNormalized() < 1f ? Color.Lerp(Color.cyan, Color.green, player.GetComponent<Ship>().getLockTimerNormalized()) : Color.red;
				}
				else
				{
					targetImage.GetComponent<Image>().color = Color.cyan;
				}
			}
			killSign.GetComponent<Image>().color = Color.Lerp(Color.red, Color.white, killTimer / 2f);
			if (killTimer < 2f)
			{
				killSign.SetActive(true);
				killCountText.enabled = true;
			}
			else
			{
				killSign.SetActive(false);
				killCountText.enabled = false;
			}
			killTimer += Time.deltaTime;

			if (canon != null)
			{
				WeaponState ws = shipRef.GetTurret().GetComponent<WeaponState>();
				if (!ws.canFire)
				{
					loadTimer.SetActive(true);
					loadTimer.GetComponent<Text>().text = ((int)(ws.fireRate - ws.timerSinceLastShot)).ToString() + "s";
				}
				else
				{
					loadTimer.SetActive(false);
				}
			}
			else
			{
				loadTimer.SetActive(false);
			}
		}
		else
		{
			health.GetComponent<Image>().fillAmount = 0f;
			if (enemiesUI != null)
			{
				foreach (GameObject go in enemiesUI)
				{
					go.SetActive(false);
				}
			}
			if (alliesUI != null)
			{
				foreach (GameObject go in alliesUI)
				{
					go.SetActive(false);
				}
			}
			if (GameObject.Find("PlayerUI") != null)
			{
				GameObject.Find("PlayerUI").SetActive(false);
			}
		}

		if (changeMissileTimer < 0.5f)
		{
			changeMissile.enabled = true;
			changeMissileTimer += Time.deltaTime;
		}
		else
		{
			changeMissile.enabled = false;
		}

		timer += Time.deltaTime;

		if (isOutZone)
		{
			outZoneTimer += Time.deltaTime;
			if (outZoneTimer < 2f || (outZoneTimer > 4f && outZoneTimer < 6f) || outZoneTimer > 8f)
			{
				outOfZone.enabled = true;
			}
			else
			{
				outOfZone.enabled = false;
			}
			if (outZoneTimer > 10f)
			{
				GameObject worldCenter = GameObject.Find("worldCenter");
				Vector3 worldToPlayer = player.transform.position - worldCenter.transform.position;
				player.transform.position = worldCenter.transform.position + worldToPlayer.normalized * worldCenter.GetComponent<outZone>().GetRadius();
				player.transform.forward = -worldToPlayer.normalized;
				outOfZone.enabled = false;
			}
		}
	}
}
