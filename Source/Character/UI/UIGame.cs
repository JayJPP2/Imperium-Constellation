using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGame : MonoBehaviour
{

	[SerializeField] GameObject player;
	[SerializeField] GameObject cursor;

	Vector3 vecteurWorldCenter;
	float norme;
	Vector3 angle;

	List<WeaponState> machineguns;
	List<GameObject> cursors;

	[SerializeField] GameObject bigCursor;

	public bool IsBigCursorAligned()
	{
		return bigCursor.GetComponent<Image>().color == Color.red;
	}

	private Transform canon;

	bool pause = false;

	public bool InPause()
	{
		return pause;
	}

	public void SetPause()
	{
		if (pause) { pause = false; }
		else { pause = true; }
	}

	private UI2D ui;

	public void Pause()
	{
		if (InPause())
		{
			Time.timeScale = 1f;
		}
		else
		{
			Time.timeScale = 0f;
		}
		ui.SetPauseUI();
		SetPause();
	}

	void Start()
	{
		ui = GameObject.FindGameObjectWithTag("UI2D").GetComponent<UI2D>();
		List<WeaponState> weapons = player.GetComponentInChildren<WeaponShootManager>().listWeaponState;
		machineguns = new List<WeaponState>();
		cursors = new List<GameObject>();
		foreach (WeaponState ws in weapons)
		{
			if (ws.type == WeaponStat.WeaponType.MachineGun)
			{
				machineguns.Add(ws);
				cursors.Add(Instantiate(cursor));
			}
		}
		foreach (GameObject go in cursors)
		{
			go.transform.SetParent(gameObject.transform, false);
		}
		StartCoroutine(InitUI());
	}

	private Ship shipRef = null;

	IEnumerator InitUI()
	{
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		WSMRef = player.GetComponentInChildren<WeaponShootManager>();
		shipRef = player.GetComponent<Ship>();
		canon = shipRef.GetTurret() != null ? shipRef.GetTurret().transform.Find("canon") : null;
	}

	private WeaponShootManager WSMRef = null;

	private static float cursorDistance = 150f;

	public static float GetCursorDistance()
	{
		return cursorDistance;
	}

	void FixedUpdate()
	{
		if (player.GetComponent<Ship>().IsAlive() && WSMRef != null)
		{
			if (!Input.GetButton("PushRight"))
			{
				if (WSMRef.GetSelectedTypeTurret() == WeaponStat.WeaponType.MachineGun)
				{
					for (int i = 0; i < machineguns.Count; i++)
					{
						cursors[i].SetActive(true);
						cursors[i].transform.position = machineguns[i].transform.position + machineguns[i].transform.forward * cursorDistance;
						cursors[i].transform.rotation = machineguns[i].transform.rotation;
					}
					bigCursor.SetActive(false);
				}
				else
				{
					for (int i = 0; i < machineguns.Count; i++)
					{
						cursors[i].SetActive(false);
					}
					if (WSMRef.GetSelectedTypeTurret() == WeaponStat.WeaponType.Turret)
					{
						bigCursor.SetActive(true);
						bigCursor.transform.position = Camera.main.WorldToScreenPoint(canon.position + canon.forward * cursorDistance);
						bigCursor.transform.position -= ui.transform.forward * Vector3.Dot(ui.transform.forward, bigCursor.transform.position - ui.transform.position);

						if (ui.GetTargetImageActive() && Vector3.Distance(bigCursor.transform.position, ui.GetTargetImagePos()) < 35f)
						{
							bigCursor.GetComponent<Image>().color = Color.red;
						}
						else
						{
							bigCursor.GetComponent<Image>().color = new Color(0f, 239f / 255f, 1f, 1f);
						}
						/*GameObject target = shipRef.GetSelectedTarget();

						if (target == null)
						{
							bigCursor.GetComponent<Image>().color = new Color(0f, 239f / 255f, 1f, 1f);
						}
						else
						{
							Vector3 toTarget = target.transform.position - canon.position;
							if (Vector3.Dot(canon.forward, toTarget) > 0f
								&& Vector3.Dot(canon.forward, toTarget.normalized) > WeaponShootManager.GetLimitCanon())
							{
								bigCursor.GetComponent<Image>().color = Color.red;
							}
							else
							{
								bigCursor.GetComponent<Image>().color = new Color(0f,239f / 255f, 1f, 1f);
							}
						}*/
					}
					else
					{
						bigCursor.SetActive(false);
					}
				}
			}
			else
			{
				for (int i = 0; i < machineguns.Count; i++)
				{
					cursors[i].SetActive(false);
				}
				bigCursor.SetActive(false);
			}
		}
		else
		{
			for (int i = 0; i < machineguns.Count; i++)
			{
				cursors[i].SetActive(false);
			}
			bigCursor.SetActive(false);
		}
	}

	private void Update()
	{
		if (Input.GetButtonDown("Pause"))
		{
			Pause();
		}
	}
}