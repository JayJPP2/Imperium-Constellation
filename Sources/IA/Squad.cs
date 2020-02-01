using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Squad
{
	private List<GameObject> units;
	private GameObject leader;

	private Squad target; // null means it is targetting player

	private GameObject player;

	public void SetTarget(Squad _target)
	{
		if (target != _target)
		{
			target = _target;
			hasMetTarget = false;
			leader.GetComponent<Unit>().InitState();
			for (int i = 0; i < units.Count; i++)
			{
				units[i].GetComponent<Unit>().InitState();
			}
		}
	}

	public Squad GetTarget()
	{
		return target;
	}

	private AIType type;

	public AIType GetAIType()
	{
		return type;
	}

	private int priority;

	public int GetPriority()
	{
		return priority;
	}

	public Vector3 GetMiddlePos()
	{
		Vector3 pos = leader.transform.position;
		foreach (GameObject go in units)
		{
			pos += go.transform.position;
		}
		pos /= units.Count + 1;
		return pos;
	}

	private bool hasMetTarget;

	public void Init(AIType _type)
	{
		type = _type;
		units = new List<GameObject>();
		leader = null;
		target = null;
		priority = 0;
		hasMetTarget = false;
		player = GameObject.FindGameObjectWithTag("Player");
	}

	public void Update()
	{
		if (!hasMetTarget)
		{
			if (type == AIType.AIAlly)
			{
				if (target.priority == 0)
				{
					leader.GetComponent<Unit>().SetTargetPos(leader.transform.position + leader.transform.forward * 1000f);
					for (int i = 0; i < units.Count; i++)
					{
						units[i].GetComponent<Unit>().SetTargetPos(leader.transform.position + leader.transform.forward * 1000f + leader.transform.right * (i % 2 == 1 ? -(i + 1) / 2 : (i + 2) / 2) * 10f);
					}
					return;
				}
				leader.GetComponent<Unit>().SetTargetPos(target.leader.transform.position);
				for (int i = 0; i < units.Count; i++)
				{
					units[i].GetComponent<Unit>().SetTargetPos(leader.transform.position + leader.transform.right * (i % 2 == 1 ? -(i + 1) / 2 : (i + 2) / 2) * 10f);
				}
			}
			else
			{
				leader.GetComponent<Unit>().SetTargetPos(target != null && target.priority > 0 ? target.leader.transform.position : player.transform.position);
				for (int i = 0; i < units.Count; i++)
				{
					units[i].GetComponent<Unit>().SetTargetPos(leader.transform.position + leader.transform.right * (i % 2 == 1 ? -(i + 1) / 2 : (i + 2) / 2) * 10f);
				}
			}
			if (Vector3.Distance(leader.transform.position, target != null && target.priority > 0 ? target.leader.transform.position : player.transform.position) < 250f)
			{
				hasMetTarget = true;
				leader.GetComponent<Unit>().FightState();
				for (int i = 0; i < units.Count; i++)
				{
					units[i].GetComponent<Unit>().FightState();
				}
				AssignTarget(leader);
				for (int i = 0; i < units.Count; i++)
				{
					AssignTarget(units[i]);
				}
			}
		}
		else
		{
			// RENFORTS SI DES UNITES SONT EN FUITE
		}
	}

	public void ChooseLeader() // WORK IN PROGRESS
	{
		if (units == null || units.Count == 0)
		{
			return;
		}
		leader = units[0];
		units.Remove(leader);
	}

	public void AddUnit(GameObject _unit) // WORK IN PROGRESS
	{
		units.Add(_unit);
		_unit.GetComponent<Unit>().SetSquad(this);
		priority += (int)_unit.GetComponent<Unit>().GetUnitType();
	}

	public void RemoveUnit(GameObject _unit) // WORK IN PROGRESS
	{
		if (_unit == leader)
		{
			ChooseLeader();
		}
		else
		{
			units.Remove(_unit);
		}
		priority -= (int)_unit.GetComponent<Unit>().GetUnitType();
	}

	public void AssignTarget(GameObject _unit) // WORK IN PROGRESS
	{
		if (target == null)
		{
			_unit.GetComponent<Unit>().SetTarget(player, true);
		}
		else
		{
			if (target.priority == 0)
			{
				return;
			}
			int maxPriority = (int)target.leader.GetComponent<Unit>().GetUnitType();
			maxPriority -= (int)Vector3.Distance(_unit.transform.position, target.leader.transform.position) / 50;
			if (leader != _unit && leader.GetComponent<Unit>().GetTarget() == target.leader)
			{
				maxPriority -= (int)leader.GetComponent<Unit>().GetUnitType();
			}
			for (int j = 0; j < units.Count; j++)
			{
				if (units[j] != _unit && units[j].GetComponent<Unit>().GetTarget() == target.leader)
				{
					maxPriority -= (int)units[j].GetComponent<Unit>().GetUnitType();
				}
			}
			GameObject newTarget = target.leader;
			for (int i = 0; i < target.units.Count; i++)
			{
				int priority = (int)target.units[i].GetComponent<Unit>().GetUnitType();
				priority -= (int)Vector3.Distance(_unit.transform.position, target.units[i].transform.position) / 50;
				if (leader != _unit && leader.GetComponent<Unit>().GetTarget() == target.units[i])
				{
					priority -= (int)leader.GetComponent<Unit>().GetUnitType();
				}
				for (int j = 0; j < units.Count; j++)
				{
					if (units[j] != _unit && units[j].GetComponent<Unit>().GetTarget() == target.units[i])
					{
						priority -= (int)units[j].GetComponent<Unit>().GetUnitType();
					}
				}
				if (priority > maxPriority)
				{
					maxPriority = priority;
					newTarget = target.units[i];
				}
			}
			_unit.GetComponent<Unit>().SetTarget(newTarget);
		}
	}
}

public enum AIType
{
	AIAlly,
	AIEnemy,
}