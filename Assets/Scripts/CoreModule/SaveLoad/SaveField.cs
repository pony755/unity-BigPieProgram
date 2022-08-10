using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveField : MonoBehaviour
{
    public int fieldStateValue;//存档栏状态值，0为开始，1为加载，2为游戏中
    public Player player;
    public TextMeshProUGUI saveNumber;
    public TextMeshProUGUI saveInfor;
    public TextMeshProUGUI saveTime;
    public Button startButton;
    public Button loadButton;
    public Button deleteButton;
    public Button autoSaveButton;
    public Image deleteComfirm;
    public Button confirmButton;
    public Button cancelButton;
    private bool saveExist = false;
    private string saveName;
    private string locatorName;
    private string locatorPath;
    private string selectorName;
    private string selectorPath;
    void Start()
    {
        saveName = "Save_" + saveNumber.text + ".sav";
        LocatorCheck();
        SelectorCheck();
        SetOnClickEvent();
    }
    void LocatorCheck()//定位器检测
    {
        locatorName = "SaveLocator.sav";
        locatorPath = Path.Combine(Application.persistentDataPath, locatorName);
        if (!File.Exists(locatorPath))
        {
            File.Create(locatorPath);
        }
    }
    void SelectorCheck()//选择器检测
    {
        selectorName = "ConstructionModeSelector.sav";
        selectorPath = Path.Combine(Application.persistentDataPath, selectorName);
        if (!File.Exists(selectorPath))
        {
            File.Create(selectorPath);
        }
    }
    void SetOnClickEvent()//设置点击事件
    {
        startButton.onClick.AddListener(StartGame);
        loadButton.onClick.AddListener(LoadGame);
        deleteButton.onClick.AddListener(DeleteConfirm);
        confirmButton.onClick.AddListener(Confirm);
        cancelButton.onClick.AddListener(Cancel);
    }
    void StartGame()//开始游戏
    {
        File.Create(Path.Combine(Application.persistentDataPath, saveName));
        File.WriteAllText(locatorPath, saveNumber.text);
        File.WriteAllText(selectorPath, "0");
        SceneManager.LoadSceneAsync("MapScene");
    }
    void SetInfor()//获取存档信息
    {
        saveInfor.text = "第" + player.awakeCount + "次苏醒/" + player.level + "-" + player.childLevel;
    }
    void SetTime()//获取保存时间
    {
        DateTime dt = DateTime.Now;
        saveTime.text = dt.ToString();
    }
    void LoadGame()//加载游戏存档
    {
        if(fieldStateValue == 1)
        {
            File.WriteAllText(locatorPath, saveNumber.text);
            File.WriteAllText(selectorPath, "1");
            SceneManager.LoadSceneAsync("MapScene");
        }
        else
        {
            player.Load(saveNumber.text);
        }
    }
    void DeleteConfirm()//确认是否删除存档
    {
        deleteComfirm.gameObject.SetActive(true);
    }
    void Confirm()//确认删除游戏存档
    {
        saveInfor.text = null;
        saveTime.text = null;
        player.Delete(saveNumber.text);
        deleteComfirm.gameObject.SetActive(false);
        loadButton.gameObject.SetActive(false);
        deleteButton.gameObject.SetActive(false);
    }
    void Cancel()//取消删除游戏存档
    {
        deleteComfirm.gameObject.SetActive(false);
    }
    void ButtonViewListener()//按钮显示监听器
    {
        if(fieldStateValue == 0)
        {
            if (saveExist)
            {
                startButton.gameObject.SetActive(false);
            }
            else
            {
                startButton.gameObject.SetActive(true);
            }
            loadButton.gameObject.SetActive(false);
            deleteButton.gameObject.SetActive(false);
        }
        else
        {
            if (saveExist)
            {
                loadButton.gameObject.SetActive(true);
                deleteButton.gameObject.SetActive(true);
            }
            else
            {
                loadButton.gameObject.SetActive(false);
                deleteButton.gameObject.SetActive(false);
            }
            startButton.gameObject.SetActive(false);
        }
    }
    void Update()
    {
        if (fieldStateValue == 2)
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        }
        if (File.Exists(Path.Combine(Application.persistentDataPath, saveName)))
        {
            saveExist = true;
        }
        ButtonViewListener();
    }
}
