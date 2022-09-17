using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Koubot.Tool;
public enum SkillType {AD,AP,ReallyDamage,Heal,Shield,Burn,Cold,Poison,Card,Excharge,AbandomCard,EX}//技能类型
public enum AnimType {Attack}//动画类型
public enum SkillPoint { Myself,AllEnemy,AllPlayers,Players,Enemies }//技能指向
public enum HeroAttribute { AP,APDef,maxMP,MP,AD,Def,maxHP,HP,Spirit,Critical,Dodge,Tired,Sneer, fragile, weakness, shieldDecrease, healDecrease,Burn, Cold,Poison,ADDecrease,ADPrecentDecrease, APDecrease, APPrecentDecrease, BurnDecrease, BurnPrecentDecrease,PoisonDecrease,PoisonPrecentDecrease,ColdDecrease,ColdPrecentDecrease }//属性
public enum HeroSkillAttribute { AP, APDef, maxMP, MP, AD, Def, maxHP, HP, Spirit, Critical, Dodge, Burn, Cold, Poison}//属性
public enum PassiveType {Hit,Dead,GameBegin,TurnStart,TurnEnd}//被动类型(决定触发时间)
public enum PassivePoint {MDamager, MMyself,MAllEnemy,MAllPlayers,MEnemiesAuto, MPlayersAuto }//被动目标(M代表自己为技能使用方,结尾字母表示回合约束)
public enum PassiveTurn {E,M,A}
[CreateAssetMenu(fileName ="skill",menuName ="Create new skill")]
public class Skill : ScriptableObject
{
    [Header("文本描述")]
    public Sprite skillImg;//技能图标
    public string skillName;//技能名
    public string description;//技能描述

    [Header("技能设置")]
    public SkillType type;//技能类型  
    public List<SkillType> typeTag;//显示框的技能类型
    public AnimType animType;//动画类型
    public int skillTired;//技能疲劳
    public int needMP;//MP消耗
    public int delayedTurn;//延时回合
    public int abandomCardNum;//主动弃牌的cost（仅对于玩家
    public bool onlyOne;//限定技能
    public bool cantReplace;//不可替换

    [Header("技能类型是否为为被动")]
    //被动类型;被动目标(M代表自己为技能使用方,后面表示目标(尾缀Auto需要设置目标数和rechoose));被动发动场合(E异回合，M同回合，A都可以)\n")
    public bool passiveSkill;
    [ConditionalHide("passiveSkill",1)] public PassiveType passiveType;
    [ConditionalHide("passiveSkill",1)] public PassivePoint passivePoint;
    [ConditionalHide("passiveSkill",1)] public PassiveTurn passiveTurn;


    [Header("主动技能(若为被动则不显示),noMe仅针对玩家有约束")]

    [ConditionalHide("passiveSkill", 0)] public SkillPoint point;//技能指向类型
    [ConditionalHide("passiveSkill", 0)] public bool noMe;//选择时不会包含自己
    [Header("如果point是Players或Enemies,可勾选此项(若为被动则随便设置)")]
    [ConditionalHide("passiveSkill", 0)] public bool autoPoint;//判断是否自动选取目标

    [Header("技能数值设置(addition为float)")]
    public int baseInt;//技能基础类
    public List<HeroSkillAttribute> attribute;//技能增益属性列表
    public List<float> addition;//加成列表
    public int pointNum;//技能目标数量
    public bool reChoose;//是否可以重复选择同一目标
    [Header("技能失败率(0-100)")]
    public int precent;//成功率

    [Header("发动技能后的属性变化(默认为加)")]
    public List<HeroAttribute> attributeCost;//技能增益属性列表
    public List<int> skillCost;//代价列表


