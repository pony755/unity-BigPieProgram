using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static PlaceCard;

public class Player : MonoBehaviour
{
    public int globalStateValue;//全局状态值，0为位于地图，1为位于其他场景
    public bool autoSaveState;//自动存档标志
    public CardState[] cardStates;//当前地图卡牌状态
    public float[] rotation;//当前地图卡牌翻转
    public int awakeCount;//苏醒计数器，记录第几次从头开始游戏
    public int level;//关卡数
    public int childLevel;//子关卡数
    public int BP;//比派
    public int nightmareSharps;//当前持有的梦魇碎片数量
    [System.Serializable]protected class SaveData
    {
        public bool autoSaveState;
        public CardState[] cardStates;
        public float[] rotation;
        public int awakeCount;
        public int level;
        public int childLevel;
        public int BP;
        public int nightmareSharps;
    }
    public void InitializeArray(int length)//初始化数组
    {
        cardStates = new CardState[length];
        rotation = new float[length];
    }
    public void Save(string saveNumber)//保存存档
    {
        string saveName = "Save_" + saveNumber + ".sav";
        SaveData saveData = new SaveData
        {
            autoSaveState = autoSaveState,
            cardStates = cardStates,
            rotation = rotation,
            awakeCount = awakeCount,
            level = level,
            childLevel = childLevel,
            BP = BP,
            nightmareSharps = nightmareSharps
        };
        SaveSystem.Save(saveName, saveData);
    }
    public void Load(string saveNumber)//加载存档
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
        autoSaveState = saveData.autoSaveState;
        cardStates = saveData.cardStates;
        rotation = saveData.rotation;
        awakeCount = saveData.awakeCount;
        level = saveData.level;
        childLevel = saveData.childLevel;
        BP = saveData.BP;
        nightmareSharps = saveData.nightmareSharps;
        Debug.Log(level);
        Debug.Log(childLevel);
        Debug.Log(BP);
        Debug.Log(nightmareSharps);
    }
    public void Delete(string saveNumber)//删除存档
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
