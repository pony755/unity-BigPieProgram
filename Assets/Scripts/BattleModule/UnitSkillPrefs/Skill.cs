using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum skillType {AD,AP,ReallyDamage,Heal,Shield,Burn,Cold,Poison,Mix,Delayed}//技能类型
public enum animType {Attack}//动画类型
public enum skillPoint { Myself,AllEnemy,AllPlayers,Players,Enemies }//技能指向
public enum heroAttribute {Atk,HP}//属性
public enum passiveType {None,Hit,Dead,Attack,GameBegin,TurnStart,TurnEnd}//被动类型(决定触发时间)
public enum passivePoint {MDamager, MMyself,MAllEnemy,MAllPlayers,MEnemiesAuto, MPlayersAuto }//被动目标(M代表自己为技能使用方,结尾字母表示回合约束)
public enum passiveTurn {E,M,A}
[CreateAssetMenu(fileName ="skill",menuName ="Create new skill")]
public class Skill : ScriptableObject
{
    [Header("文本描述")]
    public string skillName;//技能名
    public string description;//技能描述

    [Header("技能设置")]
    public skillType type;//技能类型
    public animType animType;//动画类型
    public int skillTired;//技能疲劳
    public int needMP;//MP消耗
    public int delayedTurn;//延时回合
    

    [Header("技能指向(若为被动则随便设置),noMe仅针对玩家有约束")]
    public skillPoint point;//技能指向类型
    public bool noMe;//选择时不会包含自己
    [Header("如果point是Players或Enemies,可勾选此项(若为被动则随便设置)")]   
    public bool autoPoint;//判断是否自动选取目标
    
    

    [Header("技能类型为Mix的时候设置，子技能设置(实现多段伤害，多数值伤害)")]
    public List<Skill> moreSkill;

    [Header("技能类型为被动的时候设置")]
    public passiveType passiveType;
    [Header("(被动)M代表自己为技能使用方,后面表示目标(尾缀Auto需要设置目标数和rechoose)")]
    public passivePoint passivePoint;
    [Header("(被动)E异回合，M同回合，A都可以")]
    public passiveTurn passiveTurn;
    [Header("技能数值设置(Mix子技能只需设置下列项和type)")]
    public int baseInt;//技能基础类
    public List<heroAttribute> attribute;//技能增益属性列表
    public List<float> addition;//加成列表
    public int pointNum;//技能目标数量
    public bool reChoose;//是否可以重复选择同一目标
    [Header("技能失败率(0-100)")]
    public int precent;//成功率

    [Header("发动技能后的属性变化(默认为加)")]
    public List<heroAttribute> attributeCost;//技能增益属性列表
    public List<int> skillCost;//代价列表

    [Header("AD,AP伤害(针对伤害获得的属性变化，默认为加)")]
    public List<heroAttribute> attributeGet;//增益属性列表
    public List<float> damageGet;//增益列表
    private int finalAddition(Unit unit)//计算增益部分
    {
        if(attribute == null)
            return 0;
        float add;
        int sum=0;
        for(int i=0;i<attribute.Count;i++)
        {
            if(attribute[i] == heroAttribute.Atk)
            {
                add = (float)unit.Atk * addition[i];
                sum = (int)(sum + add);
            }
        }
        return sum;
    }

    public IEnumerator JudgePlayerSkill()//玩家回合获取使用的技能名,并且更改GameManager技能目标数量变量。判断接下来的状态
    {
        if (GameManager.instance.state != BattleState.SKILL)
        {
            GameManager.instance.state = BattleState.SKILL;
            GameManager.instance.useSkill = this;
            this.JudgePlayerSkill();
        }


        GameManager.instance.pointNumber = pointNum;//设定选择的目标数量为技能目标
        if (GameManager.instance.state == BattleState.SKILL)
        {
            if (this.needMP > GameManager.instance.turnUnit[0].currentMP)
            {
                Debug.Log("mp不足");
                yield return null;
            }             
            if (point==skillPoint.Myself)
            {           
                GameManager.instance.pointNumber = 1;
                GameManager.instance.pointUnit.Add(GameManager.instance.turnUnit[0]);//添加自己作为目标
                GameManager.instance.state = BattleState.ACTION;//直接进入action
            }

            else if (point==skillPoint.AllEnemy)
            {
                GameManager.instance.pointNumber = GameManager.instance.enemyUnit.Count;//目标数量为敌人数
                foreach (var o in GameManager.instance.enemyUnit)//添加所有敌人作为目标
                {
                    GameManager.instance.pointUnit.Add(o);
                }
                GameManager.instance.state = BattleState.ACTION;//直接进入action
            }
            else if (point == skillPoint.AllPlayers)
            {
                GameManager.instance.pointNumber = GameManager.instance.playerUnit.Count;//目标数量为敌人数
                foreach (var o in GameManager.instance.playerUnit)//添加所有敌人作为目标
                {
                    GameManager.instance.pointUnit.Add(o);
                }
                GameManager.instance.state = BattleState.ACTION;//直接进入action
            }

            else if (point==skillPoint.Enemies)
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
                        System.Random r = new System.Random();
                        int enemy = r.Next(GameManager.instance.enemyUnit.Count);                           
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
            else if (point == skillPoint.Players)
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
                        System.Random r = new System.Random();
                        int player = r.Next(GameManager.instance.playerUnit.Count);
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
            if (point == skillPoint.Enemies && !reChoose)
            {
                if (pointNum > GameManager.instance.playerUnit.Count)//目标数量大于敌人数
                {
                    GameManager.instance.pointNumber = GameManager.instance.playerUnit.Count;//设定选择的目标为敌人数量
                }            
            }
            if (point == skillPoint.Players && !reChoose)
            {
                if (pointNum > GameManager.instance.enemyUnit.Count)//目标数量大于己方人数
                {
                    GameManager.instance.pointNumber = GameManager.instance.enemyUnit.Count;//设定选择的目标为敌人数量
                }              
            }
            if (point==skillPoint.Myself)
            {
                GameManager.instance.pointNumber = 1;
                GameManager.instance.pointUnit.Add(GameManager.instance.turnUnit[0]);//添加自己作为目标
            }

            else if (point == skillPoint.AllEnemy)
            {
                GameManager.instance.pointNumber = GameManager.instance.playerUnit.Count;//目标数量为己方数
                foreach (var o in GameManager.instance.playerUnit)//添加所有敌人作为目标
                {
                    GameManager.instance.pointUnit.Add(o);
                }
            }
            else if (point == skillPoint.AllPlayers)
            {
                GameManager.instance.pointNumber = GameManager.instance.enemyUnit.Count;//目标数量为己方数
                foreach (var o in GameManager.instance.enemyUnit)//添加所有敌人作为目标
                {
                    GameManager.instance.pointUnit.Add(o);
                }
            }
        }
    }

    public int finalPoint(Unit unit)//计算最终数值，在unit有函数负责判断类型然后执行对应操作
    {
        return baseInt+finalAddition(unit);
    }

}
