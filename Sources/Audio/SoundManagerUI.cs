using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManagerUI : MonoBehaviour {

    AudioSource audioSourceUI, ambientSourceMenu;
    AudioClip  uiBackSound, uiSelectSound, ambientClip;
   

    // Use this for initialization
    void Start () {

        audioSourceUI = GetComponent<AudioSource>();
        ambientSourceMenu = GetComponent<AudioSource>();

        uiBackSound = Resources.Load<AudioClip>("Audio/UI/Cancel");
        uiSelectSound = Resources.Load<AudioClip>("Audio/UI/Validate");
        ambientClip = Resources.Load<AudioClip>("Audio/Ambiance/MenuAmbiance");

        audioSourceUI.clip = uiBackSound;

        SettingsControllersMenu.GetSingleton().RefferenceAudioSource(audioSourceUI);
        SettingsControllersMenu.GetSingleton().RefferenceAudioSource(ambientSourceMenu);

     
    }

    // Update is called once per frame
    void Update () {
      
    }

    public void PlayUISoundSelect()
    {
        audioSourceUI.clip = uiSelectSound;
        audioSourceUI.Play();
    }

    public void PlayUISoundBack()
    {
        audioSourceUI.clip = uiBackSound;

        audioSourceUI.Play();
        
    }

 
}
