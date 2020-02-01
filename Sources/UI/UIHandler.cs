using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour {

    [SerializeField] List<GameObject> ButtonsList = new List<GameObject>();

    public GameObject LoadPanelAnimation;
    public RawImage RefUIBlack;
    private bool isLoading;

    private void Awake()
    {
        EventSystem.current.SetSelectedGameObject(GameObject.Find("PlayB"));
        Time.timeScale = 1f;

        isLoading = false;

        SceneController.GetSingleton().OverrideSceneID(SceneController.SceneID.MainMenu);
        Cursor.visible = false;
    }

    // Use this for initialization
    void Start () {
        GameObject parent = GameObject.Find("Canvas");
        for (int i = 0; i < parent.transform.childCount; i++)
        {
            if(gameObject.tag == "UIButton")
            {
                ButtonsList.Add(parent.transform.GetChild(i).gameObject);
            }
        }
	}

    IEnumerator CoroutineLoading()
    {
        LoadPanelAnimation.SetActive(true);
        AsyncOperation loadAsync = SceneManager.LoadSceneAsync("ModuleCreation", LoadSceneMode.Single);
        loadAsync.allowSceneActivation = false;
        Text tempText = LoadPanelAnimation.transform.GetChild(3).GetComponent<Text>();
        float cptr = 0.0f;
        float time = 0.0f;
        while (true)
        {
            LoadPanelAnimation.transform.GetChild(1).Rotate(Vector3.forward, 360.0f * Time.deltaTime);
            LoadPanelAnimation.transform.GetChild(2).Rotate(Vector3.back, 360.0f * Time.deltaTime);
            tempText.text = "Loading " + (int)((loadAsync.progress /0.9f) * 100) + "%";

            if (loadAsync.progress >= 0.9f)
            {
                cptr += Time.deltaTime;
                if (cptr > 1.0f)
                {
                    if (RefUIBlack.color.a != 1)
                    {
                        time += Time.deltaTime;
                        Color temp = RefUIBlack.color;
                        temp.a = Mathf.Lerp(0, 1, time);
                        RefUIBlack.color = temp;
                        yield return new WaitForEndOfFrame();
                    }
                    else
                    {
                        loadAsync.allowSceneActivation = true;
                        SceneController.GetSingleton().OverrideSceneID(SceneController.SceneID.Editor);
                        yield return null;
                    }
                }
            }
            yield return new WaitForEndOfFrame();
        }
    }

    public void PlayEditorBroadCastMsg()
    {
        if (!isLoading)
        {
            StartCoroutine(CoroutineLoading());
            isLoading = true;
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetSoundForButton(AudioSource audioSource)
    {
        audioSource = transform.GetComponent<AudioSource>();
    }
}
