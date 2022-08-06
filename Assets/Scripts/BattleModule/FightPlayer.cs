using System;
using System.Collections.Generic;
using UnityEngine;
using Koubot.Tool;
using System.Collections;

public class FightPlayer : MonoBehaviour
{
    private bool startTakeCardSwitch;//判断是否为开始阶段抽卡
    public Unit playerObject;
    public GameObject cardsObject;
    public GameObject cardsInCards;//接收克隆的卡牌
    [Header("卡组")]
    public int startCard;
    public int maxCard;
    public int addCardNum;
    public List<Cards> playerCardsPrefabs;//卡组
    public List<Cards> playerCards;//卡组
    public List<Cards> haveCards;//手牌
    public List<Cards> abandomCards;//弃牌堆
    [Header("玩家属性")]
    public int playerAtk;
    // Start is called before the first frame update
    void Start()
    {
        foreach(var card in playerCardsPrefabs)
        {
            GameObject A = Instantiate(card.gameObject,new Vector3(0, 0, 0), Quaternion.identity, cardsInCards.transform);
            A.transform.localPosition = new Vector3(-300, 0, 0);
            playerCards.Add(A.transform. GetComponent<Cards>());
        }    
        startTakeCardSwitch = true;
        for(int i = 0; i < startCard; i++)
            TakeCard();

    }

    // Update is called once per frame
    void Update()
    {
        
    }


    //————————————————————————卡牌—————————————————————————

    public void TakeCard()//抽一张牌
    {
        if(playerCards.Count == 0)
        {
            StartCoroutine(CardEmpyt());
            return;
        }    
        int index = Koubot.Tool.Random.RandomTool.GenerateRandomInt(0,playerCards.Count-1);
        haveCards.Add(playerCards[index]);
        playerCards[index].gameObject.transform.SetParent(GameManager.instance.CardCanvas.transform);
        playerCards.Remove(playerCards[index]);
        

        if (haveCards.Count == startCard)
            startTakeCardSwitch=false;//开局抽牌完毕，开始抽牌的标志位设false
        if (!startTakeCardSwitch)
            GameManager.instance.AdjustCards = true;//调整卡牌的标志位

    }

    public void ButtonTakeCard()//摸牌按钮
    {
        for (int i = 0; i < addCardNum; i++)
            TakeCard();
        GameManager.instance.state=BattleState.ACTIONFINISH;
        StartCoroutine(GameManager.instance.ActionFinish());
    }

    public void ResetCards()//洗弃牌堆的牌回摸牌堆
    {
        int count = abandomCards.Count;       
            for(int i=0; i<count; i++)
            {
                playerCards.Add(abandomCards[i]);
                abandomCards[i].gameObject.transform.SetParent(GameManager.instance.player.cardsInCards.transform);
            abandomCards[i].gameObject.transform.localPosition=new Vector3(0,0,0);
            }    
            abandomCards.Clear();
    }
    public IEnumerator CardAdjustPosition()//各个卡片调整自己位置
    {
        yield return new WaitForSeconds(0.2f);
        for (int i = 0; i < haveCards.Count; i++)
        {
            LeanTween.move(haveCards[i].gameObject, haveCards[i].cardAdress, 0.4f);
            
        }

    }

    IEnumerator CardEmpyt()
    {
        GameManager.instance.tips.text = "牌库抽空";
        yield return new WaitForSeconds(0.4f);
        GameManager.instance.tips.text = "";
    }

}
