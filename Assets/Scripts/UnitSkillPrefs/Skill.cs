using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum skillType {AD,AP,Heal,Shield,Burn,Cold,Poison,Mix}//��������
public enum heroAttribute {Atk}//����

[CreateAssetMenu(fileName ="skill",menuName ="Create new skill")]
public class Skill : ScriptableObject
{
    [Header("�ı�����")]
    public string skillName;//������
    public string description;//��������

    [Header("��������")]
    public bool use;//�Ƿ����ʹ��
    public skillType type;//��������
    public int skillTired;//����ƣ��

    [Header("�ж��Ƿ��Ƕ�����ʹ��")]
    public bool myself; //�ж��Ƿ��Ƕ�����ʹ��

    [Header("�ж��Ƿ��ȫ�����ʹ��")]
    public bool allEnemies;//�ж��Ƿ��ȫ�����ʹ��

    [Header("�ж��Ƿ�Ϊ������δ��ѡ���ǵз�")]
    public bool players;//�ж��Ƿ�Լ���ʹ�ã��������ǵз�

    [Header("��������ΪMix��ʱ�����ã��Ӽ�������(ʵ�ֶ���˺�������ֵ�˺�)")]
    public List<Skill> moreSkill;

    [Header("������ֵ����")]
    public int baseInt;//���ܻ�����
    public List<heroAttribute> attribute;//�������������б�
    public List<float> addition;//�ӳ��б�
    public int pointNum;//����Ŀ������
    public bool reChoose;//�Ƿ�����ظ�ѡ��ͬһĿ��

    private int finalAddition(Unit unit)//�������沿��
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

    public void JudgePlayerSkill()//��һغϻ�ȡʹ�õļ�����,���Ҹ���GameManager����Ŀ�������������жϽ�������״̬
    {
        if (GameManager.instance.state == BattleState.SKILL)
        {
            if (myself)
            {
                GameManager.instance.useSkill = this;//��ȡʹ�õļ�������
                GameManager.instance.pointNumber = 1;
                GameManager.instance.pointUnit.Add(GameManager.instance.turnUnit[0]);//����Լ���ΪĿ��
                GameManager.instance.state = BattleState.ACTION;//ֱ�ӽ���action
            }

            else if (allEnemies)
            {
                GameManager.instance.useSkill = this;//��ȡʹ�õļ�������
                GameManager.instance.pointNumber = GameManager.instance.enemyUnit.Count;//Ŀ������Ϊ������
                foreach (var o in GameManager.instance.enemyUnit)//������е�����ΪĿ��
                {
                    GameManager.instance.pointUnit.Add(o);
                }
                GameManager.instance.state = BattleState.ACTION;//ֱ�ӽ���action
            }

            else if (!players)
            {
                GameManager.instance.state = BattleState.POINTENEMY;
                GameManager.instance.useSkill = this;//��ȡʹ�õļ�������
                if (pointNum > GameManager.instance.enemyUnit.Count)//Ŀ���������ڵ�����
                {
                    GameManager.instance.pointNumber = GameManager.instance.enemyUnit.Count;//�趨ѡ���Ŀ��Ϊ��������
                }
                else
                    GameManager.instance.pointNumber = pointNum;//�趨ѡ���Ŀ������Ϊ����Ŀ��              
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
                GameManager.instance.pointUnit.Add(GameManager.instance.turnUnit[0]);//����Լ���ΪĿ��
            }

            else if (allEnemies)
            {
                GameManager.instance.pointNumber = GameManager.instance.playerUnit.Count;//Ŀ������Ϊ������
                foreach (var o in GameManager.instance.playerUnit)//������е�����ΪĿ��
                {
                    GameManager.instance.pointUnit.Add(o);
                }
            }

            else if (!players&&!reChoose)
            {
                if (pointNum > GameManager.instance.enemyUnit.Count)//Ŀ���������ڵ�����
                {
                    GameManager.instance.pointNumber = GameManager.instance.enemyUnit.Count;//�趨ѡ���Ŀ��Ϊ��������
                }
                else
                    GameManager.instance.pointNumber = pointNum;//�趨ѡ���Ŀ������Ϊ����Ŀ��              
            }



        }



    }
    public int finalPoint(Unit unit)//����������ֵ����unit�к��������ж�����Ȼ��ִ�ж�Ӧ����
    {
        return baseInt+finalAddition(unit);
    }

}
