using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public enum GetCard { T }
public class FightPlayerInFight : MonoBehaviour
{
    private bool startTakeCardSwitch;//�ж��Ƿ�Ϊ��ʼ�׶γ鿨
    public GameObject playerShield;
    public GameObject playerObject;//�������
    public GameObject cardsObject;//����ͼƬ
    public GameObject cardsInCards;//���տ�¡�Ŀ���
    [Header("����")]
    public int startCard;
    public int maxCard;
    public int addCardNum;
    public List<Cards> playerCards;//����
    public List<Cards> haveCards;//����
    public List<Cards> abandomCards;//���ƶ�
    [Header("�������(��inspect���ı����Բ���Ӱ��������ԣ�ֻ��ͨ��getset�����Ż�)")]
    [SerializeField] private string playerID;
    [SerializeField]public string PlayerID
    {
        get { return playerID; }
        set {
            playerID = value;
            playerObject.GetComponent<PlayerObject>().unitName = value;//ͬ������
        }
    }
    [SerializeField]private int playerAD;
    public int PlayerAD
    {
        get { return playerAD; }
        set { playerAD = value;
            playerObject.GetComponent<PlayerObject>().AD = value;//ͬ������
        }
    }

    [SerializeField] private int playerAP;
    public int PlayerAP
    {
        get { return playerAP; }
        set
        {
            playerAP = value;
            playerObject.GetComponent<PlayerObject>().AP = value;//ͬ������
        }
    }
    [Header("״̬��")]
    public List<GetCard> getCards;
    [Header("��Ʒ")]
    public List<ItemBase> items;
    // Start is called before the first frame update
    void Start()
    {
        startCard =GameManager.instance. tempPlayer.GetComponent<FightPlayer>().PstartCard;
        maxCard = GameManager.instance.tempPlayer.GetComponent<FightPlayer>().PmaxCard;
        addCardNum = GameManager.instance.tempPlayer.GetComponent<FightPlayer>().PaddCardNum;
        PlayerID = GameManager.instance.tempPlayer.GetComponent<FightPlayer>().playerID;
        PlayerAD = GameManager.instance.tempPlayer.GetComponent<FightPlayer>().AD;
        PlayerAP = GameManager.instance.tempPlayer.GetComponent<FightPlayer>().AP;
        foreach (var c in GameManager.instance.tempPlayer.GetComponent<FightPlayer>().itemsCode)
        {
            items.Add(AllList.instance.allItemList[c]);
        }
        foreach (var cardIndex in GameManager.instance.tempPlayer.GetComponent<FightPlayer>().cardCode)
        {
            GameObject A = Instantiate(AllList.instance.allCardList[cardIndex].gameObject,new Vector3(0, 0, 0), Quaternion.identity, cardsInCards.transform);
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
        if(GameManager.instance.fightPlayer.playerObject.GetComponent<Unit>().shield > 0)
        {
            playerShield.SetActive(true);
            playerShield.transform.GetChild(0).GetComponent<Text>().text = GameManager.instance.fightPlayer.playerObject.GetComponent<Unit>().shield.ToString();
        }
        else
            playerShield.SetActive(false);
    }
    //����������������������������������������������ɫ��������������������������������������������������������
    public List<Cards> RollCards()//roll������
    {
        List<Cards> tempCards = new List<Cards>();
        if (getCards[0] == GetCard.T)
        {
            tempCards.Add(AllList.instance.allCardList[0]);
            tempCards.Add(AllList.instance.allCardList[0]);
            tempCards.Add(AllList.instance.allCardList[0]);
        }
        getCards.Remove(getCards[0]);
        return tempCards;
    }

    public void GetItem(int code)//��ȡ��Ʒ
    {
        GameManager.instance.tempPlayer.GetComponent<FightPlayer>().itemsCode.Add(code);
        ItemBase tempItem = AllList.instance.allItemList[code];
        items.Add(tempItem);
        if (tempItem.itemType==ItemBase.ItemType.Disposable)
        {
            tempItem.SettleDisposableItem();
        }
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
                abandomCards[i].gameObject.transform.SetParent(GameManager.instance.fightPlayer.cardsInCards.transform);
            abandomCards[i].gameObject.transform.localPosition=new Vector3(0,0,0);
            }    
            abandomCards.Clear();
    }
    public IEnumerator CardAdjustPosition()//������Ƭ�����Լ�λ��
    {
        yield return new WaitForSeconds(0.2f);
        for (int i = 0; i < haveCards.Count; i++)
        {
            if(GameManager.instance.useCard!= haveCards[i])
            {
                LeanTween.scale(haveCards[i].gameObject, new Vector3(1, 1, 1), 0.2f);
                LeanTween.move(haveCards[i].gameObject, haveCards[i].cardAdress, 0.4f);
            }
               
        }

    }

    IEnumerator CardEmpyt()
    {
        GameManager.instance.tips.text = "�ƿ���";
        yield return new WaitForSeconds(0.4f);
        GameManager.instance.tips.text = "";
    }

}
