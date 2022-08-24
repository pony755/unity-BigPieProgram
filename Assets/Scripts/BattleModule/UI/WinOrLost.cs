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
    

    
    
    private void Start()
    {
        /*player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        next.onClick.AddListener(delegate () {
            if (player != null)
            {
                if (player.globalStateValue == 0)
                {
                    player.globalStateValue++;
                }
            }
            ChangeScene(); });*/
        
        
    }
    private void ChangeScene()
    {
        Scene scene = SceneManager.GetSceneByName("MapScene");
        SceneManager.MoveGameObjectToScene(player.gameObject, scene);
        SceneManager.UnloadSceneAsync("BattleScene");
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
        {
            rollSkillImg.SetActive(false);
            //点击之后按钮没回弹
        }
            
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
