using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public Button MenuBtn;
    public Button BackBtn;

    void Start()
    {
        MenuBtn.onClick.AddListener(GotoStartScene);
        BackBtn.onClick.AddListener(Back);
    }
    void Update()
    {
        
    }
    void GotoStartScene()//跳转到开始界面
    {
        SceneManager.LoadSceneAsync("StartScene");
    }
    void Back()
    {
        gameObject.SetActive(false);
    }

}
