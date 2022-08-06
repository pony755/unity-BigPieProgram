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
    public Player player;
    public TextMeshProUGUI saveNumber;
    public TextMeshProUGUI saveInfor;
    public TextMeshProUGUI saveTime;
    public Button saveButton;
    public Button loadButton;
    public Button deleteButton;
    public Button autoSaveButton;
    public Image deleteComfirm;
    public Button confirmButton;
    public Button cancelButton;
    void Start()
    {
        saveButton.onClick.AddListener(Save);
        loadButton.onClick.AddListener(Load);
        deleteButton.onClick.AddListener(DeleteConfirm);
        confirmButton.onClick.AddListener(Confirm);
        cancelButton.onClick.AddListener(Cancel);
    }
    void Save()//保存游戏存档
    {
        SetInfor();
        SetTime();
        player.Save(saveNumber.text);
    }
    void SetInfor()//获取存档信息(待定)
    {

    }
    void SetTime()//获取保存时间
    {
        DateTime dt = DateTime.Now;
        saveTime.text = dt.ToString();
    }
    void Load()//加载游戏存档
    {
        player.Load(saveNumber.text);
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
    }
    void Cancel()//取消删除游戏存档
    {
        deleteComfirm.gameObject.SetActive(false);
    }
    void ButtonViewListener()//按钮显示监听器
    {
        string saveName = "Save_" + saveNumber.text + ".sav";
        if (File.Exists(Path.Combine(Application.persistentDataPath,saveName)))
        {
            loadButton.gameObject.SetActive(true);
            deleteButton.gameObject.SetActive(true);
        }
        else
        {
            loadButton.gameObject.SetActive(false);
            deleteButton.gameObject.SetActive(false);
        }
    }
    void Update()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        ButtonViewListener();
    }
}
