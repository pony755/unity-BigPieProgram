using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public enum GetCard { T }
public class FightPlayerInFight : MonoBehaviour
{
    private bool startTakeCardSwitch;//判断是否为开始阶段抽卡
    public GameObject playerShield;
    public GameObject playerObject;//玩家替身
    public GameObject cardsObject;//卡牌图片
    public GameObject cardsInCards;//接收克隆的卡牌
    [Header("卡组")]
    public int startCard;
    public int maxCard;
    public int addCardNum;
    public List<Cards> playerCards;//卡组
    public List<Cards> haveCards;//手牌
    public List<Cards> abandomCards;//弃牌堆
    [Header("玩家属性(在inspect面板改变属性不会影响玩家属性，只有通过getset函数才会)")]
    [SerializeField] private string playerID;
    [SerializeField]public string PlayerID
    {
        get { return playerID; }
        set {
            playerID = value;
            playerObject.GetComponent<PlayerObject>().unitName = value;//同步属性
        }
    }
    [SerializeField]private int playerAD;
    public int PlayerAD
    {
        get { return playerAD; }
        set { playerAD = value;
            playerObject.GetComponent<PlayerObject>().AD = value;//同步属性
        }
    }

    [SerializeField] private int playerAP;
    public int PlayerAP
    {
        get { return playerAP; }
        set
        {
            playerAP = value;
            playerObject.GetComponent<PlayerObject>().AP = value;//同步属性
        }
    }
    [Header("状态量")]
    public List<GetCard> getCards;
    [Header("饰品")]
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
    //——————————————————————角色操作——————————————————————————
    public List<Cards> RollCards()//roll三张牌
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

    public void GetItem(int code)//获取饰品
    {
        GameManager.instance.tempPlayer.GetComponent<FightPlayer>().itemsCode.Add(code);
        ItemBase tempItem = AllList.instance.allItemList[code];
        items.Add(tempItem);
        if (tempItem.itemType==ItemBase.ItemType.Disposable)
        {
            tempItem.SettleDisposableItem();
        }
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
                abandomCards[i].gameObject.transform.SetParent(GameManager.instance.fightPlayer.cardsInCards.transform);
            abandomCards[i].gameObject.transform.localPosition=new Vector3(0,0,0);
            }    
            abandomCards.Clear();
    }
    public IEnumerator CardAdjustPosition()//各个卡片调整自己位置
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
        GameManager.instance.tips.text = "牌库抽空";
        yield return new WaitForSeconds(0.4f);
        GameManager.instance.tips.text = "";
    }

}
