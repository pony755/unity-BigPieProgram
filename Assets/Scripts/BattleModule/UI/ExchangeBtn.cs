using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExchangeBtn : MonoBehaviour
{
    public void clickHeroBtn()
    {
        GameManager.instance.pointUnit.Add(GameManager.instance.heroPreparePrefab[GameManager.instance.exchange.GetComponent<Exchange>().herosBtn.IndexOf(this.gameObject)].GetComponent<Unit>());
    }
}
