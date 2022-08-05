using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SaveSystem
{
    public static void Save(string saveName, object data)
    {
        var json = JsonUtility.ToJson(data, true);
        string path = Path.Combine(Application.persistentDataPath, saveName);
        try
        {
            File.WriteAllText(path, json);
            Debug.Log($"�ɹ�����浵��{path}");
        }
        catch(System.Exception e)
        {
            Debug.Log(e.Message);
        }
    }
    public static T Load<T>(string saveName)
    {
        string path = Path.Combine(Application.persistentDataPath, saveName);
        try
        {
            var json = File.ReadAllText(path);
            var data = JsonUtility.FromJson<T>(json);
            Debug.Log($"�ɹ���ȡ�浵��{path}");
            return data;
        }
        catch(System.Exception e)
        {
            Debug.Log(e.Message);
            return default(T);
        }
    }
    public static void Delete(string saveName)
    {
        string path = Path.Combine(Application.persistentDataPath, saveName);
        try
        {
            File.Delete(path);
            Debug.Log("�浵��ɾ��");
        }
        catch(System.Exception e)
        {
            Debug.Log(e.Message);
        }
    }
}