    //——————————————————————————————目标判断————————————————————————————————-
    public IEnumerator JudgePlayerSkill()//玩家回合获取使用的技能名,并且更改GameManager技能目标数量变量。判断接下来的状态
    {

        if (GameManager.instance.state != BattleState.SKILL)
        {
            GameManager.instance.state = BattleState.SKILL;
            GameManager.instance.useSkill = this;
            GameManager.instance.pointUnit.Clear();
            JudgePlayerSkill();
        }

        if(type==SkillType.Excharge)
        {
            GameManager.instance.skillImg.SetActive(false);
            GameManager.instance.CardCanvas.SetActive(false);
            GameManager.instance.exchange.SetActive(true);
            GameManager.instance.tips.text = "选择交换角色";
            GameManager.instance.state = BattleState.POINTPREPAREHERO;
        }
        else
        {
            GameManager.instance.pointNumber = pointNum;//设定选择的目标数量为技能目标
            if (GameManager.instance.state == BattleState.SKILL || GameManager.instance.state == BattleState.CARDTURNUNIT)
            {
                if (this.needMP > GameManager.instance.turnUnit[0].currentMP)
                {
                    Debug.Log("mp不足");
                    GameManager.instance.tips.text = "mp不足"; 
                    yield return new WaitForSeconds(0.8f);
                    GameManager.instance.tips.text = "";
                }
                else
                {
                    if (point == SkillPoint.Myself)
                    {
                        GameManager.instance.pointNumber = 1;
                        GameManager.instance.pointUnit.Add(GameManager.instance.turnUnit[0]);//添加自己作为目标
                        GameManager.instance.SkillToAction();//直接进入action
                    }

                    else if (point == SkillPoint.AllEnemy)
                    {
                        GameManager.instance.pointNumber = GameManager.instance.enemyUnit.Count;//目标数量为敌人数
                        foreach (var o in GameManager.instance.enemyUnit)//添加所有敌人作为目标
                        {
                            GameManager.instance.pointUnit.Add(o);
                        }
                        GameManager.instance.SkillToAction();//直接进入action
                    }
                    else if (point == SkillPoint.AllPlayers)
                    {
                        GameManager.instance.pointNumber = GameManager.instance.heroUnit.Count;//目标数量为敌人数
                        foreach (var o in GameManager.instance.heroUnit)//添加所有敌人作为目标
                        {
                            GameManager.instance.pointUnit.Add(o);
                        }
                        GameManager.instance.SkillToAction();//直接进入action
                    }

                    else if (point == SkillPoint.Enemies)
                    {
                        if (!reChoose)
                        {
                            if (pointNum > GameManager.instance.enemyUnit.Count)//目标数量大于敌人数
                            {
                                GameManager.instance.pointNumber = GameManager.instance.enemyUnit.Count;//设定选择的目标为敌人数量
                            }
                            if (GameManager.instance.enemyUnit[0].SneerJudge() > 0)
                                GameManager.instance.pointNumber = GameManager.instance.enemyUnit[0].SneerJudge();//可选人数变成对方已有嘲讽数
                        }



                        if (autoPoint)
                        {
                            while (GameManager.instance.pointNumber > GameManager.instance.pointUnit.Count)//添加目标
                            {

                                int enemy = Koubot.Tool.Random.RandomTool.GenerateRandomInt(0, GameManager.instance.enemyUnit.Count - 1);
                                if (!GameManager.instance.pointUnit.Contains(GameManager.instance.enemyUnit[enemy]) || reChoose)
                                {
                                    GameManager.instance.pointUnit.Add(GameManager.instance.enemyUnit[enemy]);
                                    yield return new WaitForSeconds(0.05f);
                                }
                            }
                            GameManager.instance.SkillToAction();//直接进入action
                        }
                        else
                        {
                            GameManager.instance.state = BattleState.POINTENEMY;
                        }
                    }
                    else if (point == SkillPoint.Players)
                    {
                        if (!reChoose)
                        {
                            if (pointNum > GameManager.instance.heroUnit.Count)//目标数量大于敌人数
                            {
                                GameManager.instance.pointNumber = GameManager.instance.heroUnit.Count;//设定选择的目标为敌人数量
                            }
                        }
                        else
                            GameManager.instance.pointNumber = pointNum;//设定选择的目标数量为技能目标


                        if (autoPoint)
                        {
                            while (GameManager.instance.pointNumber > GameManager.instance.pointUnit.Count)//添加目标
                            {
                                int player = Koubot.Tool.Random.RandomTool.GenerateRandomInt(0, GameManager.instance.heroUnit.Count - 1);
                                if (!GameManager.instance.pointUnit.Contains(GameManager.instance.heroUnit[player]) || reChoose)
                                {
                                    GameManager.instance.pointUnit.Add(GameManager.instance.heroUnit[player]);
                                    yield return new WaitForSeconds(0.05f);
                                }
                            }
                            GameManager.instance.SkillToAction();//直接进入action
                        }
                        else
                        {
                            GameManager.instance.state = BattleState.POINTPLAYER;
                        }
                    }
                }
               

            }
        }
        
    }

