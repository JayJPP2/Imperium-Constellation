using System;
using System.IO;
using System.Collections;
using UnityEngine;
using Cinemachine;
using RenderHeads.Media.AVProMovieCapture;
using UnityEngine.UI;

public class ScreenShotCamera : MonoBehaviour {

    private static Camera saveCamera;
    private static CinemachineDollyCart refScriptCart;
    private static CinemachineSmoothPath refScriptTrack;
    private static CaptureFromCamera refScriptVideoRecorder;
    private static Text SaveUnderWayText;

    [SerializeField] public static float Speed;
    private static float durationForVideoCapture = 4.0f;
    private static float timer = -0.09f;

    private static ScreenShotCamera Instance;

    public static ScreenShotCamera GetSingleton()
    {
        return Instance;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        SaveUnderWayText = GameObject.Find("SaveUnderWayText").GetComponent<Text>();
        SaveUnderWayText.gameObject.transform.parent.gameObject.SetActive(false);
    }


    // Use this for initialization
    void Start ()
    {
        if (saveCamera == null)
        {
            saveCamera = GetComponent<Camera>();
        }

        Speed = 14.0f;
        refScriptCart = gameObject.transform.GetChild(1).gameObject.GetComponent<CinemachineDollyCart>();
        refScriptTrack = gameObject.transform.root.GetChild(1).gameObject.GetComponent<CinemachineSmoothPath>();
        refScriptVideoRecorder = gameObject.GetComponent<CaptureFromCamera>();
        refScriptVideoRecorder._outputFolderPath = Application.persistentDataPath + "/Save_Media/";
        refScriptCart.m_Position = 0;
        refScriptCart.m_Speed = 0.0f;
    }

    public string ScreenShotName(int width, int height)
    {
        return string.Format("{0}/Save_Media/screen_{1}x{2}_{3}.png",
                             Application.persistentDataPath,
                             width, height,
                             System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
    }

    private string ScreenShotSave(string saveName)
    {
        return(Application.persistentDataPath + "/Save_Media/" + saveName + ".spp");
    }

    IEnumerator CoroutineWaitForValidation()
    {
        int cpt = 0;
        while (!refScriptVideoRecorder.StartCapture() && cpt < 10)
        {
            cpt++;
        }

        if(cpt >= 10)
        {
            yield return new WaitForEndOfFrame();
        }

        refScriptCart.m_Speed = Speed;
        refScriptCart.m_Position = 0.0f;
        SaveUnderWayText.gameObject.transform.parent.gameObject.SetActive(true);

        yield return new WaitForEndOfFrame();
    }


    public bool ScreenShotCaptureForSave(string saveName)
    {
        refScriptCart.m_Position = 0;
        refScriptCart.m_Speed = 0;

        int tierShip = (int)ConstructionModuleMain.GetSingleton().DrawedPartsInfo[0].Tier;
        GameObject.Find("DollyForSave").transform.localScale = new Vector3(1.2f * tierShip, 1.2f * tierShip, 1.2f * tierShip);

        RenderTexture rt = new RenderTexture(Screen.width, Screen.height, 24);
        saveCamera.targetTexture = rt;
        Texture2D screenShot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        saveCamera.Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        saveCamera.targetTexture = null;
        RenderTexture.active = null; // JC: added to avoid errors
        Destroy(rt);
        byte[] bytes = screenShot.EncodeToPNG();
        string filename = ScreenShotSave(saveName);

        try
        {
            File.WriteAllBytes(filename, bytes);
        }
        catch(Exception)
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/Save_Media");
            File.WriteAllBytes(filename, bytes);
        }

        refScriptVideoRecorder._forceFilename = saveName + ".mp4";

        StartCoroutine(CoroutineWaitForValidation());
        
        return true;
    }

    private int littleIndex = 0;
    private float littleTimer = 0.0f;

    // Update is called once per frame
    void FixedUpdate ()
    {
        if (refScriptCart.m_Speed != 0)
        {
            refScriptCart.m_Position = Mathf.LerpUnclamped(0.0f, refScriptTrack.PathLength, timer / durationForVideoCapture);
            timer += Time.deltaTime;

            littleTimer += Time.deltaTime;

            if(littleTimer >= .25f)
            {
                littleTimer -= .25f;
                SaveUnderWayText.text = "SAVING UNDERWAY";
                for (int i = 0; i < littleIndex % 4; i++)
                {
                    SaveUnderWayText.text += ".";
                }
                littleIndex++;
            }

            if (timer > durationForVideoCapture)
            {
                try
                {
                    refScriptVideoRecorder.StopCapture();
                }
                catch(Exception e)
                {

                }
                refScriptCart.m_Speed = 0;
                refScriptCart.m_Position = 0.0f;
                timer = -0.09f;
                ConstructionModuleMain.GetSingleton().FreezeUIWhileSaving(false);
                SaveUnderWayText.gameObject.transform.parent.gameObject.SetActive(false);
                littleIndex = 0;
                littleTimer = 0.0f;
            }
        }
    }
}
