using System;
using System.Collections;
using System.Collections.Generic;
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
    void Start()
    {
        saveButton.onClick.AddListener(Save);
        loadButton.onClick.AddListener(Load);
        deleteButton.onClick.AddListener(Delete);
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
    void Load()
    {
        player.Load(saveNumber.text);
    }
    void Delete()
    {
        saveInfor.text = null;
        saveTime.text = null;
        player.Delete(saveNumber.text);
    }
    void Update()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }
}
