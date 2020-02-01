using UnityEngine;
using System.Collections;

public class PlanetCompleted : MonoBehaviour
{
    public bool[] SelectPlanet;
    public int IDPlanetSelect;

    void Awake()
    {
        if (GameObject.FindGameObjectsWithTag("PlanetCompleted").Length > 1)
        {
            Destroy(this);
        }

        for (int i = 0; i < 6; i++)
        {
            SelectPlanet[i] = false;
        }

        DontDestroyOnLoad(this);
    }
    
}
