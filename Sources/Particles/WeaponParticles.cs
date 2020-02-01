using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponParticles : MonoBehaviour
{
    static public readonly float minReactorStartLifeTime = 0.00001f;
    private float durationIgnition = 4.0f;

    ParticleSystem[] particlesArray;
    float[] maxValueStartLifeTime;
    float[] StartSpeed;


    private void OnBecameInvisible()
    {
        foreach (var item in particlesArray)
        {
            item.Stop();
        }
    }

    private void OnBecameVisible()
    {
        foreach (var item in particlesArray)
        {
            item.Play();
        }
    }

    IEnumerator CoroutineExaust()
    {
        float timer = 0.0f;
        float maxTimer = 0.2f;

        while(timer < maxTimer)
        {
            for (int i = 0; i < particlesArray.Length; i++)
            {
                var mainTest = particlesArray[i].main;
                timer += Time.deltaTime;
                
                mainTest.startLifetime = Mathf.Lerp(minReactorStartLifeTime, maxValueStartLifeTime[i], timer / maxTimer);
                mainTest.startSpeed = Mathf.Lerp(minReactorStartLifeTime, StartSpeed[i], timer / maxTimer);

                yield return new WaitForEndOfFrame();
            }
        }
    }


    public void Fire()
    {
        StartCoroutine(CoroutineExaust());
    }

    // Use this for initialization
    public void CustomInit ()
    {
        particlesArray = GetComponentsInChildren<ParticleSystem>();
        maxValueStartLifeTime = new float[particlesArray.Length];
        StartSpeed = new float[particlesArray.Length];

        for (int i = 0; i < particlesArray.Length; i++)
        {
            maxValueStartLifeTime[i] = particlesArray[i].main.startLifetime.constant;
            StartSpeed[i] = particlesArray[i].main.startSpeed.constant;

            var mainTest = particlesArray[i].main;

            mainTest.startLifetime = mainTest.startSpeed = minReactorStartLifeTime;
        }
    }


}
