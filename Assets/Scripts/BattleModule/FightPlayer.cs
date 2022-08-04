using System;
using System.Collections.Generic;
using UnityEngine;
using Koubot.Tool;
using System.Collections;

public class FightPlayer : MonoBehaviour
{
    private bool startTakeCardSwitch;//�ж��Ƿ�Ϊ��ʼ�׶γ鿨
    public Unit playerObject;
    [Header("����")]
    public int startCard;
    public int maxCard;
    public int addCardNum;
    public List<Cards> playerCards;//����
    public List<Cards> haveCards;//����
    public List<Cards> abandomCards;//���ƶ�
    [Header("�������")]
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


    //���������������������������������������������������ơ�������������������������������������������������

    public void TakeCard()//��һ����
    {
        if(playerCards.Count == 0)
        {
            StartCoroutine(CardEmpyt());
            return;
        }    
        int index = Koubot.Tool.Random.RandomTool.GenerateRandomInt(0,playerCards.Count-1);
        Debug.Log(index);
        GameObject A = Instantiate(playerCards[index].gameObject, new Vector3(-300, 80, 0), Quaternion.identity, GameManager.instance.CardCanvas.transform);
        haveCards.Add(A.GetComponent<Cards>());//��ȡ��¡��Ľű�       
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
        GameManager.instance.tips.text = "�ƿ��ѿ�";
        yield return new WaitForSeconds(0.2f);
        GameManager.instance.tips.text = "";
    }

}
