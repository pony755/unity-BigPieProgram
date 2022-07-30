using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum skillType {AD,AP,ReallyDamage,Heal,Shield,Burn,Cold,Poison,Mix,Delayed}//��������
public enum animType {Attack}//��������
public enum skillPoint { Myself,AllEnemy,AllPlayers,Players,Enemies }//����ָ��
public enum heroAttribute {Atk,HP}//����
public enum passiveType {None,Hit,Dead,Attack,GameBegin,TurnStart,TurnEnd}//��������(��������ʱ��)
public enum passivePoint {MDamager, MMyself,MAllEnemy,MAllPlayers,MEnemiesAuto, MPlayersAuto }//����Ŀ��(M�����Լ�Ϊ����ʹ�÷�,��β��ĸ��ʾ�غ�Լ��)
public enum passiveTurn {E,M,A}
[CreateAssetMenu(fileName ="skill",menuName ="Create new skill")]
public class Skill : ScriptableObject
{
    [Header("�ı�����")]
    public string skillName;//������
    public string description;//��������

    [Header("��������")]
    public skillType type;//��������
    public animType animType;//��������
    public int skillTired;//����ƣ��
    public int needMP;//MP����
    public int delayedTurn;//��ʱ�غ�
    

    [Header("����ָ��(��Ϊ�������������),noMe����������Լ��")]
    public skillPoint point;//����ָ������
    public bool noMe;//ѡ��ʱ��������Լ�
    [Header("���point��Players��Enemies,�ɹ�ѡ����(��Ϊ�������������)")]   
    public bool autoPoint;//�ж��Ƿ��Զ�ѡȡĿ��
    
    

    [Header("��������ΪMix��ʱ�����ã��Ӽ�������(ʵ�ֶ���˺�������ֵ�˺�)")]
    public List<Skill> moreSkill;

    [Header("��������Ϊ������ʱ������")]
    public passiveType passiveType;
    [Header("(����)M�����Լ�Ϊ����ʹ�÷�,�����ʾĿ��(β׺Auto��Ҫ����Ŀ������rechoose)")]
    public passivePoint passivePoint;
    [Header("(����)E��غϣ�Mͬ�غϣ�A������")]
    public passiveTurn passiveTurn;
    [Header("������ֵ����(Mix�Ӽ���ֻ�������������type)")]
    public int baseInt;//���ܻ�����
    public List<heroAttribute> attribute;//�������������б�
    public List<float> addition;//�ӳ��б�
    public int pointNum;//����Ŀ������
    public bool reChoose;//�Ƿ�����ظ�ѡ��ͬһĿ��
    [Header("����ʧ����(0-100)")]
    public int precent;//�ɹ���

    [Header("�������ܺ�����Ա仯(Ĭ��Ϊ��)")]
    public List<heroAttribute> attributeCost;//�������������б�
    public List<int> skillCost;//�����б�

    [Header("AD,AP�˺�(����˺���õ����Ա仯��Ĭ��Ϊ��)")]
    public List<heroAttribute> attributeGet;//���������б�
    public List<float> damageGet;//�����б�
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

