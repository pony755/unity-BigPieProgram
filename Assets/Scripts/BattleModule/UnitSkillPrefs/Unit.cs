using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Koubot.Tool;
public class Unit : MonoBehaviour
{
    [HideInInspector]public Animator anim;//动画
    [HideInInspector]public Unit danger;//暂时记录伤害来源
    
    public Sprite normalSprite;
    public GameObject point;//指向
    public GameObject cardPoint;//卡片指向
    public GameObject floatPoint;//伤害
    public GameObject floatSkill;//技能浮动
    public GameObject floatState;//技能浮动
    public bool playerHero;//判断是不是己方角色

    [Header("基础属性")]
    public string unitName;
    public int unitLevel;
    public int nextExp;
    [Header("天")]
    public int AP;
    public int APDef;
    public int maxMP;
    [Header("地")]
    public int AD;
    public int Def;
    public int maxHP;
    [Header("人")]
    public int Spirit;
    public int Critical;
    public int Dodge;

    [Header("状态量")]
    public int tired;
    public int shield;//盾
    public int fragile;//易伤
    public int weakness;//虚弱
    public int shieldDecrease;//削盾
    public int healDecrease;//削恢复
    public int burn;
    public int cold;
    public int poison;
    public int currentHP;
    public int currentMP;
    public int currentExp;
    public int getExp;

    [Header("内置抗性")]
    public int ADDecrease;
    public int ADPrecentDecrease;
    public int APDecrease;
    public int APPrecentDecrease;
    public int BurnDecrease;
    public int BurnPrecentDecrease;
    public int PoisonDecrease;
    public int PoisonPrecentDecrease;
    public int ColdDecrease;
    public int ColdPrecentDecrease;







    [Header("是否为玩家替身")]
    public bool player;

    [Header("技能")]
    public List<Skill> heroSkillList;

    [Header("被动技能列表")]
    public List<Skill> passiveTurnEndList;//回合开始时触发
    public List<Skill> passiveTurnStartList;//回合开始时触发
    public List<Skill> passiveHitList;//受伤触发
    public List<Skill> passiveDeadList;//死亡触发
    public List<Skill> passiveGameBeginList;//死亡触发


    private void Awake()//属性初始化
    {
        anim = GetComponent<Animator>();
        StartCoroutine(SetPassive());
    }
    protected virtual void Start()
    {
        //初始化数据

    }

