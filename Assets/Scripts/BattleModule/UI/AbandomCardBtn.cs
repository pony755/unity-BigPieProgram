using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbandomCardBtn : MonoBehaviour
{
    public Text NumText;

    private void Update()
    {
        NumText.text =GameManager.instance.player.abandomCards.Count.ToString();
    }
    public void ClickAbandomCardBtn()
    {
        if(!GameManager.instance.AbandomCardCheck.activeInHierarchy)
            GameManager.instance.AbandomCardCheck.SetActive(true);
        else
            GameManager.instance.AbandomCardCheck.SetActive(false);
    }
}
