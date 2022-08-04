using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveLoadBtn : MonoBehaviour
{
    GameObject player;
    GameObject mapManager;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        mapManager = GameObject.FindGameObjectWithTag("MapManager");
        GetComponent<Button>().onClick.AddListener(delegate ()
        {
            SceneManager.LoadScene("SaveLoadScene", LoadSceneMode.Additive);
            mapManager.GetComponent<MapManager>().FreezeMap();
            Scene scene = SceneManager.GetSceneByName("SaveLoadScene");
            SceneManager.MoveGameObjectToScene(player, scene);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
