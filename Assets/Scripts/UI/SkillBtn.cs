using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillBtn : MonoBehaviour
{
    public Text skillText;
    public Skill skillInfo;//������Ϣ����
    


    public void addSkillName()//��ȡʹ�õļ�����,���Ҹ���GameManager����Ŀ����������,���ж���һ���׶Ρ�����Ƿ�ָ�����ܣ�ֱ�ӽ��ж����жϲ�����ACTION;
    {
        if(GameManager.instance.state==BattleState.SKILL)
        {
            if(skillInfo.myself)
            {
                GameManager.instance.state = BattleState.ACTION;
                GameManager.instance.useSkill = skillInfo;//��ȡʹ�õļ�������
                GameManager.instance.pointUnit.Add(GameManager.instance.turnUnit[0]);//����Լ���ΪĿ��
                GameManager.instance.pointNumber = 1; 
            }

            else if (skillInfo.allEnemies)
            {
                GameManager.instance.state = BattleState.ACTION;
                GameManager.instance.useSkill = skillInfo;//��ȡʹ�õļ�������
                foreach (var o in GameManager.instance.enemyUnit)//������е�����ΪĿ��
                {
                    GameManager.instance.pointUnit.Add(o);
                }                
                GameManager.instance.pointNumber = skillInfo.pointNum;
            }

            else if(!skillInfo.players)
            {
                GameManager.instance.state = BattleState.POINTENEMY;
                GameManager.instance.useSkill = skillInfo;//��ȡʹ�õļ�������
                GameManager.instance.pointNumber = skillInfo.pointNum;//�趨ѡ���Ŀ������
                
            }
            
        }      

    }
  
}
