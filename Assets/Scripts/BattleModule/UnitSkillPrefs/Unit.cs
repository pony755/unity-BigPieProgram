using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Unit : MonoBehaviour
{
    [HideInInspector]public Animator anim;//动画
    [HideInInspector]public Unit damger;//暂时记录伤害来源
    public Sprite normalSprite;
    public GameObject point;//指向
    public GameObject floatPoint;//伤害
    public GameObject floatSkill;//技能浮动
    public bool playerHero;//判断是不是己方角色
    public string unitName;
    public int unitLevel;

    public int tired;
    public int Atk;
    public int Def;

    public int maxHP;
    public int currentHP;
    public int maxMP;
    public int currentMP;
    public int currentExp=0;
    public int nextExp=50;

    [Header("技能")]
    public List<Skill> heroSkillList;

    [Header("被动技能列表")]
    public List<Skill> passiveTurnEndList;//回合开始时触发
    public List<Skill> passiveTurnStartList;//回合开始时触发
    public List<Skill> passiveAttackList;//攻击触发
    public List<Skill> passiveHitList;//受伤触发
    public List<Skill> passiveDeadList;//死亡触发
    public List<Skill> passiveGameBeginList;//死亡触发

    private void Start()
    {
        anim = GetComponent<Animator>();
        StartCoroutine(SetPassive());

    }

    private void Update()
    {
        if (GameManager.instance. state != BattleState.POINTENEMY || GameManager.instance.state != BattleState.POINTPLAYER || GameManager.instance.state != BattleState.POINTALL)//非这三个阶段，灭图标
            point.SetActive(false);
        if((GameManager.instance.state == BattleState.POINTENEMY&&!playerHero)|| (GameManager.instance.state == BattleState.POINTPLAYER && playerHero)|| GameManager.instance.state == BattleState.POINTALL)
        {
            if(currentHP > 0)
                 point.SetActive(true);
            if ((!GameManager.instance.useSkill.reChoose && GameManager.instance.pointUnit.Contains(this))|| (GameManager.instance.useSkill.noMe&&GameManager.instance.turnUnit[0]==this))
            {
                point.SetActive(false);
            }
        }

        
    }
    IEnumerator SetPassive()
    {
        
        foreach (var p in heroSkillList)
        {
            if (p.passiveType == passiveType.Hit)
            {
                passiveHitList.Add(p);
            }
            else if (p.passiveType == passiveType.Dead)
            {
                passiveDeadList.Add(p);
            }
            else if (p.passiveType == passiveType.Attack)
            {
                passiveAttackList.Add(p);
            }
            else if (p.passiveType == passiveType.GameBegin)
            {
                passiveGameBeginList.Add(p);
            }
            else if (p.passiveType == passiveType.TurnStart)
            {
                passiveTurnStartList.Add(p);
            }
            else if (p.passiveType == passiveType.TurnEnd)
            {
                passiveTurnEndList.Add(p);
            }
        } 
        yield return new WaitForSeconds(0.3f);
        PassiveGameBegin();
    }

    
    IEnumerator Settle(Unit turnUnit, Skill skill)
    {
        if (skill.delayedTurn > 0 && !GameManager.instance.delayedSwitch)
        {

            GameManager.instance.delayedTurn.Add(GameManager.instance.turn + skill.delayedTurn);
            GameManager.instance.delayedTurnUnit.Add(turnUnit);
            GameManager.instance.delayedSkill.Add(skill);
            GameManager.instance.delayedPointUnit.Add(this);
        }
        else
        {
            skill.SkillSettleAD(turnUnit,this);

        }
        yield return null;

    }
   

       
     
    public void skillSettle(Unit turnUnit, Skill skill)//结算技能
    {
        for (int i=0;i< skill.attributeCost.Count;i++)//结算发动代价
        {
            if (skill.attributeCost[i] == heroAttribute.Atk)
                turnUnit.Atk = turnUnit.Atk + skill.skillCost[i];
            else if (skill.attributeCost[i] == heroAttribute.HP)
                turnUnit.currentHP = turnUnit.currentHP + skill.skillCost[i];
        }
        turnUnit.currentMP = turnUnit.currentMP - skill.needMP;
        turnUnit.tired=turnUnit.tired+skill.skillTired;
        
        if (skill.delayedTurn > 0&&!GameManager.instance.delayedSwitch)
        {

            GameManager.instance.delayedTurn.Add(GameManager.instance.turn+skill.delayedTurn);
            GameManager.instance.delayedTurnUnit.Add(turnUnit);
            GameManager.instance.delayedSkill.Add(skill);
            GameManager.instance.delayedPointUnit.Add(this);
        }

        else
        {
            if (skill.type == skillType.Mix)
            {
                foreach (var o in skill.moreSkill)
                {
                    StartCoroutine( Settle(turnUnit, o));
                }
            }
            else
            {
                StartCoroutine(Settle(turnUnit, skill));
            }
        }
    }


    //——————————————————被动事件(绑定到对应动画)————————————————————————————
    public void PassiveGameBegin()
    {
        if (passiveGameBeginList.Count > 0)
        {
            foreach (var o in passiveGameBeginList)
            {
                StartCoroutine(PassiveSettle(o));
            }
        }
    }
    public void PassiveTurnEnd()//回合开始时
    {
        if (passiveTurnEndList.Count > 0)
        {
            foreach (var o in passiveTurnEndList)
            {
                StartCoroutine(PassiveSettle(o));
            }
        }
    }
    public void PassiveTurnStart()//回合开始时
    {
        if (passiveTurnStartList.Count > 0)
        {
            foreach (var o in passiveTurnStartList)
            {
                StartCoroutine(PassiveSettle(o));
            }
        }
    }
    public void PassiveAttack()//攻击时
    {
        if (passiveAttackList.Count > 0)
        {
            foreach (var o in passiveAttackList)
            {
                StartCoroutine(PassiveSettle(o));
            }
        }
    }
    public void PassiveHit()//被打时
    {
        if(passiveHitList.Count>0)
        {
             foreach(var o in passiveHitList)
            {
                StartCoroutine(PassiveSettle(o));
            }
        }
        damger = null;
    }

    public void PassiveDead()//死亡时
    {

        if (passiveDeadList.Count > 0)
        {
            foreach (var o in passiveDeadList)
            {

                StartCoroutine(PassiveSettle(o));

            }
        }
    }
    
    IEnumerator PassiveSettle(Skill o)
    {
        bool Go=true;//判断是否继续运行
        int tempPointNum = o.pointNum;
        List<Unit> tempUnits = new List<Unit>();
        System.Random r = new System.Random();
        if (o.passiveTurn==passiveTurn.E)
        {
            if (!(((GameManager.instance.state == BattleState.ENEMYTURN || GameManager.instance.state == BattleState.ENEMYTURNSTART || GameManager.instance.state == BattleState.ENEMYFINISH) && this.playerHero)
             || (GameManager.instance.state == BattleState.ACTIONFINISH || GameManager.instance.state == BattleState.PLAYERTURNSTART) && !this.playerHero))
                Go = false;
        }
        else if(o.passiveTurn==passiveTurn.M)
        {
            if(!(((GameManager.instance.state == BattleState.ENEMYTURN || GameManager.instance.state == BattleState.ENEMYTURNSTART || GameManager.instance.state == BattleState.ENEMYFINISH) && !this.playerHero)
            || ((GameManager.instance.state == BattleState.ACTIONFINISH || GameManager.instance.state == BattleState.PLAYERTURNSTART) && this.playerHero)))
                Go=false;
        }

        if(Go)
        {
            FloatSkillShow(this, o, new Color32(190, 190, 190, 255));
            if (o.passivePoint == passivePoint.MDamager)
                if (damger != null)
                {
                    
                    damger.skillSettle(this, o);
                }
                    
            else if (o.passivePoint == passivePoint.MMyself)
                this.skillSettle(this, o);
            else if (o.passivePoint == passivePoint.MAllEnemy)
            {
                for (int i = 0; i < GameManager.instance.enemyUnit.Count; i++)
                {
                    GameManager.instance.enemyUnit[i].skillSettle(this, o);
                }
            }
            else if (o.passivePoint == passivePoint.MAllPlayers)
            {
                for (int i = 0; i < GameManager.instance.playerUnit.Count; i++)
                {
                    GameManager.instance.playerUnit[i].skillSettle(this, o);
                }
            }
            else if (o.passivePoint == passivePoint.MEnemiesAuto)
            {

                if (!o.reChoose)
                {
                    if (playerHero && (o.pointNum > GameManager.instance.enemyUnit.Count))
                        tempPointNum = GameManager.instance.enemyUnit.Count;
                    else if (!playerHero && (o.pointNum > GameManager.instance.playerUnit.Count))
                        tempPointNum = GameManager.instance.playerUnit.Count;
                }

                if (playerHero)
                {
                    foreach (var i in GameManager.instance.enemyUnit)
                    {
                        tempUnits.Add(i);
                    }

                }
                else
                {
                    foreach (var i in GameManager.instance.playerUnit)
                    {
                        tempUnits.Add(i);
                    }
                }
                for (int j = 0; j < tempPointNum; j++)
                {
                    int k = r.Next(tempUnits.Count);
                    tempUnits[k].skillSettle(this, o);
                    if (!o.reChoose)
                        tempUnits.Remove(tempUnits[k]);
                    yield return new WaitForSeconds(0.05f);

                }

            }
            else if (o.passivePoint == passivePoint.MPlayersAuto)
            {
                if (!o.reChoose)
                {
                    if (playerHero && (o.pointNum > GameManager.instance.playerUnit.Count))
                        tempPointNum = GameManager.instance.playerUnit.Count;
                    else if (!playerHero && (o.pointNum > GameManager.instance.enemyUnit.Count))
                        tempPointNum = GameManager.instance.enemyUnit.Count;
                }

                if (!playerHero)
                {
                    foreach (var i in GameManager.instance.enemyUnit)
                    {
                        tempUnits.Add(i);
                    }

                }
                else
                {
                    foreach (var i in GameManager.instance.playerUnit)
                    {
                        tempUnits.Add(i);
                    }
                }
                for (int j = 0; j < tempPointNum; j++)
                {
                    int k = r.Next(tempUnits.Count);
                    tempUnits[k].skillSettle(this, o);
                    if (!o.reChoose)
                        tempUnits.Remove(tempUnits[k]);
                    yield return new WaitForSeconds(0.05f);

                }

            }
        }
        
    }

    public void FloatSkillShow(Unit unit, Skill skill, Color color)//技能字样显示函数
    {
        unit.floatSkill.transform.GetChild(0).GetComponent<TMP_Text>().text = skill.skillName;
        unit.floatSkill.transform.GetChild(0).GetComponent<TMP_Text>().color = color;
        Instantiate(unit.floatSkill, unit.transform.position, Quaternion.identity);
    }

    //——————————————————鼠标事件————————————————————————————
    private void OnMouseEnter()//进入选择动画
    {
        if (GameManager.instance.backPanel.activeInHierarchy)
            return;
        if (GameManager.instance.state == BattleState.PLAYERTURN && playerHero)//己方玩家回合
        {
            if (tired == 0)
                anim.Play("choose");
        }
        if (point.activeInHierarchy)
             anim.Play("choose");

        

    }
    private void OnMouseExit()//退出回归原样
    {
        if (point.activeInHierarchy|| GameManager.instance.state == BattleState.PLAYERTURN)
           anim.Play("idle");

    }
    private void OnMouseDown()//点击
    {
        if (GameManager.instance.backPanel.activeInHierarchy)
            return;
        if (tired == 0)
        {
            if ((GameManager.instance.state == BattleState.PLAYERTURN && playerHero))
            {
                anim.Play("idle");
                GameManager.instance.SkillShow(this);//传入角色
            }
        }

        if (point.activeInHierarchy)
        {
            GameManager.instance.pointUnit.Add(this);//传入对应预制体
            if (!GameManager.instance.useSkill.reChoose)
                anim.Play("idle");
        }
            

           

        
    }

}
