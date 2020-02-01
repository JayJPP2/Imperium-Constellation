using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsControllersMenu : MonoBehaviour {

    private static SettingsControllersMenu Instance;

    public AudioMixer audioMixer;
    public Dropdown resolutionsDropdown;
    private Slider[] SlidersArray;

    private List<AudioSource>[] ArrayOfSounds;

    private float[] ValueIDChannel;

    private enum IDChannel
    {
        Main,
        UI,
        Ambiance,
        Action,
        CountIDChannel
    }

    public static SettingsControllersMenu GetSingleton()
    {
        if(Instance == null)
        {
            GameObject Go = new GameObject();
            Go.name = "DontDestroyOnLoad";
            Instance = Go.AddComponent<SettingsControllersMenu>();
            Instance.Awake();
            DontDestroyOnLoad(Go);
        }
        return Instance;
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if(Instance == null)
        {
            Instance = this;
        }

        audioMixer = Resources.Load<AudioMixer>("Audio/Mixer/Main");

        ValueIDChannel = new float[(int)IDChannel.CountIDChannel];

        GameObject goTemp = GameObject.Find("Sliders");

        if (goTemp != null)
        {
            SlidersArray = new Slider[(int)IDChannel.CountIDChannel];

            GameObject.Find("SettingsMenu").SetActive(false);

            for (int i = 0; i < (int)IDChannel.CountIDChannel; i++)
            {
                SlidersArray[i] = goTemp.transform.GetChild(i).GetComponent<Slider>();
            }
        }

        ArrayOfSounds = new List<AudioSource>[(int)IDChannel.CountIDChannel];

        for (int i = 0; i < (int)IDChannel.CountIDChannel; i++)
        {
            ArrayOfSounds[i] = new List<AudioSource>();
            if(SlidersArray != null)
            {
                ValueIDChannel[i] = SlidersArray[i].value;
            }
        }
    }

    Resolution[] resolutions;
    Dictionary<string, int> optionsMap;
    void Start()
    {
        resolutions = Screen.resolutions;
        resolutionsDropdown.ClearOptions();
        List<string> options = new List<string>();
        optionsMap = new Dictionary<string, int>();
        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option =  resolutions[i].width + "x" + resolutions[i].height + " Hz " +resolutions[i].refreshRate;
            if (resolutions[i].refreshRate >= 60)
            {
                options.Add(option);
                optionsMap.Add(option, i);
            }

            if(resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }
        resolutionsDropdown.AddOptions(options);
        resolutionsDropdown.value = currentResolutionIndex;
        resolutionsDropdown.RefreshShownValue();
        //SetQuality(QualitySettings.names.Length - 1);

    }

    public void SetResolution(int resolutionIndex)
    {   
       Resolution resolution = resolutions[optionsMap[resolutionsDropdown.options[resolutionIndex].text]];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    //public void SetQuality(int qualityIndex)
    //{
    //    QualitySettings.SetQualityLevel(qualityIndex);
    //}

    public void SetFullscreen(bool fullscreen)
    {
        if (fullscreen) Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
        else Screen.fullScreenMode = FullScreenMode.Windowed;

    }

    public void OnSave()
    {
        for (int i = 0; i < (int)IDChannel.CountIDChannel; i++)
        {
            //Debug.Log("index: " + i + " count: " + ArrayOfSounds[i].Count);
            if (SlidersArray[i].value != ValueIDChannel[i])
            {
                ValueIDChannel[i] = SlidersArray[i].value;
                for (int j = 0; j < ArrayOfSounds[i].Count; j++)
                {
                    ArrayOfSounds[i][j].volume = ValueIDChannel[i];
                }
            }
        }
    }

    public void RefferenceAudioSource(AudioSource newAS)
    {
        //"'/=(')=\'"\\
        int value = -1;
        //Debug.Log(newAS.outputAudioMixerGroup.name);
        if (newAS.outputAudioMixerGroup.name == IDChannel.Main.ToString())
        {
            AudioSource aS = ArrayOfSounds[(int)IDChannel.Main].Find(x => x.clip.name == newAS.clip.name);
            if (aS != null)
            {
               aS = newAS;
            }
            else
            {
                ArrayOfSounds[(int)IDChannel.Main].Add(newAS);
            }
           
            value = 0;
        }
        else if(newAS.outputAudioMixerGroup.name == IDChannel.Action.ToString())
        {
            ArrayOfSounds[(int)IDChannel.Action].Add(newAS);
            value = 3;
        }
        else if (newAS.outputAudioMixerGroup.name == IDChannel.UI.ToString())
        {
            ArrayOfSounds[(int)IDChannel.UI].Add(newAS);
            value = 1;
        }
        else if (newAS.outputAudioMixerGroup.name == IDChannel.Ambiance.ToString())
        {
            ArrayOfSounds[(int)IDChannel.Ambiance].Add(newAS);
            value = 2;
        }
        else
        {
            Debug.LogError("WrongID AudioSource mixer");
            return;
        }

        newAS.volume = ValueIDChannel[value];
        //Debug.Log(newAS.name  + newAS.volume);
    }

    //public void UpdateSoundVolume()
    //{
    //    for (int i = 0; i < SlidersArray.Length; i++)
    //    {
    //        float sliderValue = (SlidersArray[i].value * 100 );
    //        if(audioMixer.name == IDChannel.Action.ToString())
    //        {
    //            audioMixer.SetFloat(IDChannel.Action.ToString(), sliderValue);
    //        }
    //    }
      
    //}

    public void QuitApplication()
    {
        Application.Quit();
    }
}
    