    public IEnumerator JudgePlayerSkill()//��һغϻ�ȡʹ�õļ�����,���Ҹ���GameManager����Ŀ�������������жϽ�������״̬
    {
        if (GameManager.instance.state != BattleState.SKILL)
        {
            GameManager.instance.state = BattleState.SKILL;
            GameManager.instance.useSkill = this;
            this.JudgePlayerSkill();
        }


        GameManager.instance.pointNumber = pointNum;//�趨ѡ���Ŀ������Ϊ����Ŀ��
        if (GameManager.instance.state == BattleState.SKILL)
        {
            if (this.needMP > GameManager.instance.turnUnit[0].currentMP)
            {
                Debug.Log("mp����");
                yield return null;
            }             
            if (point==skillPoint.Myself)
            {           
                GameManager.instance.pointNumber = 1;
                GameManager.instance.pointUnit.Add(GameManager.instance.turnUnit[0]);//����Լ���ΪĿ��
                GameManager.instance.state = BattleState.ACTION;//ֱ�ӽ���action
            }

            else if (point==skillPoint.AllEnemy)
            {
                GameManager.instance.pointNumber = GameManager.instance.enemyUnit.Count;//Ŀ������Ϊ������
                foreach (var o in GameManager.instance.enemyUnit)//������е�����ΪĿ��
                {
                    GameManager.instance.pointUnit.Add(o);
                }
                GameManager.instance.state = BattleState.ACTION;//ֱ�ӽ���action
            }
            else if (point == skillPoint.AllPlayers)
            {
                GameManager.instance.pointNumber = GameManager.instance.playerUnit.Count;//Ŀ������Ϊ������
                foreach (var o in GameManager.instance.playerUnit)//������е�����ΪĿ��
                {
                    GameManager.instance.pointUnit.Add(o);
                }
                GameManager.instance.state = BattleState.ACTION;//ֱ�ӽ���action
            }

            else if (point==skillPoint.Enemies)
            {
                if (!reChoose)
                {
                    if (pointNum > GameManager.instance.enemyUnit.Count)//Ŀ���������ڵ�����
                    {
                        GameManager.instance.pointNumber = GameManager.instance.enemyUnit.Count;//�趨ѡ���Ŀ��Ϊ��������
                    }                   
                }
                


                if (autoPoint)
                {
                    while (GameManager.instance.pointNumber > GameManager.instance.pointUnit.Count)//���Ŀ��
                    {
                        System.Random r = new System.Random();
                        int enemy = r.Next(GameManager.instance.enemyUnit.Count);                           
                        if (!GameManager.instance.pointUnit.Contains(GameManager.instance.enemyUnit[enemy]) || reChoose)
                        {                 
                            GameManager.instance.pointUnit.Add(GameManager.instance.enemyUnit[enemy]);
                            yield return new WaitForSeconds(0.05f);
                        }                                                
                    }
                    GameManager.instance.state = BattleState.ACTION;//ֱ�ӽ���action
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
                    if (pointNum > GameManager.instance.playerUnit.Count)//Ŀ���������ڵ�����
                    {
                        GameManager.instance.pointNumber = GameManager.instance.playerUnit.Count;//�趨ѡ���Ŀ��Ϊ��������
                    }
                }
                else
                    GameManager.instance.pointNumber = pointNum;//�趨ѡ���Ŀ������Ϊ����Ŀ��


                if (autoPoint)
                {
                    while (GameManager.instance.pointNumber > GameManager.instance.pointUnit.Count)//���Ŀ��
                    {
                        System.Random r = new System.Random();
                        int player = r.Next(GameManager.instance.playerUnit.Count);
                        if (!GameManager.instance.pointUnit.Contains(GameManager.instance.playerUnit[player]) || reChoose)
                        {
                            GameManager.instance.pointUnit.Add(GameManager.instance.playerUnit[player]);
                            yield return new WaitForSeconds(0.05f);
                        }
                    }
                    GameManager.instance.state = BattleState.ACTION;//ֱ�ӽ���action
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
                if (pointNum > GameManager.instance.playerUnit.Count)//Ŀ���������ڵ�����
                {
                    GameManager.instance.pointNumber = GameManager.instance.playerUnit.Count;//�趨ѡ���Ŀ��Ϊ��������
                }            
            }
            if (point == skillPoint.Players && !reChoose)
            {
                if (pointNum > GameManager.instance.enemyUnit.Count)//Ŀ���������ڼ�������
                {
                    GameManager.instance.pointNumber = GameManager.instance.enemyUnit.Count;//�趨ѡ���Ŀ��Ϊ��������
                }              
            }
            if (point==skillPoint.Myself)
            {
                GameManager.instance.pointNumber = 1;
                GameManager.instance.pointUnit.Add(GameManager.instance.turnUnit[0]);//����Լ���ΪĿ��
            }

            else if (point == skillPoint.AllEnemy)
            {
                GameManager.instance.pointNumber = GameManager.instance.playerUnit.Count;//Ŀ������Ϊ������
                foreach (var o in GameManager.instance.playerUnit)//������е�����ΪĿ��
                {
                    GameManager.instance.pointUnit.Add(o);
                }
            }
            else if (point == skillPoint.AllPlayers)
            {
                GameManager.instance.pointNumber = GameManager.instance.enemyUnit.Count;//Ŀ������Ϊ������
                foreach (var o in GameManager.instance.enemyUnit)//������е�����ΪĿ��
                {
                    GameManager.instance.pointUnit.Add(o);
                }
            }
        }
    }

    public int finalPoint(Unit unit)//����������ֵ����unit�к��������ж�����Ȼ��ִ�ж�Ӧ����
    {
        return baseInt+finalAddition(unit);
    }

}
