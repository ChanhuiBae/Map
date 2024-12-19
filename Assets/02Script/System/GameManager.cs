using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneName
{

}
public class GameManager : Singleton<GameManager>
{
    [SerializeField]
    private PlayerData playerData;
    private ExplorationData explorationData;
    private SettingData setData;

    // private TableName table;

    private void Awake()
    {
        base.Awake();
        dataPath = Application.persistentDataPath;

        playerData = new PlayerData();
        setData = new SettingData();
        MapData mapData;

        if(!TryGetComponent<MapData>(out mapData))
        {
            Debug.Log("GameManager - Awake - mapData");
        }
        else
        {
            explorationData = new ExplorationData(mapData);
        }
    }

    public void CreateUserData()
    {
        playerData.uidCounter = 0;
        //playerDataTable.TryGetValue(0, out TableEntity_Player info);
        //playerData.Level = info.Level;

        SaveData();
    }

    #region Save&Load
    private string dataPath;
    public void SaveData()
    {
        string path = dataPath + "/player";
        string data = JsonUtility.ToJson(playerData);
        File.WriteAllText(path, data);
        path = dataPath + "/exploration";
        data = JsonUtility.ToJson(explorationData);
        File.WriteAllText(path, data);
        path = dataPath + "/setting";
        data = JsonUtility.ToJson(setData);
        File.WriteAllText(path, data);
    }

    public bool LoadData()
    {
        if (File.Exists(dataPath + "/player"))
        {
            string data = File.ReadAllText(dataPath + "/player");
            playerData = JsonUtility.FromJson<PlayerData>(data);

            if (File.Exists(dataPath + "/setting"))
            {
                data = File.ReadAllText(dataPath + "/setting");
                setData = JsonUtility.FromJson<SettingData>(data);

                return true;
            }
        }

        return false;
    }

    public bool LoadExplorationData()
    {
        if (File.Exists(dataPath + "/exploration"))
        {
            string data = File.ReadAllText(dataPath + "/exploration");
            explorationData = JsonUtility.FromJson<ExplorationData>(data);
            return true;
        }
        return false;
    }

    public void DeleteData()
    {
        File.Delete(dataPath);
    }

    public bool CheckData()
    {
        if (File.Exists(dataPath + "/player") && File.Exists(dataPath + "/setting"))
        {
            return true;
        }
        return false;
    }
    #endregion

    #region updateUserData
    public void CreateUserData(int playerUid)
    {
        SaveData();
    }
    #endregion

    #region Setter
    public void SetExplorationData()
    {

    }


/*    public void SetVolumBGM(int value)
    {
        setData.bgm = value;
        soundManager.SetVolumBGM(value);
    }

    public void SetVolumSFX(int value)
    {
        setData.sfx = value;
        soundManager.SetVolumSFX(value);
    }
*/
    #endregion
    #region Getter
    public PlayerData PlayerInfo
    {
        get => playerData;
    }

    public ExplorationData Exploration
    {
        get => explorationData;
    }

    public MapData MapData
    {
        get => explorationData.mapData;
    }

    public SettingData GetSettingData
    {
        get => setData;
    }
    #endregion


    #region LoadingLogic
    private SceneName nextScene;
    public SceneName NextScene
    {
        get => nextScene;
    }

    public void AsyncLoadNextScene(SceneName scene)
    {
        SaveData();
        nextScene = scene;
        SceneManager.LoadScene("LoadingScene");
    }
    #endregion 
}
