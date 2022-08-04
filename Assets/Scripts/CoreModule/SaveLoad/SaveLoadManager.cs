using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SaveLoadManager : MonoBehaviour
{
    public GameObject player;
    public Button backButton;
    // Start is called before the first frame update
    void Start()
    {
        backButton = GameObject.Find("BackButton").GetComponent<Button>();
        backButton.onClick.AddListener(delegate () {
            player = GameObject.FindGameObjectWithTag("Player");
            Scene scene = SceneManager.GetSceneByName("MapScene");
            SceneManager.MoveGameObjectToScene(player, scene);
            SceneManager.UnloadSceneAsync("SaveLoadScene");
        });
    }

    // Update is called once per frame
    void Update()
    {
        if(player != null)
        {
            if (player.GetComponent<Player>().globalStateValue == 0)
            {
                player.GetComponent<Player>().globalStateValue++;
            }
        }
    }
}
