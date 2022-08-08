using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using UnityEngine.SceneManagement;

public class SaveSelect : MonoBehaviour
{
    public GameObject saveLocator;
    public TextMeshProUGUI saveNumber;
    private bool saveExist = false;
    private string saveName;
    private string locatorName;
    private string locatorPath;
    // Start is called before the first frame update
    void Start()
    {
        saveName = "Save_" + saveNumber.text + ".sav";
        locatorName = "SaveLocator.sav";
        locatorPath = Path.Combine(Application.persistentDataPath, locatorName);
        if (!File.Exists(locatorPath))
        {
            File.Create(locatorPath);
        } 
        GetComponent<Button>().onClick.AddListener(delegate ()
        {
            if (!saveExist)
            {
                File.Create(Path.Combine(Application.persistentDataPath, saveName));
                File.WriteAllText(locatorPath,saveNumber.text);
                SceneManager.LoadSceneAsync("MapScene");
            }
        });
    }

    // Update is called once per frame
    void Update()
    {
        if(File.Exists(Path.Combine(Application.persistentDataPath, saveName)))
        {
            saveNumber.color = Color.red;
            saveExist = true;
        }
    }
}
