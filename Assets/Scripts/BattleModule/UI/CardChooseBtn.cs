using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class CardChooseBtn : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Cards card;
    [Header("CardShow")]
    public Image BcardFrame;
    public Image BcardBase;
    public Image BcardImage;
    public Text BcardName;
    public Text BcardQualityText;
    public Text BcardText;
    public bool finish;
    private bool scaleSwitch;

    private void Start()
    {
        finish = false;
        scaleSwitch=false;
        SetCardBtnShow();

    }

    private void Update()
    {
        
        
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (scaleSwitch == false)
        {
            scaleSwitch = true;
            LeanTween.scale(gameObject, new Vector3(1.2f, 1.2f, 1.2f), 0.2f);
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (scaleSwitch == true)
        {
            scaleSwitch = false;
            LeanTween.scale(gameObject, new Vector3(1f, 1f, 1f), 0.2f);
        }
    }

    public void CardClick()
    {
        GameManager.instance.tempPlayer.GetComponent<FightPlayer>().cardCode.Add(AllList.instance.allCardList.IndexOf(card));
        finish=true;
    }
    public void Pass()
    {
        finish = true;
    }
    private void SetCardBtnShow()
    {
        SetBtnQualityText();
        BcardImage.sprite = card.cardSkillSprite;
        BcardName.text =card.cardSkill.skillName;
        SetCardType();
        BcardText.text = card.cardSkill.description;
    }//设置卡牌初始样式
    private void SetBtnQualityText()
    {
        BcardQualityText.text = card.cardQuality.ToString();
        if (card.cardQuality.ToString() =="N")
        {
            BcardQualityText.color = Color.white;
        }
        else if (card.cardQuality.ToString() == "R")
        {
            BcardQualityText.color = Color.blue;
        }
        else if (card.cardQuality.ToString() == "SR")
        {
            BcardQualityText.color = new Color32(255, 181, 0, 255);
        }
        else if (card.cardQuality.ToString() == "UR")
        {
            BcardQualityText.color = Color.magenta;
        }
    }
    private void SetCardType()
    {

        if (card.cardType.ToString() == "AD")
        {
            BcardFrame.sprite = Resources.Load<Sprite>("fightCards/phyframe");
            BcardBase.sprite = Resources.Load<Sprite>("fightCards/fightCard_AD");
        }
        else if (card.cardType.ToString() == "AP")
        {
            BcardFrame.sprite = Resources.Load<Sprite>("fightCards/phyframe");
            BcardBase.sprite = Resources.Load<Sprite>("fightCards/fightCard_AP");
        }
        else if (card.cardType.ToString() == "Spirit")
        {
            BcardFrame.sprite = Resources.Load<Sprite>("fightCards/phyframe");
            BcardBase.sprite = Resources.Load<Sprite>("fightCards/fightCard_Spirit");
        }
        else if (card.cardType.ToString() == "Special")
        {
            BcardFrame.sprite = Resources.Load<Sprite>("fightCards/phyframe");
            BcardBase.sprite = Resources.Load<Sprite>("fightCards/fightCard_Else");
        }
    }
}
