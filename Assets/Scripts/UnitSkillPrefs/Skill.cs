using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum skillType {AD,AP,Heal,Shield,Burn,Cold,Poison,Mix}//技能类型
public enum heroAttribute {Atk}//属性

[CreateAssetMenu(fileName ="skill",menuName ="Create new skill")]
public class Skill : ScriptableObject
{
    [Header("文本描述")]
    public string skillName;//技能名
    public string description;//技能描述

    [Header("技能设置")]
    public bool use;//是否可以使用
    public skillType type;//技能类型
    public int skillTired;//技能疲劳

    [Header("判断是否是对自身使用")]
    public bool myself; //判断是否是对自身使用

    [Header("判断是否对全体敌人使用")]
    public bool allEnemies;//判断是否对全体敌人使用

    [Header("判断是否为己方，未勾选则是敌方")]
    public bool players;//判断是否对己方使用，不是则是敌方

    [Header("技能类型为Mix的时候设置，子技能设置(实现多段伤害，多数值伤害)")]
    public List<Skill> moreSkill;

    [Header("技能数值设置")]
    public int baseInt;//技能基础类
    public List<heroAttribute> attribute;//技能增益属性列表
    public List<float> addition;//加成列表
    public int pointNum;//技能目标数量
    public bool reChoose;//是否可以重复选择同一目标

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

    public void JudgePlayerSkill()//玩家回合获取使用的技能名,并且更改GameManager技能目标数量变量。判断接下来的状态
    {
        if (GameManager.instance.state == BattleState.SKILL)
        {
            if (myself)
            {
                GameManager.instance.useSkill = this;//读取使用的技能索引
                GameManager.instance.pointNumber = 1;
                GameManager.instance.pointUnit.Add(GameManager.instance.turnUnit[0]);//添加自己作为目标
                GameManager.instance.state = BattleState.ACTION;//直接进入action
            }

            else if (allEnemies)
            {
                GameManager.instance.useSkill = this;//读取使用的技能索引
                GameManager.instance.pointNumber = GameManager.instance.enemyUnit.Count;//目标数量为敌人数
                foreach (var o in GameManager.instance.enemyUnit)//添加所有敌人作为目标
                {
                    GameManager.instance.pointUnit.Add(o);
                }
                GameManager.instance.state = BattleState.ACTION;//直接进入action
            }

            else if (!players)
            {
                GameManager.instance.state = BattleState.POINTENEMY;
                GameManager.instance.useSkill = this;//读取使用的技能索引
                if (pointNum > GameManager.instance.enemyUnit.Count)//目标数量大于敌人数
                {
                    GameManager.instance.pointNumber = GameManager.instance.enemyUnit.Count;//设定选择的目标为敌人数量
                }
                else
                    GameManager.instance.pointNumber = pointNum;//设定选择的目标数量为技能目标              
            }
        }
    }

    public void EnemyUse()
    {
        if (GameManager.instance.state == BattleState.ENEMYTURN)
        {
            if (myself)
            {
                GameManager.instance.pointNumber = 1;
                GameManager.instance.pointUnit.Add(GameManager.instance.turnUnit[0]);//添加自己作为目标
            }

            else if (allEnemies)
            {
                GameManager.instance.pointNumber = GameManager.instance.playerUnit.Count;//目标数量为己方数
                foreach (var o in GameManager.instance.playerUnit)//添加所有敌人作为目标
                {
                    GameManager.instance.pointUnit.Add(o);
                }
            }

            else if (!players&&!reChoose)
            {
                if (pointNum > GameManager.instance.enemyUnit.Count)//目标数量大于敌人数
                {
                    GameManager.instance.pointNumber = GameManager.instance.enemyUnit.Count;//设定选择的目标为敌人数量
                }
                else
                    GameManager.instance.pointNumber = pointNum;//设定选择的目标数量为技能目标              
            }



        }



    }
    public int finalPoint(Unit unit)//计算最终数值，在unit有函数负责判断类型然后执行对应操作
    {
        return baseInt+finalAddition(unit);
    }

}
