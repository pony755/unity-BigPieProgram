using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] public int level;
    [SerializeField] public int childlevel;
    [SerializeField] public int BP;
    [SerializeField] public int nightmareSharps;

    public const string saveName = "SaveTest.sav";
    [System.Serializable]protected class SaveData
    {
        public int level;
        public int childlevel;
        public int BP;
        public int nightmareSharps;
    }
    public void Save()//保存存档
    {
        SaveData saveData = new SaveData();
        saveData.level = level;
        saveData.childlevel = childlevel;
        saveData.BP = BP;
        saveData.nightmareSharps = nightmareSharps;
        SaveSystem.Save(saveName, saveData);
    }
    public void Load()//加载存档
    {
        SaveData saveData;
        if (File.Exists(Path.Combine(Application.persistentDataPath, saveName)))
        {
            saveData = SaveSystem.Load<SaveData>(saveName);
        }
        else
        {
            saveData = new SaveData();
        }
        level = saveData.level;
        childlevel = saveData.childlevel;
        BP = saveData.BP;
        nightmareSharps = saveData.nightmareSharps;
        Debug.Log(level);
        Debug.Log(childlevel);
        Debug.Log(BP);
        Debug.Log(nightmareSharps);
    }
    [UnityEditor.MenuItem("Developer/Delete player save file")]//开发者选项：删除存档
    public static void Delete()
    {
        SaveSystem.Delete(saveName);
    }
    void Start()
    {
        
    }
    void Update()
    {
        
    }
}
