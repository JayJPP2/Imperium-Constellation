using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using Newtonsoft.Json.Linq;
using UnityEngine.UI;
using Cinemachine;
using System;

public class InitGame : MonoBehaviour
{

	[SerializeField] public RawImage RefUIBlack;
	[SerializeField] public GameObject UI;
	[SerializeField] public GameObject Trailer_Camera;

	private bool introDone;


	IEnumerator TempoFadeIn()
	{
		GameObject camHolder = GameObject.Find("Intro_Cam");
		yield return new WaitForSeconds(.5f);
		Transform pos = GameObject.Find("Enemy(Clone)").transform;
		float time = 0.0f;

		camHolder.transform.position = pos.position;

		camHolder.GetComponent<CinemachineVirtualCamera>().LookAt = pos;

		while (RefUIBlack.color.a != 0)
		{
			time += Time.deltaTime;
			Color temp = RefUIBlack.color;
			temp.a = Mathf.Lerp(1, 0, time);
			RefUIBlack.color = temp;
			yield return new WaitForEndOfFrame();
		}

		time = 0.0f;

		yield return new WaitForSeconds(8.0f);

		while (RefUIBlack.color.a != 1)
		{
			time += Time.deltaTime;
			Color temp = RefUIBlack.color;
			temp.a = Mathf.Lerp(0, 1, time);
			RefUIBlack.color = temp;
			yield return new WaitForEndOfFrame();
		}

		time = 0.0f;

		yield return new WaitForSeconds(1.5f);

		Transform pos2 = GameObject.Find("Ally(Clone)").transform;
		camHolder.transform.position = pos2.position - pos.forward * 5.0f + pos.up * 2.0f;

		camHolder.GetComponent<CinemachineVirtualCamera>().LookAt = pos2;
		camHolder.GetComponent<CinemachineVirtualCamera>().Follow = pos2;

		while (RefUIBlack.color.a != 0)
		{
			time += Time.deltaTime;
			Color temp = RefUIBlack.color;
			temp.a = Mathf.Lerp(1, 0, time);
			RefUIBlack.color = temp;
			yield return new WaitForEndOfFrame();
		}

		yield return new WaitForSeconds(15.0f);
		GameObject.Find("Intro_Cam").SetActive(false);
		UI.SetActive(true);
		introDone = true;
		foreach (MeshCollider mc in colliders)
		{
			mc.enabled = true;
		}
		GameObject[] asteroids = GameObject.FindGameObjectsWithTag("Asteroid");
		GameObject player = GameObject.FindGameObjectWithTag("Player");
		foreach (GameObject go in asteroids)
		{
			if (Vector3.Distance(go.transform.position, player.transform.position) < go.transform.lossyScale.x * 1.5f)
			{
				go.SetActive(false);
			}
		}
		StartCoroutine(player.GetComponent<Ship>().GetPossibleTargets(0.5f));
	}

	private Coroutine saveCoro;

	// Use this for initialization
	void Awake()
	{
		GameObject temp = LoadShipManagement.LoadShip(GameManager.nameVessel);
		temp.transform.SetParent(GameObject.Find("Ship").transform);
		temp.GetComponent<WeaponShootManager>().GetShipReference(GameObject.Find("Ship").GetComponent<Ship>());

		introDone = false;
		saveCoro = StartCoroutine(TempoFadeIn());

		colliders = GameObject.Find("Ship").GetComponentsInChildren<MeshCollider>();
		foreach (MeshCollider mc in colliders)
		{
			mc.enabled = false;
		}
	}

	private MeshCollider[] colliders;

	private void Update()
	{
		if (Input.GetButtonDown("Jump") && !introDone)
		{
			introDone = true;
			StopCoroutine(saveCoro);
			try
			{
				GameObject.Find("Intro_Cam").SetActive(false);
			}
			catch (Exception e)
			{
			}
			UI.SetActive(true);
			RefUIBlack.gameObject.SetActive(false);
			foreach (MeshCollider mc in colliders)
			{
				mc.enabled = true;
			}
			GameObject[] asteroids = GameObject.FindGameObjectsWithTag("Asteroid");
			GameObject player = GameObject.FindGameObjectWithTag("Player");
			foreach (GameObject go in asteroids)
			{
				if (Vector3.Distance(go.transform.position, player.transform.position) < go.transform.lossyScale.x * 1.5f)
				{
					go.SetActive(false);
				}
			}
			StartCoroutine(GameObject.FindGameObjectWithTag("Player").GetComponent<Ship>().GetPossibleTargets(0.5f));
		}
		else if (Input.GetKeyDown(KeyCode.Keypad0))
		{
			Trailer_Camera.SetActive(!Trailer_Camera.activeSelf);
			UI.SetActive(!UI.activeSelf);
		}
        else if(Input.GetKeyDown(KeyCode.Keypad1))
        {
            UI.SetActive(!UI.activeSelf);
        }
    }
}
