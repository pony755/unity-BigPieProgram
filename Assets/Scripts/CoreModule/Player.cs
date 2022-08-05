using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static PlaceCard;

public class Player : MonoBehaviour
{
    public int globalStateValue;//ȫ��״ֵ̬��0Ϊλ�ڵ�ͼ��1Ϊλ����������
    public CardState[] cardStates;
    public float[] rotation;
    public int level;
    public int childLevel;
    public int BP;
    public int nightmareSharps;
    [System.Serializable]protected class SaveData
    {
        public CardState[] cardStates;
        public float[] rotation;
        public int level;
        public int childLevel;
        public int BP;
        public int nightmareSharps;
    }
    public void InitializeArray(int length)//��ʼ������
    {
        cardStates = new CardState[length];
        rotation = new float[length];
    }
    public void Save(string saveNumber)//����浵
    {
        string saveName = "Save_" + saveNumber + ".sav";
        SaveData saveData = new SaveData
        {
            cardStates = cardStates,
            rotation = rotation,
            level = level,
            childLevel = childLevel,
            BP = BP,
            nightmareSharps = nightmareSharps
        };
        SaveSystem.Save(saveName, saveData);
    }
    public void Load(string saveNumber)//���ش浵
    {
        string saveName = "Save_" + saveNumber + ".sav";
        SaveData saveData;
        if (File.Exists(Path.Combine(Application.persistentDataPath, saveName)))
        {
            saveData = SaveSystem.Load<SaveData>(saveName);
        }
        else
        {
            saveData = new SaveData();
        }
        globalStateValue = 0;
        cardStates = saveData.cardStates;
        rotation = saveData.rotation;
        level = saveData.level;
        childLevel = saveData.childLevel;
        BP = saveData.BP;
        nightmareSharps = saveData.nightmareSharps;
        Debug.Log(level);
        Debug.Log(childLevel);
        Debug.Log(BP);
        Debug.Log(nightmareSharps);
    }
    public void Delete(string saveNumber)//ɾ���浵
    {
        string saveName = "Save_" + saveNumber + ".sav";
        SaveSystem.Delete(saveName);
    }
    void Start()
    {
        
    }
    void Update()
    {
        
    }
}
