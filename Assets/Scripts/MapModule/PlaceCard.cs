using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlaceCard : MonoBehaviour
{
    public CardState cardState;
    public CardType cardType;
    public MapManager mapManager;
    public enum CardState//¿¨ÅÆ×´Ì¬
    {
        hide,back,face
    };

    public enum CardType//¿¨ÅÆÀàÐÍ
    {
        battle,eliteBattle,randomEvent,shop,hotel,treasure,portal,placeOfGod
    };


    // Start is called before the first frame update
    void Start()
    {
        cardState = CardState.back;
        mapManager = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>();
    }

    public void OnMouseUp()
    {
        if(cardState.Equals(CardState.hide)||cardState.Equals(CardState.face))
        {
            return;
        }
        OpenCard();
    }

    void OpenCard()//·­ÅÆ
    {
        StartCoroutine(TurnAnimation());
        cardState = CardState.face;
        if (cardType.Equals(CardType.battle))
        {
            StartCoroutine(EnterBattle());
        }
    }

    IEnumerator TurnAnimation()//·­ÅÆ¶¯»­
    {
        float angle = 0;
        for(int i = 0; i < 360; i++)
        {
            angle = (float)(angle + 0.5);
            transform.eulerAngles = new Vector3(0, angle, 0);
            yield return new WaitForSeconds(0.005f);
        }
    }

    IEnumerator EnterBattle()//½øÈëÕ½¶·
    {
        yield return new WaitForSeconds(3.0f);
        SceneManager.LoadScene("BattleScene", LoadSceneMode.Additive);
        mapManager.FreezeMap();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
