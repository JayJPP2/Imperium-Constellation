using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentManager : MonoBehaviour {

    float min = -500.0f;
    float max = 500.0f;

    [SerializeField] GameObject[] prefabAsteroids;

    // Use this for initialization
    void Start ()
    {
		GameObject worldCenter = GameObject.Find("worldCenter");
		max = worldCenter.GetComponent<outZone>().GetRadius();
		min = -max;
		max *= 2f;
		min *= 2f;
		AsteroidsFill(worldCenter.transform.position);
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    void AsteroidsFill(Vector3 center)
    {
        Transform ts = GameObject.Find("Environment").transform;
		int nbAsteroids = Random.Range(200, 800);

		for (int i = 0; i < nbAsteroids; i++)
        {
            
            Quaternion rotation = new Quaternion(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
            Vector3 position = new Vector3(Random.Range(min, max), Random.Range(min, max), Random.Range(min, max));
			float scale = Random.Range(0f, 1f) < 0.8f ? Random.Range(2f, 25f) : Random.Range(25f, 100f);
            GameObject go = Instantiate(prefabAsteroids[Random.Range(0, prefabAsteroids.Length)]);
            go.transform.position = position + center;
            go.transform.rotation = rotation;
			go.transform.localScale = new Vector3(scale, scale, scale);

            go.transform.SetParent(ts, true);
            go.GetComponent<Rigidbody>().velocity = Vector3.zero;
			go.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.Continuous;
        }
    }
}
