using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceParticles : MonoBehaviour
{
    static public readonly float minReactorStartLifeTime = 0.00001f;
    private float durationIgnition = 4.0f;
    private float cptDuration = -1.0f;

    static private Ship ShipRef = null;
    static private ConstructionModuleMain CMMRef = null;

    private bool EngineAreMinOut;
    private bool EngineIsMinimal;
    private float lastThrottleValue = -1.0f;
    private bool isIA;
    
    private enum IDRef
    {
        StartLifeTime,
        minimalStartLifeTime,
        StartSpeed,
        minimalStartSpeed
    }

    //Particles System
    Dictionary<ParticleSystem, float[]> particlesSystemsDictionnary;

    private void Start()
    {
        particlesSystemsDictionnary = new Dictionary<ParticleSystem, float[]>();

        isIA = false;

        if ((SceneController.IDScene == SceneController.SceneID.MainGameScene || SceneController.IDScene == SceneController.SceneID.UniverseMap) && ShipRef == null)
        {
            try
            {
                ShipRef = gameObject.transform.parent.parent.parent.GetComponent<Ship>();
            }
            catch(Exception e)
            {
                ShipRef = null;
                Debug.LogError(gameObject.name + " : " + e.Message);
            }
            CMMRef = null;
        }
        else if(SceneController.IDScene == SceneController.SceneID.Editor)
        {

            CMMRef = ConstructionModuleMain.GetSingleton();
            ShipRef = null;
        }

        ParticleSystem[] tempArray = GetComponentsInChildren<ParticleSystem>();

        for (int i = 0; i < tempArray.Length; i++)
        {
            ParticleSystem temp = tempArray[i];

            if (temp != null)
            {
                float[] holder = new float[4];
                holder[(int)IDRef.StartLifeTime] = temp.main.startLifetime.constant;
                holder[(int)IDRef.StartSpeed] = temp.main.startSpeed.constant;

                holder[(int)IDRef.minimalStartLifeTime] = temp.main.startLifetime.constant * 0.6f;
                holder[(int)IDRef.minimalStartSpeed] = temp.main.startSpeed.constant * 0.6f;

                particlesSystemsDictionnary.Add(temp, holder);

                var mainTest = temp.main;

                mainTest.startLifetime = mainTest.startSpeed = minReactorStartLifeTime;
            }

        }

        EngineAreMinOut = false;
        EngineIsMinimal = false;

        if(transform.root.tag != "Player")
        {
            isIA = true;
            foreach (ParticleSystem system in particlesSystemsDictionnary.Keys)
            {
                var mainTest = system.main;
                mainTest.startSpeed = particlesSystemsDictionnary[system][(int)IDRef.StartSpeed];
                mainTest.startLifetime = particlesSystemsDictionnary[system][(int)IDRef.StartLifeTime];
            }
        }
    }

    private void OnBecameInvisible()
    {
        foreach (var item in particlesSystemsDictionnary.Keys)
        {
            item.Stop();
        }
    }

    private void OnBecameVisible()
    {
        foreach (var item in particlesSystemsDictionnary.Keys)
        {
            item.Play();
        }
    }

    private void UpdateGame()
    {
        if(ShipRef == null || ShipRef.Throttle == lastThrottleValue)
        {
            return;
        }

        foreach (ParticleSystem system in particlesSystemsDictionnary.Keys)
        {
            var mainTest = system.main;
            mainTest.startSpeed = Mathf.Lerp(particlesSystemsDictionnary[system][(int)IDRef.minimalStartSpeed], particlesSystemsDictionnary[system][(int)IDRef.StartSpeed], ShipRef == null ? 0.0f : ShipRef.Throttle);
            mainTest.startLifetime = Mathf.Lerp(particlesSystemsDictionnary[system][(int)IDRef.minimalStartLifeTime], particlesSystemsDictionnary[system][(int)IDRef.StartLifeTime], ShipRef == null ? 0.0f : ShipRef.Throttle);
        }
        lastThrottleValue = ShipRef.Throttle;
    }

    private void UpdateEditor()
    {
        if (CMMRef.ShipFiredUp)
        {
            EngineIsMinimal = true;
            if (CMMRef.ShipFiredUpMax)
            {
                if(cptDuration > durationIgnition)
                {
                    return;
                }
                EngineAreMinOut = false;
                cptDuration += Time.deltaTime;
                foreach (ParticleSystem system in particlesSystemsDictionnary.Keys)
                {
                    var mainTest = system.main;
                    mainTest.startLifetime = Mathf.Lerp(particlesSystemsDictionnary[system][(int)IDRef.minimalStartLifeTime], particlesSystemsDictionnary[system][(int)IDRef.StartLifeTime], cptDuration / durationIgnition);
                    mainTest.startSpeed = Mathf.Lerp(particlesSystemsDictionnary[system][(int)IDRef.minimalStartSpeed], particlesSystemsDictionnary[system][(int)IDRef.StartSpeed], cptDuration / durationIgnition);
                }
            }
            else
            {
                if(EngineAreMinOut)
                {
                    return;
                }
                foreach (ParticleSystem system in particlesSystemsDictionnary.Keys)
                {
                    var mainTest = system.main;
                    mainTest.startSpeed = particlesSystemsDictionnary[system][(int)IDRef.minimalStartSpeed];
                    mainTest.startLifetime = particlesSystemsDictionnary[system][(int)IDRef.minimalStartLifeTime];
                }
                EngineAreMinOut = true;
                cptDuration = 0.0f;
            }
        }
        else
        {
            if(!EngineIsMinimal)
            {
                return;
            }
            EngineAreMinOut = false;
            foreach (ParticleSystem system in particlesSystemsDictionnary.Keys)
            {
                var mainTest = system.main;
                mainTest.startLifetime = mainTest.startSpeed = minReactorStartLifeTime;
            }
            cptDuration = 0.0f;
            EngineIsMinimal = false;
        }
    }

    private void Update()
    {
        if (isIA)
            return;

        if (SceneController.IDScene == SceneController.SceneID.MainGameScene)
        {
            UpdateGame();
        }
        else if(SceneController.IDScene == SceneController.SceneID.Editor)
        {
            UpdateEditor();
        }
    }
}
