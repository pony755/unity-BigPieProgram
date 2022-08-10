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
    public int fieldStateValue;//�浵��״ֵ̬��0Ϊ��ʼ��1Ϊ���أ�2Ϊ��Ϸ��
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
    void LocatorCheck()//��λ�����
    {
        locatorName = "SaveLocator.sav";
        locatorPath = Path.Combine(Application.persistentDataPath, locatorName);
        if (!File.Exists(locatorPath))
        {
            File.Create(locatorPath);
        }
    }
    void SelectorCheck()//ѡ�������
    {
        selectorName = "ConstructionModeSelector.sav";
        selectorPath = Path.Combine(Application.persistentDataPath, selectorName);
        if (!File.Exists(selectorPath))
        {
            File.Create(selectorPath);
        }
    }
    void SetOnClickEvent()//���õ���¼�
    {
        startButton.onClick.AddListener(StartGame);
        loadButton.onClick.AddListener(LoadGame);
        deleteButton.onClick.AddListener(DeleteConfirm);
        confirmButton.onClick.AddListener(Confirm);
        cancelButton.onClick.AddListener(Cancel);
    }
    void StartGame()//��ʼ��Ϸ
    {
        File.Create(Path.Combine(Application.persistentDataPath, saveName));
        File.WriteAllText(locatorPath, saveNumber.text);
        File.WriteAllText(selectorPath, "0");
        SceneManager.LoadSceneAsync("MapScene");
    }
    void SetInfor()//��ȡ�浵��Ϣ
    {
        saveInfor.text = "��" + player.awakeCount + "������/" + player.level + "-" + player.childLevel;
    }
    void SetTime()//��ȡ����ʱ��
    {
        DateTime dt = DateTime.Now;
        saveTime.text = dt.ToString();
    }
    void LoadGame()//������Ϸ�浵
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
        loadButton.gameObject.SetActive(false);
        deleteButton.gameObject.SetActive(false);
    }
    void Cancel()//ȡ��ɾ����Ϸ�浵
    {
        deleteComfirm.gameObject.SetActive(false);
    }
    void ButtonViewListener()//��ť��ʾ������
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
