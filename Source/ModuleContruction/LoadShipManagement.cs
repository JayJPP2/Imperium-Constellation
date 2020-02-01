using System;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.EventSystems;

public class LoadShipManagement : MonoBehaviour
{
    public static LoadShipManagement Instance;

    private struct ToggleTextureLink
    {
        public Toggle toggle;
        public Texture textureToggle;

        public ToggleTextureLink(Toggle toggle, Texture textureToggle)
        {
            this.toggle = toggle;
            this.textureToggle = textureToggle;
        }
    }


    private GameObject loadPageShip;
    private GameObject toggleGroupLoad;
    private GameObject togglePrefabLoad;
    private GameObject listToggleGroupLoad;
    private GameObject listParent;
    private Dictionary<string, ToggleTextureLink> ToggleDictionnary;
    private Button SelectButtonLoadPanel;
    public Button CancelButtonLoadPanel { get; private set; }
    private Button DeleteButtonLoadPanel;

    public static LoadShipManagement GetSingleton()
    {
        return Instance;
    }

    // Use this for initialization
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("Try Create multiple ConstructionModuleMain ");
            return;
        }

        DeleteButtonLoadPanel = GameObject.Find("Delete").GetComponent<Button>();
        DeleteButtonLoadPanel.interactable = false;
        SelectButtonLoadPanel = GameObject.Find("Select").GetComponent<Button>();
        SelectButtonLoadPanel.interactable = false;
        CancelButtonLoadPanel = GameObject.Find("Cancel").GetComponent<Button>();


        loadPageShip = GameObject.Find("LoadList");
        listParent = GameObject.Find("ViewContentLoadList");
        loadPageShip.SetActive(false);

        toggleGroupLoad = GameObject.Find("TogglesGroupLoadPanel");
        togglePrefabLoad = Resources.Load<GameObject>("Vessels_Parts/UI/PrefabToggleLoadPanel");

        if (ToggleDictionnary == null)
        {
            ToggleDictionnary = new Dictionary<string, ToggleTextureLink>();
        }

        UpdateLoadPanel();
    }

    public static GameObject LoadShipOneBlock(string path, int id = 0)
    {
        GameObject toBeReturn = null;
        TextAsset readerPart = Resources.Load<TextAsset>(path);
        string filePartData = readerPart.text;

        var JSON_ShipData = JObject.Parse(filePartData);
        toBeReturn = Instantiate(Resources.Load<GameObject>(JSON_ShipData["PrefabPath"].Value<string>()));
        toBeReturn.name = JSON_ShipData["name"].Value<string>();
        GameObject childOf = toBeReturn.transform.GetChild(0).gameObject;
        JArray WeaponsArray = JSON_ShipData["weapons"].Value<JArray>();
        if(childOf.transform.childCount > 1)
        {
            childOf.AddComponent<WeaponShootManager>();
            childOf.GetComponent<WeaponShootManager>().typeOfShip = Shoot.Origin.enemy;

            for (int i = 1; i < childOf.transform.childCount; i++)
            {
                if(childOf.transform.GetChild(i).name.Contains("Weapon"))
                {
                    int holder;
                    if(!int.TryParse(childOf.transform.GetChild(i).name.Split('_').Last(), out holder))
                    {
                        return null;
                    }
                    int valueToFind = -1;

                    for (int j = 0; j < WeaponsArray.Count; j++)
                    {
                        JArray temp = WeaponsArray[j]["spawnIDs"].Value<JArray>();
                        for (int jk = 0; jk < temp.Count; jk++)
                        {
                            if(temp[jk].Value<int>() == holder)
                            {
                                valueToFind = j;
                                j = WeaponsArray.Count;
                            }
                        }
                    }

                    GameObject part = childOf.transform.GetChild(i).gameObject;
                    WeaponState newWS = part.AddComponent<WeaponState>();
                    newWS.canFire = true;
                    newWS.fireRate = 60 / WeaponsArray[valueToFind]["fireRate"].Value<float>();
                    newWS.projectilePrefab = Resources.Load<GameObject>(WeaponsArray[valueToFind]["projectilPrefabPath"].Value<string>());
                    newWS.projectileImpactPrefab = Resources.Load<GameObject>(WeaponsArray[valueToFind]["projectilImpactPrefabPath"].Value<string>());
                    newWS.timerSinceLastShot = 0.0f;
                    newWS.speedShoot = WeaponsArray[valueToFind]["speedShot"].Value<float>();
                    newWS.parent = childOf;
                    newWS.spawn = part.transform;
                    newWS.Range = WeaponsArray[valueToFind]["range"].Value<uint>();
                    newWS.damage = WeaponsArray[valueToFind]["damage"].Value<uint>();
                    newWS.type = (WeaponStat.WeaponType)WeaponsArray[valueToFind]["type"].Value<int>();
                    childOf.GetComponent<WeaponShootManager>().AddWeaponState(newWS);
                    newWS.SetUpPool();

                }
            }
        }

        return toBeReturn;
    }

    public static GameObject LoadShip(string name, int id = 0, string prefabPath = null)
    {
        GameObject toBeReturn = new GameObject();

        StreamReader reader = null;
        string fileData = null;

        if(prefabPath == null)
        {
            reader = new StreamReader(Application.persistentDataPath + "/Save/Vessels_Save/" + name + ".json");

            if (reader == null)
            {
                throw new FileLoadException("Couldn't Load file");
            }

            fileData = reader.ReadToEnd();
            reader.Close();
        }
        else
        {
            TextAsset readerPart = Resources.Load<TextAsset>(prefabPath);
            fileData = readerPart.text;
        }

        JObject JSON_File_Root = JObject.Parse(fileData);
        toBeReturn.name = JSON_File_Root["name"].Value<string>();

        toBeReturn.tag = "Player";

        JArray datas = JSON_File_Root["parts"].Value<JArray>();

        TextAsset Stats = Resources.Load<TextAsset>("Jsons/Weapon_Stats");
        string statsData = Stats.text;

        var JSON_File_Stat = JObject.Parse(statsData);

        bool WeaponAdded = false;

        for (int j = 0; j < datas.Count; j++)
        {
            GameObject partHolder = new GameObject();
            partHolder.transform.SetParent(toBeReturn.transform);
            partHolder.name = datas[j]["name"].Value<string>();
            JArray subDatas = datas[j]["subParts"].Value<JArray>();
            for (int i = 0; i < subDatas.Count; i++)
            {
                GameObject part = Instantiate(Resources.Load<GameObject>(subDatas[i]["PrefabPath"].Value<string>()));
                part.name = subDatas[i]["name"].Value<string>();
                part.transform.position = new Vector3(subDatas[i]["offset_X"].Value<float>(), subDatas[i]["offset_Y"].Value<float>(), subDatas[i]["offset_Z"].Value<float>());
                part.transform.rotation = new Quaternion(subDatas[i]["rotation_X"].Value<float>(), subDatas[i]["rotation_Y"].Value<float>(), subDatas[i]["rotation_Z"].Value<float>(), subDatas[i]["rotation_W"].Value<float>());
                part.transform.SetParent(partHolder.transform);

                bool hasWeaponSpecific = false;

                if(subDatas[i]["moduleType"].Value<string>() == VesselsModulePartData.TypeOfModule.CentralHub.ToString())
                {
                    if(datas[j]["specificWeaponName"] != null)
                    {
                        hasWeaponSpecific = true;
                    }
                }

                if (subDatas[i]["moduleType"].Value<string>() == VesselsModulePartData.TypeOfModule.Weapon.ToString() || hasWeaponSpecific)
                {
                    if(!WeaponAdded)
                    {
                        toBeReturn.AddComponent<WeaponShootManager>();
                        if(id == 0)
                        {
                            toBeReturn.GetComponent<WeaponShootManager>().typeOfShip = Shoot.Origin.player;
                        }
                        else if(id == 1)
                        {
                            toBeReturn.GetComponent<WeaponShootManager>().typeOfShip = Shoot.Origin.ally;
                        }
                        else if(id == 2)
                        {
                            toBeReturn.GetComponent<WeaponShootManager>().typeOfShip = Shoot.Origin.enemy;
                        }
                        WeaponAdded = true;
                    }

                    JToken Find;
                    if (hasWeaponSpecific)
                    {
                        Find = JSON_File_Stat["uniqueIDReferencial"].Value<JArray>().Where(x => x.Value<string>() == datas[j]["specificWeaponName"].Value<string>()).Select(x => x.Value<JToken>()).First();
                    }
                    else
                    {
                        Find = JSON_File_Stat["uniqueIDReferencial"].Value<JArray>().Where(x => x.Value<string>() == subDatas[i]["name"].Value<string>()).Select(x => x.Value<JToken>()).First();
                    }

                    int index = JSON_File_Stat["uniqueIDReferencial"].Value<JArray>().IndexOf(Find);

                    WeaponStat temp = new WeaponStat(JSON_File_Stat["datas"][index].Value<JObject>(), null);

                    WeaponState newWS = null;
                    if (hasWeaponSpecific)
                    {
                        foreach(var item in part.transform.GetComponentsInChildren<Transform>())
                        {
                            if(item.name == datas[j]["specificWeaponName"].Value<string>())
                            {
                                newWS = item.gameObject.AddComponent<WeaponState>();
                                break;
                            }
                        }
                    }
                    else
                    {
                        newWS = part.AddComponent<WeaponState>();
                    }
                    newWS.canFire = true;
                    newWS.fireRate = 60 / temp.FireRate;
                    newWS.projectilePrefab = Resources.Load<GameObject>(temp.projectilPrefabPath);
                    newWS.projectileImpactPrefab = Resources.Load<GameObject>(temp.projectilImpactPrefabPath);
                    newWS.timerSinceLastShot = 0.0f;
                    newWS.speedShoot = temp.speedShot;
                    newWS.parent = toBeReturn;
                    newWS.spawn = part.transform.GetChild(0);
                    newWS.Range = temp.range;
                    newWS.damage = temp.damage;
                    newWS.type = temp.weaponType;
                    toBeReturn.GetComponent<WeaponShootManager>().AddWeaponState(newWS);
                    newWS.SetUpPool();
                }

            }
        }
        toBeReturn.GetComponent<WeaponShootManager>().lateStart();

        return toBeReturn;
    }
	
    public void CleanDictionnary(string nameToClean)
    {
        if(ToggleDictionnary.ContainsKey(nameToClean))
        {
            try
            {
                Destroy(ToggleDictionnary[nameToClean].toggle);
                Destroy(ToggleDictionnary[nameToClean].textureToggle);
            }
            finally
            {
               
            }
        }
    }

    public void CleanSelectDictionnary()
    {
        foreach (var item in ToggleDictionnary)
        {
            if(item.Value.toggle.isOn)
            {
                item.Value.toggle.isOn = false;
                break;
            }
        }
    }
    
    public void ShowUpLoadPanel()
    {
        if(ConstructionModuleMain.GetSingleton().RejectUserInput)
        {
            return;
        }

        loadPageShip.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        if(ToggleDictionnary.Count > 0)
        {
            EventSystem.current.SetSelectedGameObject(ToggleDictionnary.First().Value.toggle.gameObject);
            SelectButtonLoadPanel.GetComponent<LoadPanelButtonBehavior>().UpdateValues();
            CancelButtonLoadPanel.GetComponent<LoadPanelButtonBehavior>().UpdateValues();
            DeleteButtonLoadPanel.GetComponent<LoadPanelButtonBehavior>().UpdateValues();
        }
        else
        {
            EventSystem.current.SetSelectedGameObject(CancelButtonLoadPanel.gameObject);
        }
    }

    public void FullCleanLoadPanel()
    {
        if (ToggleDictionnary.Count > 1)
        {
            foreach(var Keys in ToggleDictionnary.Keys)
            {
                Destroy(ToggleDictionnary[Keys].toggle);
                Destroy(ToggleDictionnary[Keys].textureToggle);
            }
            ToggleDictionnary.Clear();
            ToggleDictionnary = null;
        }
    }

    public void UpdatePreviewVideo(string filenameFlat, Toggle newToggle = null)
    {
        if (newToggle == null)
        {
            newToggle = ToggleDictionnary[filenameFlat].toggle;
        }

        if (File.Exists(Application.persistentDataPath + "/Save_Media/" + filenameFlat + ".mp4"))
        {
            VideoPlayer videoclip = newToggle.transform.GetChild(1).GetChild(0).GetComponent<VideoPlayer>();
            videoclip.url = Application.persistentDataPath + "/Save_Media/" + filenameFlat + ".mp4";
            videoclip.isLooping = true;
            videoclip.time = 0.0f;
        }
    }

    public void UpdatePreviewImage(string filenameFlat, Toggle newToggle = null)
    {
        if(newToggle == null)
        {
            newToggle = ToggleDictionnary[filenameFlat].toggle;
        }

        if (File.Exists(Application.persistentDataPath + "/Save_Media/" + filenameFlat + ".spp"))
        {
            RawImage imagePicture = newToggle.transform.GetChild(1).GetChild(0).GetComponent<RawImage>();
            byte[] imageBytes = File.ReadAllBytes(Application.persistentDataPath + "/Save_Media/" + filenameFlat + ".spp");
            Texture2D newText = new Texture2D(2, 2);
            newText.LoadImage(imageBytes);

            imagePicture.texture = newText;

            if(ToggleDictionnary.ContainsKey(filenameFlat))
            {
                ToggleDictionnary[filenameFlat] = new ToggleTextureLink(newToggle, newText);
            }

            imagePicture.name = filenameFlat + "_SaveScreen";
        }
        else
        {
            newToggle.transform.GetChild(1).gameObject.SetActive(false);
        }
    }

    public void UpdateTextToggle(string filenameFlat, Toggle newToggle = null)
    {
        if (newToggle == null)
        {
            newToggle = ToggleDictionnary[filenameFlat].toggle;
        }

        StreamReader reader;
        try
        {
           reader = new StreamReader(Application.persistentDataPath + "/Save/Vessels_Save/" + filenameFlat + ".json");
        }
        catch(Exception e)
        {
            Debug.LogWarning(e.Message);
            return;
        }

        string fileData = reader.ReadToEnd();
        reader.Close();

        JObject JSON_File_Root = JObject.Parse(fileData);
        Text toggleText = newToggle.transform.GetChild(0).GetChild(0).GetComponent<Text>();
        toggleText.text = "Ship Name: " + JSON_File_Root["name"].Value<string>();
        int moduleUsed = 0;
        int moduleTotal = 0;
        int reactor = 0;
        int generator = 0;
        int weapon = 0;


        JArray array = JSON_File_Root["parts"].Value<JArray>();

        for (int i = 1; i < array.Count; i++)
        {
            moduleTotal++;
            if (array[i]["subParts"].Value<JArray>().Count != 0)
            {
                moduleUsed++;
                if (array[i]["subParts"].Value<JArray>()[0]["moduleType"].Value<string>() == VesselsModulePartData.TypeOfModule.Reactor.ToString())
                {
                    reactor++;
                }
                else if (array[i]["subParts"].Value<JArray>()[0]["moduleType"].Value<string>() == VesselsModulePartData.TypeOfModule.Generator.ToString())
                {
                    generator++;
                }
                else if (array[i]["subParts"].Value<JArray>()[0]["moduleType"].Value<string>() == VesselsModulePartData.TypeOfModule.Weapon.ToString())
                {
                    weapon++;
                }
            }

        }

        toggleText.text += "\nModules: " + moduleUsed + "/" + moduleTotal + "\nGenerator: " + generator + "\nReactor: " + reactor + "\nWeapon: " + weapon;

    }

    public void UpdateLoadPanel()
    {
        string[] filesNames;

        try
        {
            filesNames = Directory.GetFiles(Application.persistentDataPath + "/Save/Vessels_Save/");
            Directory.GetFiles(Application.persistentDataPath + "/Save_Media/");
        }
        catch(DirectoryNotFoundException)
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/Save/Vessels_Save/");
            Directory.CreateDirectory(Application.persistentDataPath + "/Save_Media/");
            filesNames = Directory.GetFiles(Application.persistentDataPath + "/Save/Vessels_Save/");
        }

        foreach (string filename in filesNames)
        {
            
            string filenameFlat = filename.Split('/').Last();
            filenameFlat = filenameFlat.Split('.')[0];

            //Only adding new Saves;
            if (ToggleDictionnary.ContainsKey(filenameFlat))
            {
                continue;
            }

            try
            {
                GameObject newToggle = Instantiate(togglePrefabLoad);
                newToggle.name = filenameFlat;

                newToggle.transform.SetParent(listParent.transform.GetChild(0), false);
                newToggle.GetComponent<Toggle>().group = toggleGroupLoad.GetComponent<ToggleGroup>();

                newToggle.GetComponent<Toggle>().onValueChanged.AddListener((value) =>
                {
                    ToolsUI.ToogleHorizontalUpdateUI(newToggle.GetComponent<Toggle>());
                    UpdatePreviewVideoForSelection(newToggle.GetComponent<Toggle>());
                });

                UpdatePreviewImage(filenameFlat, newToggle.GetComponent<Toggle>());
                UpdatePreviewVideo(filenameFlat, newToggle.GetComponent<Toggle>());
                UpdateTextToggle(filenameFlat, newToggle.GetComponent<Toggle>());

                ToggleDictionnary.Add(filenameFlat, new ToggleTextureLink(newToggle.GetComponent<Toggle>(), newToggle.transform.GetChild(1).GetChild(0).GetComponent<RawImage>().texture));

                Graphics.Blit(newToggle.transform.GetChild(1).GetChild(0).GetComponent<RawImage>().texture, newToggle.transform.GetChild(1).GetChild(0).GetComponent<VideoPlayer>().targetTexture);

            }
            catch(Exception e)
            {
                Debug.Log(e.StackTrace + e.Message + "\nSave Corrupted");
            }
        }

        for (int i = 0; i < ToggleDictionnary.Count; i++)
        {
            Navigation navig = ToggleDictionnary.ElementAt(i).Value.toggle.navigation;

            if(i > 0)
            {
                navig.selectOnUp = ToggleDictionnary.ElementAt(i - 1).Value.toggle;
            }

            if(i < ToggleDictionnary.Count - 1)
            {
                navig.selectOnDown = ToggleDictionnary.ElementAt(i + 1).Value.toggle;
            }

            navig.selectOnRight = CancelButtonLoadPanel;
            ToggleDictionnary.ElementAt(i).Value.toggle.navigation = navig;
        }
    }

    //Check if there is a panel toggle that is on then return true of false if false index is set to -1 else give the corresponding index
    public bool HasPanelOn(out int index)
    {
        index = -1;
        foreach (var item in ToggleDictionnary)
        {
            if(item.Value.toggle.isOn)
            {
                index = item.Value.toggle.gameObject.transform.GetSiblingIndex();
                return true;
            }
        }
        return false;
    }

    public void UpdatePreviewVideoForSelection(Toggle toggle)
    {
        Transform toggleGO = toggle.gameObject.transform;
        Transform transformTarget = toggleGO.GetChild(1).GetChild(0);
        if (toggle.isOn)
        {
            Graphics.Blit(ToggleDictionnary[toggle.gameObject.name].textureToggle, transformTarget.GetComponent<VideoPlayer>().targetTexture);
            transformTarget.GetComponent<VideoPlayer>().Play();
            transformTarget.GetComponent<RawImage>().texture = transformTarget.GetComponent<VideoPlayer>().targetTexture;
        }
        else
        {
            transformTarget.GetComponent<VideoPlayer>().frame = 0;
            transformTarget.GetComponent<VideoPlayer>().Stop();
            Graphics.Blit(ToggleDictionnary[toggle.gameObject.name].textureToggle, transformTarget.GetComponent<VideoPlayer>().targetTexture);
            transformTarget.GetComponent<RawImage>().texture = ToggleDictionnary[toggle.gameObject.name].textureToggle;
        }

        var temp = ToggleDictionnary.Where(x => x.Value.toggle.isOn == true).Select(x => x.Key);
        if (temp.Count() != 0)
        {
            DeleteButtonLoadPanel.interactable = true;
            SelectButtonLoadPanel.interactable = true;
        }
        else
        {
            DeleteButtonLoadPanel.interactable = false;
            SelectButtonLoadPanel.interactable = false;
        }
    }

    public void LoadSelected()
    {
        GameObject load = ToggleDictionnary == null ? null : ToggleDictionnary.Where(x => x.Value.toggle.isOn == true).Select(x => x.Value.toggle.gameObject).First();
        GameManager.LoadVesselID(load.transform.name);
        string saveUp = GameManager.nameVessel;
        ConstructionModuleMain.GetSingleton().ResetCleanVessel();
        ConstructionModuleMain.GetSingleton().LoadVesselFromFileToEditor(saveUp);
        CleanSelectDictionnary();
        CancelLoadPanel(true);
    }

    public void DeleteSelectedSave()
    {
        try
        {
            var temp = ToggleDictionnary.Where(x => x.Value.toggle.isOn == true).Select(x => x.Key);
            if (temp.Count() != 0)
            {
                string key = temp.First();
                int indexToSelectAfterDelete = ToggleDictionnary[key].toggle.gameObject.transform.GetSiblingIndex();
                if(indexToSelectAfterDelete != 0)
                {
                    indexToSelectAfterDelete--;
                }
                ToggleDictionnary[key].toggle.isOn = false;
                ConstructionModuleMain.GetSingleton().CleanOneSaveFromFiles(key);
                DestroyImmediate(ToggleDictionnary[key].toggle.gameObject);
                ToggleDictionnary.Remove(key);


                UpdateLoadPanel();
                EventSystem.current.SetSelectedGameObject(null);
                if(listParent.transform.GetChild(0).childCount > 1)
                {
                    EventSystem.current.SetSelectedGameObject(listParent.transform.GetChild(0).GetChild(indexToSelectAfterDelete).gameObject);
                }
                else
                {
                    EventSystem.current.SetSelectedGameObject(CancelButtonLoadPanel.gameObject);
                }
            }
        }
        finally
        {

        }
    }

    public void CancelLoadPanel(bool forceClosing = false)
    {
        try
        {
            var temp = ToggleDictionnary.Where(x => x.Value.toggle.isOn == true).Select(x => x.Value);
            if(temp.Count() != 0 && !forceClosing)
            {
                Toggle toggle = temp.First().toggle;
                toggle.isOn = false;
                ToolsUI.ToogleHorizontalUpdateUI(toggle);
            }
            else
            {
                DeleteButtonLoadPanel.targetGraphic.color = DeleteButtonLoadPanel.colors.normalColor;
                DeleteButtonLoadPanel.interactable = false;
                SelectButtonLoadPanel.targetGraphic.color = SelectButtonLoadPanel.colors.normalColor;
                SelectButtonLoadPanel.interactable = false;
                loadPageShip.SetActive(false);
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(GameObject.Find("Load"));
            }
        }
        finally
        {

        }

        
    }
}
