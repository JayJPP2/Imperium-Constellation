using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager Instance;

    public static string nameVessel;

    public static GameManager GetSingleton()
    {
        return Instance;
    }

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    void Start ()
    {
        nameVessel = "Undefined";
    }

    public static void LoadVesselID(string vesselName)
    {
        nameVessel = vesselName;
    }

}
