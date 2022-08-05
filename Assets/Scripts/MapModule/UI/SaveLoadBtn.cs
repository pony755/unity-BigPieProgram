using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveLoadBtn : MonoBehaviour
{
    GameObject player;
    public MapManager mapManager;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        GetComponent<Button>().onClick.AddListener(delegate ()
        {
            SceneManager.LoadSceneAsync("SaveLoadScene", LoadSceneMode.Additive);
            mapManager.BackUpMap();
            mapManager.FreezeMap();
            Scene scene = SceneManager.GetSceneByName("SaveLoadScene");
            SceneManager.MoveGameObjectToScene(player, scene);
        });
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
