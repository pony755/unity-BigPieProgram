using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapInfor : MonoBehaviour
{
    public Player player;
    private GameObject BPCounter;
    private GameObject NSCounter;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        BPCounter = transform.GetChild(0).gameObject;
        NSCounter = transform.GetChild(1).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        BPCounter.GetComponent<TextMeshProUGUI>().text = player.BP.ToString();
        NSCounter.GetComponent<TextMeshProUGUI>().text = player.nightmareSharps.ToString();
    }
}
