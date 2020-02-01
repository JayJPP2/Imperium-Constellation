using UnityEngine;
using UnityEditor;

public class LevelData
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string Reward { get; private set; }
    public LevelManager.LevelType type { get; set; }
    public GameObject go {get; private set;}

    public LevelData()
    {
        this.Name = "";
        this.Description = "";
        this.Reward = "";
        this.type = LevelManager.LevelType.Undefined;
    }

    public LevelData(string Name, string Description,string _Reward, GameObject go)
    {
        this.Name = Name;
        this.Description = Description;
        this.Reward = _Reward;
        this.type = LevelManager.LevelType.Available;
        this.go = go;
    }

    public void ChangeSelectionState(bool isSelected)
    {
        if(isSelected)
        {
            go.GetComponent<Renderer>().material.color = Color.green;
            type = LevelManager.LevelType.Selected;
        }
        else
        {
            go.GetComponent<Renderer>().material.color = Color.white;
            type = LevelManager.LevelType.Available;
        }
    }
}