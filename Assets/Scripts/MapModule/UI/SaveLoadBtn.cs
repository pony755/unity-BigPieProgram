using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveLoadBtn : MonoBehaviour
{
    Player player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        GetComponent<Button>().onClick.AddListener(delegate ()
        {
            player.Save();
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
