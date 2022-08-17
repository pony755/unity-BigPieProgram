using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

public enum SkillRoll {L1,L2,L3,M1,M2,M3,H1,H2,H3}
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


    [Header("角色名")]
    public string unitName;
    [Header("基础属性")]
    public int skillNum;
    public int unitLevel;
    public int maxLevel;
    public int[] nextExp;
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

    [Header("游戏内状态量")]
    public int currentHP;
    public int currentExp;
    

    [Header("战斗状态量(每局战斗都刷新成默认值,不需要保存读取)")]
    public int tired;
    public int sneer;//嘲讽
    public int shield;//盾
    public int fragile;//易伤
    public int weakness;//虚弱
    public int shieldDecrease;//削盾
    public int healDecrease;//削恢复
    public int burn;
    public int cold;
    public int poison;   
    public int currentMP;
    public int getExp;
    public List<SkillRoll> skillRoll;
    
    

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

    [Header("技能")]
    public List<Skill> heroSkillList;

    [Header("技能池")]
    public List<Skill> currencyLSkillList;
    public List<Skill> currencyMSkillList;
    public List<Skill> currencyHSkillList;
    public List<Skill> exclusiveLSkillList;
    public List<Skill> exclusiveMSkillList;
    public List<Skill> exclusiveHSkillList;

    //[Header("技能编号列表")]
    [HideInInspector] public List<int> heroSkillListCode;

    //[Header("战斗技能池编号(存储池子情况")]
    [HideInInspector] public List<int> currencyFightLSkillList;
    [HideInInspector] public List<int> currencyFightMSkillList;
    [HideInInspector] public List<int> currencyFightHSkillList;
    [HideInInspector] public List<int> exclusiveFightLSkillList;
    [HideInInspector] public List<int> exclusiveFightMSkillList;
    [HideInInspector] public List<int> exclusiveFightHSkillList;
    [Header("是否为玩家替身")]
    public bool player;

    [Header("被动技能列表")]
    public List<Skill> passiveTurnEndList;//回合开始时触发
    public List<Skill> passiveTurnStartList;//回合开始时触发
    public List<Skill> passiveHitList;//受伤触发
    public List<Skill> passiveDeadList;//死亡触发
    public List<Skill> passiveGameBeginList;//死亡触发



    [System.Serializable]
    public class SaveUnitData
    {
        public int skillNum;
        public int unitLevel;
        public int nextExp;

        public int AP;
        public int APDef;
        public int maxMP;

        public int AD;
        public int Def;
        public int maxHP;

        public int Spirit;
        public int Critical;
        public int Dodge;


        public int currentHP;
        public int currentExp;


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

        public List<int> currencyFightLSkillList;
        public List<int> currencyFightMSkillList;
        public List<int> currencyFightHSkillList;
        public List<int> exclusiveFightLSkillList;
        public List<int> exclusiveFightMSkillList;
        public List<int> exclusiveFightHSkillList;
        public List<int> heroSkillListCode;
    }
    protected virtual void Awake()//属性初始化
    {
        //初始化数据
        if (playerHero)
        {
            UnitLoad();
            SetSkill();
        }
                      
    }
    protected virtual void Start()
    {
        nextExp = new int[maxLevel + 1];
        nextExp[1] = 100;
        for (int i = 2; i < maxLevel + 1; i++)
        {
            nextExp[i] = Mathf.RoundToInt(nextExp[i - 1] * 1.1f);
        }
        currentMP = maxMP;
        anim = GetComponent<Animator>();
        StartCoroutine(SetPassive());
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
            if(!playerHero&&SneerJudge()>0&&sneer==0)
            {
                point.SetActive(false);
            }
        }
        if (currentHP > maxHP)
        {
            currentHP = maxHP;
        }
        if (currentHP <= 0)
        {
            currentHP = 0;
            anim.Play("dead");
            Invoke("CheckDead", 0.2f);
        }
    }
    private void CheckDead()//死亡判断结算函数
    {
        if (currentHP == 0)
        {
            GetComponent<BoxCollider>().enabled = false;//关闭碰撞体脚本
            if (GameManager.instance.heroUnit.Contains(this))
            {
                GameManager.instance.heroUnit.Remove(this);
            }
            if (GameManager.instance.enemyUnit.Contains(this))
            {
                GameManager.instance.enemyUnit.Remove(this);
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
            else if (skill.attributeCost[i] == HeroAttribute.Sneer)
                SingleSettle(ref sneer, skill.skillCost[i]);

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
                skill.SkillSettleCard(turnUnit);
            if (skill.type == SkillType.AbandomCard)
                skill.SkillSettleAbandomCard(turnUnit);
            if (skill.type == SkillType.AttributeAdjust)
                skill.SkillSettleAdjust(turnUnit, this);
            if (skill.type == SkillType.Excharge)
                skill.SkillSettleExchange(turnUnit, this);
        }
        
        yield return null;

    }

    public void SkillCost(Skill skill)
    {
        //结算发动代价
        AttributeSettle(skill);
        currentMP -= skill.needMP;
        tired += skill.skillTired;
    }

    public void SkillSettle(Unit turnUnit, Skill skill)//结算技能
    {

        turnUnit.SkillCost(skill);    
        if(skill.onlyOne)
        {
            skill.SkillRemove(turnUnit);
        }
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
                StartCoroutine(Settle(turnUnit, skill));
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
        if (currentMP < o.needMP)
        {
            Debug.Log("被动 " + o.skillName + " 所需MP不足");
        }
        else if (GameManager.instance.Probility(o.precent))
        {
            Debug.Log(o.skillName + " 发动失败");
            SkillCost(o);

        }     
        else
        {
            bool Go = true;//判断是否继续运行
            int tempPointNum = o.pointNum;
            List<Unit> tempUnits = new List<Unit>();
            if (o.passiveTurn == PassiveTurn.E)
            {
                if (!(((GameManager.instance.state == BattleState.ENEMYTURN || GameManager.instance.state == BattleState.ENEMYTURNSTART || GameManager.instance.state == BattleState.ENEMYFINISH) && this.playerHero)
                 || (GameManager.instance.state == BattleState.ACTIONFINISH || GameManager.instance.state == BattleState.PLAYERTURNSTART) && !this.playerHero))
                    Go = false;
            }
            else if (o.passiveTurn == PassiveTurn.M)
            {
                if (!(((GameManager.instance.state == BattleState.ENEMYTURN || GameManager.instance.state == BattleState.ENEMYTURNSTART || GameManager.instance.state == BattleState.ENEMYFINISH) && !this.playerHero)
                || ((GameManager.instance.state == BattleState.ACTIONFINISH || GameManager.instance.state == BattleState.PLAYERTURNSTART) && this.playerHero)))
                    Go = false;
            }

            if (Go)
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
                    if (playerHero)
                    {
                        for (int i = 0; i < GameManager.instance.enemyUnit.Count; i++)
                        {
                            GameManager.instance.enemyUnit[i].SkillSettle(this, o);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < GameManager.instance.heroUnit.Count; i++)
                        {
                            GameManager.instance.heroUnit[i].SkillSettle(this, o);
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
                        for (int i = 0; i < GameManager.instance.heroUnit.Count; i++)
                        {
                            GameManager.instance.heroUnit[i].SkillSettle(this, o);
                        }
                    }

                }
                if (o.passivePoint == PassivePoint.MEnemiesAuto)
                {

                    if (!o.reChoose)
                    {
                        if (playerHero && (o.pointNum > GameManager.instance.enemyUnit.Count))
                            tempPointNum = GameManager.instance.enemyUnit.Count;
                        else if (!playerHero && (o.pointNum > GameManager.instance.heroUnit.Count))
                            tempPointNum = GameManager.instance.heroUnit.Count;
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
                        foreach (var i in GameManager.instance.heroUnit)
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
                if (o.passivePoint == PassivePoint.MPlayersAuto)
                {
                    if (!o.reChoose)
                    {
                        if (playerHero && (o.pointNum > GameManager.instance.heroUnit.Count))
                            tempPointNum = GameManager.instance.heroUnit.Count;
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
                        foreach (var i in GameManager.instance.heroUnit)
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
       
        
    }


    //——————————————————————————字样浮动————————————————————————————

    public void FloatPointShow(int number,Color color)
    {
        if(this.player)
        {
            return;
        }
        floatPoint.transform.GetChild(0).GetComponent<TMP_Text>().text = number.ToString();
        floatPoint.transform.GetChild(0).GetComponent<TMP_Text>().color = color;
        Instantiate(floatPoint, this.transform.position + new Vector3(0, 1, 0), Quaternion.identity);
    }

    public void FloatTextShow(Unit unit, string text, Color color)//技能字样显示函数
    {
        if (this.player)
        {
            return;
        }
        unit.floatSkill.transform.GetChild(0).GetComponent<TMP_Text>().text = text;
        unit.floatSkill.transform.GetChild(0).GetComponent<TMP_Text>().color = color;
        Instantiate(unit.floatSkill, unit.transform.position, Quaternion.identity);
    }
    public void FloatStateShow(Unit unit, string text, Color color)//技能字样显示函数
    {
        if (player)
        {
            return;
        }
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
    //——————————————————roll技能函数——————————————————————————

    public List<Skill> SkillRollList(SkillRoll a)
    {
        List<Skill> tempList = new List<Skill>();
    

        return tempList;
    }
    /*protected Skill SingleSkillRoll(ref List<Skill> aList,int aPrecent, ref List<Skill> bList, int bPrecent)
    {
        Skill tempSkill=new Skill();
        if(Koubot.Tool.Random.RandomTool.GenerateRandomInt(0,99)<aPrecent)
        {
            tempSkill = Koubot.Tool.Random.RandomTool.RandomGetOne(aList);
            aList.Remove(tempSkill);
        }






        return tempSkill;
    }*/
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

    public void TurnBeginSettle()//回合开始状态处理
    {
        if (shield > 0)
            shield -= 5;
        if(shield <= 0)
            shield = 0;
    }
    public void TurnEndSettle()
    {
        if (burn > 0)
            burn -= 2;
        if(burn <= 0)
            burn = 0;

        if(cold > 0)
            cold -= 3;
        if (cold <= 0)
            cold = 0;

        if(sneer>0)
            sneer -= 1;
    }

   public int SneerJudge()//判断己方队伍有没有人嘲讽
    {
        int sneerNum=0;
        if(playerHero)
        {
            foreach(var p in GameManager.instance.heroUnit)
            {
                if(p.sneer>0)
                {
                    sneerNum+=1;
                    break;
                }
            }
        }
        else if (!playerHero)
        {
            foreach (var p in GameManager.instance.enemyUnit)
            {
                if (p.sneer > 0)
                {
                    sneerNum +=1;
                    break;
                }
            }
        }
        return sneerNum;
    }//判断队伍内有没有嘲讽

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



    //—————————————————————————初始化———————————————————————————
    private void SetSkill()
    {
        //先清空原来预设的初始技能，再读取本地数据的技能
        heroSkillList.Clear();
        foreach (var code in heroSkillListCode)
            heroSkillList.Add(GameManager.instance.allListObject.GetComponent<AllList>().allSkillList[code]);

    }

    //——————————————————————————存取删————————————————————————————————

    /*string GetSaveNumber()
    {
        string saveNumber;
        string locatorName = "SaveLocator.sav";
        saveNumber = File.ReadAllText(Path.Combine(Application.persistentDataPath, locatorName));
        return saveNumber;
    }*/
    public void UnitSave()//创建存档
    {
        string unitSaveName = "Save_BattleHero_" + unitName + ".sav";
        SaveUnitData saveData = new SaveUnitData
        {
        skillNum = skillNum,
        unitLevel = unitLevel,


        AP= AP,
        APDef= APDef,
        maxMP= maxMP,

        AD= AD,
        Def= Def,
        maxHP= maxHP,

        Spirit= Spirit,
        Critical= Critical,
        Dodge= Dodge,

        currentHP= currentHP,
        currentExp= currentExp,

        ADDecrease = ADDecrease,
        ADPrecentDecrease= ADPrecentDecrease,
        APDecrease= APDecrease,
        APPrecentDecrease= APPrecentDecrease,
        BurnDecrease= BurnDecrease,
        BurnPrecentDecrease= BurnPrecentDecrease,
        PoisonDecrease= PoisonDecrease,
        PoisonPrecentDecrease= PoisonPrecentDecrease,
        ColdDecrease = ColdDecrease,
        ColdPrecentDecrease= ColdPrecentDecrease,
        currencyFightLSkillList= currencyFightLSkillList,
        currencyFightMSkillList= currencyFightMSkillList,
        currencyFightHSkillList=currencyFightHSkillList,
        exclusiveFightLSkillList= exclusiveFightLSkillList,
        exclusiveFightMSkillList= exclusiveFightMSkillList,
        exclusiveFightHSkillList= exclusiveFightHSkillList,
        heroSkillListCode = heroSkillListCode
        };

        SaveSystem.Save(unitSaveName, saveData);
    }

    private void UnitLoad()//读取存档
    {
        string saveUnitName = "Save_BattleHero_" + unitName + ".sav";
        SaveUnitData saveData;
        if (File.Exists(Path.Combine(Application.persistentDataPath, saveUnitName)))
        {
            saveData = SaveSystem.Load<SaveUnitData>(saveUnitName);
            skillNum = saveData.skillNum;
            unitLevel = saveData.unitLevel;


            AP = saveData.AP;
            APDef = saveData.APDef;
            maxMP = saveData.maxMP;

            AD = saveData.AD;
            Def = saveData.Def;
            maxHP = saveData.maxHP;

            Spirit = saveData.Spirit;
            Critical = saveData.Critical;
            Dodge = saveData.Dodge;

            currentHP = saveData.currentHP;
            currentExp = saveData.currentExp;

            ADDecrease = saveData.ADDecrease;
            ADPrecentDecrease = saveData.ADPrecentDecrease;
            APDecrease = saveData.APDecrease;
            APPrecentDecrease = saveData.APPrecentDecrease;
            BurnDecrease = saveData.BurnDecrease;
            BurnPrecentDecrease = saveData.BurnPrecentDecrease;
            PoisonDecrease = saveData.PoisonDecrease;
            PoisonPrecentDecrease = saveData.PoisonPrecentDecrease;
            ColdDecrease = saveData.ColdDecrease;
            ColdPrecentDecrease = saveData.ColdPrecentDecrease;
            currencyFightLSkillList = saveData.currencyFightLSkillList;
            currencyFightMSkillList = saveData.currencyFightMSkillList;
            currencyFightHSkillList = saveData.currencyFightHSkillList;
            exclusiveFightLSkillList = saveData.exclusiveFightLSkillList;
            exclusiveFightMSkillList = saveData.exclusiveFightMSkillList;
            exclusiveFightHSkillList = saveData.exclusiveFightHSkillList;
            heroSkillListCode = saveData.heroSkillListCode;
        }
        else//无存档时，给战斗技能池和技能编好号赋值后再创建存档
        {
            //先对战斗池子的各编好列表初始化,再存入初始数据
            SetAllSkillList();
            UnitSave();         
        }

    }

    private void SetFightSkillList(List<Skill> tempSkills,List<int> tempCodes)//将技能预设列表转换成编号列表
    {
        foreach(var s in tempSkills)
        {
            int Code = GameManager.instance.allListObject.GetComponent<AllList>().allSkillList.IndexOf(s);
            if(!heroSkillListCode.Contains(Code))
                 tempCodes.Add(Code);
        }
            
    }
    private void SetAllSkillList()
    {
        //先存已有技能进编号列表
        foreach (var s in heroSkillList)
           heroSkillListCode.Add(GameManager.instance.allListObject.GetComponent<AllList>().allSkillList.IndexOf(s));
        SetFightSkillList(currencyLSkillList,currencyFightLSkillList);
        SetFightSkillList(currencyMSkillList, currencyFightMSkillList);
        SetFightSkillList(currencyHSkillList, currencyFightHSkillList);
        SetFightSkillList(exclusiveLSkillList, exclusiveFightLSkillList);
        SetFightSkillList(exclusiveMSkillList, exclusiveFightMSkillList);
        SetFightSkillList(exclusiveHSkillList, exclusiveFightHSkillList);
    }
    public void UnitDelete()//删除存档
    {
        string saveUnitName = "Save_BattleHero_" + unitName + ".sav";
        SaveSystem.Delete(saveUnitName);
    }

    public void FightFinishLoad() //战斗后读取存档数据(除了血和目前经验值)便于对数据调整
    {
        string saveUnitName = "Save_BattleHero_" + unitName + ".sav";
        SaveUnitData saveData;
        if (File.Exists(Path.Combine(Application.persistentDataPath, saveUnitName)))
        {
            saveData = SaveSystem.Load<SaveUnitData>(saveUnitName);
            skillNum = saveData.skillNum;
            unitLevel = saveData.unitLevel;


            AP = saveData.AP;
            APDef = saveData.APDef;
            maxMP = saveData.maxMP;

            AD = saveData.AD;
            Def = saveData.Def;
            maxHP = saveData.maxHP;

            Spirit = saveData.Spirit;
            Critical = saveData.Critical;
            Dodge = saveData.Dodge;


            ADDecrease = saveData.ADDecrease;
            ADPrecentDecrease = saveData.ADPrecentDecrease;
            APDecrease = saveData.APDecrease;
            APPrecentDecrease = saveData.APPrecentDecrease;
            BurnDecrease = saveData.BurnDecrease;
            BurnPrecentDecrease = saveData.BurnPrecentDecrease;
            PoisonDecrease = saveData.PoisonDecrease;
            PoisonPrecentDecrease = saveData.PoisonPrecentDecrease;
            ColdDecrease = saveData.ColdDecrease;
            ColdPrecentDecrease = saveData.ColdPrecentDecrease;
            currencyFightLSkillList = saveData.currencyFightLSkillList;
            currencyFightMSkillList = saveData.currencyFightMSkillList;
            currencyFightHSkillList = saveData.currencyFightHSkillList;
            exclusiveFightLSkillList = saveData.exclusiveFightLSkillList;
            exclusiveFightMSkillList = saveData.exclusiveFightMSkillList;
            exclusiveFightHSkillList = saveData.exclusiveFightHSkillList;
            heroSkillListCode = saveData.heroSkillListCode;
        }
    }

    protected void MaxHpUp(int a)//提高maxHp的方法
    {
        maxHP += a;
        currentHP += a;
    }
}
