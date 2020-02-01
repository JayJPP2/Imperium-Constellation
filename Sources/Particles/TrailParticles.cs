using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TrailParticles : MonoBehaviour {

    static public readonly float minReactorStartLifeTime = 0.00001f;
    private float durationIgnition = 4.0f;
    private float cptDuration = -1.0f;

    static private Ship ShipRef = null;
    static private ConstructionModuleMain CMMRef = null;

    private float lastThrottleValue = -1.0f;
    private bool isIA;

    private enum IDRefTrailParticle
    {
        StartSpeed,
        StartLifeTime
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
                ShipRef = gameObject.transform.root.GetComponent<Ship>();
            }
            catch (Exception e)
            {
                ShipRef = null;
                Debug.LogError(gameObject.name + " : " + e.Message);
            }
            CMMRef = null;
        }
        else if (SceneController.IDScene == SceneController.SceneID.Editor)
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
                float[] holder = new float[2];
                holder[(int)IDRefTrailParticle.StartLifeTime] = temp.main.startLifetime.constant;
                holder[(int)IDRefTrailParticle.StartSpeed] = temp.main.startSpeed.constant;

                particlesSystemsDictionnary.Add(temp, holder);

                var mainTest = temp.main;

                mainTest.startLifetime = mainTest.startSpeed = minReactorStartLifeTime;
            }

        }

        if (transform.root.tag != "Player")
        {
            isIA = true;
            foreach (ParticleSystem system in particlesSystemsDictionnary.Keys)
            {
                var mainTest = system.main;
                mainTest.startSpeed = particlesSystemsDictionnary[system][(int)IDRefTrailParticle.StartSpeed];
                mainTest.startLifetime = particlesSystemsDictionnary[system][(int)IDRefTrailParticle.StartLifeTime];
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
        if (ShipRef == null || ShipRef.Throttle == lastThrottleValue)
        {
            return;
        }

        foreach (ParticleSystem system in particlesSystemsDictionnary.Keys)
        {
            var mainTest = system.main;
            mainTest.startSpeed = Mathf.Lerp(minReactorStartLifeTime, particlesSystemsDictionnary[system][(int)IDRefTrailParticle.StartSpeed], ShipRef == null ? 0.0f : ShipRef.Throttle);
            mainTest.startLifetime = Mathf.Lerp(minReactorStartLifeTime, particlesSystemsDictionnary[system][(int)IDRefTrailParticle.StartLifeTime], ShipRef == null ? 0.0f : ShipRef.Throttle);
        }
        lastThrottleValue = ShipRef.Throttle;
    }

    private void Update()
    {
        if (isIA)
        {
            /*foreach(var item in particlesSystemsDictionnary.Keys)
            {
                Destroy(item.gameObject);
            }
            Destroy(this);*/
            return;
        }

        if (SceneController.IDScene == SceneController.SceneID.MainGameScene)
        {
            UpdateGame();
        }
    }
}
