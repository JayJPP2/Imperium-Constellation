using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using UnityEngine.EventSystems;
using System.Linq;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    private GameObject PlayButton;

    public static LevelManager Instance;
    private int saveSelectedButton;
    private GameObject PlanetDescription;

    private GameObject PlanetPreviewSphere;
    private GameObject PlanetPreviewSphereUI;
    private GameObject Hologram;
    private bool isLoading;



    [Header("Player")]
    public GameObject Player;
    public GameObject LevelToReach;
    public float speed = 10f;

    public GameObject LoadPanelAnimation;
    public RawImage RefUIBlack;

    public List<Toggle> TogglePlanetsList { get; private set; }

    public enum LevelType
    {
        Selected = 0, Available, Complete, Failed, Undefined
    }

    private static int ActualSystemLoaded;
    private static GameObject planetSelected;
    private GameObject[] planetPrefabArray;

    private int[] idPlanet;

    Dictionary<GameObject, LevelData> levelsDictionnary;

    static public bool needToFireUp = false;
    static public bool isSelected = false;
    static public bool isComplete = false;

    GameObject refCanvasPreview;

    GameObject TogglePlanetSelectionPrefab;
    public GameObject ToggleGroup;

    AudioClip ValidateClip;

    public static LevelManager GetSingleton()
    {
        if (Instance == null)
        {
            return null;
        }
        return Instance;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        isLoading = false;

        LoadPanelAnimation.SetActive(false);

        PlayButton = GameObject.Find("SelectLevelButton");

        saveSelectedButton = -1;
        ActualSystemLoaded = 0;

        //Pour la pres ----------------------------------------------------//


        levelsDictionnary = new Dictionary<GameObject, LevelData>();
        TogglePlanetsList = new List<Toggle>();
        TextAsset reader = Resources.Load<TextAsset>("LoadLevelJsons/SystemSolar");
        string fileData = reader.text;

        var JSON_File_Root = JObject.Parse(fileData);

        if (JSON_File_Root["Universe"].Value<JArray>().Count != 0)
        {
            JArray Systems = JSON_File_Root["Universe"].Value<JArray>();
            for (int j = 0; j < Systems.Count; j++)
            {
                var system = Systems[j];
                if (ActualSystemLoaded == system["System_ID"].Value<int>())
                {
                    Transform systemRoot = GameObject.Find("Levels").transform;

                    GameObject starPrefab = Resources.Load<GameObject>(system["Star"]["PrefabPath"].Value<string>());
                    GameObject instance = Instantiate(starPrefab);
                    instance.transform.SetParent(systemRoot);
                    instance.transform.position = new Vector3(system["Star"]["Star_Position"]["x"].Value<float>(), system["Star"]["Star_Position"]["y"].Value<float>(), system["Star"]["Star_Position"]["z"].Value<float>());
                    instance.name = system["Star"]["Star_Name"].Value<string>();
                    instance.transform.localScale = new Vector3(system["Star"]["Star_Scale"]["x"].Value<float>(), system["Star"]["Star_Scale"]["y"].Value<float>(), system["Star"]["Star_Scale"]["z"].Value<float>());
                    JArray planets = system["Planets"].Value<JArray>();
                    planetPrefabArray = new GameObject[planets.Count];

                    //Toggle
                    TogglePlanetSelectionPrefab = Resources.Load<GameObject>("PlanetSelectionUI/Button");

                    GameObject newToggle = null;
                    for (int i = 0; i < planets.Count; i++)
                    {
                        newToggle = Instantiate(TogglePlanetSelectionPrefab);
                        newToggle.transform.SetParent(ToggleGroup.transform.GetChild(0).GetChild(0).GetChild(0), false);
                        newToggle.name = planets[i]["Planet_ID"].Value<string>();
                        newToggle.transform.GetChild(0).GetComponent<Text>().text = newToggle.name;
                        newToggle.AddComponent<AudioSource>().clip = ValidateClip;
                        TogglePlanetsList.Add(newToggle.GetComponent<Toggle>());

                        idPlanet = new int[planets.Count];
                        idPlanet[i] = planets[i]["Planet_Name"].Value<int>();

                        planetPrefabArray[i] = Resources.Load<GameObject>(planets[i]["Planet_Graphics_PrefabPath"].Value<string>());
                        GameObject instancePlanet = Instantiate(planetPrefabArray[i]);
                        instancePlanet.transform.SetParent(systemRoot);
                        instancePlanet.transform.position = new Vector3(planets[i]["Planet_Position"]["x"].Value<float>(), planets[i]["Planet_Position"]["y"].Value<float>(), planets[i]["Planet_Position"]["z"].Value<float>());
                        instancePlanet.transform.localScale = new Vector3(planets[i]["Planet_Scale"]["x"].Value<float>(), planets[i]["Planet_Scale"]["y"].Value<float>(), planets[i]["Planet_Scale"]["z"].Value<float>());
                        instancePlanet.name = planets[i]["Planet_ID"].Value<string>();
                        LevelData newLeveldata = new LevelData(instancePlanet.name, planets[i]["Planet_Description"].Value<string>(), planets[i]["Planet_Reward"].Value<string>(), instancePlanet);
                        levelsDictionnary.Add(instancePlanet, newLeveldata);

                        /*if (GameObject.FindGameObjectWithTag("PlanetCompleted").GetComponent<PlanetCompleted>().SelectPlanet[i] == true)
                        {
                            Destroy(TogglePlanetsList[i].GetComponent<BoutonSelectBehavior>());
                            TogglePlanetsList[i].targetGraphic.color = new Color(0f, 1f, 0f);
                        }*/
                    }

                    for (int i = 1; i < TogglePlanetsList.Count - 1; i++)
                    {
                        Navigation navTemp = TogglePlanetsList[i].navigation;
                        navTemp.selectOnUp = TogglePlanetsList[i - 1];
                        navTemp.selectOnDown = TogglePlanetsList[i + 1];
                        TogglePlanetsList[i].navigation = navTemp;
                    }

                    Navigation navTempFirst = TogglePlanetsList[0].navigation;
                    navTempFirst.selectOnUp = TogglePlanetsList[TogglePlanetsList.Count - 1];
                    navTempFirst.selectOnDown = TogglePlanetsList[1];
                    TogglePlanetsList[0].navigation = navTempFirst;

                    Navigation navTempLast = TogglePlanetsList[TogglePlanetsList.Count - 1].navigation;
                    navTempLast.selectOnUp = TogglePlanetsList[TogglePlanetsList.Count - 2];
                    navTempLast.selectOnDown = TogglePlanetsList[0];
                    TogglePlanetsList[TogglePlanetsList.Count - 1].navigation = navTempLast;


                    systemRoot.gameObject.SetActive(false);
                    break;
                }
            }
        }
        refCanvasPreview = GameObject.Find("LoadLevelCanvas");
        PlanetDescription = refCanvasPreview.transform.Find("PlanetDescription").gameObject;
        PlanetDescription.SetActive(false);

        PlanetPreviewSphere = GameObject.Find("PlanetPreviewSphere");
        PlanetPreviewSphereUI = GameObject.Find("BackgroundOfPlanetPreview");
        PlanetPreviewSphere.SetActive(false);
        Hologram = GameObject.Find("Mesh_halo_projecteur02");
        Hologram.SetActive(false);
        //PlanetPreviewSphereUI.SetActive(false);
    }

    void Start()
    {
        planetSelected = null;
        needToFireUp = false;
        isComplete = false;


    }

    public void RemoveCanvasPreview()
    {
        if (planetSelected != null)
        {
            levelsDictionnary[planetSelected].ChangeSelectionState(false);
        }



    }

    public void SetOrbitCanvas(int index)
    {
        Hologram.SetActive(true);
        planetSelected = levelsDictionnary.ElementAt(index).Key;
        Debug.Log(index);
        Debug.Log(levelsDictionnary);
        PlanetDescription.transform.GetChild(0).GetComponent<TextMesh>().text = levelsDictionnary[planetSelected].Name;
        PlanetDescription.transform.GetChild(1).GetComponent<TextMesh>().text = levelsDictionnary[planetSelected].Description;
        PlanetDescription.transform.GetChild(2).GetComponent<TextMesh>().text = levelsDictionnary[planetSelected].Reward;

        PlanetPreviewSphere.GetComponent<MeshRenderer>().material = planetSelected.GetComponent<MeshRenderer>().material;
    }

    public void ClearOrbitCanvas()
    {
        PlanetDescription.transform.GetChild(0).GetComponent<TextMesh>().text = "";
        PlanetDescription.transform.GetChild(1).GetComponent<TextMesh>().text = "";
        PlanetDescription.transform.GetChild(2).GetComponent<TextMesh>().text = "";
        PlanetDescription.SetActive(false);
        PlanetPreviewSphere.SetActive(false);
        Hologram.SetActive(false);
        //PlanetPreviewSphereUI.SetActive(false);
    }

    public void CancelSelection()
    {
        levelsDictionnary[planetSelected].ChangeSelectionState(false);
        planetSelected = null;

    }

    IEnumerator CoroutineLoadingToGame()
    {
        LoadPanelAnimation.SetActive(true);
        AsyncOperation loadAsync = SceneManager.LoadSceneAsync("Game", LoadSceneMode.Single);
        loadAsync.allowSceneActivation = false;
        Text tempText = LoadPanelAnimation.transform.GetChild(3).GetComponent<Text>();
        float cptr = 0.0f;
        float time = 0.0f;
        while (true)
        {
            LoadPanelAnimation.transform.GetChild(1).Rotate(Vector3.forward, 360.0f * Time.deltaTime);
            LoadPanelAnimation.transform.GetChild(2).Rotate(Vector3.back, 360.0f * Time.deltaTime);
            tempText.text = "Loading " + (int)((loadAsync.progress / 0.9f) * 100) + "%";

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
                        SceneController.GetSingleton().OverrideSceneID(SceneController.SceneID.MainGameScene);
                        yield return null;
                    }
                }
            }
            yield return new WaitForEndOfFrame();
        }
    }



    public void StartSelectingPlanet()
    {
        if (!BoutonSelectBehavior.isSelected)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(TogglePlanetsList[0].gameObject);
            PlanetDescription.SetActive(true);
            PlanetPreviewSphere.SetActive(true);
        }
        else
        {
            if(!isLoading)
            {
                StartCoroutine(CoroutineLoadingToGame());
                isLoading = true;
            }
        }
    }

    public void PlanetHasBeenSelected(int index)
    {
        saveSelectedButton = index;
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(PlayButton);
        GameObject.FindGameObjectWithTag("PlanetCompleted").GetComponent<PlanetCompleted>().IDPlanetSelect = idPlanet[index];
        PlayButton.transform.GetChild(0).GetComponent<TextMesh>().text = "Launch";
    }

    public void Cancel()
    {
        if (BoutonSelectBehavior.isSelected)
        {
            TogglePlanetsList[saveSelectedButton].isOn = false;
            PlayButton.transform.GetChild(0).GetComponent<TextMesh>().text = "Select";
            TogglePlanetsList[saveSelectedButton].GetComponent<BoutonSelectBehavior>().OnCancel(null);
            ClearOrbitCanvas();
        }
        else
        {
            ConstructionModuleMain.GetSingleton().GoBackToEditor();
        }
        saveSelectedButton = -1;
    }

    private static Dictionary<string, object> _parameters = new Dictionary<string, object>();

    public static object GetParameter(string key)
    {
        if (_parameters.ContainsKey(key))
        {
            return _parameters[key];
        }
        return null;
    }

    public static void SetParameter(string key, object value)
    {
        if (_parameters.ContainsKey(key))
        {
            _parameters[key] = value;
        }
        else
        {
            _parameters.Add(key, value);
        }
    }

    public static void ClearParameters()
    {
        _parameters.Clear();
    }

    public static void LoadLevel(string level)
    {
        LoadLevel(level);
    }
}