    public IEnumerator EnemyUse()
    {
        if (GameManager.instance.state == BattleState.ENEMYTURN)
        {
            GameManager.instance.pointNumber = pointNum;
            if (point == SkillPoint.Enemies && !reChoose)
            {
                if (pointNum > GameManager.instance.heroUnit.Count)//目标数量大于敌人数
                {
                    GameManager.instance.pointNumber = GameManager.instance.heroUnit.Count;//设定选择的目标为敌人数量
                }
                if (GameManager.instance.heroUnit[0].SneerJudge() > 0)
                    GameManager.instance.pointNumber = GameManager.instance.heroUnit[0].SneerJudge();//可选人数变成对方已有嘲讽数
            }
            
            if (point==SkillPoint.Myself)
            {
                GameManager.instance.pointNumber = 1;
                GameManager.instance.pointUnit.Add(GameManager.instance.turnUnit[0]);//添加自己作为目标
            }

            else if (point == SkillPoint.AllEnemy)
            {
                GameManager.instance.pointNumber = GameManager.instance.heroUnit.Count;//目标数量为己方数
                foreach (var o in GameManager.instance.heroUnit)//添加所有敌人作为目标
                {
                    GameManager.instance.pointUnit.Add(o);
                }
            }
            else if (point == SkillPoint.AllPlayers)
            {
                GameManager.instance.pointNumber = GameManager.instance.enemyUnit.Count;//目标数量为己方数
                foreach (var o in GameManager.instance.enemyUnit)//添加所有敌人作为目标
                {
                    GameManager.instance.pointUnit.Add(o);
                }
            }
            if (point == SkillPoint.Enemies)
            {
                while (GameManager.instance.pointNumber > GameManager.instance.pointUnit.Count)//添加玩家作为目标
                {
                    if (!GameManager.instance.useSkill.reChoose)
                    {
                        int player = Koubot.Tool.Random.RandomTool.GenerateRandomInt(0, GameManager.instance.heroUnit.Count - 1);
                        if (GameManager.instance.heroUnit[0].SneerJudge()>0)//有嘲讽的情况
                        {
                            if(GameManager.instance.heroUnit[player].sneer>0)
                                GameManager.instance.pointUnit.Add(GameManager.instance.heroUnit[player]);
                        }
                        else
                        {                           
                            if (!GameManager.instance.pointUnit.Contains(GameManager.instance.heroUnit[player]))
                                GameManager.instance.pointUnit.Add(GameManager.instance.heroUnit[player]);
                        }
                    }
                    else
                    {
                        yield return new WaitForSeconds(0.1f);
                        GameManager.instance.pointUnit.Add(GameManager.instance.heroUnit[Koubot.Tool.Random.RandomTool.GenerateRandomInt(0, GameManager.instance.heroUnit.Count - 1)]);
                    }
                }
            }
            else if (point == SkillPoint.Players)
            {
                while (GameManager.instance.pointNumber > GameManager.instance.pointUnit.Count)//添加玩家作为目标
                {
                    if (!GameManager.instance.useSkill.reChoose)
                    {
                        int player = Koubot.Tool.Random.RandomTool.GenerateRandomInt(0, GameManager.instance.enemyUnit.Count - 1);
                        if (!GameManager.instance.pointUnit.Contains(GameManager.instance.enemyUnit[player]))
                            GameManager.instance.pointUnit.Add(GameManager.instance.enemyUnit[player]);
                    }
                    else
                    {
                        GameManager.instance.pointUnit.Add(GameManager.instance.enemyUnit[Koubot.Tool.Random.RandomTool.GenerateRandomInt(0, GameManager.instance.enemyUnit.Count - 1)]);
                    }
                }
            }
        }
    }


    //——————————————————————————————————————技能结算——————————————————————————————————

