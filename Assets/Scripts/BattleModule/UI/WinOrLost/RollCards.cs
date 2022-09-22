using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RollCards : MonoBehaviour
{
    public List<CardChooseBtn> cardsBtn;
    public GameObject nextBtn;
    public bool nextSwitch;

    private void Start()
    {
        nextSwitch = false;
        nextBtn.GetComponent<Button>().onClick.AddListener(delegate () {
            nextSwitch = true;
        }); 
        
    }

    private void Update()
    {
        if(CheckFinish()&&nextSwitch==false)
        {
            nextSwitch=true;
        }
    }
    public void RollCardShow()
    {
        foreach (var c in cardsBtn)
            c.gameObject.SetActive(false);
        List<Cards> tempCards=new List<Cards>();
        tempCards= GameManager.instance.fightPlayer.GetComponent<FightPlayerInFight>().RollCards();
       for(int i=0;i<tempCards.Count;i++)
        {
            cardsBtn[i].card=tempCards[i];
            cardsBtn[i].gameObject.SetActive(true);
        }
       gameObject.SetActive(true);
    }

    private bool CheckFinish()
    {
        bool finish=false;
        foreach(var c in cardsBtn)
        {
            if(c.finish==true)
                finish=true;
        }
        return finish;
    }

}
