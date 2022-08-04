using System;
using System.Collections.Generic;
using UnityEngine;
using Koubot.Tool;
using System.Collections;

public class FightPlayer : MonoBehaviour
{
    private bool startTakeCardSwitch;//判断是否为开始阶段抽卡
    public Unit playerObject;
    [Header("卡组")]
    public int startCard;
    public int maxCard;
    public int addCardNum;
    public List<Cards> playerCards;//卡组
    public List<Cards> haveCards;//手牌
    public List<Cards> abandomCards;//弃牌堆
    [Header("玩家属性")]
    public int playerAtk;
    // Start is called before the first frame update
    void Start()
    {
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
        Debug.Log(index);
        GameObject A = Instantiate(playerCards[index].gameObject, new Vector3(-300, 80, 0), Quaternion.identity, GameManager.instance.CardCanvas.transform);
        haveCards.Add(A.GetComponent<Cards>());//获取克隆体的脚本       
        playerCards.Remove(playerCards[index]);

        if(haveCards.Count == startCard)
            startTakeCardSwitch=false;
        if(!startTakeCardSwitch)
            StartCoroutine(CardAdjustPosition());

    }

    public void ButtonTakeCard()
    {
        for (int i = 0; i < addCardNum; i++)
            TakeCard();
        GameManager.instance.state=BattleState.ACTIONFINISH;
        StartCoroutine(GameManager.instance.ActionFinish());
    }


    IEnumerator CardAdjustPosition()
    {
        yield return new WaitForSeconds(0.2f);
        foreach (var card in haveCards)
            LeanTween.move(card.gameObject, card.cardAdress, 0.4f);
    }

    IEnumerator CardEmpyt()
    {
        GameManager.instance.tips.text = "牌库已空";
        yield return new WaitForSeconds(0.2f);
        GameManager.instance.tips.text = "";
    }

}