    public int FinalPoint(Unit unit)//计算技能基础数值数值，在unit有函数负责判断类型然后执行对应操作
    {
        int sum = 0;
        if (attribute != null)
        {
            
            for (int i = 0; i < attribute.Count; i++)
            {
                if (attribute[i] == HeroSkillAttribute.AP)
                    Single(unit.AP, addition[i], ref sum);
                else if (attribute[i] == HeroSkillAttribute.APDef)
                    Single(unit.APDef, addition[i], ref sum);
                else if (attribute[i] == HeroSkillAttribute.maxMP)
                    Single(unit.maxMP, addition[i], ref sum);
                else if (attribute[i] == HeroSkillAttribute.MP)
                    Single(unit.currentMP, addition[i], ref sum);
                else if (attribute[i] == HeroSkillAttribute.AD)
                    Single(unit.AD, addition[i], ref sum);
                else if (attribute[i] == HeroSkillAttribute.Def)
                    Single(unit.Def, addition[i], ref sum);
                else if (attribute[i] == HeroSkillAttribute.maxHP)
                    Single(unit.maxHP, addition[i], ref sum);
                else if (attribute[i] == HeroSkillAttribute.HP)
                    Single(unit.currentHP, addition[i], ref sum);
                else if (attribute[i] == HeroSkillAttribute.Spirit)
                    Single(unit.Spirit, addition[i], ref sum);
                else if (attribute[i] == HeroSkillAttribute.Critical)
                    Single(unit.Critical, addition[i], ref sum);
                else if (attribute[i] == HeroSkillAttribute.Dodge)
                    Single(unit.Dodge, addition[i], ref sum);
                else if (attribute[i] == HeroSkillAttribute.Burn)
                    Single(unit.burn, addition[i], ref sum);
                else if (attribute[i] == HeroSkillAttribute.Cold)
                    Single(unit.cold, addition[i], ref sum);
                else if (attribute[i] == HeroSkillAttribute.Poison)
                    Single(unit.poison, addition[i], ref sum);
            }
        
        }

        return baseInt+sum;
    }

