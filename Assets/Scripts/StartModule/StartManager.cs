using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartManager : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject saveFields;
    public Button startBtn;
    public Button continueBtn;
    public Button settingsBtn;
    public Button quitBtn;
    public Button backBtn;
    // Start is called before the first frame update
    void Start()
    {
        startBtn.onClick.AddListener(StartGame);
        continueBtn.onClick.AddListener(ContinueGame);
        quitBtn.onClick.AddListener(QuitGame);
        backBtn.onClick.AddListener(Back);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void StartGame()//开始游戏
    {
        mainMenu.SetActive(false);
        saveFields.SetActive(true);
        for(int i = 0; i < saveFields.transform.childCount; i++)
        {
            saveFields.transform.GetChild(i).GetComponent<SaveField>().fieldStateValue = 0;
        }
        backBtn.gameObject.SetActive(true);
    }
    void ContinueGame()//继续游戏
    {
        mainMenu.SetActive(false);
        saveFields.SetActive(true);
        for (int i = 0; i < saveFields.transform.childCount; i++)
        {
            saveFields.transform.GetChild(i).GetComponent<SaveField>().fieldStateValue = 1;
        }
        backBtn.gameObject.SetActive(true);
    }
    void QuitGame()//退出游戏
    {
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
    }
    void Back()//返回主菜单
    {
        saveFields.SetActive(false);
        mainMenu.SetActive(true);
        backBtn.gameObject.SetActive(false);
    }
}
