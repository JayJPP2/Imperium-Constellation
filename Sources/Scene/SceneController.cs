using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : ScriptableObject {

    public static SceneController Instance;

    public enum SceneID
    {
        MainMenu,
        Editor,
        UniverseMap,
        MainGameScene
    };

    private SceneController()
    {

    }

    public static SceneController GetSingleton()
    {
        if(Instance == null)
        {
            Instance = (SceneController)CreateInstance("SceneController");
        }
        return Instance;
    }

    private void Awake()
    {
        if(Instance == null)
        {   
            Instance = this;
        }
    }

    public void OverrideSceneID(SceneID newID)
    {
        IDScene = newID;
    }

    public static SceneID IDScene { get; private set; }

    public void PlayEditor()
    {
        SceneManager.LoadScene("ModuleCreation");
        IDScene = SceneID.Editor;
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("Game");
        IDScene = SceneID.MainGameScene;
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("Menu");
        IDScene = SceneID.MainMenu;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
