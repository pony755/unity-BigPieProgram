using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class WinOrLost : MonoBehaviour
{
    [HideInInspector]public Player player;
    //public GameObject levelUp;
    
    
    public GameObject firstImg;
    public GameObject rollSkillImg;

    public Button nextBtn;
    
    
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        nextBtn.onClick.AddListener(delegate () {
            player.ChangeScene("BattleScene");
            });
        
    }

    public void FirstNextBtn()
    {
        ToSkillRoll();
    }

    public void SkillChooseBtn()
    {
        FreshSkillRoll();
    }
    private void ToSkillRoll()
    {
        if (firstImg.activeInHierarchy)
            firstImg.SetActive(false);
        if (CheckRollSkill()!=null)
        {
            rollSkillImg.GetComponent<RollSkillImage>().Show(CheckRollSkill());
         }      
        else
        {
            //ToRollCard();
        }
    }
    private void FreshSkillRoll()
    {
        if (rollSkillImg.activeInHierarchy)
            rollSkillImg.SetActive(false);

            
        if (CheckRollSkill() != null)
        {
            rollSkillImg.GetComponent<RollSkillImage>().Show(CheckRollSkill());
        }
        else
        {
            //ToRollCard();
        }
    }

    private Unit CheckRollSkill()
    {
        Unit unit = null;
        foreach(var p in GameManager.instance.heroUnit)
        {
            if (p.skillRoll.Count > 0)
            {
                unit = p;
                break;
            }
        }
        return unit;
    }
}
