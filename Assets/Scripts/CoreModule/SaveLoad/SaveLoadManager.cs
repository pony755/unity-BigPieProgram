using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.IO;

public class SaveLoadManager : MonoBehaviour
{
    public GameObject player;
    public GameObject saveFields;
    public Button backButton;
    private int fieldCount;
    public string[] saveInfor;
    public string[] saveTime;
    private const string saveName = "SLData.sav";
    [System.Serializable]protected class SLData
    {
        public string[] saveInfor;
        public string[] saveTime;
    }
    public void Save()//±£¥Ê¥Êµµ
    {
        SLData slData = new SLData
        {
            saveInfor = saveInfor,
            saveTime = saveTime
        };
        SaveSystem.Save(saveName, slData);
    }
    public void Load()//º”‘ÿ¥Êµµ
    {
        SLData slData;
        if (File.Exists(Path.Combine(Application.persistentDataPath, saveName)))
        {
            slData = SaveSystem.Load<SLData>(saveName);
        }
        else
        {
            slData = new SLData()
            {
                saveInfor = new string[fieldCount],
                saveTime = new string[fieldCount]
            };
        }
        saveInfor = slData.saveInfor;
        saveTime = slData.saveTime;
    }
    void SaveField()//±£¥Ê¥Êµµ¿∏–≈œ¢
    {
        GameObject saveField;
        TextMeshProUGUI infor;
        TextMeshProUGUI time;
        for (int i = 0; i < fieldCount; i++)
        {
            saveField = saveFields.transform.GetChild(i).gameObject;
            infor = saveField.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            time = saveField.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
            saveInfor[i] = infor.text;
            saveTime[i] = time.text;
        }
    }
    void LoadField()//º”‘ÿ¥Êµµ¿∏–≈œ¢
    {
        GameObject saveField;
        TextMeshProUGUI infor;
        TextMeshProUGUI time;
        for(int i = 0; i < fieldCount; i++)
        {
            saveField = saveFields.transform.GetChild(i).gameObject;
            infor = saveField.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            time = saveField.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
            infor.text = saveInfor[i];
            time.text = saveTime[i];
        }
    }
    void Start()
    {
        saveFields = GameObject.Find("Save Fields");
        fieldCount = saveFields.transform.childCount;
        saveInfor = new string[fieldCount];
        saveTime = new string[fieldCount];
        Load();
        LoadField();
        backButton.onClick.AddListener(delegate () {
            SaveField();
            Save();
            player = GameObject.FindGameObjectWithTag("Player");
            Scene scene = SceneManager.GetSceneByName("MapScene");
            SceneManager.MoveGameObjectToScene(player, scene);
            SceneManager.UnloadSceneAsync("SaveLoadScene");
        });
    }
    // Update is called once per frame
    void Update()
    {
        if(player != null)
        {
            if (player.GetComponent<Player>().globalStateValue == 0)
            {
                player.GetComponent<Player>().globalStateValue++;
            }
        }
    }
}
