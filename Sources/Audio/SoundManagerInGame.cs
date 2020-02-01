using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManagerInGame : MonoBehaviour
{

    AudioSource AmbientSource;
    [SerializeField] AudioClip AmbientClip;

    #region uselesscode
    //AudioDistortionFilter disto;
    //AudioLowPassFilter lowPass;
    //// Ajout d'un filtre lowpass = Consiste à ne laisser passer que les sons graves
    //// On considère qu'il "étouffe" le son
    //lowPass = gameObject.AddComponent<AudioLowPassFilter>();
    //    lowPass.cutoffFrequency = 200;

    //    // Ajout d'un filtre distortion = Consite à ajouter du "gain" de volume en entrée d'un son, afin de le faire saturer
    //    disto = gameObject.AddComponent<AudioDistortionFilter>();
    //    disto.distortionLevel = 0.6f;
    //    audioSourceReactor = gameObject.GetComponent<AudioSource>();
    #endregion

    void Start()
    {
        AmbientSource = gameObject.GetComponent<AudioSource>();
        SettingsControllersMenu.GetSingleton().RefferenceAudioSource(AmbientSource);
    }
    
   

    // Méthode SwitchSound qui gérera le son du moteur
    public void SwitchSound()
    {
       
    }

    public void SoundShoot()
    {

    }

    public void PlayAmbientSound()
    {
        AmbientSource.Play();
       

    }


    void Update()
    {
        SwitchSound();
    }
}
