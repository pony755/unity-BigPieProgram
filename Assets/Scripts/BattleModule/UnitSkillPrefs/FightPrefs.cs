using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FightPrefs : MonoBehaviour
{
    
    public GameObject[] fightHeros;
    
    void Start()
    {
        StartCoroutine(Load());
    } 

    IEnumerator Load()
    {       
        StartCoroutine(GameManager.instance.SetHeros());
        yield return null;
    }
}