    protected virtual void Update()
    {
        if (GameManager.instance.state == BattleState.CARDTURNUNIT && playerHero&&tired==0)
            cardPoint.SetActive(true);
        else
            cardPoint.SetActive(false);
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
            if (p.passiveType == PassiveType.Hit)
            {
                passiveHitList.Add(p);
            }
            else if (p.passiveType == PassiveType.Dead)
            {
                passiveDeadList.Add(p);
            }
            else if (p.passiveType == PassiveType.GameBegin)
            {
                passiveGameBeginList.Add(p);
            }
            else if (p.passiveType == PassiveType.TurnStart)
            {
                passiveTurnStartList.Add(p);
            }
            else if (p.passiveType == PassiveType.TurnEnd)
            {
                passiveTurnEndList.Add(p);
            }
        } 
        yield return new WaitForSeconds(0.5f);
        PassiveGameBegin();
    }

    
 
    private void AttributeSettle(Skill skill)//结算技能发动后属性变换
    {
        for (int i = 0; i < skill.attributeCost.Count; i++)
        {
            if (skill.attributeCost[i] == HeroAttribute.AP)
                SingleSettle(ref AP, skill.skillCost[i]);               
            else if (skill.attributeCost[i] == HeroAttribute.APDef)
                SingleSettle(ref APDef, skill.skillCost[i]);
            else if (skill.attributeCost[i] == HeroAttribute.maxMP)
                SingleSettle(ref maxMP, skill.skillCost[i]);
            else if (skill.attributeCost[i] == HeroAttribute.MP)
                SingleSettle(ref currentMP, skill.skillCost[i]);
            else if (skill.attributeCost[i] == HeroAttribute.AD)
                SingleSettle(ref AD, skill.skillCost[i]);
            else if (skill.attributeCost[i] == HeroAttribute.Def)
                SingleSettle(ref Def, skill.skillCost[i]);
            else if (skill.attributeCost[i] == HeroAttribute.maxHP)
                SingleSettle(ref maxHP, skill.skillCost[i]);           
            else if (skill.attributeCost[i] == HeroAttribute.HP)
                SingleSettle(ref currentHP, skill.skillCost[i]);
            else if (skill.attributeCost[i] == HeroAttribute.Spirit)
                SingleSettle(ref Spirit, skill.skillCost[i]);
            else if (skill.attributeCost[i] == HeroAttribute.Critical)
                SingleSettle(ref Critical, skill.skillCost[i]);
            else if (skill.attributeCost[i] == HeroAttribute.Dodge)
                SingleSettle(ref Dodge, skill.skillCost[i]);
            else if (skill.attributeCost[i] == HeroAttribute.Tired)
                SingleSettle(ref tired, skill.skillCost[i]);
            else if (skill.attributeCost[i] == HeroAttribute.fragile)
                SingleSettle(ref fragile, skill.skillCost[i]);
            else if (skill.attributeCost[i] == HeroAttribute.weakness)
                SingleSettle(ref weakness, skill.skillCost[i]);
            else if (skill.attributeCost[i] == HeroAttribute.shieldDecrease)
                SingleSettle(ref shieldDecrease, skill.skillCost[i]);
            else if (skill.attributeCost[i] == HeroAttribute.Burn)
                SingleSettle(ref burn, skill.skillCost[i]);
            else if (skill.attributeCost[i] == HeroAttribute.Cold)
                SingleSettle(ref cold, skill.skillCost[i]);
            else if (skill.attributeCost[i] == HeroAttribute.Poison)
                SingleSettle(ref poison, skill.skillCost[i]);
            else if (skill.attributeCost[i] == HeroAttribute.ADDecrease)
                SingleSettle(ref ADDecrease, skill.skillCost[i]);
            else if (skill.attributeCost[i] == HeroAttribute.ADPrecentDecrease)
                SingleSettle(ref ADPrecentDecrease, skill.skillCost[i]);
            else if (skill.attributeCost[i] == HeroAttribute.APDecrease)
                SingleSettle(ref APDecrease, skill.skillCost[i]);
            else if (skill.attributeCost[i] == HeroAttribute.APPrecentDecrease)
                SingleSettle(ref APPrecentDecrease, skill.skillCost[i]);
            else if (skill.attributeCost[i] == HeroAttribute.BurnDecrease)
                SingleSettle(ref BurnDecrease, skill.skillCost[i]);
            else if (skill.attributeCost[i] == HeroAttribute.BurnPrecentDecrease)
                SingleSettle(ref BurnPrecentDecrease, skill.skillCost[i]);
            else if (skill.attributeCost[i] == HeroAttribute.PoisonDecrease)
                SingleSettle(ref PoisonDecrease, skill.skillCost[i]);
            else if (skill.attributeCost[i] == HeroAttribute.PoisonPrecentDecrease)
                SingleSettle(ref PoisonPrecentDecrease, skill.skillCost[i]);
            else if (skill.attributeCost[i] == HeroAttribute.ColdDecrease)
                SingleSettle(ref ColdDecrease, skill.skillCost[i]);
            else if (skill.attributeCost[i] == HeroAttribute.ColdPrecentDecrease)
                SingleSettle(ref ColdPrecentDecrease, skill.skillCost[i]);

        }
    }

    private void SingleSettle(ref int Attribute,int b)
    {
        Attribute += b;
        if (Attribute < 0)
            Attribute = 0;
        
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
            if (skill.type == SkillType.AD)
                skill.SkillSettleAD(turnUnit, this);
            if (skill.type == SkillType.AP)
                skill.SkillSettleAP(turnUnit, this);
            if (skill.type == SkillType.ReallyDamage)
                skill.SkillSettleReallyDamage(turnUnit, this);
            if (skill.type == SkillType.Heal)
                skill.SkillSettleHeal(turnUnit, this);
            if (skill.type == SkillType.Shield)
                skill.SkillSettleShield(turnUnit, this);
            if (skill.type == SkillType.Burn)
                skill.SkillSettleBurn(turnUnit, this);
            if (skill.type == SkillType.Cold)
                skill.SkillSettleCold(turnUnit, this);
            if (skill.type == SkillType.Poison)
                skill.SkillSettlePoison(turnUnit, this);
            if (skill.type == SkillType.Card)
                skill.SkillSettleCard(turnUnit, this);
            if (skill.type == SkillType.AttributeAdjust)
                skill.SkillSettleAdjust(turnUnit, this);

        }
        yield return null;

    }

    public void SkillSettle(Unit turnUnit, Skill skill)//结算技能
    {

         //结算发动代价
         turnUnit.AttributeSettle(skill);
         turnUnit.currentMP -= skill.needMP;
         turnUnit.tired += skill.skillTired;

               
        //结算是否为延时
        if (skill.delayedTurn > 0&&!GameManager.instance.delayedSwitch)
        {

            GameManager.instance.delayedTurn.Add(GameManager.instance.turn+skill.delayedTurn);
            GameManager.instance.delayedTurnUnit.Add(turnUnit);
            GameManager.instance.delayedSkill.Add(skill);
            GameManager.instance.delayedPointUnit.Add(this);
        }

        else
        {    
            if(GameManager.instance.Probility(Dodge))
            {
                FloatStateShow(this, "闪避", Color.white);
                return;
            }

            if (skill.type == SkillType.Mix)
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
    public void PassiveTurnEnd()//回合结束时
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
    public void PassiveHit()//被打时
    {
        if(passiveHitList.Count>0)
        {
             foreach(var o in passiveHitList)
            {
                StartCoroutine(PassiveSettle(o));
            }
        }
        danger = null;
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
        if (o.passiveTurn==PassiveTurn.E)
        {
            if (!(((GameManager.instance.state == BattleState.ENEMYTURN || GameManager.instance.state == BattleState.ENEMYTURNSTART || GameManager.instance.state == BattleState.ENEMYFINISH) && this.playerHero)
             || (GameManager.instance.state == BattleState.ACTIONFINISH || GameManager.instance.state == BattleState.PLAYERTURNSTART) && !this.playerHero))
                Go = false;
        }
        else if(o.passiveTurn==PassiveTurn.M)
        {
            if(!(((GameManager.instance.state == BattleState.ENEMYTURN || GameManager.instance.state == BattleState.ENEMYTURNSTART || GameManager.instance.state == BattleState.ENEMYFINISH) && !this.playerHero)
            || ((GameManager.instance.state == BattleState.ACTIONFINISH || GameManager.instance.state == BattleState.PLAYERTURNSTART) && this.playerHero)))
                Go=false;
        }

        if(Go)
        {        
            FloatTextShow(this, o.skillName, new Color32(190, 190, 190, 255));

            if (o.passivePoint == PassivePoint.MDamager)
                if (danger != null)
                {             
                    danger.SkillSettle(this, o);
                }                   
            if (o.passivePoint == PassivePoint.MMyself)
                this.SkillSettle(this, o);
            if (o.passivePoint == PassivePoint.MAllEnemy)
            {
                    Debug.Log("sb");
                    if(playerHero)
                    {
                        for (int i = 0; i < GameManager.instance.enemyUnit.Count; i++)
                        {
                            GameManager.instance.enemyUnit[i].SkillSettle(this, o);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < GameManager.instance.playerUnit.Count; i++)
                        {
                            GameManager.instance.playerUnit[i].SkillSettle(this, o);
                        }
                    }
                
            }
            if (o.passivePoint == PassivePoint.MAllPlayers)
            {
                    if (!playerHero)
                    {
                        for (int i = 0; i < GameManager.instance.enemyUnit.Count; i++)
                        {
                            GameManager.instance.enemyUnit[i].SkillSettle(this, o);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < GameManager.instance.playerUnit.Count; i++)
                        {
                            GameManager.instance.playerUnit[i].SkillSettle(this, o);
                        }
                    }

                }
            if (o.passivePoint == PassivePoint.MEnemiesAuto)
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
                    
                    int k = Koubot.Tool.Random.RandomTool.GenerateRandomInt(0, tempUnits.Count-1);
                    tempUnits[k].SkillSettle(this, o);
                    if (!o.reChoose)
                        tempUnits.Remove(tempUnits[k]);
                    yield return new WaitForSeconds(0.05f);

                }

            }
            if (o.passivePoint == PassivePoint.MPlayersAuto)
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
                    int k = Koubot.Tool.Random.RandomTool.GenerateRandomInt(0, tempUnits.Count - 1);
                    tempUnits[k].SkillSettle(this, o);
                    if (!o.reChoose)
                        tempUnits.Remove(tempUnits[k]);
                    yield return new WaitForSeconds(0.05f);

                }

            }
        }
        
    }


    //——————————————————————————字样浮动————————————————————————————

    public void FloatPointShow(int number,Color color)
    {
        floatPoint.transform.GetChild(0).GetComponent<TMP_Text>().text = number.ToString();
        floatPoint.transform.GetChild(0).GetComponent<TMP_Text>().color = color;
        Instantiate(floatPoint, this.transform.position + new Vector3(0, 1, 0), Quaternion.identity);
    }

    public void FloatTextShow(Unit unit, string text, Color color)//技能字样显示函数
    {
        unit.floatSkill.transform.GetChild(0).GetComponent<TMP_Text>().text = text;
        unit.floatSkill.transform.GetChild(0).GetComponent<TMP_Text>().color = color;
        Instantiate(unit.floatSkill, unit.transform.position, Quaternion.identity);
    }
    public void FloatStateShow(Unit unit, string text, Color color)//技能字样显示函数
    {
        unit.floatState.transform.GetChild(0).GetComponent<TMP_Text>().text = text;
        unit.floatState.transform.GetChild(0).GetComponent<TMP_Text>().color = color;
        Instantiate(unit.floatState, this.transform.position + new Vector3(0, 1, 0), Quaternion.identity);
    }

    //——————————————————————————角色升级————————————————————————————————

    public virtual void LevelUp2()
    {
        return;
    }
    public virtual void LevelUp3()
    {
        return;
    }
    public virtual void LevelUp4()
    {
        return;
    }
    public virtual void LevelUp5()
    {
        return;
    }
    public virtual void LevelUp6()
    {
        return;
    }
    public virtual void LevelUp7()
    {
        return;
    }
    public virtual void LevelUp8()
    {
        return;
    }
    public virtual void LevelUp9()
    {
        return;
    }
    public virtual void LevelUp10()
    {
        return;
    }
    public virtual void LevelUp11()
    {
        return;
    }
    public virtual void LevelUp12()
    {
        return;
    }
    public virtual void LevelUp13()
    {
        return;
    }
    public virtual void LevelUp14()
    {
        return;
    }
    public virtual void LevelUp15()
    {
        return;
    }
    public virtual void Awaking()
    {
        return;
    }
    //——————————————————技能计算函数——————————————————————————
    public int Decrease(int final,int decrease,float decreasePrecent)
    {
        int a = (int)((final - decrease) * (float)(100 - decreasePrecent) / 100);
        if (a < 0)
            a = 0;
        return a;
    }

    //——————————————————状态函数——————————————————————————
    public void BurnDamage()
    {
        if (burn == 0)
            return;
        currentHP -= Decrease(burn,BurnDecrease,BurnPrecentDecrease);
        FloatPointShow(Decrease(burn, BurnDecrease, BurnPrecentDecrease), new Color32(231, 115, 49, 255));
        burn -= 3;
        if(burn < 0)
            burn = 0;
    }
    public void PoisonDamage()
    {
        if (poison == 0)
            return;
        currentHP -= Decrease(poison, PoisonDecrease,PoisonPrecentDecrease);
        FloatPointShow(Decrease(poison, PoisonDecrease, PoisonPrecentDecrease), new Color32(157,207,73, 255));
        poison -= 2;
        if (poison < 0)
            poison = 0;
    }
    public void ColdDecreaseDamage(ref int damage)
    {
        if (cold == 0)
            return;
        damage-= Decrease(cold, ColdDecrease, ColdPrecentDecrease);
        Debug.Log(damage);
        FloatStateShow(this,"冻僵",new Color32(97,198,236,255));
        cold -= 5;
        if (cold < 0)
            cold = 0;
    }





    //——————————————————鼠标事件————————————————————————————
    private void OnMouseEnter()//进入选择动画
    {
        if (GameManager.instance.backPanel.activeInHierarchy|| GameManager.instance.AbandomCardCheck.activeInHierarchy)
            return;
        if (GameManager.instance.state == BattleState.PLAYERTURN && playerHero)//己方玩家回合
        {
            if (tired == 0)
                anim.Play("choose");
        }
        if (point.activeInHierarchy|| cardPoint.activeInHierarchy)
             anim.Play("choose");

        

    }
    private void OnMouseExit()//退出回归原样
    {
        if (point.activeInHierarchy|| GameManager.instance.state == BattleState.PLAYERTURN)
           anim.Play("idle");
        if (cardPoint.activeInHierarchy || GameManager.instance.state == BattleState.PLAYERTURN)
            anim.Play("idle");

    }
    private void OnMouseDown()//点击
    {
        if (GameManager.instance.backPanel.activeInHierarchy || GameManager.instance.AbandomCardCheck.activeInHierarchy)
            return;
        if (tired == 0)
        {
            if ((GameManager.instance.state == BattleState.PLAYERTURN && playerHero))
            {
                anim.Play("idle");
                GameManager.instance.SkillShow(this);//传入角色
                GameManager.instance.CardCanvas.SetActive(false);
            }
        }

        if (point.activeInHierarchy)
        {
            GameManager.instance.pointUnit.Add(this);//传入对应预制体
            if (!GameManager.instance.useSkill.reChoose)
                anim.Play("idle");
        }
        if (cardPoint.activeInHierarchy)
        {
            GameManager.instance.turnUnit.Add(this);//传入对应预制体
                anim.Play("idle");
            StartCoroutine( GameManager.instance.useSkill.JudgePlayerSkill());
        }

    }

}
