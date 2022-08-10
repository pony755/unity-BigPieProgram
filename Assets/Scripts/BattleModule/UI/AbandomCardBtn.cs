using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbandomCardBtn : MonoBehaviour
{
    public Text NumText;

    private void Update()
    {
        NumText.text =GameManager.instance.fightPlayerCards.abandomCards.Count.ToString();
    }
    public void ClickAbandomCardBtn()
    {
        if(!GameManager.instance.AbandomCardCheck.activeInHierarchy)
        {
            foreach(var card in GameManager.instance.fightPlayerCards.abandomCards)
            {
                card.gameObject.transform.localPosition = card.cardAbandomAdress;
            }
            GameManager.instance.AbandomCardCheck.SetActive(true);
        }
            
        else
            GameManager.instance.AbandomCardCheck.SetActive(false);
    }
}
