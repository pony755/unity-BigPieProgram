using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CardsShow : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.gameObject.transform.GetChild(0).transform.GetChild(0).transform.GetComponent<Text>().text=GameManager.instance.fightPlayer.playerCards.Count.ToString();
    }
    public void ShowNum()
    {
        this.gameObject.transform.GetChild(0).gameObject.SetActive(true);
    }
}
