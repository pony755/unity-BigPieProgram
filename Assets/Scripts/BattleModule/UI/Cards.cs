using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Cards : MonoBehaviour
{
    [Header("Image")]
    public Image cardFrame;  
    public Image cardBase;
    public Image cardImage;

    [Header("Sprite")]
    public Sprite cardFrameSprite;
    public Sprite cardBaseSprite;   
    public Sprite cardSkillSprite;
    public Skill cardSkill;
    public Text cardName;
    public Text cardText;

    public Vector3 cardAdress;//手卡位置
    public Vector3 cardAbandomAdress;//弃牌栏位置
    void Start()
    {
        this.gameObject.GetComponent<Animator>().enabled = false;
        cardAdress.y = 80;
        cardAdress.z = 0;
        CardPosition();
        cardFrame.sprite = cardFrameSprite;
        cardBase.sprite = cardBaseSprite;
        cardImage.sprite = cardSkillSprite;
        cardName.text = cardSkill.skillName;
        cardText.text = cardSkill.description;
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.instance.fightPlayerCards.haveCards.Contains(this))
               CardPosition();
        
    }




    public void ScaleCard()//鼠标进入事件
    {
            if ((GameManager.instance.state == BattleState.PLAYERTURN&&!GameManager.instance.fightPlayerCards.abandomCards.Contains(this) )|| GameManager.instance.state == BattleState.ABANDOMCARD)
            {
                this.transform.SetAsLastSibling();
                LeanTween.move(this.gameObject, new Vector3(cardAdress.x, cardAdress.y + 100f, cardAdress.z), 0.3f);

            }

        
        
    }
    public void DownCard()//只在玩家回合有效，鼠标退出事件
    {
            if ((GameManager.instance.state == BattleState.PLAYERTURN&& !GameManager.instance.fightPlayerCards.abandomCards.Contains(this)) || GameManager.instance.state == BattleState.ABANDOMCARD)
            {
              if(!GameManager.instance.fightPlayerCards.abandomCards.Contains(this))
            {
                this.transform.SetSiblingIndex(GameManager.instance.fightPlayerCards.haveCards.IndexOf(this));
                LeanTween.move(this.gameObject, cardAdress, 0.3f);
            }
                
            }     
 
    }



    public void ClickUseCard()//卡片点击事件
    {
            if (GameManager.instance.state == BattleState.PLAYERTURN&& !GameManager.instance.fightPlayerCards.abandomCards.Contains(this))
            {
                for (int i = 0; i <= GameManager.instance.heroUnit.Count; i++)
                {

                    if (i == GameManager.instance.heroUnit.Count)
                    {
                        StartCoroutine(FalseTips());
                        //预留音效
                        return;
                    }
                    if (GameManager.instance.heroUnit[i].tired == 0)
                        break;
                }
                LeanTween.move(this.gameObject, new Vector3(990f, 200f, 0), 0.3f);
                GameManager.instance.useCard = this;
                GameManager.instance.useSkill = cardSkill;
                if (cardSkill.cardPointUnit)
                {
                    GameManager.instance.tips.text = "选择一名行动方";
                    GameManager.instance.state = BattleState.CARDTURNUNIT;

                }
                else
                {
                    GameManager.instance.turnUnit.Add(GameManager.instance.fightPlayerCards.playerObject.GetComponent<Unit>());
                    StartCoroutine(cardSkill.JudgePlayerSkill());
                }
            }
            if(GameManager.instance.state==BattleState.ABANDOMCARD)
                CardDestory();

       
    }

    IEnumerator FalseTips()//显示tips
    {
        GameManager.instance.tips.text = "所有己方角色处于疲劳";
        yield return new WaitForSeconds(0.3f);
        GameManager.instance.tips.text = "";
    }

    public void OrigenFloatCard()//复原卡片样式
    {
        this.gameObject.transform.localScale = new Vector3(1f,1f,1f);
        cardBase.color = new Color(cardBase.color.r, cardBase.color.g, cardBase.color.b, 1);
        cardImage.color = new Color(cardBase.color.r, cardBase.color.g, cardBase.color.b, 1);
    }

    public void CardDestory()//弃牌
    {

        GameManager.instance.fightPlayerCards.abandomCards.Add(this);
        GameManager.instance.fightPlayerCards.haveCards.Remove(this);
        OrigenFloatCard();
        this.gameObject.transform.SetParent(GameManager.instance.AbandomCardCheck.transform.GetChild(3).transform.GetChild(0).transform.GetChild(0));
        this.cardAbandomAdress.x = 100 + ((GameManager.instance.fightPlayerCards.abandomCards.IndexOf(this) % 5) * 160);
        this.cardAbandomAdress.y = -150 - ((GameManager.instance.fightPlayerCards.abandomCards.IndexOf(this) / 5) * 260);
        //this.cardAbandomAdress.x = 0;
        //this.cardAbandomAdress.y = 0;
        this.cardAbandomAdress.z = 0;
        
        GameManager.instance.AbandomCardCheck.transform.GetChild(3).transform.GetChild(0).transform.GetChild(0).GetComponent<RectTransform>().sizeDelta=new Vector2(0,Mathf.Max(340f,(float)(280+(GameManager.instance.fightPlayerCards.abandomCards.IndexOf(this) / 5) * 260)));

        this.gameObject.GetComponent<Animator>().enabled = false;
        GameManager.instance.AdjustCards = true;
    }

    public void PressAbandomCard()//按住弃牌堆的牌放大
    {
        if (GameManager.instance.fightPlayerCards.abandomCards.Contains(this))
            LeanTween.scale(this.gameObject, new Vector3(1.2f, 1.2f, 1.2f), 0.1f);
         
    }
    public void RealeaseAbandomCard()//按住弃牌堆的牌放大后松开恢复原样
    {
        if (GameManager.instance.fightPlayerCards.abandomCards.Contains(this))
            LeanTween.scale(this.gameObject, new Vector3(1f, 1f, 1f), 0.1f);

    }
    
    public void CardPosition()//卡牌内置位置调整
    {
        if (GameManager.instance.fightPlayerCards.haveCards.Count <= 5)
            cardAdress.x = 450 + GameManager.instance.fightPlayerCards.haveCards.IndexOf(this) * 260;
        else if (GameManager.instance.fightPlayerCards.haveCards.Count <= 7)
            cardAdress.x = 450 + GameManager.instance.fightPlayerCards.haveCards.IndexOf(this) * 170;
        else if (GameManager.instance.fightPlayerCards.haveCards.Count <= 9)
            cardAdress.x = 450 + GameManager.instance.fightPlayerCards.haveCards.IndexOf(this) * 130;
        else if (GameManager.instance.fightPlayerCards.haveCards.Count <= 11)
            cardAdress.x = 450 + GameManager.instance.fightPlayerCards.haveCards.IndexOf(this) * 105;
        else if (GameManager.instance.fightPlayerCards.haveCards.Count <= 13)
            cardAdress.x = 450 + GameManager.instance.fightPlayerCards.haveCards.IndexOf(this) * 88;
        else if (GameManager.instance.fightPlayerCards.haveCards.Count <= 15)
            cardAdress.x = 450 + GameManager.instance.fightPlayerCards.haveCards.IndexOf(this) * 73;
        else if (GameManager.instance.fightPlayerCards.haveCards.Count <= 18)
            cardAdress.x = 450 + GameManager.instance.fightPlayerCards.haveCards.IndexOf(this) * 60;
        else if (GameManager.instance.fightPlayerCards.haveCards.Count <= 22)
            cardAdress.x = 450 + GameManager.instance.fightPlayerCards.haveCards.IndexOf(this) * 50;
        else if (GameManager.instance.fightPlayerCards.haveCards.Count <= 27)
            cardAdress.x = 450 + GameManager.instance.fightPlayerCards.haveCards.IndexOf(this) * 40;
        else if (GameManager.instance.fightPlayerCards.haveCards.Count <= 30)
            cardAdress.x = 440 + GameManager.instance.fightPlayerCards.haveCards.IndexOf(this) * 37;
        else if (GameManager.instance.fightPlayerCards.haveCards.Count <= 36)
            cardAdress.x = 440 + GameManager.instance.fightPlayerCards.haveCards.IndexOf(this) * 30;
        else if (GameManager.instance.fightPlayerCards.haveCards.Count <= 40)
            cardAdress.x = 428 + GameManager.instance.fightPlayerCards.haveCards.IndexOf(this) * 28;
        else
            cardAdress.x = 428 + GameManager.instance.fightPlayerCards.haveCards.IndexOf(this) * 25;
    }


}