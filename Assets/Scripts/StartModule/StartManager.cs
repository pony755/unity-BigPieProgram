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
    // Start is called before the first frame update
    void Start()
    {
        startBtn.onClick.AddListener(StartGame);
        continueBtn.onClick.AddListener(ContinueGame);
        quitBtn.onClick.AddListener(QuitGame);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void StartGame()
    {
        mainMenu.SetActive(false);
        saveFields.SetActive(true);
        for(int i = 0; i < saveFields.transform.childCount; i++)
        {
            saveFields.transform.GetChild(i).GetComponent<SaveField>().fieldStateValue = 0;
        }
    }
    void ContinueGame()
    {
        mainMenu.SetActive(false);
        saveFields.SetActive(true);
        for (int i = 0; i < saveFields.transform.childCount; i++)
        {
            saveFields.transform.GetChild(i).GetComponent<SaveField>().fieldStateValue = 1;
        }
    }
    void QuitGame()
    {
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
    }
}
