using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartManager : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject saveSelect;
    public Button startBtn;
    public Button continueBtn;
    public Button settingsBtn;
    public Button quitBtn;
    // Start is called before the first frame update
    void Start()
    {
        startBtn.onClick.AddListener(StartGame);
        quitBtn.onClick.AddListener(QuitGame);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void StartGame()
    {
        mainMenu.SetActive(false);
        saveSelect.SetActive(true);
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