    private void Single(int Attribute,float add,ref int sum)
    {
        sum = (int)(sum + (float)Attribute * add);
    }


    
    //————————————————————默认的实现技能函数（可以根据不同的需求在此重载）—————————————————————————
    public virtual void SkillSettleAD(Unit turnUnit,Unit pointUnit)
    {
        if (GameManager.instance.Probility(pointUnit.Dodge))
        {
            pointUnit.FloatStateShow(pointUnit, "闪避", Color.white);
            return;
        }
        int damage = this.FinalPoint(turnUnit);//原始数据
            if (GameManager.instance.Probility(turnUnit.Critical))//暴击
        {
            turnUnit.FloatStateShow(turnUnit,"精准",Color.yellow);
            damage *= 2;
        }
         turnUnit.ColdDecreaseDamage(ref damage);//冰冻
        damage -= turnUnit.weakness;
        damage= (int)(((float)pointUnit.Decrease(damage, pointUnit.ADDecrease, pointUnit.ADPrecentDecrease) )* (float)(1-((float)pointUnit.Def / (float)(pointUnit.Def + 100)))) + pointUnit.fragile;//最终数值
        if (pointUnit.playerHero)
        {
            if (damage >= GameManager.instance.fightPlayerCards.playerObject.GetComponent<Unit>().shield)
            {

                damage -= GameManager.instance.fightPlayerCards.playerObject.GetComponent<Unit>().shield;
                GameManager.instance.fightPlayerCards.playerObject.GetComponent<Unit>().shield = 0;
            }
            else
            {
                GameManager.instance.fightPlayerCards.playerObject.GetComponent<Unit>().shield -= damage;
                damage = 0;
            }
        }


        if (damage>=pointUnit.shield)
        {
            damage-=pointUnit.shield;
            pointUnit.shield = 0;
        }
        else
        {
            pointUnit.shield -= damage;
            damage = 0;
        }
        if (damage > 0 )
            {
                if(!turnUnit.player)
                   pointUnit.danger = turnUnit;//暂时记录伤害来源
                pointUnit.currentHP -=  damage;                
                pointUnit.FloatPointShow(damage,Color.red);               
            }
        pointUnit.BurnDamage();//烧伤
        if (pointUnit.currentHP > 0&&pointUnit.player==false)
            pointUnit.anim.Play("hit");
    }
    public virtual void SkillSettleAP(Unit turnUnit, Unit pointUnit)
    {
        if (GameManager.instance.Probility(pointUnit.Dodge))
        {
            pointUnit.FloatStateShow(pointUnit, "闪避", Color.white);
            return;
        }
        int damage = this.FinalPoint(turnUnit);//原始数据
        if (GameManager.instance.Probility(turnUnit.Critical))//暴击
        {
            turnUnit.FloatStateShow(turnUnit, "精准", Color.yellow);
            damage *= 2;
        }
        turnUnit.ColdDecreaseDamage(ref damage);//冰冻
        damage -= turnUnit.weakness;
        damage = (int)(((float)pointUnit.Decrease(damage, pointUnit.APDecrease, pointUnit.APPrecentDecrease)) * (float)(1 - ((float)pointUnit.APDef / (float)(pointUnit.APDef + 100)))) + pointUnit.fragile;//最终数值
        if (pointUnit.playerHero)
        {
            if (damage >= GameManager.instance.fightPlayerCards.playerObject.GetComponent<Unit>().shield)
            {

                damage -= GameManager.instance.fightPlayerCards.playerObject.GetComponent<Unit>().shield;
                GameManager.instance.fightPlayerCards.playerObject.GetComponent<Unit>().shield = 0;
            }
            else
            {
                GameManager.instance.fightPlayerCards.playerObject.GetComponent<Unit>().shield -= damage;
                damage = 0;
            }
        }
        if (damage >= pointUnit.shield)
        {
            damage -= pointUnit.shield;
            pointUnit.shield = 0;
        }
        else
        {
            pointUnit.shield -= damage;
            damage = 0;
        }
        if (damage > 0)
        {
            if (!turnUnit.player)
                pointUnit.danger = turnUnit;//暂时记录伤害来源
            pointUnit.currentHP -= damage;
            pointUnit.FloatPointShow(damage, Color.blue);
        }
        pointUnit.BurnDamage();//烧伤
        if (pointUnit.currentHP > 0)
            pointUnit.anim.Play("hit");
    }
    public virtual void SkillSettleReallyDamage(Unit turnUnit, Unit pointUnit)
    {
        if (GameManager.instance.Probility(pointUnit.Dodge))
        {
            pointUnit.FloatStateShow(pointUnit, "闪避", Color.white);
            return;
        }
        int damage = this.FinalPoint(turnUnit);//原始数据
        if (GameManager.instance.Probility(turnUnit.Critical))//暴击
        {
            turnUnit.FloatStateShow(turnUnit, "精准", Color.yellow);
            damage *= 2;
        }
        turnUnit.ColdDecreaseDamage(ref damage);//冰冻
        damage -= turnUnit.weakness;
        damage += pointUnit.fragile;//最终数值
        if (damage > 0)
        {
            if (!turnUnit.player)
                pointUnit.danger = turnUnit;//暂时记录伤害来源
            pointUnit.currentHP -= damage;
            pointUnit.FloatPointShow(damage, Color.white);
        }
        pointUnit.BurnDamage();//烧伤
        if (pointUnit.currentHP > 0)
            pointUnit.anim.Play("hit");
    }
    public virtual void SkillSettleHeal(Unit turnUnit, Unit pointUnit)
    {
        int heal = this.FinalPoint(turnUnit);//原始数据
        if (GameManager.instance.Probility(turnUnit.Critical))//暴击
        {
            turnUnit.FloatStateShow(turnUnit, "精准", Color.yellow);
            heal *= 2;
        }
        heal-= pointUnit.healDecrease;
        if (heal <= 0)
            heal = 0;
        if(heal> 0)
        {
            pointUnit.currentHP += heal;
            pointUnit.FloatPointShow(heal, Color.green);
        }             
    }
    public virtual void SkillSettleShield(Unit turnUnit, Unit pointUnit)
    {
        int shield = this.FinalPoint(turnUnit);//原始数据      
        if (GameManager.instance.Probility(turnUnit.Critical))//暴击
        {
            turnUnit.FloatStateShow(turnUnit, "精准", Color.yellow);
            shield *= 2;
        }
        shield -= pointUnit.shieldDecrease;
        if (shield <= 0)
            shield = 0;
        if (shield > 0)
        {
            pointUnit.shield += shield;
            pointUnit.FloatStateShow(pointUnit,"护盾",Color.white);
            Debug.Log("护盾:" + shield);
        }
    }
    public virtual void SkillSettleBurn(Unit turnUnit, Unit pointUnit)
    {
        if (GameManager.instance.Probility(pointUnit.Dodge))
        {
            pointUnit.FloatStateShow(pointUnit, "闪避", Color.white);
            return;
        }
        int burn = this.FinalPoint(turnUnit);//原始数据
        if (GameManager.instance.Probility(turnUnit.Critical))//暴击
        {
            turnUnit.FloatStateShow(turnUnit, "精准", Color.yellow);
            burn *= 2;
        }
        pointUnit.burn += burn;
        pointUnit.FloatStateShow(pointUnit, "烧伤", new Color32(231,115,49,225));
    }
    public virtual void SkillSettlePoison(Unit turnUnit, Unit pointUnit)
    {
        if (GameManager.instance.Probility(pointUnit.Dodge))
        {
            pointUnit.FloatStateShow(pointUnit, "闪避", Color.white);
            return;
        }
        int poison = this.FinalPoint(turnUnit);//原始数据
        if (GameManager.instance.Probility(turnUnit.Critical))//暴击
        {
            turnUnit.FloatStateShow(turnUnit, "精准", Color.yellow);
            poison *= 2;
        }
        pointUnit.poison += poison;
        pointUnit.FloatStateShow(pointUnit, "中毒", new Color32(157,207,73,255));
    }
    public virtual void SkillSettleCold(Unit turnUnit, Unit pointUnit)
    {
        if (GameManager.instance.Probility(pointUnit.Dodge))
        {
            pointUnit.FloatStateShow(pointUnit, "闪避", Color.white);
            return;
        }
        int cold = this.FinalPoint(turnUnit);//原始数据
        if (GameManager.instance.Probility(turnUnit.Critical))//暴击
        {
            turnUnit.FloatStateShow(turnUnit, "精准", Color.yellow);
            cold *= 2;
        }
        pointUnit.cold += cold;
        pointUnit.FloatStateShow(pointUnit, "冰冻", new Color32(97,198,236,255));
    }
    public virtual void SkillSettleCard(Unit turnUnit)
    {
        int card = FinalPoint(turnUnit);//原始数据
        if(card>0)
        {
            turnUnit.FloatStateShow(turnUnit, "抽卡", Color.magenta);
            for(int i = 0; i < card; i++)
            {
                GameManager.instance.fightPlayerCards.TakeCard();
            }
        }
            
        if (card < 0)
        {
            turnUnit.FloatStateShow(turnUnit, "弃卡", Color.black);
            for (int i = 0; i < -card; i++)
            {
                GameManager.instance.fightPlayerCards.haveCards[Koubot.Tool.Random.RandomTool.GenerateRandomInt(0, GameManager.instance.fightPlayerCards.haveCards.Count-1)].CardDestory();
            }
        }          
    }
    public virtual void SkillSettleAbandomCard(Unit turnUnit)
    {
        int card = FinalPoint(turnUnit);//原始数据
        if (card > 0)
        {
            GameManager.instance.abandomCardNum += card;
        }
    }
    public virtual void SkillSettleEX(Unit turnUnit,Unit pointUnit)
    {


    }
   
