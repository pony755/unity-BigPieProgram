using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseBtn : MonoBehaviour
{
    public MapManager mapManager;
    public GameObject pauseMenu;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(delegate ()
        {
            pauseMenu.SetActive(true);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
