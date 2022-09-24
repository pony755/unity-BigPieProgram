using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FightPrefs : MonoBehaviour
{

    public List<GameObject> fightHeros;
    public List<GameObject> fightPrepareHeros;

    void Start()
    {
        BeginSetHeros();
    } 


   public void BeginSetHeros()
    {
        foreach(var hero in GameManager.instance.tempPlayer.GetComponent<FightPlayer>().fightHeroCode)
            fightHeros.Add(AllList.instance.allHero[hero]);
        
        foreach (var hero2 in GameManager.instance.tempPlayer.GetComponent<FightPlayer>().fightPrepareHeroCode)
            fightPrepareHeros.Add(AllList.instance.allHero[hero2]);

        StartCoroutine(GameManager.instance.SetHeros());
    }
}
