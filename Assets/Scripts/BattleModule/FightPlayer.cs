using System;
using System.Collections.Generic;
using UnityEngine;
using Koubot.Tool;
using System.Collections;

public class FightPlayer : MonoBehaviour
{
    private bool startTakeCardSwitch;//�ж��Ƿ�Ϊ��ʼ�׶γ鿨
    public Unit playerObject;
    public GameObject cardsObject;
    public GameObject cardsInCards;//���տ�¡�Ŀ���
    [Header("����")]
    public int startCard;
    public int maxCard;
    public int addCardNum;
    public List<Cards> playerCardsPrefabs;//����
    public List<Cards> playerCards;//����
    public List<Cards> haveCards;//����
    public List<Cards> abandomCards;//���ƶ�
    [Header("�������")]
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


    //���������������������������������������������������ơ�������������������������������������������������

    public void TakeCard()//��һ����
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
            startTakeCardSwitch=false;//���ֳ�����ϣ���ʼ���Ƶı�־λ��false
        if (!startTakeCardSwitch)
            GameManager.instance.AdjustCards = true;//�������Ƶı�־λ

    }

    public void ButtonTakeCard()//���ư�ť
    {
        for (int i = 0; i < addCardNum; i++)
            TakeCard();
        GameManager.instance.state=BattleState.ACTIONFINISH;
        StartCoroutine(GameManager.instance.ActionFinish());
    }

    public void ResetCards()//ϴ���ƶѵ��ƻ����ƶ�
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
    public IEnumerator CardAdjustPosition()//������Ƭ�����Լ�λ��
    {
        yield return new WaitForSeconds(0.2f);
        for (int i = 0; i < haveCards.Count; i++)
        {
            LeanTween.move(haveCards[i].gameObject, haveCards[i].cardAdress, 0.4f);
            
        }

    }

    IEnumerator CardEmpyt()
    {
        GameManager.instance.tips.text = "�ƿ���";
        yield return new WaitForSeconds(0.4f);
        GameManager.instance.tips.text = "";
    }

}
