using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManagerInLevelMap : MonoBehaviour {

    private AudioSource _audioSource;
    private void Awake()
    {
     
        _audioSource = GetComponent<AudioSource>();
        SettingsControllersMenu.GetSingleton().RefferenceAudioSource(_audioSource);
    }

    public void Update()
    {
        PlayMusic();
        
    }

    public void PlayMusic()
    {
        if (_audioSource.isPlaying) return;
        _audioSource.Play();
        
    }

    public void StopMusic()
    {
        _audioSource.Stop();
    }
}
