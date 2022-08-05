using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Koubot.Tool;
public enum SkillType {AD,AP,ReallyDamage,Heal,Shield,Burn,Cold,Poison,Mix,AttributeAdjust,Card}//��������
public enum AnimType {Attack}//��������
public enum SkillPoint { Myself,AllEnemy,AllPlayers,Players,Enemies }//����ָ��
public enum HeroAttribute {Atk,HP}//����
public enum PassiveType {None,Hit,Dead,GameBegin,TurnStart,TurnEnd}//��������(��������ʱ��)
public enum PassivePoint {MDamager, MMyself,MAllEnemy,MAllPlayers,MEnemiesAuto, MPlayersAuto }//����Ŀ��(M�����Լ�Ϊ����ʹ�÷�,��β��ĸ��ʾ�غ�Լ��)
public enum PassiveTurn {E,M,A}
[CreateAssetMenu(fileName ="skill",menuName ="Create new skill")]
public class Skill : ScriptableObject
{
    [Header("�ı�����")]
    public Sprite skillImg;//����ͼ��
    public string skillName;//������
    public string description;//��������

    [Header("��������")]
    public SkillType type;//��������
    public AnimType animType;//��������
    public int skillTired;//����ƣ��
    public int needMP;//MP����
    public int delayedTurn;//��ʱ�غ�
    

    [Header("����ָ��(��Ϊ�������������),noMe����������Լ��")]
    public SkillPoint point;//����ָ������
    public bool noMe;//ѡ��ʱ��������Լ�
    [Header("���point��Players��Enemies,�ɹ�ѡ����(��Ϊ�������������)")]   
    public bool autoPoint;//�ж��Ƿ��Զ�ѡȡĿ��
    
    

    [Header("��������ΪMix��ʱ�����ã��Ӽ�������(ʵ�ֶ���˺�������ֵ�˺�)")]
    public List<Skill> moreSkill;

    [Header("�������ã��Ƿ�ѡ���ɫ��ΪpointUnit")]
    public bool cardPointUnit;

    [Header("��������Ϊ������ʱ������")]
    public PassiveType passiveType;
    [Header("(����)M�����Լ�Ϊ����ʹ�÷�,�����ʾĿ��(β׺Auto��Ҫ����Ŀ������rechoose)")]
    public PassivePoint passivePoint;
    [Header("(����)E��غϣ�Mͬ�غϣ�A������")]
    public PassiveTurn passiveTurn;
    [Header("������ֵ����(Mix�Ӽ���ֻ�������������type)")]
    public int baseInt;//���ܻ�����
    public List<HeroAttribute> attribute;//�������������б�
    public List<float> addition;//�ӳ��б�
    public int pointNum;//����Ŀ������
    public bool reChoose;//�Ƿ�����ظ�ѡ��ͬһĿ��
    [Header("����ʧ����(0-100)")]
    public int precent;//�ɹ���

    [Header("�������ܺ�����Ա仯(Ĭ��Ϊ��)")]
    public List<HeroAttribute> attributeCost;//�������������б�
    public List<int> skillCost;//�����б�

