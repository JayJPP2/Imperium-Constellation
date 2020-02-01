using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManagerEditor : MonoBehaviour {

    AudioSource EditorSource;
    [SerializeField] AudioClip EngineSound, LoadingSound, ValidateSound, CancelSound, DeleteSound;
    
	// Use this for initialization
	void Start () {
        EditorSource = gameObject.GetComponent<AudioSource>();
        SettingsControllersMenu.GetSingleton().RefferenceAudioSource(EditorSource);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void PlayEngineSoundForPreview()
    {
        EditorSource.clip = EngineSound;
        EditorSource.Play();
        
    }

    public void PlayLoadingSound()
    {
        EditorSource.clip = LoadingSound;
        EditorSource.Play();
       
    }

    public void PlayValidateSound()
    {
        EditorSource.clip = ValidateSound;
        EditorSource.Play();
      
    }

    public void PlayCancelSound()
    {
        EditorSource.clip = CancelSound;
        EditorSource.Play();
      
    }

    public void PlayDeleteSaveSound()
    {
        EditorSource.clip = DeleteSound;
        EditorSource.Play();
      
    }

}
