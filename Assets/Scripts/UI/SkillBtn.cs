using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillBtn : MonoBehaviour
{
    public Text skillText;
    public Skill skillInfo;//技能信息加载
    


    public void addSkillName()//获取使用的技能名,并且更改GameManager技能目标数量变量,并判断下一个阶段。如果是非指定技能，直接进行对象判断并进入ACTION;
    {
        if(GameManager.instance.state==BattleState.SKILL)
        {
            if(skillInfo.myself)
            {
                GameManager.instance.state = BattleState.ACTION;
                GameManager.instance.useSkill = skillInfo;//读取使用的技能索引
                GameManager.instance.pointUnit.Add(GameManager.instance.turnUnit[0]);//添加自己作为目标
                GameManager.instance.pointNumber = 1; 
            }

            else if (skillInfo.allEnemies)
            {
                GameManager.instance.state = BattleState.ACTION;
                GameManager.instance.useSkill = skillInfo;//读取使用的技能索引
                foreach (var o in GameManager.instance.enemyUnit)//添加所有敌人作为目标
                {
                    GameManager.instance.pointUnit.Add(o);
                }                
                GameManager.instance.pointNumber = skillInfo.pointNum;
            }

            else if(!skillInfo.players)
            {
                GameManager.instance.state = BattleState.POINTENEMY;
                GameManager.instance.useSkill = skillInfo;//读取使用的技能索引
                GameManager.instance.pointNumber = skillInfo.pointNum;//设定选择的目标数量
                
            }
            
        }      

    }
  
}
