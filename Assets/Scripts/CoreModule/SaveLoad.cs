using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveLoad : MonoBehaviour
{
    public static Player LoadPlayer(Player player)
    {
        if (!PlayerPrefs.HasKey("BP"))
        {
            PlayerPrefs.SetInt("BP", 0);
        }
        player.BP = PlayerPrefs.GetInt("BP");
        if (!PlayerPrefs.HasKey("NightmareSharps"))
        {
            PlayerPrefs.SetInt("NightmareSharps", 0);
        }
        player.nightmareSharps = PlayerPrefs.GetInt("NightmareSharps");
        return player;
    }
    public static void SavePlayer(Player player)
    {
        PlayerPrefs.SetInt("BP", player.BP);
        PlayerPrefs.SetInt("NightmareSharps", player.nightmareSharps);
        PlayerPrefs.Save();
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
