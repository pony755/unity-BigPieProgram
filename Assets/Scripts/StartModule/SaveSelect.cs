using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using UnityEngine.SceneManagement;

public class SaveSelect : MonoBehaviour
{
    private bool saveExist = false;
    public TextMeshProUGUI saveNumber;
    private string saveName;
    // Start is called before the first frame update
    void Start()
    {
        saveName = "Save_" + saveNumber.text + ".sav";
        GetComponent<Button>().onClick.AddListener(delegate ()
        {
            if (!saveExist)
            {
                File.Create(Path.Combine(Application.persistentDataPath, saveName));
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
