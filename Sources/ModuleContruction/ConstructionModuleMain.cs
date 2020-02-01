using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class ConstructionModuleMain : MonoBehaviour
{
    public static ConstructionModuleMain Instance;

    public enum EditorCameraSTATE
    {
        FREE,
        PREVIEWMODE,
        ANCHORPOINTFREEZE,
        FIREREACTOR,
        SAVING,
        CountCAMERA_STATE
    }

    public enum SceneGlobalSTATE
    {
        EDITOR_MODE,
        LEVEL_SELECTION
    }

    GameObject EditorCamera;
    GameObject PreviewCamera;
    GameObject FreezeOnAnchorCamera;
    GameObject FireModeCamera;

    Dictionary<VesselsModulePartData, GameObject> ModulePartsDictionnaryData;

    GameObject TogglePartSelectionPrefab;
    GameObject TogglePartSelectionPrefabHUB;

    GameObject ToggleGroup;
    GameObject TogglesPrefab;

    public GameObject[] DrawedParts { get; private set; }
    public VesselsModulePartData[] DrawedPartsInfo { get; private set; }
    string[] DrawPartsInfoSaveID;
    GameObject DrawedPartsParent;


    GameObject CentralHubHUD;

    public Toggle[] ToggleList {get; private set; }
    public List<Toggle>[] ToggleModuleList { get; private set; } 
    public List<GameObject> ToggleHoriList { get; private set; }

    GameObject toggleHoriPrefab;
    GameObject scrollHubAnchPoints;
    ToggleGroup groupAnchHoriToggle;

    GameObject confirmSavePanel;

    GameObject ScrollToggleModuleParts;

    Text descriptionText;
    Text statText;

    Text descriptionHub;
    Text statHub;

    private string LoadUniqueID;
    private GameObject LoadPanel;
    public bool RejectUserInput { get; private set; }

    private bool SaveIsDirty;

    InputField nameInputFieldText;

    public bool isActivatedFromScript { get; private set; }

    public bool ShipFiredUp { get; private set; }
    public bool ShipFiredUpMax { get; private set;}
    public GameObject previewToggle { get; private set; }
    public GameObject fireUpMax { get; private set; }
    public GameObject fireOnOFF { get; private set; }

    public EditorCameraSTATE camState { get; private set; }

    public SceneGlobalSTATE gSTATE { get; private set; }

    private EventSystem eventSystem;

    private GameObject MainCanvas;

    private GameObject SelectLevelMain;
    private GameObject SelectLevelButton;

    private GameObject Remove_All_Button;
    private bool HubSelected;

    private Selectable leftArrowHUBSelector;
    private Selectable rightArrowHUBSelector;

    private Selectable upArrowLoadPanel;
    private Selectable downArrowLoadPanel;

    private Button playButton;
    private Button saveButton;
    private Button loadButton;
    private bool forceDeactivate;

    private GameObject surveilanceCameras;
    private GameObject welbetRobots;
    private RawImage RawImageBlack;

    public static ConstructionModuleMain GetSingleton()
    {
        return Instance;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if(this == Instance)
        {
            InitEachTime();
            return;
        }
        else
        {
            Debug.LogError("Try Create multiple ConstructionModuleMain ");
            GetSingleton().InitEachTime();
            return;
        }

        SaveIsDirty = false;

        RawImageBlack = GameObject.Find("RawImageFadeIn").GetComponent<RawImage>();


        leftArrowHUBSelector = GameObject.Find("HUD - FlecheL").GetComponent<Selectable>();
        rightArrowHUBSelector = GameObject.Find("HUD - FlecheR").GetComponent<Selectable>();

        upArrowLoadPanel = GameObject.Find("fleche_vers_haut").GetComponent<Selectable>();
        downArrowLoadPanel = GameObject.Find("fleche_vers_bas").GetComponent<Selectable>();

        LoadPanel = GameObject.Find("LoadList");

        eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();

        //Load Data and feed to infos
        TextAsset reader = Resources.Load<TextAsset>("Vessels_Parts");
        string fileData = reader.text;

        ModulePartsDictionnaryData = new Dictionary<VesselsModulePartData, GameObject>();
        
        var JSON_File_Root = JObject.Parse(fileData);
        string mainFolderLoad = JSON_File_Root["mainFolder"].Value<string>();

        ToggleModuleList = new List<Toggle>[(int)VesselsModulePartData.TypeOfModule.ModuleCount];

        if (mainFolderLoad.Contains("/"))
        {
            throw new System.Exception("mainFolderLoad can't contains /.");
        }

        ScrollToggleModuleParts = GameObject.Find("ScrollToggleModuleParts");

        JArray JSON_Parts = JSON_File_Root["parts"].Value<JArray>();

        for (int i = 0; i < JSON_Parts.Count; i++)
        {
            TextAsset readerPart = Resources.Load<TextAsset>(JSON_Parts[i]["path"].Value<string>());
            string filePartData = readerPart.text;

            var JSON_filePart_Root = JObject.Parse(filePartData);

            TextAsset reader_Stat = Resources.Load<TextAsset>(JSON_Parts[i]["path_Stat"].Value<string>());
            string fileStat = reader_Stat.text;
            var JSON_StatFile_Root = JObject.Parse(fileStat);

            ToggleModuleList[i] = new List<Toggle>();

            if (JSON_filePart_Root["folder"] != null)
            {
                string folder = JSON_filePart_Root["folder"].Value<string>();

                JArray datas = JSON_filePart_Root["datas"].Value<JArray>();

                for (int j = 0; j < datas.Count; j++)
                {
                    VesselsModulePartData newPartToLoad = new VesselsModulePartData();
                    string path = mainFolderLoad + folder + datas[j]["subFolders"] + datas[j]["prebabFileName"];

                    newPartToLoad.LoadFromJSON(datas[j].Value<JObject>(), (VesselsModulePartData.TypeOfModule)JSON_filePart_Root["moduleType"].Value<int>(), path);

                    //Part Statistics
                    if (JSON_StatFile_Root != null)
                    {
                        JToken Find = JSON_StatFile_Root["uniqueIDReferencial"].Value<JArray>().Where(x => x.Value<string>() == newPartToLoad.UniqueID).Select(x => x.Value<JToken>()).First();
                        int index = JSON_StatFile_Root["uniqueIDReferencial"].Value<JArray>().IndexOf(Find);
                        VesselsPartStat newStat = null;
                        if((VesselsModulePartData.TypeOfModule)JSON_Parts[i]["modulePartName"].Value<int>() == VesselsModulePartData.TypeOfModule.CentralHub)
                        {
                            newStat = new CentralHubStat(JSON_StatFile_Root["datas"][index].Value<JObject>(), newPartToLoad);
                        }
                        else if((VesselsModulePartData.TypeOfModule)JSON_Parts[i]["modulePartName"].Value<int>() == VesselsModulePartData.TypeOfModule.Reactor)
                        {
                            newStat = new ReactorStat(JSON_StatFile_Root["datas"][index].Value<JObject>(), newPartToLoad);
                        }
                        else if((VesselsModulePartData.TypeOfModule)JSON_Parts[i]["modulePartName"].Value<int>() == VesselsModulePartData.TypeOfModule.Generator)
                        {
                            newStat = new GeneratorStat(JSON_StatFile_Root["datas"][index].Value<JObject>(), newPartToLoad);
                        }
                        else if((VesselsModulePartData.TypeOfModule)JSON_Parts[i]["modulePartName"].Value<int>() == VesselsModulePartData.TypeOfModule.Weapon)
                        {
                            newStat = new WeaponStat(JSON_StatFile_Root["datas"][index].Value<JObject>(), newPartToLoad);
                        }
                        newPartToLoad.LinkPartStat(newStat);
                    }

                    GameObject GoToLoad = Resources.Load<GameObject>(path);
                    if (GoToLoad != null)
                    {
                        ModulePartsDictionnaryData.Add(newPartToLoad, GoToLoad);
                    }
                    else
                    {
                        throw new UnassignedReferenceException("Can't Find Reference at " + path);
                    }
                }
            }
            else
            {
                Debug.Log("Error can't find parameter in " + JSON_Parts[i]["modulePartName"].Value<string>() + " modulePart file");
            }
        }

        //UI Alteration from data

        TogglePartSelectionPrefab = Resources.Load<GameObject>("Vessels_Parts/UI/PrefabToggleModule");
        TogglePartSelectionPrefabHUB = Resources.Load<GameObject>("Vessels_Parts/UI/PrefabToggleModuleHUB");

        ToggleGroup = GameObject.Find("TogglesGroup");
        TogglesPrefab = Resources.Load<GameObject>("Vessels_Parts/UI/PrefabToggle");

        ToggleList = new Toggle[(int)VesselsModulePartData.TypeOfModule.ModuleCount];
        for (int i = 0; i < (int)VesselsModulePartData.TypeOfModule.ModuleCount; i++)
        {
            var arrayType = ModulePartsDictionnaryData.Where(x => x.Key.Type == (VesselsModulePartData.TypeOfModule)i).Select(x => x.Key).ToArray();

            int temp = i;


            GameObject newToggle = null;

            if (i != 0)
            {
                newToggle = Instantiate(TogglesPrefab);
                newToggle.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = ((VesselsModulePartData.TypeOfModule)JSON_Parts[i]["modulePartName"].Value<int>()).ToString();
                newToggle.GetComponent<Toggle>().group = ToggleGroup.GetComponent<ToggleGroup>();
                newToggle.GetComponent<Toggle>().onValueChanged.AddListener((value) =>
                {
                    ToogleUpdateVisualUpperToggle(newToggle.GetComponent<Toggle>(), temp);
                });
                newToggle.name = ((VesselsModulePartData.TypeOfModule)i).ToString();
                ToggleList[i] = newToggle.GetComponent<Toggle>();
                newToggle.GetComponent<Toggle>().targetGraphic = GameObject.Find("Pointer").GetComponent<Image>();
                newToggle.GetComponent<Toggle>().interactable = false;
                newToggle.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>(JSON_Parts[i]["path_UI_Toggle"].Value<string>());
                newToggle.transform.SetParent(ScrollToggleModuleParts.transform.GetChild(0).GetChild(0));
            }

            Transform Child;

            if (i == 0)
            {
                Child = GameObject.Find("ViewContentHUB").transform.GetChild(0);
            }
            else
            {
                Child = newToggle.transform.GetChild(1).GetChild(0);
            }

            GameObject newToggleModuleEmpty = Instantiate(i > 0 ? TogglePartSelectionPrefab : TogglePartSelectionPrefabHUB);

            if (i > 0)
            {
                Destroy(newToggleModuleEmpty.transform.GetChild(1).gameObject);
            }
            newToggleModuleEmpty.name = "Empty";
            newToggleModuleEmpty.transform.SetParent(Child);
            newToggleModuleEmpty.GetComponent<Toggle>().group = Child.GetComponent<ToggleGroup>();
            newToggleModuleEmpty.GetComponent<Image>().color = Color.clear;

            if (i == 0)
            {
                newToggleModuleEmpty.GetComponent<Toggle>().onValueChanged.AddListener((value) =>
                {
                    if (value)
                    {
                        newToggleModuleEmpty.GetComponent<Toggle>().Select();
                        ChangedSelectedModule("Empty");
                        UpdateDescriptionHub(null);
                    }
                });
            }
            else
            {
                newToggleModuleEmpty.transform.GetChild(0).GetComponent<Text>().text = "";
                newToggleModuleEmpty.GetComponent<Toggle>().onValueChanged.AddListener((value) =>
                {
                    if (value)
                    {
                        newToggleModuleEmpty.GetComponent<Toggle>().Select();
                        ChangedSelectedModule("Empty");
                        UpdateDescription();
                    }
                });
            }

            ToggleModuleList[i].Add(newToggleModuleEmpty.GetComponent<Toggle>());

            //Every Module load;
            foreach (var item in arrayType)
            {
                GameObject newToggleModule;
                if (i == 0)
                {
                    newToggleModule = Instantiate(TogglePartSelectionPrefabHUB);
                    newToggleModule.GetComponent<Image>().sprite = item.miniaturePreviewTexture;
                    newToggleModule.name = newToggleModule.GetComponent<Toggle>().name = item.UniqueID;
                    newToggleModule.transform.SetParent(Child);
                    newToggleModule.GetComponent<Toggle>().group = Child.GetComponent<ToggleGroup>();

                    newToggleModule.GetComponent<Toggle>().onValueChanged.AddListener((value) =>
                    {
                        if (value)
                        {
                            VesselsModulePartData tempRef = item;
                            ChangedSelectedModule(tempRef.UniqueID);
                            UpdateDescriptionHub(tempRef.PartStat);
                        }
                    });
                }
                else
                {
                    newToggleModule = Instantiate(TogglePartSelectionPrefab);
                    newToggleModule.transform.GetChild(0).GetComponent<Text>().text = newToggleModule.transform.GetChild(0).GetComponent<Text>().name = item.DisplayName;
                    newToggleModule.GetComponent<Image>().sprite = item.miniaturePreviewTexture;
                    newToggleModule.name = newToggleModule.GetComponent<Toggle>().name = item.UniqueID;
                    newToggleModule.transform.SetParent(Child);
                    newToggleModule.GetComponent<Toggle>().group = Child.GetComponent<ToggleGroup>();

                    newToggleModule.GetComponent<Toggle>().onValueChanged.AddListener((value) =>
                    {
                        if (value)
                        {
                            VesselsModulePartData tempRef = item;
                            ChangedSelectedModule(tempRef.UniqueID);
                            UpdateDescription(tempRef.PartStat);
                        }
                    });
                }
               
                ToggleModuleList[i].Add(newToggleModule.GetComponent<Toggle>());
            }

            for (int y = 0; y < ToggleModuleList[i].Count; y++)
            {
                Navigation tempNavigation = ToggleModuleList[i][y].navigation;
                if(y + 1 < ToggleModuleList[i].Count)
                {
                    tempNavigation.selectOnRight = ToggleModuleList[i][y + 1];
                }
                if(y > 0)
                {
                    tempNavigation.selectOnLeft = ToggleModuleList[i][y - 1];
                }
                ToggleModuleList[i][y].navigation = tempNavigation;
            }
        }

        ////////////////////////////////////////////////////\/\/\/\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
  
        descriptionText = GameObject.Find("Description").GetComponent<Text>();
        descriptionHub = GameObject.Find("DescriptionHUB").GetComponent<Text>();
        statText = GameObject.Find("Stat").GetComponent<Text>();
        statHub = GameObject.Find("StatHUB").GetComponent<Text>();
        descriptionText.text = statText.text =  "";

        scrollHubAnchPoints = GameObject.Find("HubAnchPointsUI");
        

        toggleHoriPrefab = Resources.Load<GameObject>("Vessels_Parts/UI/PrefabToggleHori");

        groupAnchHoriToggle = GameObject.Find("TogglesGroupAnchPoint").GetComponent<ToggleGroup>();

        nameInputFieldText = GameObject.Find("NameShipInputField").GetComponent<InputField>();

        confirmSavePanel = GameObject.Find("SaveRequest");
        

        MainCanvas = GameObject.Find("MainCanvas");

        EditorCamera = GameObject.Find("Editor_Camera");
        PreviewCamera = GameObject.Find("Preview_Camera");
        FreezeOnAnchorCamera = GameObject.Find("FreezeCameras");
        FireModeCamera = GameObject.Find("FireMode_Camera");

        Remove_All_Button = GameObject.Find("Remove_All");
       
        previewToggle = GameObject.Find("Preview");
        fireUpMax = GameObject.Find("Max Throttle");
        fireOnOFF = GameObject.Find("Fire On/OFF");

        CentralHubHUD = GameObject.Find("CentralHUB_HUD");

        previewToggle.GetComponent<Toggle>().onValueChanged.AddListener(x =>
        { ToggleModuleUpdate(previewToggle.GetComponent<Toggle>()); });

        fireUpMax.GetComponent<Toggle>().onValueChanged.AddListener(x =>
        { ToggleModuleUpdate(fireUpMax.GetComponent<Toggle>()); });

        fireOnOFF.GetComponent<Toggle>().onValueChanged.AddListener(x =>
        {
            ToggleModuleUpdate(fireOnOFF.GetComponent<Toggle>());
            if (!x)
            {
                fireUpMax.GetComponent<Toggle>().isOn = false;
            }
        });
    
        SelectLevelMain = GameObject.Find("SelectLevel");
        SelectLevelButton = GameObject.Find("SelectLevelButton");
        
        playButton = GameObject.Find("Play").GetComponent<Button>();
        saveButton = GameObject.Find("Save").GetComponent<Button>();
        loadButton = GameObject.Find("Load").GetComponent<Button>();

        surveilanceCameras = GameObject.Find("SecuritySystemCameras");
        welbetRobots = GameObject.Find("MeshRobotSoudeur");

        InitEachTime();
    }

    IEnumerator FadeIN()
    {
        float time = 0.0f;
        RawImageBlack.color = Color.black;
        while (RawImageBlack.color.a != 0)
        {
            time += Time.deltaTime;
            Color temp = RawImageBlack.color;
            temp.a = Mathf.Lerp(1, 0, time);
            RawImageBlack.color = temp;
            yield return new WaitForEndOfFrame();
        }
    }

    public void InitEachTime()
    {
        StartCoroutine(FadeIN());

        RejectUserInput = false;
        forceDeactivate = false;
        SceneController.GetSingleton().OverrideSceneID(SceneController.SceneID.Editor);

        DrawedParts = new GameObject[1];
        DrawedPartsInfo = new VesselsModulePartData[1];

        DrawedPartsParent = GameObject.Find("PartsParent");

        for (int i = 0; i < DrawedParts.Length; i++)
        {
            DrawedParts[i] = new GameObject();
            DrawedParts[i].name = ((VesselsModulePartData.TypeOfModule)i).ToString();
            DrawedParts[i].transform.SetParent(DrawedPartsParent.transform);
            DrawedPartsInfo[i] = new VesselsModulePartData();
        }

        for (int i = 0; i < ToggleModuleList.Length; i++)
        {
            ToggleModuleList[i][0].isOn = true;
        }

        scrollHubAnchPoints.SetActive(false);
        confirmSavePanel.SetActive(false);

        LoadUniqueID = "";

        previewToggle.SetActive(false);
        fireUpMax.SetActive(false);
        fireOnOFF.SetActive(false);

        isActivatedFromScript = false;

        descriptionText.transform.parent.gameObject.SetActive(false);

        ChangeCameraState(EditorCameraSTATE.FREE);

        gSTATE = SceneGlobalSTATE.EDITOR_MODE;

        Remove_All_Button.SetActive(false);

        ScrollToggleModuleParts.SetActive(false);

        SelectLevelMain.SetActive(false);

        playButton.interactable = false;
        saveButton.interactable = false;

        EventSystem.current = eventSystem;
        eventSystem.SetSelectedGameObject(null);
        eventSystem.SetSelectedGameObject(CentralHubHUD.transform.GetChild(0).gameObject);

        surveilanceCameras.SetActive(false);
        welbetRobots.SetActive(false);

    }

    private void HUBArrowBehavior(float value)
    {
        if(value < 0)
        {
            leftArrowHUBSelector.targetGraphic.color = leftArrowHUBSelector.colors.highlightedColor;
        }
        else if( value > 0)
        {
            rightArrowHUBSelector.targetGraphic.color = rightArrowHUBSelector.colors.highlightedColor;
        }
        else
        {
            leftArrowHUBSelector.targetGraphic.color = leftArrowHUBSelector.colors.normalColor;
            rightArrowHUBSelector.targetGraphic.color = rightArrowHUBSelector.colors.normalColor;
        }
    }

    private void LoadPanelBehavior(float value)
    {
        if (value > 0)
        {
            upArrowLoadPanel.targetGraphic.color = upArrowLoadPanel.colors.highlightedColor;
        }
        else if (value < 0)
        {
            downArrowLoadPanel.targetGraphic.color = downArrowLoadPanel.colors.highlightedColor;
        }
        else
        {
            upArrowLoadPanel.targetGraphic.color = upArrowLoadPanel.colors.normalColor;
            downArrowLoadPanel.targetGraphic.color = downArrowLoadPanel.colors.normalColor;
        }
    }

    private void Update()
    {
        if(LoadPanel.activeSelf)
        {
            LoadPanelBehavior(Input.GetAxis("Vertical"));
        }
        else if(CentralHubHUD.activeSelf)
        {
            HUBArrowBehavior(Input.GetAxis("Horizontal"));
        }


        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            MainCanvas.SetActive(!MainCanvas.activeSelf);
        }
    }

    public void SetUpScrollListHub(VesselsModulePartData myhub)
    {
        if (ToggleHoriList == null)
        {
            ToggleHoriList = new List<GameObject>();
        }
        else
        {
            for (int i = 0; i < ToggleHoriList.Count; i++)
            {
                Destroy(ToggleHoriList[i]);
            }
            ToggleHoriList.Clear();
        }

        Transform ViewContentTransform = scrollHubAnchPoints.transform;
        GameObject miniaturePreview = ViewContentTransform.GetChild(0).gameObject;
        miniaturePreview.GetComponent<RawImage>().texture = myhub.miniatureImageTexture;
        for (int i = 0; i < myhub.ConnectionPoints.Count; i++)
        {
            GameObject newToggle = Instantiate(toggleHoriPrefab);
            newToggle.name = "Toggle_Anchor_" + miniaturePreview.transform.childCount;
            newToggle.transform.SetParent(miniaturePreview.transform, false);
            newToggle.transform.localScale = Vector3.one;
            newToggle.transform.localPosition = myhub.ConnectionPoints[i].MiniatureLocalPosition;

            if(myhub.ConnectionPoints[i].MiniatureLocalPosition_2 != Vector3.zero)
            {
                GameObject tempOther = Instantiate(newToggle.transform.GetChild(0).gameObject);
                tempOther.transform.SetParent(newToggle.transform, false);
                tempOther.transform.localPosition = myhub.ConnectionPoints[i].MiniatureLocalPosition_2;
            }
            int holderI = i;

            newToggle.GetComponent<Toggle>().onValueChanged.AddListener((value) => {
                if (value)
                {
                    ToggleAnchorPointUIUpdate(myhub.ConnectionPoints[holderI].typeAllowed);
                    if((int)myhub.ConnectionPoints[holderI].typeAllowed.Length > 1)
                    {
                        List<VesselsModulePartData.TierPart> tierTemp = new List<VesselsModulePartData.TierPart>();
                        for (int t = 1; t < 4; t++)
                        {
                            int minAnchorValue = 4;
                            for (int a = 0; a < myhub.ConnectionPoints[holderI].points.Count; a++)
                            {
                                if (myhub.ConnectionPoints[holderI].points[a].RestrictedConnection.Contains((VesselsModulePartData.TypeOfModule)t))
                                {
                                    if (minAnchorValue > (int)myhub.ConnectionPoints[holderI].points[a].TypeAnchor)
                                    {
                                        minAnchorValue = (int)myhub.ConnectionPoints[holderI].points[a].TypeAnchor;
                                    }
                                }

                                if(myhub.ConnectionPoints[holderI].points[a].SpecificTier != null)
                                {
                                    tierTemp.Add((VesselsModulePartData.TierPart)myhub.ConnectionPoints[holderI].points[a].SpecificTier);
                                }
                            }

                            List<Toggle> refToggleList = ToggleModuleList[t];
                            for (int b = 0; b < refToggleList.Count; b++)
                            {
                                var KeyValue = ModulePartsDictionnaryData.Where(x => x.Key.UniqueID == refToggleList[b].name).Select(x => x.Key).FirstOrDefault();
                                if (KeyValue != null)
                                {
                                    VesselsModulePartData tempKeyValue = KeyValue;

                                    if(tierTemp.Count != 0)
                                    {
                                        if(tierTemp.Contains(tempKeyValue.Tier) || tempKeyValue.UniqueID == "Empty")
                                        {
                                            refToggleList[b].gameObject.SetActive(true);
                                        }
                                        else
                                        {
                                            refToggleList[b].gameObject.SetActive(false);
                                        }
                                    }
                                    else if ((
                                    (int)tempKeyValue.typeOfAnchor >= minAnchorValue &&
                                    (tempKeyValue.Tier == myhub.Tier || (int)tempKeyValue.Tier == (int)myhub.Tier - 1)) || tempKeyValue.UniqueID == "Empty")
                                    {
                                        refToggleList[b].gameObject.SetActive(true);
                                    }
                                    else
                                    {
                                        refToggleList[b].gameObject.SetActive(false);
                                    }
                                }
                            }

                            List<Toggle> tempThird = refToggleList.FindAll(x => x.gameObject.activeSelf == true);

                            for (int j = 0; j < tempThird.Count; j++)
                            {
                                Navigation tempNavigation = tempThird[j].navigation;
                                tempNavigation.selectOnLeft = tempNavigation.selectOnRight = null;

                                if (j + 1 < tempThird.Count)
                                {
                                    tempNavigation.selectOnRight = tempThird[j + 1];
                                }
                                if (j > 0)
                                {
                                    tempNavigation.selectOnLeft = tempThird[j - 1];
                                }
                                tempThird[j].navigation = tempNavigation;
                            }

                            refToggleList.ForEach(x => x.gameObject.SetActive(true));
                        }

                        tierTemp.Clear();
                    }
                    else
                    {
                        List<Toggle> refToggleList = ToggleModuleList[(int)myhub.ConnectionPoints[holderI].typeAllowed[0]];
                        for (int b = 0; b < refToggleList.Count; b++)
                        {
                            var KeyValue = ModulePartsDictionnaryData.Where(x => x.Key.UniqueID == refToggleList[b].name).Select(x => x.Key).FirstOrDefault();
                            if (KeyValue != null)
                            {
                                VesselsModulePartData tempKeyValue = KeyValue;

                                if (myhub.ConnectionPoints[holderI].points[0].SpecificTier != null)
                                {
                                    if ((VesselsModulePartData.TierPart)myhub.ConnectionPoints[holderI].points[0].SpecificTier == tempKeyValue.Tier ||
                                    tempKeyValue.UniqueID == "Empty")
                                    {
                                        refToggleList[b].gameObject.SetActive(true);
                                    }
                                    else
                                    {
                                        refToggleList[b].gameObject.SetActive(false);
                                    }
                                }
                                else if ((
                                tempKeyValue.typeOfAnchor >= myhub.ConnectionPoints[holderI].points[0].TypeAnchor &&
                                (tempKeyValue.Tier == myhub.Tier || (int)tempKeyValue.Tier == (int)myhub.Tier - 1)) || tempKeyValue.UniqueID == "Empty")
                                {
                                    refToggleList[b].gameObject.SetActive(true);
                                }
                                else
                                {
                                    refToggleList[b].gameObject.SetActive(false);
                                }
                            }
                        }

                        List<Toggle> tempThird = refToggleList.FindAll(x => x.gameObject.activeSelf == true);

                        for (int j = 0; j < tempThird.Count; j++)
                        {
                            Navigation tempNavigation = tempThird[j].navigation;
                            tempNavigation.selectOnLeft = tempNavigation.selectOnRight = null;

                            if (j + 1 < tempThird.Count)
                            {
                                tempNavigation.selectOnRight = tempThird[j + 1];
                            }
                            if (j > 0)
                            {
                                tempNavigation.selectOnLeft = tempThird[j - 1];
                            }
                            tempThird[j].navigation = tempNavigation;
                        }

                        refToggleList.ForEach(x => x.gameObject.SetActive(true));
                    }


                }
                ToggleUpdateVisualAnchorPoints(newToggle.GetComponent<Toggle>());
                ToolsUI.ToogleHorizontalUpdateUI(newToggle.GetComponent<Toggle>());
            });

            newToggle.GetComponent<Toggle>().group = groupAnchHoriToggle;
            ToggleHoriList.Add(newToggle);
        }

        for (int i = 0; i < ToggleHoriList.Count; i++)
        {
            Navigation tempNavi = ToggleHoriList[i].GetComponent<Toggle>().navigation;

            if(i == 0)
            {
                tempNavi.selectOnLeft = ToggleHoriList[ToggleHoriList.Count - 1].GetComponent<Toggle>();
            }
            else
            {
                tempNavi.selectOnLeft = ToggleHoriList[i - 1].GetComponent<Toggle>();
            }

            if(i == ToggleHoriList.Count - 1)
            {
                tempNavi.selectOnRight = ToggleHoriList[0].GetComponent<Toggle>();
            }
            else
            {
                tempNavi.selectOnRight = ToggleHoriList[i + 1].GetComponent<Toggle>();
            }

            ToggleHoriList[i].GetComponent<Toggle>().navigation = tempNavi;
        }

        GameObject hubOlder = DrawedParts[0];
        DrawedParts = new GameObject[myhub.ConnectionPoints.Count + 1];
        DrawedPartsInfo = new VesselsModulePartData[myhub.ConnectionPoints.Count + 1];
        DrawPartsInfoSaveID = new string[myhub.ConnectionPoints.Count];
        DrawedParts[0] = hubOlder;
        DrawedPartsInfo[0] = myhub;

        for (int i = 1; i < DrawedParts.Length; i++)
        {
            DrawedParts[i] = new GameObject();
            DrawedParts[i].name = "ConnectionPoints_" + (i  - 1);
            DrawedParts[i].transform.SetParent(DrawedPartsParent.transform, false);
            DrawedPartsInfo[i] = new VesselsModulePartData();
            DrawPartsInfoSaveID[i - 1] = "Empty";
        }

        previewToggle.SetActive(true);
        Canvas.ForceUpdateCanvases();
    }

    public void ToggleAnchorPointUIUpdate(VesselsModulePartData.TypeOfModule[] typeAllowed = null)
    {
        int value = -1;
        descriptionText.transform.parent.gameObject.SetActive(true);
        ScrollToggleModuleParts.SetActive(true);

        int toggleIndex = ToggleHoriList == null ? -1 : ToggleHoriList.Where(x => x.GetComponent<Toggle>().isOn == true).Select(x => ToggleHoriList.IndexOf(x) + 1).FirstOrDefault();


        for (int i = 1; i < ToggleList.Length; i++)
        {
            ToggleList[i].interactable = false;
            ToggleList[i].transform.GetChild(0).GetComponent<Image>().color = ToggleList[i].colors.disabledColor;
            if (typeAllowed != null)
            {
                for (int j = 0; j < typeAllowed.Length; j++)
                {
                    if((int)typeAllowed[j] == i)
                    {
                        ToggleList[i].interactable = true;
                        ToggleList[i].transform.GetChild(0).GetComponent<Image>().color = ToggleList[i].colors.normalColor;
                        if (value == -1)
                        {
                            if (DrawedPartsInfo[toggleIndex].UniqueID != "Undefined" && DrawedPartsInfo[toggleIndex].UniqueID != "Empty")
                            {
                                Toggle toggleSave = ToggleModuleList[i].Find(x => x.name == DrawedPartsInfo[toggleIndex].UniqueID);
                                if (toggleSave != null)
                                {
                                    value = i;
                                }
                            }
                            else
                            {
                                value = i;
                            }
                        } 
                    }
                }
            }
        }

        isActivatedFromScript = true;
        ToogleUpdateUI(value, true);
    }

    public bool ChangedSelectedModule(string uniqueID, int loadFromFile = -1)
    {
        int getGameObjectIndex = (ToggleHoriList == null || loadFromFile != -1) ? (loadFromFile != -1 ? loadFromFile : 0) : ToggleHoriList.Where(x => x.GetComponent<Toggle>().isOn == true).Select(x => ToggleHoriList.IndexOf(x)).FirstOrDefault() + 1;

        int indexPart = 0;
        if(DrawedPartsInfo[getGameObjectIndex].UniqueID == uniqueID)
        {
            return false;
        }
        else if(DrawedPartsInfo[getGameObjectIndex].UniqueID != "Undefined")
        {
            int childCount = DrawedParts[getGameObjectIndex].transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                DestroyImmediate(DrawedParts[getGameObjectIndex].transform.GetChild(0).gameObject);
            }
        }

        if(uniqueID == "Empty")
        {
            DrawedPartsInfo[getGameObjectIndex] = new VesselsModulePartData();

            fireOnOFF.SetActive(false);

            for (int i = 1; i < DrawedPartsInfo.Length; i++)
            {
                if (DrawedPartsInfo[i].Type == VesselsModulePartData.TypeOfModule.Reactor)
                {
                    fireOnOFF.SetActive(true);
                    i = DrawedPartsInfo.Length;
                }
            }

            ValidateSaveButtonIsOn();

            return false;
        }
        var InfoParts = ModulePartsDictionnaryData.Where(x => x.Key.UniqueID == uniqueID).Select(x => x.Key);

        VesselsModulePartData holderPartData = InfoParts.First();
        Vector3 position, rotation;
        position = rotation = Vector3.zero;
        AnchorPoint[] definedAnchor = null;


        if(HubSelected)
        {
            string nameFixed = DrawedParts[(int)VesselsModulePartData.TypeOfModule.CentralHub].transform.GetChild(0).name.Substring(0, DrawedParts[(int)VesselsModulePartData.TypeOfModule.CentralHub].transform.GetChild(0).name.Length - 2);

            var holderVar = ModulePartsDictionnaryData.Where(x => x.Key.UniqueID == nameFixed).Select(x => x.Key);

            VesselsModulePartData InfoCentralHub = holderVar.First();


            definedAnchor = InfoCentralHub.ConnectionPoints[getGameObjectIndex - 1].points.Where(x => x.SpecificTier != null && x.SpecificTier == holderPartData.Tier).Select(x => x).ToArray();

            //TryAnotherSearch
            if (definedAnchor == null || definedAnchor.Length == 0)
            {
                definedAnchor = InfoCentralHub.ConnectionPoints[getGameObjectIndex - 1].points.Where(x => InfoCentralHub.ConnectionPoints[getGameObjectIndex - 1].typeAllowed.Contains(holderPartData.Type) && x.TypeAnchor == holderPartData.typeOfAnchor && indexPart <= (int)x.TypeAnchor && (x.RestrictedConnection == null ? true : x.RestrictedConnection[0] == holderPartData.Type)).Select(x => x).ToArray();
            }

            if (definedAnchor != null && definedAnchor.Length != 0)
            {
                    rotation = definedAnchor[0].AnchorPointsRotation[indexPart];
                    position = definedAnchor[0].AnchorPoints[indexPart];
            }
            else
            {
                definedAnchor = InfoCentralHub.ConnectionPoints[getGameObjectIndex - 1].points.Where(x => InfoCentralHub.ConnectionPoints[getGameObjectIndex - 1].typeAllowed.Contains(holderPartData.Type) && x.TypeAnchor <= holderPartData.typeOfAnchor && indexPart <= (int)x.TypeAnchor && (x.RestrictedConnection == null ? true : x.RestrictedConnection[0] == holderPartData.Type)).Select(x => x).ToArray();
                if (definedAnchor != null && definedAnchor.Length != 0)
                {
                    rotation = definedAnchor[0].AnchorPointsRotation[indexPart];
                    position = definedAnchor[0].AnchorPoints[indexPart];
                }
                else
                {
                    return false;
                }
            }
        }

        GameObject tempGO = Instantiate(ModulePartsDictionnaryData[InfoParts.First()]);
        DrawedPartsInfo[getGameObjectIndex] = holderPartData;

        tempGO.transform.SetParent(DrawedParts[getGameObjectIndex].transform, false);

        tempGO.transform.localPosition = position;

        if (rotation != Vector3.zero)
        {
            DrawedParts[getGameObjectIndex].transform.GetChild(indexPart).rotation = Quaternion.Euler(rotation.x, rotation.y, rotation.z);
        }

        tempGO.name = DrawedPartsInfo[getGameObjectIndex].UniqueID + "_" + indexPart;

        if (holderPartData.Type != VesselsModulePartData.TypeOfModule.CentralHub)
        {
            for (int i = 0; i < (int)definedAnchor[0].TypeAnchor; i++)
            {
                rotation = definedAnchor[0].AnchorPointsRotation[i + 1];
                position = definedAnchor[0].AnchorPoints[i + 1];

                GameObject tempGONext = Instantiate(ModulePartsDictionnaryData[InfoParts.First()]);

                tempGONext.transform.SetParent(DrawedParts[getGameObjectIndex].transform, false);

                tempGONext.transform.localPosition = position;

                if (rotation != Vector3.zero)
                {
                    DrawedParts[getGameObjectIndex].transform.GetChild(i+1).rotation = Quaternion.Euler(rotation.x, rotation.y, rotation.z);
                }

                tempGONext.name = DrawedPartsInfo[getGameObjectIndex].UniqueID + "_" + (i + 1);

            }
        }
        else
        {
            if (holderPartData.Tier == VesselsModulePartData.TierPart.Tier_1)
            {
                DrawedPartsParent.transform.position = Vector3.zero;
            }
            else if (holderPartData.Tier == VesselsModulePartData.TierPart.Tier_2)
            {
                DrawedPartsParent.transform.position = new Vector3(0, 1, 0);
            }
        }

        fireOnOFF.SetActive(false);

        //Allow Fire On if there are reactor attached
        for (int i = 1; i < DrawedPartsInfo.Length; i++)
        {
            if(DrawedPartsInfo[i].Type == VesselsModulePartData.TypeOfModule.Reactor)
            {
                fireOnOFF.SetActive(true);
                i = DrawedPartsInfo.Length;
            }
        }

        if ((loadFromFile != -1 && !HubSelected) || (loadFromFile == 0 && !HubSelected))
        {
            SelectHub();
        }

        ValidateSaveButtonIsOn();

        return true;
    }

    public void SelectHub()
    {
        if(DrawedPartsInfo[0].UniqueID == "Empty" || DrawedPartsInfo[0].UniqueID == "Undefined")
        {
            return;
        }

        SetUpScrollListHub(DrawedPartsInfo[0]);
        scrollHubAnchPoints.SetActive(true);

        eventSystem.SetSelectedGameObject(null);
        eventSystem.SetSelectedGameObject(scrollHubAnchPoints);

        Remove_All_Button.SetActive(true);
        HubSelected = true;

        CentralHubHUD.SetActive(false);

        descriptionText.transform.parent.gameObject.SetActive(false);
        ToggleGroup.SetActive(false);
    }

    public void ToggleModuleUpdate(Toggle toggleToUpdate)
    {
        if (toggleToUpdate.isOn)
        {
            toggleToUpdate.GetComponent<Image>().color = Color.grey;
        }
        else
        {
            toggleToUpdate.GetComponent<Image>().color = Color.white;
        }
    }

    public void ToogleUpdateVisualUpperToggle(Toggle toggleCheck, int id)
    {
        if(toggleCheck.isOn)
        {
            if (id == 1)
            {
                toggleCheck.targetGraphic.gameObject.GetComponent<Image>().sprite = toggleCheck.spriteState.disabledSprite;
            }
            else if (id == 2)
            {
                toggleCheck.targetGraphic.gameObject.GetComponent<Image>().sprite = toggleCheck.spriteState.highlightedSprite;
            }
            else if(id == 3)
            {
                toggleCheck.targetGraphic.gameObject.GetComponent<Image>().sprite = toggleCheck.spriteState.pressedSprite;
            }
            
        }
    }

    private Transform FreezeCameraTransitions()
    {
        if(FreezeOnAnchorCamera.transform.GetChild(0).gameObject.activeSelf)
        {
            FreezeOnAnchorCamera.transform.GetChild(0).gameObject.SetActive(false);
            FreezeOnAnchorCamera.transform.GetChild(1).gameObject.SetActive(true);
            return FreezeOnAnchorCamera.transform.GetChild(1);
        }
        else
        {
            FreezeOnAnchorCamera.transform.GetChild(0).gameObject.SetActive(true);
            FreezeOnAnchorCamera.transform.GetChild(1).gameObject.SetActive(false);
            return FreezeOnAnchorCamera.transform.GetChild(0);
        }
    }

    public void ToggleUpdateVisualAnchorPoints(Toggle toggle)
    {
        int toggleIndex = ToggleHoriList == null ? -1 : ToggleHoriList.Where(x => x.GetComponent<Toggle>().isOn == true).Select(x => ToggleHoriList.IndexOf(x) + 1).FirstOrDefault();

        if (toggleIndex == 0)
        {
            if (ShipFiredUp)
            {
                ChangeCameraState(EditorCameraSTATE.FIREREACTOR);
            }
            else
            {
                ChangeCameraState(EditorCameraSTATE.FREE);
            }
            ToggleAnchorPointUIUpdate();
            descriptionText.transform.parent.gameObject.SetActive(false);
            ScrollToggleModuleParts.SetActive(false);
        }
        else if (toggle.isOn)
        {
            //Tricks to see if there is result or not since no result means 0 wich is also the first index of the list
            toggleIndex--;

            Transform CameraTobeAssigned = FreezeCameraTransitions();
            CameraTobeAssigned.position = DrawedPartsInfo[0].ConnectionPoints[toggleIndex].CameraForAnchorViewPosition;
            CameraTobeAssigned.rotation = DrawedPartsInfo[0].ConnectionPoints[toggleIndex].CameraForAnchorViewQuaternion;
            ChangeCameraState(EditorCameraSTATE.ANCHORPOINTFREEZE);

            if (DrawedPartsInfo[toggleIndex + 1].UniqueID != "Undefined" && DrawedPartsInfo[toggleIndex + 1].UniqueID != "Empty")
            {
                int valueHolder = (int)DrawedPartsInfo[toggleIndex + 1].Type;
                ScrollToggleModuleParts.transform.GetChild(0).GetChild(0).GetChild(valueHolder - 1).GetComponent<Toggle>().isOn = true;
                Toggle save = ToggleModuleList[valueHolder].Where(x => x.name == DrawedPartsInfo[toggleIndex + 1].UniqueID).Select(x => x).First();
                save.isOn = true;
                eventSystem.SetSelectedGameObject(null);
                eventSystem.SetSelectedGameObject(save.gameObject);
            }
        }
    }

    public void UpdateCameras()
    {
        if (camState == EditorCameraSTATE.FREE)
        {
            previewToggle.GetComponent<Toggle>().isOn = false;
            EditorCamera.SetActive(true);
            PreviewCamera.SetActive(false);
            FreezeOnAnchorCamera.SetActive(false);
            FireModeCamera.SetActive(false);
        }
        else if(camState == EditorCameraSTATE.PREVIEWMODE)
        {
            EditorCamera.SetActive(false);
            PreviewCamera.SetActive(true);
            FreezeOnAnchorCamera.SetActive(false);
            FireModeCamera.SetActive(false);
        }
        else if(camState == EditorCameraSTATE.ANCHORPOINTFREEZE)
        {
            previewToggle.GetComponent<Toggle>().isOn = false;
            EditorCamera.SetActive(false);
            PreviewCamera.SetActive(false);
            FreezeOnAnchorCamera.SetActive(true);
            FireModeCamera.SetActive(false);
        }
        else if(camState == EditorCameraSTATE.FIREREACTOR)
        {
            FireModeCamera.transform.position = (Vector3)DrawedPartsInfo[0].fireCamPosition;
            FireModeCamera.transform.rotation = (Quaternion)DrawedPartsInfo[0].fireCamRotation;
            previewToggle.GetComponent<Toggle>().isOn = false;
            EditorCamera.SetActive(false);
            PreviewCamera.SetActive(false);
            FreezeOnAnchorCamera.SetActive(false);
            FireModeCamera.SetActive(true);
        }
    }

    public void ChangeCameraState(int state)
    {
        if(state >= 0 && state < (int)EditorCameraSTATE.CountCAMERA_STATE)
        {
            camState = (EditorCameraSTATE)state;
            UpdateCameras();
        }
    }

    public void ChangeCameraState(EditorCameraSTATE state)
    {
        camState = state;
        UpdateCameras();
    }

    public void ToogleUpdateUI(int id, bool affectsBoolean = false)
    {
        if(id != -1)
        {
            if(id != 0)
            {
                ToggleList[id].isOn = true;
                int toggleIndex = ToggleHoriList == null ? -1 : ToggleHoriList.Where(x => x.GetComponent<Toggle>().isOn == true).Select(x => ToggleHoriList.IndexOf(x) + 1).FirstOrDefault();

                if(!isActivatedFromScript && !affectsBoolean)
                {
                    if (DrawedPartsInfo[toggleIndex].Type != ((VesselsModulePartData.TypeOfModule)id))
                    {
                        for (int i = 0; i < ToggleModuleList.Length; i++)
                        {
                            ToggleModuleList[i].ForEach(x => x.isOn = x.name == "Empty" ? true : false);
                        }

                        eventSystem.SetSelectedGameObject(null);
                        eventSystem.SetSelectedGameObject(ToggleModuleList[id][0].gameObject);

                        ChangedSelectedModule("Empty");
                    }
                }
                else
                {
                    isActivatedFromScript = false;
                    eventSystem.SetSelectedGameObject(null);
      
                    if (DrawedPartsInfo[toggleIndex].UniqueID != "Undefined" && DrawedPartsInfo[toggleIndex].UniqueID != "Empty")
                    {
                        Toggle toggleSave = ToggleModuleList[id].Find(x => x.name == DrawedPartsInfo[toggleIndex].UniqueID);

                        for (int i = 1; i < ToggleModuleList.Length; i++)
                        {
                            if(i != id)
                            {
                                ToggleModuleList[i][0].isOn = true;
                            }
                        }

                        if(toggleSave != null)
                        {
                            toggleSave.isOn = true;
                            eventSystem.SetSelectedGameObject(toggleSave.gameObject);
                        }
                    }
                    else
                    {
                        for (int i = 1; i < ToggleModuleList.Length; i++)
                        {
                            ToggleModuleList[i].ForEach(x => x.isOn = x.name == "Empty" ? true : false);
                        }

                        eventSystem.SetSelectedGameObject(ToggleModuleList[id][0].gameObject);
                    }
                }
            }
        }
    }

    public void UpdateDescriptionHub(VesselsPartStat stat = null)
    {
        if (stat == null)
        {
            descriptionHub.text = "";
            statHub.text = "";
        }
        else
        {
            statHub.text = stat.ToString();
            descriptionHub.text = stat.ModulePartData.DescriptionParts;
        }
    }

    public void UpdateDescription(VesselsPartStat stat = null)
    {
        if(stat == null)
        {
            descriptionText.text = "";
            statText.text = "";
        }
        else
        {
            statText.text = stat.ToString();
            descriptionText.text = stat.ModulePartData.DescriptionParts;
        }
    }

    /// <summary> Version 0.1
    /// Play The scene Game
    /// </summary>
    
    public void PlayGame()
    {
        if(RejectUserInput)
        {
            return;
        }

        if (GameManager.nameVessel != "Undefined")
        {
            if(ShipFiredUp)
            {
                StartCoroutine(StartDelayedFireMode(0.0f));
                fireOnOFF.GetComponent<Toggle>().isOn = false;
                fireUpMax.GetComponent<Toggle>().isOn = false;
            }
            gSTATE = SceneGlobalSTATE.LEVEL_SELECTION;
            SwitchToLevelSelection();
        }
    }

    public void ValidateSaveButtonIsOn()
    {
        if(fireOnOFF.activeSelf && nameInputFieldText.text.Length > 3)
        {
            saveButton.interactable = true;
        }
        else
        {
            saveButton.interactable = false;
            playButton.interactable = false;
        }
    }

    public void SwitchToLevelSelection()
    {
        surveilanceCameras.SetActive(true);
        welbetRobots.SetActive(true);
        SelectLevelMain.SetActive(true);
        eventSystem.SetSelectedGameObject(null);
        eventSystem.SetSelectedGameObject(SelectLevelButton);
        MainCanvas.SetActive(false);
    }

    public void GoBackToEditor()
    {
        SelectLevelMain.SetActive(false);
        surveilanceCameras.SetActive(false);
        welbetRobots.SetActive(false);
        MainCanvas.SetActive(true);
        eventSystem.SetSelectedGameObject(null);
        eventSystem.SetSelectedGameObject(GameObject.Find("Play"));
        gSTATE = SceneGlobalSTATE.EDITOR_MODE;
    }

    public void ResetCleanVessel()
    {
        if (DrawedPartsInfo[0].UniqueID != "Undefined")
        {
            for (int i = 0; i < DrawedPartsInfo.Length; i++)
            {
                DrawedPartsInfo[i] = new VesselsModulePartData();
                int childCountHolder = DrawedParts[i].transform.childCount;
                for (int j = 0; j < childCountHolder; j++)
                {
                    DestroyImmediate(DrawedParts[i].transform.GetChild(0).gameObject);
                }
                if(i > 0)
                {
                    DestroyImmediate(DrawedParts[i]);
                }
            }

            DrawPartsInfoSaveID = null;
            DrawedPartsInfo = new VesselsModulePartData[1];
            DrawedPartsInfo[0] = new VesselsModulePartData();
        }

        if (ToggleHoriList != null)
        {
            for (int i = 0; i < ToggleHoriList.Count; i++)
            {
                Destroy(ToggleHoriList[i]);
            }

            ToggleHoriList.Clear();
            ToggleHoriList = null;
        }

        LoadUniqueID = nameInputFieldText.text = "";
        GameManager.nameVessel = "Undefined";

        scrollHubAnchPoints.SetActive(false);
        Remove_All_Button.SetActive(false);
        HubSelected = false;

        ScrollToggleModuleParts.SetActive(false);
        previewToggle.SetActive(false);
        fireUpMax.SetActive(false);
        fireOnOFF.SetActive(false);
        CentralHubHUD.SetActive(true);

   
        for (int i = 0; i < ToggleModuleList.Length; i++)
        {
            ToggleModuleList[i][0].isOn = true;
            if(i > 0)
            {
                ToggleModuleList[i][0].gameObject.GetComponent<ToggleModule>().ResetPosition();
            }
        }

        previewToggle.GetComponent<Toggle>().isOn = false;
        fireOnOFF.GetComponent<Toggle>().isOn = false;
        fireUpMax.GetComponent<Toggle>().isOn = false;
        ShipFiredUpMax = ShipFiredUp = false;

        playButton.interactable = false;
        saveButton.interactable = false;
        ChangeCameraState(EditorCameraSTATE.FREE);

        eventSystem.SetSelectedGameObject(null);
        eventSystem.SetSelectedGameObject(CentralHubHUD.transform.GetChild(0).gameObject);
    }
    
    IEnumerator FadeInShipMat(Material[] arrayOfMats)
    {
        RejectUserInput = true;
        for (int i = 0; i < arrayOfMats.Length; i++)
        {
            arrayOfMats[i].SetFloat("_DissolveAmount", 1.0f);
            arrayOfMats[i].SetFloat("_DissolveSwapAmount", 1.0f);
        }
        yield return new WaitForSeconds(0.2f);

        float value = 1.0f;
        float maxTime = 1.0f;
        float time = 0.0f;
        while (value != 0.0f)
        {
            value = Mathf.Lerp(1.0f, 0.0f, time / maxTime);
            for (int i = 0; i < arrayOfMats.Length; i++)
            {
                arrayOfMats[i].SetFloat("_DissolveAmount", value);
            }
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        for (int i = 0; i < arrayOfMats.Length; i++)
        {
            arrayOfMats[i].SetFloat("_DissolveAmount", 1.0f);
            arrayOfMats[i].SetFloat("_DissolveSwapAmount", 0.0f);
        }

        yield return new WaitForSeconds(.5f);

        value = 1.0f;
        maxTime = 1.5f;
        time = 0.0f;
        while (value != 0.0f)
        {
            value = Mathf.Lerp(1.0f, 0.0f, time / maxTime);
            for (int i = 0; i < arrayOfMats.Length; i++)
            {
                arrayOfMats[i].SetFloat("_DissolveAmount", value);
            }
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        RejectUserInput = false;


        Navigation temp = loadButton.navigation;
        temp.mode = Navigation.Mode.Explicit;
        loadButton.navigation = temp;
    }

    public void LoadVesselFromFileToEditor(string name)
    {
        GameManager.nameVessel = name;
        LoadUniqueID = name;
        StreamReader reader = new StreamReader(Application.persistentDataPath + "/Save/Vessels_Save/" + name + ".json");

        if (reader == null)
        {
            throw new FileLoadException("Couldn't Load file");
        }

        string fileData = reader.ReadToEnd();
        reader.Close();

        JObject JSON_File_Root = JObject.Parse(fileData);

        nameInputFieldText.text = JSON_File_Root["name"].Value<string>();

        JArray datas = JSON_File_Root["parts"].Value<JArray>();

        for (int j = 0; j < datas.Count; j++)
        {
            JArray subDatas = datas[j]["subParts"].Value<JArray>();
            for (int i = 0; i < subDatas.Count; i++)
            {
                if (subDatas[i]["name"] != null)
                {
                    ChangedSelectedModule(subDatas[i]["name"].Value<string>(), j);
                }
            }
        }

        List<Material> arrayOfMats;

        arrayOfMats = new List<Material>();
        for (int j = 0; j < datas.Count; j++)
        {
            for (int k = 0; k < DrawedParts[j].transform.childCount; k++)
            {
                MeshRenderer[] tempMeshRender = DrawedParts[j].transform.GetChild(k).GetComponentsInChildren<MeshRenderer>();
                for (int u = 0; u < tempMeshRender.Length; u++)
                {
                    for (int l = 0; l < tempMeshRender[u].materials.Length; l++)
                    {
                        arrayOfMats.Add(tempMeshRender[u].materials[l]);
                    }
                }
            }
        }

        Navigation temp = loadButton.navigation;
        temp.mode = Navigation.Mode.None;
        loadButton.navigation = temp;

        StartCoroutine(FadeInShipMat(arrayOfMats.ToArray()));
        arrayOfMats.Clear();
        ValidateSaveButtonIsOn();
        playButton.interactable = saveButton.interactable;
    }

    public void CleanOneSaveFromFiles(string saveName)
    {
        try
        {
            File.Delete(Application.persistentDataPath + "/Save/Vessels_Save/" + saveName +".json");
            File.Delete(Application.persistentDataPath + "/Save_Media/" + saveName + ".spp");
            File.Delete(Application.persistentDataPath + "/Save_Media/" + saveName + ".mp4");
        }
        catch(Exception e)
        {

        }
    }

    public void SaveVesselToFile(bool EraseOldSave = true)
    {
        if (RejectUserInput)
        {
            return;
        }

        if (DrawedPartsInfo[0].UniqueID == "Undefined")
        {
            return;
        }

        if (ShipFiredUp)
        {
            forceDeactivate = true;
            fireOnOFF.GetComponent<Toggle>().isOn = false;
            float timer = 0.0f;
            while(timer < 1.5f)
            {
                timer += Time.deltaTime;
            }
        }

        //CleanUP Previous File
        if (LoadUniqueID != nameInputFieldText.text && LoadUniqueID != "")
        {
            if(!confirmSavePanel.activeSelf)
            {
                confirmSavePanel.SetActive(true);
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(confirmSavePanel.transform.GetChild(3).gameObject);
                return;
            }
            else if (EraseOldSave)
            {
                CleanOneSaveFromFiles(LoadUniqueID);
                LoadShipManagement.GetSingleton().CleanDictionnary(LoadUniqueID);
            }
            confirmSavePanel.SetActive(false);
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(GameObject.Find("Save"));
        }

        string JsonString = "";
        JsonString += "{\n";
        JsonString += "\t\"name\": \""+ nameInputFieldText.text + "\",\n";
        JsonString += "\t\"parts\": [\n";
        bool needBracket = true;
        for (int i = 0; i < DrawedPartsParent.transform.childCount; i++)
        {
            needBracket = true;
            if (i == 0)
            {
                foreach (var item in DrawedPartsParent.transform.GetChild(0).GetChild(0).GetChild(0).GetComponentsInChildren<Transform>())
                {
                    if(item.tag == "TurretT2")
                    {
                        JsonString += "\t\t{\n\t\t\t\"specificWeaponName\": \"" + item.gameObject.name + "\",\n";
                        needBracket = false;
                        break;
                    }
                }
            }

            if(needBracket)
            {
                JsonString += "\t\t{\n\t\t\t\"name\": \"" + DrawedPartsParent.transform.GetChild(i).name + "\",\n";
            }
            else
            {
                JsonString += "\t\t\t\"name\": \"" + DrawedPartsParent.transform.GetChild(i).name + "\",\n";
            }
            JsonString += "\t\t\t\"subParts\": [\n";
            for (int j = 0; j < DrawedPartsParent.transform.GetChild(i).childCount; j++)
            {
                string nameFixed = DrawedPartsInfo[i].UniqueID;
                Vector3 transPosition = DrawedPartsParent.transform.GetChild(i).GetChild(j).transform.position;
                Quaternion transRotation = DrawedPartsParent.transform.GetChild(i).GetChild(j).transform.rotation;
                JsonString += "\t\t\t\t{\n\t\t\t\t\t\"name\": \"" + nameFixed + "\",\n";
                JsonString += "\t\t\t\t\t\"PrefabPath\": \"" + DrawedPartsInfo[i].PrefabPath + "\",\n";
                JsonString += "\t\t\t\t\t\"moduleType\": \"" + DrawedPartsInfo[i].Type + "\",\n";
                JsonString += "\t\t\t\t\t\"offset_X\": " + transPosition.x + ",\n\t\t\t\t\t\"offset_Y\": " + transPosition.y + ",\n\t\t\t\t\t\"offset_Z\": " + transPosition.z +
                    ",\n\t\t\t\t\t\"rotation_X\": " + transRotation.x + ",\n\t\t\t\t\t\"rotation_Y\": " + transRotation.y + ",\n\t\t\t\t\t\"rotation_Z\": " + transRotation.z +
                    ",\n\t\t\t\t\t\"rotation_W\": " + transRotation.w + "\n";
                JsonString += "\t\t\t\t}";
                if(j < DrawedPartsParent.transform.GetChild(i).childCount - 1)
                {
                    JsonString += ",";
                }
                JsonString += "\n";
            }
            JsonString += "\t\t\t]\n";
            JsonString += "\t\t}";
            if(i < DrawedPartsParent.transform.childCount - 1)
            {
                JsonString += ",";
            }
            JsonString += "\n";
        }
        JsonString += "\t]\n";
        JsonString += "}";

        Directory.CreateDirectory(Application.persistentDataPath + "/Save/Vessels_Save/");
        StreamWriter writer = new StreamWriter(Application.persistentDataPath +"/Save/Vessels_Save/" + nameInputFieldText.text + ".json");
        writer.Write(JsonString);
        writer.Close();
        GameManager.nameVessel = nameInputFieldText.text;


        FreezeUIWhileSaving(true);
        ScreenShotCamera.GetSingleton().ScreenShotCaptureForSave(GameManager.nameVessel);
        LoadShipManagement.GetSingleton().UpdateLoadPanel();


        Navigation temp =  saveButton.navigation;
        temp.mode = Navigation.Mode.None;
        saveButton.navigation = temp;

        if (LoadUniqueID == nameInputFieldText.text)
        {
            LoadShipManagement.GetSingleton().UpdatePreviewVideo(nameInputFieldText.text);
            LoadShipManagement.GetSingleton().UpdatePreviewImage(nameInputFieldText.text);
            LoadShipManagement.GetSingleton().UpdateTextToggle(nameInputFieldText.text);
        }
        else
        {
            LoadUniqueID = nameInputFieldText.text;
        }

        playButton.interactable = true;
        SaveIsDirty = false;
    }

    public void BackOutModuleWithCancel()
    {
        int toggleIndex = ToggleHoriList == null ? -1 : ToggleHoriList.Where(x => x.GetComponent<Toggle>().isOn == true).Select(x => ToggleHoriList.IndexOf(x)).First();

        if(DrawPartsInfoSaveID[toggleIndex] != DrawedPartsInfo[toggleIndex + 1].UniqueID)
        {
            ChangedSelectedModule(DrawPartsInfoSaveID[toggleIndex]);
        }
    }

    public void SaveUpModuleSelected()
    {

        int toggleIndex = ToggleHoriList == null ? -1 : ToggleHoriList.Where(x => x.GetComponent<Toggle>().isOn == true).Select(x => ToggleHoriList.IndexOf(x)).First();

        if (DrawPartsInfoSaveID[toggleIndex] == DrawedPartsInfo[toggleIndex + 1].UniqueID && !SaveIsDirty)
        {
            SaveIsDirty = false;
            playButton.interactable = true;
        }
        else
        {
            DrawPartsInfoSaveID[toggleIndex] = DrawedPartsInfo[toggleIndex + 1].UniqueID;
            SaveIsDirty = true;
            playButton.interactable = false;
        }
    }

    public void FreezeUIWhileSaving(bool Freeze)
    {
        if(Freeze)
        {
            camState = EditorCameraSTATE.SAVING;
        }
        else
        {
            camState = EditorCameraSTATE.FREE;
            Navigation temp = saveButton.navigation;
            temp.mode = Navigation.Mode.Explicit;
            saveButton.navigation = temp;
        }
    }

    public void ActivatePreviewCamera()
    {
        EditorCamera.SetActive(!EditorCamera.activeSelf);
        if (EditorCamera.activeSelf)
        {
            ChangeCameraState(EditorCameraSTATE.FREE);
        }
        else
        {
            ChangeCameraState(EditorCameraSTATE.PREVIEWMODE);
        }
    }

    public void ClearSelectedConnectionPoint()
    {
        int getGameObjectIndex = ToggleHoriList == null ? -1 : ToggleHoriList.Where(x => x.GetComponent<Toggle>().isOn == true).Select(x => ToggleHoriList.IndexOf(x)).FirstOrDefault() + 1;

        if (getGameObjectIndex == -1)
        {
            return;
        }

        int childHolder = DrawedParts[getGameObjectIndex].transform.childCount;
        for (int i = 0; i < childHolder; i++)
        {
            DestroyImmediate(DrawedParts[getGameObjectIndex].transform.GetChild(0).gameObject);
        }
    }

    IEnumerator StartDelayedFireMode(float valueToWait)
    {
        yield return new WaitForSeconds(valueToWait);
        ShipFiredUp = !ShipFiredUp;
        fireUpMax.SetActive(ShipFiredUp);
        if (ShipFiredUp == false)
        {
            ShipFiredUpMax = false;
        }
    }

    public void FiredUpVessel()
    {
        if (RejectUserInput)
        {
            return;
        }

        else if(forceDeactivate)
        {
            StartCoroutine(StartDelayedFireMode(0.0f));
            ChangeCameraState(EditorCameraSTATE.FREE);
            previewToggle.GetComponent<Toggle>().interactable = true;
            forceDeactivate = false;
            return;
        }

        FireModeCamera.SetActive(!FireModeCamera.activeSelf);
        if(FireModeCamera.activeSelf)
        {
            previewToggle.GetComponent<Toggle>().interactable = false;
            ChangeCameraState(EditorCameraSTATE.FIREREACTOR);
            StartCoroutine(StartDelayedFireMode(2.2f));
        }
        else
        {
            previewToggle.GetComponent<Toggle>().interactable = true;
            ChangeCameraState(EditorCameraSTATE.FREE);
            StartCoroutine(StartDelayedFireMode(0.0f));
        }  
    }

    public void MaxThrottle()
    {
        if (RejectUserInput)
        {
            return;
        }

        if (ShipFiredUp == false)
        {
            ShipFiredUpMax = false;
        }
        else
        {
            ShipFiredUpMax = !ShipFiredUpMax;
        }
    }
}
