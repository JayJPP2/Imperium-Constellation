using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class outZone : MonoBehaviour {

    Vector3 pos;
    [SerializeField] float radius = 100f;

	public float GetRadius()
	{
		return radius;
	}

    GameObject player;

	// Use this for initialization
	void Start () {
        pos = transform.position;
        player = null;
     //   StartCoroutine(VerifyPlayerInZone(5f));
	}

    IEnumerator VerifyPlayerInZone(float timer)
    {
        while (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            yield return new WaitForEndOfFrame();
        }
        while (true)
        {
            yield return new WaitForSeconds(timer);
            if (Vector3.Distance(player.transform.position, pos) > radius * 1.10f)
            {
				//player.GetComponent<Ship>().TakeOutZoneDamage();
				GameObject.FindGameObjectWithTag("UI2D").GetComponent<UI2D>().OutOfZone(true);
            }
			else
			{
				GameObject.FindGameObjectWithTag("UI2D").GetComponent<UI2D>().OutOfZone(false);
			}
		}
    }
}
