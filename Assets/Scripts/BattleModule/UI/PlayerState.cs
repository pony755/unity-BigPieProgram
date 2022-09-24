using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerState : MonoBehaviour
{
    public Text playerID;
    public Text pAD;
    public Text pAP;
    public Text pAddCard;
    public Text pMaxCard;
    public List<Image> heroImg;
    public List<Image> prepareHeroImg;
    public List<iconImg> itemsImg;
    public GameObject itemShow;
    private FightPlayerInFight Player;
 
    private void Start()
    {
        itemShow.SetActive(false);
    }
    private void Update()
    {
        
    }
   
    public void ClickShowHide()
    {
        if (gameObject.activeSelf)
        {
            ObjectHide();
        }
        else
            ObjectShow();
    }
    private void ObjectShow()
    {
        Player = GameManager.instance.fightPlayer;
        foreach(var u in GameManager.instance.heroUnit)
            u.GetComponent<BoxCollider>().enabled = false;
        foreach (var u in GameManager.instance.enemyUnit)
            u.GetComponent<BoxCollider>().enabled = false;
        playerID.text = Player.PlayerID;
        pAD.text = Player.PlayerAD.ToString();
        pAP.text = Player.PlayerAP.ToString();
        pAddCard.text = Player.addCardNum.ToString();
        pMaxCard.text = Player.maxCard.ToString();
        for (int i = 0; i < GameManager.instance.heroUnit.Count; i++)
        {
            heroImg[i].sprite = GameManager.instance.heroUnit[i].normalSprite;
            heroImg[i].gameObject.SetActive(true);
        }
        for (int i = 0; i < GameManager.instance.heroPreparePrefab.Count; i++)
        {
            prepareHeroImg[i].sprite = GameManager.instance.heroPreparePrefab[i].GetComponent<Unit>().normalSprite;
            prepareHeroImg[i].gameObject.SetActive(true);
        }
        for (int i = 0; i < Player.items.Count; i++)
        {
            itemsImg[i].SetIcon(Player.items[i],itemShow.GetComponent<iconShow>());

        }
        gameObject.SetActive(true);
    }
    private void ObjectHide()
    {
        foreach (var u in GameManager.instance.heroUnit)
            u.GetComponent<BoxCollider>().enabled = true ;
        foreach (var u in GameManager.instance.enemyUnit)
            u.GetComponent<BoxCollider>().enabled = true;
        foreach (var hero in heroImg)
        {
            hero.gameObject.SetActive(false);
        }
        foreach (var hero in prepareHeroImg)
        {
            hero.gameObject.SetActive(false);
        }
        foreach (var icon in itemsImg)
        {
            icon.GetComponent<iconImg>().ResetIcon();

        }
        playerID.text = "";
        pAD.text = "";
        pAP.text = "";
        pAddCard.text = "";
        pMaxCard.text = "";
        gameObject.SetActive(false);
    }


}
