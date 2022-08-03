using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlaceCard : MonoBehaviour
{
    public Player player;
    public CardState cardState;
    public CardType cardType;
    public MapManager mapManager;
    public bool isLinked = false;//联动标志，标记是否已经进行了相邻卡牌状态转换操作
    public bool isEmbedded = false;//嵌入标志，标记是否含有梦魇碎片
    public enum CardState//卡牌状态
    {
        hide,back,face
    };
    public enum CardType//卡牌类型
    {
        battle,eliteBattle,randomEvent,shop,inn,treasure,portal,placeOfGod
    };
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        mapManager = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>();
    }
    private void OnMouseUp()//鼠标点击
    {
        if (mapManager.isTurning)
        {
            return;
        }
        if(cardState.Equals(CardState.hide)||cardState.Equals(CardState.face))
        {
            return;
        }
        TurnCard();
    }
    void TurnCard()//翻牌
    {
        mapManager.isTurning = true;
        cardState = CardState.face;
        if (isEmbedded)
        {
            player.nightmareSharps++;
        }
        StartCoroutine(TurnAnimation());
        if (cardType.Equals(CardType.battle))
        {
            //StartCoroutine(EnterBattle());
        }
    }
    IEnumerator TurnAnimation()//翻牌动画
    {
        float angle = 0;
        for(int i = 0; i < 360; i++)
        {
            angle += 0.5f;
            transform.eulerAngles = new Vector3(0, angle, 0);
            yield return new WaitForSeconds(0.0025f);
        }
        mapManager.isTurning = false;
    }
    IEnumerator EnterBattle()//进入战斗
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("BattleScene", LoadSceneMode.Additive);
        mapManager.FreezeMap();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