    private int FinalAddition(Unit unit)//�������沿��
    {
        if(attribute == null)
            return 0;
        float add;
        int sum=0;
        for(int i=0;i<attribute.Count;i++)
        {
            if(attribute[i] == HeroAttribute.Atk)
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
            GameManager.instance.pointUnit.Clear();
            this.JudgePlayerSkill();
        }


        GameManager.instance.pointNumber = pointNum;//�趨ѡ���Ŀ������Ϊ����Ŀ��
        if (GameManager.instance.state == BattleState.SKILL|| GameManager.instance.state == BattleState.CARDTURNUNIT)
        {
            if (this.needMP > GameManager.instance.turnUnit[0].currentMP)
            {
                Debug.Log("mp����");
                yield return null;
            }             
            if (point==SkillPoint.Myself)
            {           
                GameManager.instance.pointNumber = 1;
                GameManager.instance.pointUnit.Add(GameManager.instance.turnUnit[0]);//����Լ���ΪĿ��
                GameManager.instance.state = BattleState.ACTION;//ֱ�ӽ���action
            }

            else if (point==SkillPoint.AllEnemy)
            {
                GameManager.instance.pointNumber = GameManager.instance.enemyUnit.Count;//Ŀ������Ϊ������
                foreach (var o in GameManager.instance.enemyUnit)//������е�����ΪĿ��
                {
                    GameManager.instance.pointUnit.Add(o);
                }
                GameManager.instance.state = BattleState.ACTION;//ֱ�ӽ���action
            }
            else if (point == SkillPoint.AllPlayers)
            {
                GameManager.instance.pointNumber = GameManager.instance.playerUnit.Count;//Ŀ������Ϊ������
                foreach (var o in GameManager.instance.playerUnit)//������е�����ΪĿ��
                {
                    GameManager.instance.pointUnit.Add(o);
                }
                GameManager.instance.state = BattleState.ACTION;//ֱ�ӽ���action
            }

            else if (point==SkillPoint.Enemies)
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

                        int enemy = Koubot.Tool.Random.RandomTool.GenerateRandomInt(0, GameManager.instance.enemyUnit.Count - 1); 
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
            else if (point == SkillPoint.Players)
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
                        int player = Koubot.Tool.Random.RandomTool.GenerateRandomInt(0, GameManager.instance.playerUnit.Count - 1);
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
            if (point == SkillPoint.Enemies && !reChoose)
            {
                if (pointNum > GameManager.instance.playerUnit.Count)//Ŀ���������ڵ�����
                {
                    GameManager.instance.pointNumber = GameManager.instance.playerUnit.Count;//�趨ѡ���Ŀ��Ϊ��������
                }            
            }
            if (point == SkillPoint.Players && !reChoose)
            {
                if (pointNum > GameManager.instance.enemyUnit.Count)//Ŀ���������ڼ�������
                {
                    GameManager.instance.pointNumber = GameManager.instance.enemyUnit.Count;//�趨ѡ���Ŀ��Ϊ��������
                }              
            }
            if (point==SkillPoint.Myself)
            {
                GameManager.instance.pointNumber = 1;
                GameManager.instance.pointUnit.Add(GameManager.instance.turnUnit[0]);//����Լ���ΪĿ��
            }

            else if (point == SkillPoint.AllEnemy)
            {
                GameManager.instance.pointNumber = GameManager.instance.playerUnit.Count;//Ŀ������Ϊ������
                foreach (var o in GameManager.instance.playerUnit)//������е�����ΪĿ��
                {
                    GameManager.instance.pointUnit.Add(o);
                }
            }
            else if (point == SkillPoint.AllPlayers)
            {
                GameManager.instance.pointNumber = GameManager.instance.enemyUnit.Count;//Ŀ������Ϊ������
                foreach (var o in GameManager.instance.enemyUnit)//������е�����ΪĿ��
                {
                    GameManager.instance.pointUnit.Add(o);
                }
            }
        }
    }

    public int FinalPoint(Unit unit)//����������ֵ����unit�к��������ж�����Ȼ��ִ�ж�Ӧ����
    {
        return baseInt+FinalAddition(unit);
    }


    //����������������������������������������Ĭ�ϵ�ʵ�ּ��ܺ��������Ը��ݲ�ͬ�������ڴ����أ���������������������������������������������������
    public virtual void SkillSettleAD(Unit turnUnit,Unit pointUnit)
    {
        if (this.type == SkillType.AD)
        {
            int damage = this.FinalPoint(turnUnit) -pointUnit. Def;
            if (damage < 0)
                damage = 0;

            if (damage > 0 && pointUnit.currentHP > 0)
            {
                if(!turnUnit.player)
                   pointUnit.danger = turnUnit;//��ʱ��¼�˺���Դ
                pointUnit.currentHP -=  damage;
                Debug.Log(pointUnit.unitName + "�ܵ���" + damage + "�������˺�");
                pointUnit.floatPoint.transform.GetChild(0).GetComponent<TMP_Text>().text = damage.ToString();
                pointUnit.floatPoint.transform.GetChild(0).GetComponent<TMP_Text>().color = Color.red;
                Instantiate(pointUnit.floatPoint, pointUnit.transform.position + new Vector3(0, 1, 0), Quaternion.identity);
                pointUnit.anim.Play("hit");
            }
        }
    }









}