    public void SkillRemove(Unit turnUnit)//移除技能
    {
            turnUnit.heroSkillList.Remove(this);
            turnUnit.passiveHitList.Remove(this);
            turnUnit.passiveTurnEndList.Remove(this);
            turnUnit.passiveTurnStartList.Remove(this);
            turnUnit.passiveTurnStartList.Remove(this);
            turnUnit.passiveDeadList.Remove(this);
    }
    public virtual void SkillSettleExchange(Unit turnUnit,Unit pointUnit)
    {
        //交换存档体信息
        int temp;
        int index = GameManager.instance.heroUnit.IndexOf(turnUnit);//获取发动方位置索引
        temp=GameManager.instance.tempPlayer.GetComponent<FightPlayer>().fightHeroCode[index];
        GameManager.instance.tempPlayer.GetComponent<FightPlayer>().fightHeroCode[index] = GameManager.instance.tempPlayer.GetComponent<FightPlayer>().fightPrepareHeroCode[GameManager.instance.heroPreparePrefab.IndexOf(pointUnit.gameObject)];
        GameManager.instance.tempPlayer.GetComponent<FightPlayer>().fightPrepareHeroCode[GameManager.instance.heroPreparePrefab.IndexOf(pointUnit.gameObject)] = temp;
        
        //交换实际克隆体
        GameManager.instance.heroPrefab[index] = pointUnit.gameObject;
        GameManager.instance.heroPrefab[index].transform.SetParent(GameManager.instance.battleBackGround.transform);
        GameManager.instance.heroPrefab[index].transform.localPosition = GameManager.instance.playerStations[index].position;
        GameManager.instance.heroPrefab[index].GetComponent<SpriteRenderer>().sortingOrder = index;
        GameManager.instance.heroUnit[index]= GameManager.instance.heroPrefab[index].GetComponent<Unit>();//替换unit进列表
        //读取数据
        GameManager.instance.Hub[index].SetHub(GameManager.instance.heroUnit[index]);
        GameManager.instance.Hub[index].gameObject.SetActive(true);//显示对应角色状态栏
        turnUnit.gameObject.transform.SetParent(GameManager.instance.heroPrepare.transform);
        turnUnit.gameObject.transform.localPosition=new Vector3(0,0,0);
        GameManager.instance.heroPreparePrefab[GameManager.instance.heroPreparePrefab.IndexOf(pointUnit.gameObject)] = turnUnit.gameObject;
    }

}

