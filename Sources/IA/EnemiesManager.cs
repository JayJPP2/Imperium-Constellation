using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class EnemiesManager : MonoBehaviour
{
	[SerializeField] GameObject basicEnemy;

	[SerializeField] GameObject basicAlly;

	[SerializeField] GameObject creditsManager;

	bool enemiesAlive;

	Squad[] enemySquads;
	Squad[] allySquads;

	float[] enemyTimers;
	float[] allyTimers;
	float[] enemyMaxTimers;
	float[] allyMaxTimers;

	private GameObject player;

	private const float MIN_TIME = 10f;
	private const float MAX_TIME = 30f;

	// Use this for initialization
	void Start()
	{
		player = GameObject.FindGameObjectWithTag("Player");
		enemiesAlive = true;
		victoryUI.SetActive(false);
		InitAllies(5, 5);
		InitEnemies(10, 5);
		enemyTimers = new float[enemySquads.Length];
		enemyMaxTimers = new float[enemySquads.Length];
		for (int i = 0; i < enemySquads.Length; i++)
		{
			enemyTimers[i] = 0f;
			enemyMaxTimers[i] = Random.Range(MIN_TIME, MAX_TIME);
			AssignTarget(enemySquads[i]);
		}
		allyTimers = new float[allySquads.Length];
		allyMaxTimers = new float[allySquads.Length];
		for (int i = 0; i < allySquads.Length; i++)
		{
			allyTimers[i] = 0f;
			allyMaxTimers[i] = Random.Range(MIN_TIME, MAX_TIME);
			AssignTarget(allySquads[i]);
		}
		StartCoroutine(CheckEnemiesAllDead());
	}

	[SerializeField] GameObject victoryUI;

	IEnumerator CheckEnemiesAllDead()
	{
		while (enemiesAlive)
		{
			yield return new WaitForSeconds(0.25f);
			GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
			enemiesAlive = false;
			for (int i = 0; i < enemies.Length; i++)
			{
				if (enemies[i].transform.parent == null)
				{
					enemiesAlive = true;
				}
			}
		}
		victoryUI.SetActive(true);
		creditsManager.GetComponent<CreditsManager>().AddCredits(10000);
        GameObject.FindGameObjectWithTag("PlanetCompleted").GetComponent<PlanetCompleted>().SelectPlanet[GameObject.FindGameObjectWithTag("PlanetCompleted").GetComponent<PlanetCompleted>().IDPlanetSelect] = true;
		yield return new WaitForSeconds(3f);
		SceneManager.LoadScene("ModuleCreation");
	}

	// Update is called once per frame
	void Update()
	{
		for (int i = 0; i < enemySquads.Length; i++)
		{
			enemyTimers[i] += Time.deltaTime;
			if (enemyTimers[i] > enemyMaxTimers[i])
			{
				enemyTimers[i] = 0f;
				enemyMaxTimers[i] = Random.Range(MIN_TIME, MAX_TIME);
				AssignTarget(enemySquads[i]);
			}
			enemySquads[i].Update();
			if (enemySquads[i].GetTarget() != null && enemySquads[i].GetTarget().GetPriority() == 0)
			{
				enemyTimers[i] = 0f;
				enemyMaxTimers[i] = Random.Range(MIN_TIME, MAX_TIME);
				AssignTarget(enemySquads[i]);
			}
		}
		for (int i = 0; i < allySquads.Length; i++)
		{
			allyTimers[i] += Time.deltaTime;
			if (allyTimers[i] > allyMaxTimers[i])
			{
				allyTimers[i] = 0f;
				allyMaxTimers[i] = Random.Range(MIN_TIME, MAX_TIME);
				AssignTarget(allySquads[i]);
			}
			allySquads[i].Update();
			if (allySquads[i].GetTarget() != null && allySquads[i].GetTarget().GetPriority() == 0)
			{
				allyTimers[i] = 0f;
				allyMaxTimers[i] = Random.Range(MIN_TIME, MAX_TIME);
				AssignTarget(allySquads[i]);
			}

		}
	}

	private void InitAllies(int nbSquads, int nbUnitsPerSquad) // WORK IN PROGRESS
	{
		allySquads = new Squad[nbSquads];
		Vector3 worldCenter = transform.position;
		float radius = GetComponent<outZone>().GetRadius() * 0.25f;
		for (int i = 0; i < nbSquads; i++)
		{
			allySquads[i] = new Squad();
			allySquads[i].Init(AIType.AIAlly);
			Vector3 leaderPos = worldCenter + Vector3.up * Random.Range(-radius, radius) + Vector3.right * Random.Range(-radius, radius) + Vector3.forward * Random.Range(-radius, radius);
			for (int j = 0; j < nbUnitsPerSquad; j++)
			{
				GameObject go = Instantiate(basicAlly, leaderPos + basicAlly.transform.right * (j % 2 == 0 ? -j / 2 : (j / 2) + 1) * 10f, basicAlly.transform.rotation);
				go.GetComponent<Unit>().Init();
				allySquads[i].AddUnit(go);
			}
			allySquads[i].ChooseLeader();
		}
	}

	private void InitEnemies(int nbSquads, int nbUnitsPerSquad) // WORK IN PROGRESS
	{
		enemySquads = new Squad[nbSquads];
		Vector3 worldCenter = transform.position;
		float radius = GetComponent<outZone>().GetRadius() * 0.25f;
		for (int i = 0; i < nbSquads; i++)
		{
			enemySquads[i] = new Squad();
			enemySquads[i].Init(AIType.AIEnemy);
			Vector3 leaderPos = worldCenter + Vector3.up * Random.Range(-radius, radius) + Vector3.right * Random.Range(-radius, radius) + Vector3.forward * Random.Range(-radius, radius);
			for (int j = 0; j < nbUnitsPerSquad; j++)
			{
				GameObject go = Instantiate(basicEnemy, leaderPos + basicEnemy.transform.right * (j % 2 == 0 ? -j / 2 : (j / 2) + 1) * 10f, basicEnemy.transform.rotation);
				go.GetComponent<Unit>().Init();
				enemySquads[i].AddUnit(go);
			}
			enemySquads[i].ChooseLeader();
		}
	}

	private void AssignTarget(Squad _squad) // WORK IN PROGRESS
	{
		if (_squad.GetPriority() == 0)
		{
			return;
		}
		int maxPriority = -1000;
		int index = -1;
		if (_squad.GetAIType() == AIType.AIAlly)
		{
			for (int i = 0; i < enemySquads.Length; i++)
			{
				int priority = enemySquads[i].GetPriority();
				if (priority > 0)
				{
					priority -= (int)Vector3.Distance(_squad.GetMiddlePos(), enemySquads[i].GetMiddlePos()) / 50;
					for (int j = 0; j < allySquads.Length; j++)
					{
						if (allySquads[j] != _squad && allySquads[j].GetTarget() == enemySquads[i])
						{
							priority -= allySquads[j].GetPriority();
						}
					}
					if (priority > maxPriority)
					{
						index = i;
						maxPriority = priority;
					}
				}
			}
			if (index != -1)
			{
				_squad.SetTarget(enemySquads[index]);
			}
		}
		else
		{
			for (int i = 0; i < allySquads.Length; i++)
			{
				int priority = allySquads[i].GetPriority();
				if (priority > 0)
				{
					priority -= (int)Vector3.Distance(_squad.GetMiddlePos(), allySquads[i].GetMiddlePos()) / 50;
					for (int j = 0; j < enemySquads.Length; j++)
					{
						if (enemySquads[j] != _squad && enemySquads[j].GetTarget() == allySquads[i])
						{
							priority -= enemySquads[j].GetPriority();
						}
					}
					if (priority > maxPriority)
					{
						index = i;
						maxPriority = priority;
					}
				}
			}
			if (index != -1)
			{
				_squad.SetTarget(allySquads[index]);
			}
			else
			{
				maxPriority = -1000;
			}
			int playerPriority = 60 - (int)Vector3.Distance(_squad.GetMiddlePos(), player.transform.position) / 50;
			for (int j = 0; j < enemySquads.Length; j++)
			{
				if (enemySquads[j] != _squad && enemySquads[j].GetTarget() == null)
				{
					playerPriority -= enemySquads[j].GetPriority();
				}
			}
			if (maxPriority < playerPriority)
			{
				_squad.SetTarget(null);
			}
		}
	}
}