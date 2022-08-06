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
    void Save()//������Ϸ�浵
    {
        SetInfor();
        SetTime();
        player.Save(saveNumber.text);
    }
    void SetInfor()//��ȡ�浵��Ϣ(����)
    {

    }
    void SetTime()//��ȡ����ʱ��
    {
        DateTime dt = DateTime.Now;
        saveTime.text = dt.ToString();
    }
    void Load()//������Ϸ�浵
    {
        player.Load(saveNumber.text);
    }
    void DeleteConfirm()//ȷ���Ƿ�ɾ���浵
    {
        deleteComfirm.gameObject.SetActive(true);
    }
    void Confirm()//ȷ��ɾ����Ϸ�浵
    {
        saveInfor.text = null;
        saveTime.text = null;
        player.Delete(saveNumber.text);
        deleteComfirm.gameObject.SetActive(false);
    }
    void Cancel()//ȡ��ɾ����Ϸ�浵
    {
        deleteComfirm.gameObject.SetActive(false);
    }
    void ButtonViewListener()//��ť��ʾ������
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
