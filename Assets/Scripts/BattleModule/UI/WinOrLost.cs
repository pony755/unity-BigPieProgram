using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class WinOrLost : MonoBehaviour
{
    [HideInInspector]public Player player;
    //public GameObject levelUp;

    [HideInInspector] public enum SettleState { None,First,RollSkill,RollCard,Finish}
    public SettleState settleCurrentState;
    public GameObject firstImg;
    public GameObject rollSkillImg;
    public GameObject rollCardImg;



    private void Start()
    {
        settleCurrentState = SettleState.None;
        firstImg.SetActive(false);
        rollSkillImg.SetActive(false);
        rollCardImg.SetActive(false);
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
    private void Update()
    {
        //初始
        if(settleCurrentState == SettleState.None&& !firstImg.activeInHierarchy)
        {
            settleCurrentState = SettleState.First;
            firstImg.SetActive(true);
        }

        //胜利界面
        if (firstImg.GetComponent<firstImg>().winNextSwitch==true&& settleCurrentState == SettleState.First)//胜利界面结算完后进入下一阶段
        {
            settleCurrentState = SettleState.RollSkill;
            firstImg.SetActive(false);
        }

        //roll技能界面
        if(settleCurrentState == SettleState.RollSkill)
        {
            if (CheckRollSkill()!=null && !rollSkillImg.activeInHierarchy)
            {
                rollSkillImg.GetComponent<RollSkillImage>().StartShow(CheckRollSkill());//初始化抽技能并显示
            }
            if (rollSkillImg.GetComponent<RollSkillImage>().nextSwitch == true)
            {
                rollSkillImg.GetComponent<RollSkillImage>().resetRollSkillImage();//一次抽技能结算完后重置状态并关闭
            }
            if(CheckRollSkill()==null&& !rollSkillImg.activeInHierarchy)//当关闭的时候所以角色抽技能已结算完成进入下一阶段
            {
                settleCurrentState = SettleState.RollCard;
            }
        }

        //roll卡牌界面
        if(settleCurrentState == SettleState.RollCard)
        {
            if(GameManager.instance.tempPlayer.GetComponent<FightPlayer>().getCards.Count>0)
            {
                rollCardImg.GetComponent<RollCards>().RollCardShow();
            }
            if(rollCardImg.GetComponent<RollCards>().nextSwitch==true)
            {
                rollCardImg.SetActive(false);
                settleCurrentState = SettleState.Finish;
            }
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
    }//依次检测角色状态，若有角色学会技能则返回该Unit
}
