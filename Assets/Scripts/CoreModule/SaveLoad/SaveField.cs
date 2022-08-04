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
    public int saveNumber;
    public TextMeshProUGUI saveInfor;
    public TextMeshProUGUI saveTime;
    public Button saveButton;
    public Button loadButton;
    public Button deleteButton;
    public Button autoSaveButton;
    void Start()
    {
        saveNumber = Convert.ToInt32(transform.GetChild(0).GetComponent<TextMeshProUGUI>().text);
        saveInfor = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        saveTime = transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        saveButton = transform.GetChild(3).GetComponent<Button>();
        loadButton = transform.GetChild(4).GetComponent<Button>();
        deleteButton = transform.GetChild(5).GetComponent<Button>();
        autoSaveButton = transform.GetChild(6).GetComponent<Button>();
        saveButton.onClick.AddListener(SaveGame);
    }
    void SaveGame()
    {
        SetInfor();
        SetTime();
        player.Save();
    }
    void SetInfor()
    {

    }
    void SetTime()//获取保存时间
    {
        DateTime dt = DateTime.Now;
        saveTime.text = dt.ToLongDateString();
    }
    
    void Update()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }
}
