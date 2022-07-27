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
        none,battle,eliteBattle,randomEvent,shop,hotel,treasure,portal,placeOfGod
    };


    // Start is called before the first frame update
    void Start()
    {
        cardState = CardState.back;
    }

    public void OnMouseUp()
    {
        if(cardState == CardState.face||cardState == CardState.hide)
        {
            return;
        }
        OpenCard();
    }

    void OpenCard()
    {
        transform.eulerAngles = new Vector3(0, 180, 0);
        cardState = CardState.face;
        mapManager.AddCardInList(this);
        StartCoroutine(EnterBattle());
    }

    IEnumerator EnterBattle()
    {
        yield return new WaitForSeconds(3.0f);
        SceneManager.LoadScene("BattleScene", LoadSceneMode.Additive);
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
