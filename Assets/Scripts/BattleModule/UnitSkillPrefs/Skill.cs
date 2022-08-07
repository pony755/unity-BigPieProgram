using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Koubot.Tool;
public enum SkillType {AD,AP,ReallyDamage,Heal,Shield,Burn,Cold,Poison,Mix,  AttributeAdjust,Card}//技能类型
public enum AnimType {Attack}//动画类型
public enum SkillPoint { Myself,AllEnemy,AllPlayers,Players,Enemies }//技能指向
public enum HeroAttribute { AP,APDef,maxMP,MP,AD,Def,maxHP,HP,Spirit,Critical,Dodge,Tired,Burn,Cold,Poison,ADDecrease,ADPrecentDecrease, APDecrease, APPrecentDecrease, BurnDecrease, BurnPrecentDecrease,PoisonDecrease,PoisonPrecentDecrease,ColdDecrease,ColdPrecentDecrease }//属性
public enum HeroSkillAttribute { AP, APDef, maxMP, MP, AD, Def, maxHP, HP, Spirit, Critical, Dodge, Burn, Cold, Poison}//属性
public enum PassiveType {None,Hit,Dead,GameBegin,TurnStart,TurnEnd}//被动类型(决定触发时间)
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
    public AnimType animType;//动画类型
    public int skillTired;//技能疲劳
    public int needMP;//MP消耗
    public int delayedTurn;//延时回合
    

    [Header("技能指向(若为被动则随便设置),noMe仅针对玩家有约束")]
    public SkillPoint point;//技能指向类型
    public bool noMe;//选择时不会包含自己
    [Header("如果point是Players或Enemies,可勾选此项(若为被动则随便设置)")]   
    public bool autoPoint;//判断是否自动选取目标

    [Header("技能类型为AttributeAdjust的时候设置，调整目标属性（默认为加）")]
    public HeroAttribute adjustAttribute;

    [Header("技能类型为Mix的时候设置，子技能设置(实现多段伤害，多数值伤害)")]
    public List<Skill> moreSkill;

    [Header("卡牌设置：是否选择角色作为pointUnit")]
    public bool cardPointUnit;

    [Header("技能类型为被动的时候设置")]
    public PassiveType passiveType;
    [Header("(被动)M代表自己为技能使用方,后面表示目标(尾缀Auto需要设置目标数和rechoose)")]
    public PassivePoint passivePoint;
    [Header("(被动)E异回合，M同回合，A都可以")]
    public PassiveTurn passiveTurn;

    [Header("技能数值设置(Mix子技能只需设置下列项和type)(addition为float)")]
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
            this.JudgePlayerSkill();
        }


        GameManager.instance.pointNumber = pointNum;//设定选择的目标数量为技能目标
        if (GameManager.instance.state == BattleState.SKILL|| GameManager.instance.state == BattleState.CARDTURNUNIT)
        {
            if (this.needMP > GameManager.instance.turnUnit[0].currentMP)
            {
                Debug.Log("mp不足");
                yield return null;
            }             
            if (point==SkillPoint.Myself)
            {           
                GameManager.instance.pointNumber = 1;
                GameManager.instance.pointUnit.Add(GameManager.instance.turnUnit[0]);//添加自己作为目标
                GameManager.instance.state = BattleState.ACTION;//直接进入action
            }

            else if (point==SkillPoint.AllEnemy)
            {
                GameManager.instance.pointNumber = GameManager.instance.enemyUnit.Count;//目标数量为敌人数
                foreach (var o in GameManager.instance.enemyUnit)//添加所有敌人作为目标
                {
                    GameManager.instance.pointUnit.Add(o);
                }
                GameManager.instance.state = BattleState.ACTION;//直接进入action
            }
            else if (point == SkillPoint.AllPlayers)
            {
                GameManager.instance.pointNumber = GameManager.instance.playerUnit.Count;//目标数量为敌人数
                foreach (var o in GameManager.instance.playerUnit)//添加所有敌人作为目标
                {
                    GameManager.instance.pointUnit.Add(o);
                }
                GameManager.instance.state = BattleState.ACTION;//直接进入action
            }

            else if (point==SkillPoint.Enemies)
            {
                if (!reChoose)
                {
                    if (pointNum > GameManager.instance.enemyUnit.Count)//目标数量大于敌人数
                    {
                        GameManager.instance.pointNumber = GameManager.instance.enemyUnit.Count;//设定选择的目标为敌人数量
                    }                   
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
                    GameManager.instance.state = BattleState.ACTION;//直接进入action
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
                    if (pointNum > GameManager.instance.playerUnit.Count)//目标数量大于敌人数
                    {
                        GameManager.instance.pointNumber = GameManager.instance.playerUnit.Count;//设定选择的目标为敌人数量
                    }
                }
                else
                    GameManager.instance.pointNumber = pointNum;//设定选择的目标数量为技能目标


                if (autoPoint)
                {
                    while (GameManager.instance.pointNumber > GameManager.instance.pointUnit.Count)//添加目标
                    {
                        int player = Koubot.Tool.Random.RandomTool.GenerateRandomInt(0, GameManager.instance.playerUnit.Count - 1);
                        if (!GameManager.instance.pointUnit.Contains(GameManager.instance.playerUnit[player]) || reChoose)
                        {
                            GameManager.instance.pointUnit.Add(GameManager.instance.playerUnit[player]);
                            yield return new WaitForSeconds(0.05f);
                        }
                    }
                    GameManager.instance.state = BattleState.ACTION;//直接进入action
                }
                else
                {
                    GameManager.instance.state = BattleState.POINTPLAYER;
                }
            }
            
        }
    }

    public void EnemyUse()
    {
        if (GameManager.instance.state == BattleState.ENEMYTURN)
        {
            GameManager.instance.pointNumber = pointNum;
            if (point == SkillPoint.Enemies && !reChoose)
            {
                if (pointNum > GameManager.instance.playerUnit.Count)//目标数量大于敌人数
                {
                    GameManager.instance.pointNumber = GameManager.instance.playerUnit.Count;//设定选择的目标为敌人数量
                }            
            }
            if (point == SkillPoint.Players && !reChoose)
            {
                if (pointNum > GameManager.instance.enemyUnit.Count)//目标数量大于己方人数
                {
                    GameManager.instance.pointNumber = GameManager.instance.enemyUnit.Count;//设定选择的目标为敌人数量
                }              
            }
            if (point==SkillPoint.Myself)
            {
                GameManager.instance.pointNumber = 1;
                GameManager.instance.pointUnit.Add(GameManager.instance.turnUnit[0]);//添加自己作为目标
            }

            else if (point == SkillPoint.AllEnemy)
            {
                GameManager.instance.pointNumber = GameManager.instance.playerUnit.Count;//目标数量为己方数
                foreach (var o in GameManager.instance.playerUnit)//添加所有敌人作为目标
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
        }
    }


    //——————————————————————————————————————技能结算——————————————————————————————————

    public int FinalPoint(Unit unit)//计算最终数值，在unit有函数负责判断类型然后执行对应操作
    {
        if (attribute == null)
            return 0;
        int sum = 0;
        for (int i = 0; i < attribute.Count; i++)
        {
            if (attribute[i] == HeroSkillAttribute.AP)
                Single(unit.AP,addition[i],ref sum);
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

        return baseInt+sum;
    }

    private void Single(int Attribute,float add,ref int sum)
    {
        sum = (int)(sum + (float)Attribute * add);
    }

    
    //————————————————————默认的实现技能函数（可以根据不同的需求在此重载）—————————————————————————
    public virtual void SkillSettleAD(Unit turnUnit,Unit pointUnit)
    {
 
            int damage = this.FinalPoint(turnUnit) -pointUnit. Def;
            if (damage > 0 )
            {
                if(!turnUnit.player)
                   pointUnit.danger = turnUnit;//暂时记录伤害来源
                pointUnit.currentHP -=  damage;
                Debug.Log(pointUnit.unitName + "受到了" + damage + "点物理伤害");
                pointUnit.FloatPointShow(damage,Color.red);
                if(pointUnit.currentHP > 0)
                     pointUnit.anim.Play("hit");
            }

    }
    public virtual void SkillSettleAdjust(Unit turnUnit, Unit pointUnit)//结算技能发动后属性变换
    {

          if ( adjustAttribute== HeroAttribute.AP)
            pointUnit.AP+=FinalPoint(turnUnit);
        else if (adjustAttribute == HeroAttribute.APDef)
            pointUnit.APDef += FinalPoint(turnUnit);
        else if (adjustAttribute == HeroAttribute.maxMP)
            pointUnit.maxMP += FinalPoint(turnUnit);
        else if (adjustAttribute == HeroAttribute.MP)
            pointUnit.currentMP += FinalPoint(turnUnit);
        else if (adjustAttribute == HeroAttribute.AD)
            pointUnit.AD += FinalPoint(turnUnit);
        else if (adjustAttribute == HeroAttribute.Def)
            pointUnit.Def += FinalPoint(turnUnit);
        else if (adjustAttribute == HeroAttribute.maxHP)
            pointUnit.maxHP += FinalPoint(turnUnit);
        else if (adjustAttribute == HeroAttribute.HP)
            pointUnit.currentHP += FinalPoint(turnUnit);
        else if (adjustAttribute == HeroAttribute.Spirit)
            pointUnit.Spirit += FinalPoint(turnUnit);
        else if (adjustAttribute == HeroAttribute.Critical)
            pointUnit.Critical += FinalPoint(turnUnit);
        else if (adjustAttribute == HeroAttribute.Dodge)
            pointUnit.Dodge += FinalPoint(turnUnit);
        else if (adjustAttribute == HeroAttribute.Tired)
            pointUnit.tired += FinalPoint(turnUnit);
        else if (adjustAttribute == HeroAttribute.Burn)
            pointUnit.burn += FinalPoint(turnUnit);
        else if (adjustAttribute == HeroAttribute.Cold)
            pointUnit.cold += FinalPoint(turnUnit);
        else if (adjustAttribute == HeroAttribute.Poison)
            pointUnit.poison += FinalPoint(turnUnit);
        else if (adjustAttribute == HeroAttribute.ADDecrease)
            pointUnit.ADDecrease += FinalPoint(turnUnit);
        else if (adjustAttribute == HeroAttribute.ADPrecentDecrease)
            pointUnit.ADPrecentDecrease += FinalPoint(turnUnit);
        else if (adjustAttribute == HeroAttribute.APDecrease)
            pointUnit.APDecrease += FinalPoint(turnUnit);
        else if (adjustAttribute == HeroAttribute.APPrecentDecrease)
            pointUnit.APPrecentDecrease += FinalPoint(turnUnit);
        else if (adjustAttribute == HeroAttribute.BurnDecrease)
            pointUnit.BurnDecrease += FinalPoint(turnUnit);
        else if (adjustAttribute == HeroAttribute.BurnPrecentDecrease)
            pointUnit.BurnPrecentDecrease += FinalPoint(turnUnit);
        else if (adjustAttribute == HeroAttribute.PoisonDecrease)
            pointUnit.PoisonDecrease += FinalPoint(turnUnit);
        else if (adjustAttribute == HeroAttribute.PoisonPrecentDecrease)
            pointUnit.PoisonPrecentDecrease += FinalPoint(turnUnit);
        else if (adjustAttribute == HeroAttribute.ColdDecrease)
            pointUnit.ColdDecrease += FinalPoint(turnUnit);
        else if (adjustAttribute == HeroAttribute.ColdPrecentDecrease)
            pointUnit.ColdPrecentDecrease += FinalPoint(turnUnit);

    }









}
