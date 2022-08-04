using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Cards : MonoBehaviour
{

    public Image cardBase;
    public Sprite cardBaseSprite;
    public Image cardPic;
    public Sprite cardSkillSprite;
    public Skill cardSkill;

    public Vector3 cardAdress;
    void Start()
    {
        cardAdress.y = 80;
        cardAdress.z = 0;
        CardPosition();
        cardBase.sprite = cardBaseSprite;
        cardPic.sprite = cardSkillSprite;
        //LeanTween.move(this.gameObject, cardAdress, 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        CardPosition();
    }




    public void ScaleCard()
    {
        if(GameManager.instance.state==BattleState.PLAYERTURN)
        {
            this.transform.SetAsLastSibling();
            LeanTween.move(this.gameObject, new Vector3(cardAdress.x, cardAdress.y+100f,cardAdress.z), 0.3f);

        }
        
    }
    public void DownCard()
    {
        this.transform.SetSiblingIndex(GameManager.instance.player.haveCards.IndexOf(this));
        LeanTween.move(this.gameObject,cardAdress, 0.3f);
 
    }

    public void CardPosition()
    {
        if (GameManager.instance.player.haveCards.Count <= 5)
            cardAdress.x = 450 + GameManager.instance.player.haveCards.IndexOf(this) * 260;
        else if (GameManager.instance.player.haveCards.Count <= 7)
            cardAdress.x = 450 + GameManager.instance.player.haveCards.IndexOf(this) * 170;
        else if (GameManager.instance.player.haveCards.Count <= 9)
            cardAdress.x = 450 + GameManager.instance.player.haveCards.IndexOf(this) * 130;
        else if (GameManager.instance.player.haveCards.Count <= 11)
            cardAdress.x = 450 + GameManager.instance.player.haveCards.IndexOf(this) * 105;
        else if (GameManager.instance.player.haveCards.Count <= 13)
            cardAdress.x = 450 + GameManager.instance.player.haveCards.IndexOf(this) * 88;
        else if (GameManager.instance.player.haveCards.Count <= 15)
            cardAdress.x = 450 + GameManager.instance.player.haveCards.IndexOf(this) * 73;
        else if (GameManager.instance.player.haveCards.Count <= 18)
            cardAdress.x = 450 + GameManager.instance.player.haveCards.IndexOf(this) * 60;
        else if (GameManager.instance.player.haveCards.Count <= 22)
            cardAdress.x = 450 + GameManager.instance.player.haveCards.IndexOf(this) * 50;
        else if (GameManager.instance.player.haveCards.Count <= 27)
            cardAdress.x = 450 + GameManager.instance.player.haveCards.IndexOf(this) * 40;
        else if (GameManager.instance.player.haveCards.Count <= 30)
            cardAdress.x = 440 + GameManager.instance.player.haveCards.IndexOf(this) * 37;
        else if (GameManager.instance.player.haveCards.Count <= 36)
            cardAdress.x = 440 + GameManager.instance.player.haveCards.IndexOf(this) * 30;
        else if (GameManager.instance.player.haveCards.Count <= 40)
            cardAdress.x = 428 + GameManager.instance.player.haveCards.IndexOf(this) * 28;
        else
            cardAdress.x = 428 + GameManager.instance.player.haveCards.IndexOf(this) * 25;
    }
}
