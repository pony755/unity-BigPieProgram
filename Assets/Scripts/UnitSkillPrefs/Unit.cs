using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public Animator anim;//动画
    public BattleHub hub;//状态框

    public bool playerHero;//判断是不是己方角色
    public string unitName;
    public int unitLevel;

    public int tired;
    public int Atk;
    public int Def;

    public int maxHP;
    public int currentHP;

    [Header("技能")]
    public List<Skill> heroSkillList;


    [Header("游戏内数组编号")]
    public int gameCode;//游戏内角色编号
    public void Start()
    {
         anim=GetComponent<Animator>();
    }





    //---------------------------------动画函数---------------------------------------------
    public void Bling()
    {
        anim.Play("bling");
    }
    public void Idle()
    {
        anim.Play("idle");
    }


    public void HubUpdate()
    {
        hub.SetHub(this);
    }

    private void CheckDead()//死亡判断结算函数
    {
        if (currentHP <= 0)
        {
            currentHP = 0;
            this.GetComponent<BoxCollider>().enabled = false;//关闭碰撞体脚本
            if(GameManager.instance.playerUnit.Contains(this))
            {
                GameManager.instance.playerUnit.Remove(this);
            }
            if (GameManager.instance.enemyUnit.Contains(this))
            {
                GameManager.instance.enemyUnit.Remove(this);
            }
            anim.Play("dead");
        }
    }
    private void Settle(Unit turnUnit, Skill skill)
    {
        if(skill.type==skillType.AD)
        {
            Debug.Log(unitName + "受到了" + (skill.finalPoint(turnUnit) - Def) + "点物理伤害");
            currentHP=currentHP-(skill.finalPoint(turnUnit)-Def);

        }
        HubUpdate();
        CheckDead();
    }
    public void skillSettle(Unit turnUnit, Skill skill)//结算技能
    {
        turnUnit.tired=turnUnit.tired+skill.skillTired;
        if(skill.type==skillType.Mix)
        {
            foreach(var o in skill.moreSkill)
            {
                Settle(turnUnit, o);
            }
        }
        else
        {
            Settle(turnUnit, skill);
        }
    }
    //——————————————————鼠标事件————————————————————————————
    private void OnMouseEnter()//进入闪烁
    {
        if(tired==0)
        {
            if (GameManager.instance.state == BattleState.PLAYERTURN && playerHero)//己方玩家回合
                anim.Play("bling");
        }
        


        if (GameManager.instance.state == BattleState.POINTENEMY && !playerHero&&!GameManager.instance.pointUnit.Contains(this))
            anim.Play("bling");
    }
    private void OnMouseExit()//退出回归原样
    {
        if (GameManager.instance.state == BattleState.PLAYERTURN && playerHero)
            anim.Play("idle");

        if (GameManager.instance.state == BattleState.POINTENEMY && !playerHero)
            anim.Play("idle");
    }
    private void OnMouseDown()//点击显示技能栏
    {
        if (tired == 0)
        {
            if (GameManager.instance.state == BattleState.PLAYERTURN && playerHero)
            {
                anim.Play("idle");
                GameManager.instance.SkillShow(gameCode);//传入角色编号
            }
        }
        

        if (GameManager.instance.state == BattleState.POINTENEMY && !playerHero)
        {
            if(!GameManager.instance.pointUnit.Contains(this))//不在列表内，则加入列表
            {
                GameManager.instance.pointUnit.Add(GameManager.instance.enemyUnit[gameCode]);//传入对应敌人预制体
            }
           
        }
        
    }

}
