using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Koubot.Tool;
public enum SkillType {AD,AP,ReallyDamage,Heal,Shield,Burn,Cold,Poison,Card,Excharge,AbandomCard,EX}//��������
public enum AnimType {Attack}//��������
public enum SkillPoint { Myself,AllEnemy,AllPlayers,Players,Enemies }//����ָ��
public enum HeroAttribute { AP,APDef,maxMP,MP,AD,Def,maxHP,HP,Spirit,Critical,Dodge,Tired,Sneer, fragile, weakness, shieldDecrease, healDecrease,Burn, Cold,Poison,ADDecrease,ADPrecentDecrease, APDecrease, APPrecentDecrease, BurnDecrease, BurnPrecentDecrease,PoisonDecrease,PoisonPrecentDecrease,ColdDecrease,ColdPrecentDecrease }//����
public enum HeroSkillAttribute { AP, APDef, maxMP, MP, AD, Def, maxHP, HP, Spirit, Critical, Dodge, Burn, Cold, Poison}//����
public enum PassiveType {Hit,Dead,GameBegin,TurnStart,TurnEnd}//��������(��������ʱ��)
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
    public List<SkillType> typeTag;//��ʾ��ļ�������
    public AnimType animType;//��������
    public int skillTired;//����ƣ��
    public int needMP;//MP����
    public int delayedTurn;//��ʱ�غ�
    public int abandomCardNum;//�������Ƶ�cost�����������
    public bool onlyOne;//�޶�����
    public bool cantReplace;//�����滻

    [Header("���������Ƿ�ΪΪ����")]
    //��������;����Ŀ��(M�����Լ�Ϊ����ʹ�÷�,�����ʾĿ��(β׺Auto��Ҫ����Ŀ������rechoose));������������(E��غϣ�Mͬ�غϣ�A������)\n")
    public bool passiveSkill;
    [ConditionalHide("passiveSkill",1)] public PassiveType passiveType;
    [ConditionalHide("passiveSkill",1)] public PassivePoint passivePoint;
    [ConditionalHide("passiveSkill",1)] public PassiveTurn passiveTurn;


    [Header("��������(��Ϊ��������ʾ),noMe����������Լ��")]

    [ConditionalHide("passiveSkill", 0)] public SkillPoint point;//����ָ������
    [ConditionalHide("passiveSkill", 0)] public bool noMe;//ѡ��ʱ��������Լ�
    [Header("���point��Players��Enemies,�ɹ�ѡ����(��Ϊ�������������)")]
    [ConditionalHide("passiveSkill", 0)] public bool autoPoint;//�ж��Ƿ��Զ�ѡȡĿ��

    [Header("������ֵ����(additionΪfloat)")]
    public int baseInt;//���ܻ�����
    public List<HeroSkillAttribute> attribute;//�������������б�
    public List<float> addition;//�ӳ��б�
    public int pointNum;//����Ŀ������
    public bool reChoose;//�Ƿ�����ظ�ѡ��ͬһĿ��
    [Header("����ʧ����(0-100)")]
    public int precent;//�ɹ���

    [Header("�������ܺ�����Ա仯(Ĭ��Ϊ��)")]
    public List<HeroAttribute> attributeCost;//�������������б�
    public List<int> skillCost;//�����б�


    //������������������������������������������������������������Ŀ���жϡ���������������������������������������������������������������-
    public IEnumerator JudgePlayerSkill()//��һغϻ�ȡʹ�õļ�����,���Ҹ���GameManager����Ŀ�������������жϽ�������״̬
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
            GameManager.instance.tips.text = "ѡ�񽻻���ɫ";
            GameManager.instance.state = BattleState.POINTPREPAREHERO;
        }
        else
        {
            GameManager.instance.pointNumber = pointNum;//�趨ѡ���Ŀ������Ϊ����Ŀ��
            if (GameManager.instance.state == BattleState.SKILL || GameManager.instance.state == BattleState.CARDTURNUNIT)
            {
                if (this.needMP > GameManager.instance.turnUnit[0].currentMP)
                {
                    Debug.Log("mp����");
                    GameManager.instance.tips.text = "mp����"; 
                    yield return new WaitForSeconds(0.8f);
                    GameManager.instance.tips.text = "";
                }
                else
                {
                    if (point == SkillPoint.Myself)
                    {
                        GameManager.instance.pointNumber = 1;
                        GameManager.instance.pointUnit.Add(GameManager.instance.turnUnit[0]);//����Լ���ΪĿ��
                        GameManager.instance.SkillToAction();//ֱ�ӽ���action
                    }

                    else if (point == SkillPoint.AllEnemy)
                    {
                        GameManager.instance.pointNumber = GameManager.instance.enemyUnit.Count;//Ŀ������Ϊ������
                        foreach (var o in GameManager.instance.enemyUnit)//������е�����ΪĿ��
                        {
                            GameManager.instance.pointUnit.Add(o);
                        }
                        GameManager.instance.SkillToAction();//ֱ�ӽ���action
                    }
                    else if (point == SkillPoint.AllPlayers)
                    {
                        GameManager.instance.pointNumber = GameManager.instance.heroUnit.Count;//Ŀ������Ϊ������
                        foreach (var o in GameManager.instance.heroUnit)//������е�����ΪĿ��
                        {
                            GameManager.instance.pointUnit.Add(o);
                        }
                        GameManager.instance.SkillToAction();//ֱ�ӽ���action
                    }

                    else if (point == SkillPoint.Enemies)
                    {
                        if (!reChoose)
                        {
                            if (pointNum > GameManager.instance.enemyUnit.Count)//Ŀ���������ڵ�����
                            {
                                GameManager.instance.pointNumber = GameManager.instance.enemyUnit.Count;//�趨ѡ���Ŀ��Ϊ��������
                            }
                            if (GameManager.instance.enemyUnit[0].SneerJudge() > 0)
                                GameManager.instance.pointNumber = GameManager.instance.enemyUnit[0].SneerJudge();//��ѡ������ɶԷ����г�����
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
                            GameManager.instance.SkillToAction();//ֱ�ӽ���action
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
                            if (pointNum > GameManager.instance.heroUnit.Count)//Ŀ���������ڵ�����
                            {
                                GameManager.instance.pointNumber = GameManager.instance.heroUnit.Count;//�趨ѡ���Ŀ��Ϊ��������
                            }
                        }
                        else
                            GameManager.instance.pointNumber = pointNum;//�趨ѡ���Ŀ������Ϊ����Ŀ��


                        if (autoPoint)
                        {
                            while (GameManager.instance.pointNumber > GameManager.instance.pointUnit.Count)//���Ŀ��
                            {
                                int player = Koubot.Tool.Random.RandomTool.GenerateRandomInt(0, GameManager.instance.heroUnit.Count - 1);
                                if (!GameManager.instance.pointUnit.Contains(GameManager.instance.heroUnit[player]) || reChoose)
                                {
                                    GameManager.instance.pointUnit.Add(GameManager.instance.heroUnit[player]);
                                    yield return new WaitForSeconds(0.05f);
                                }
                            }
                            GameManager.instance.SkillToAction();//ֱ�ӽ���action
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
                if (pointNum > GameManager.instance.heroUnit.Count)//Ŀ���������ڵ�����
                {
                    GameManager.instance.pointNumber = GameManager.instance.heroUnit.Count;//�趨ѡ���Ŀ��Ϊ��������
                }
                if (GameManager.instance.heroUnit[0].SneerJudge() > 0)
                    GameManager.instance.pointNumber = GameManager.instance.heroUnit[0].SneerJudge();//��ѡ������ɶԷ����г�����
            }
            
            if (point==SkillPoint.Myself)
            {
                GameManager.instance.pointNumber = 1;
                GameManager.instance.pointUnit.Add(GameManager.instance.turnUnit[0]);//����Լ���ΪĿ��
            }

            else if (point == SkillPoint.AllEnemy)
            {
                GameManager.instance.pointNumber = GameManager.instance.heroUnit.Count;//Ŀ������Ϊ������
                foreach (var o in GameManager.instance.heroUnit)//������е�����ΪĿ��
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
            if (point == SkillPoint.Enemies)
            {
                while (GameManager.instance.pointNumber > GameManager.instance.pointUnit.Count)//��������ΪĿ��
                {
                    if (!GameManager.instance.useSkill.reChoose)
                    {
                        int player = Koubot.Tool.Random.RandomTool.GenerateRandomInt(0, GameManager.instance.heroUnit.Count - 1);
                        if (GameManager.instance.heroUnit[0].SneerJudge()>0)//�г�������
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
                while (GameManager.instance.pointNumber > GameManager.instance.pointUnit.Count)//��������ΪĿ��
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


    //�������������������������������������������������������������������������������ܽ��㡪������������������������������������������������������������������

    public int FinalPoint(Unit unit)//���㼼�ܻ�����ֵ��ֵ����unit�к��������ж�����Ȼ��ִ�ж�Ӧ����
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


    
    //����������������������������������������Ĭ�ϵ�ʵ�ּ��ܺ��������Ը��ݲ�ͬ�������ڴ����أ���������������������������������������������������
    public virtual void SkillSettleAD(Unit turnUnit,Unit pointUnit)
    {
        if (GameManager.instance.Probility(pointUnit.Dodge))
        {
            pointUnit.FloatStateShow(pointUnit, "����", Color.white);
            return;
        }
        int damage = this.FinalPoint(turnUnit);//ԭʼ����
            if (GameManager.instance.Probility(turnUnit.Critical))//����
        {
            turnUnit.FloatStateShow(turnUnit,"��׼",Color.yellow);
            damage *= 2;
        }
         turnUnit.ColdDecreaseDamage(ref damage);//����
        damage -= turnUnit.weakness;
        damage= (int)(((float)pointUnit.Decrease(damage, pointUnit.ADDecrease, pointUnit.ADPrecentDecrease) )* (float)(1-((float)pointUnit.Def / (float)(pointUnit.Def + 100)))) + pointUnit.fragile;//������ֵ
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
                   pointUnit.danger = turnUnit;//��ʱ��¼�˺���Դ
                pointUnit.currentHP -=  damage;                
                pointUnit.FloatPointShow(damage,Color.red);               
            }
        pointUnit.BurnDamage();//����
        if (pointUnit.currentHP > 0&&pointUnit.player==false)
            pointUnit.anim.Play("hit");
    }
    public virtual void SkillSettleAP(Unit turnUnit, Unit pointUnit)
    {
        if (GameManager.instance.Probility(pointUnit.Dodge))
        {
            pointUnit.FloatStateShow(pointUnit, "����", Color.white);
            return;
        }
        int damage = this.FinalPoint(turnUnit);//ԭʼ����
        if (GameManager.instance.Probility(turnUnit.Critical))//����
        {
            turnUnit.FloatStateShow(turnUnit, "��׼", Color.yellow);
            damage *= 2;
        }
        turnUnit.ColdDecreaseDamage(ref damage);//����
        damage -= turnUnit.weakness;
        damage = (int)(((float)pointUnit.Decrease(damage, pointUnit.APDecrease, pointUnit.APPrecentDecrease)) * (float)(1 - ((float)pointUnit.APDef / (float)(pointUnit.APDef + 100)))) + pointUnit.fragile;//������ֵ
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
                pointUnit.danger = turnUnit;//��ʱ��¼�˺���Դ
            pointUnit.currentHP -= damage;
            pointUnit.FloatPointShow(damage, Color.blue);
        }
        pointUnit.BurnDamage();//����
        if (pointUnit.currentHP > 0)
            pointUnit.anim.Play("hit");
    }
    public virtual void SkillSettleReallyDamage(Unit turnUnit, Unit pointUnit)
    {
        if (GameManager.instance.Probility(pointUnit.Dodge))
        {
            pointUnit.FloatStateShow(pointUnit, "����", Color.white);
            return;
        }
        int damage = this.FinalPoint(turnUnit);//ԭʼ����
        if (GameManager.instance.Probility(turnUnit.Critical))//����
        {
            turnUnit.FloatStateShow(turnUnit, "��׼", Color.yellow);
            damage *= 2;
        }
        turnUnit.ColdDecreaseDamage(ref damage);//����
        damage -= turnUnit.weakness;
        damage += pointUnit.fragile;//������ֵ
        if (damage > 0)
        {
            if (!turnUnit.player)
                pointUnit.danger = turnUnit;//��ʱ��¼�˺���Դ
            pointUnit.currentHP -= damage;
            pointUnit.FloatPointShow(damage, Color.white);
        }
        pointUnit.BurnDamage();//����
        if (pointUnit.currentHP > 0)
            pointUnit.anim.Play("hit");
    }
    public virtual void SkillSettleHeal(Unit turnUnit, Unit pointUnit)
    {
        int heal = this.FinalPoint(turnUnit);//ԭʼ����
        if (GameManager.instance.Probility(turnUnit.Critical))//����
        {
            turnUnit.FloatStateShow(turnUnit, "��׼", Color.yellow);
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
        int shield = this.FinalPoint(turnUnit);//ԭʼ����      
        if (GameManager.instance.Probility(turnUnit.Critical))//����
        {
            turnUnit.FloatStateShow(turnUnit, "��׼", Color.yellow);
            shield *= 2;
        }
        shield -= pointUnit.shieldDecrease;
        if (shield <= 0)
            shield = 0;
        if (shield > 0)
        {
            pointUnit.shield += shield;
            pointUnit.FloatStateShow(pointUnit,"����",Color.white);
            Debug.Log("����:" + shield);
        }
    }
    public virtual void SkillSettleBurn(Unit turnUnit, Unit pointUnit)
    {
        if (GameManager.instance.Probility(pointUnit.Dodge))
        {
            pointUnit.FloatStateShow(pointUnit, "����", Color.white);
            return;
        }
        int burn = this.FinalPoint(turnUnit);//ԭʼ����
        if (GameManager.instance.Probility(turnUnit.Critical))//����
        {
            turnUnit.FloatStateShow(turnUnit, "��׼", Color.yellow);
            burn *= 2;
        }
        pointUnit.burn += burn;
        pointUnit.FloatStateShow(pointUnit, "����", new Color32(231,115,49,225));
    }
    public virtual void SkillSettlePoison(Unit turnUnit, Unit pointUnit)
    {
        if (GameManager.instance.Probility(pointUnit.Dodge))
        {
            pointUnit.FloatStateShow(pointUnit, "����", Color.white);
            return;
        }
        int poison = this.FinalPoint(turnUnit);//ԭʼ����
        if (GameManager.instance.Probility(turnUnit.Critical))//����
        {
            turnUnit.FloatStateShow(turnUnit, "��׼", Color.yellow);
            poison *= 2;
        }
        pointUnit.poison += poison;
        pointUnit.FloatStateShow(pointUnit, "�ж�", new Color32(157,207,73,255));
    }
    public virtual void SkillSettleCold(Unit turnUnit, Unit pointUnit)
    {
        if (GameManager.instance.Probility(pointUnit.Dodge))
        {
            pointUnit.FloatStateShow(pointUnit, "����", Color.white);
            return;
        }
        int cold = this.FinalPoint(turnUnit);//ԭʼ����
        if (GameManager.instance.Probility(turnUnit.Critical))//����
        {
            turnUnit.FloatStateShow(turnUnit, "��׼", Color.yellow);
            cold *= 2;
        }
        pointUnit.cold += cold;
        pointUnit.FloatStateShow(pointUnit, "����", new Color32(97,198,236,255));
    }
    public virtual void SkillSettleCard(Unit turnUnit)
    {
        int card = FinalPoint(turnUnit);//ԭʼ����
        if(card>0)
        {
            turnUnit.FloatStateShow(turnUnit, "�鿨", Color.magenta);
            for(int i = 0; i < card; i++)
            {
                GameManager.instance.fightPlayerCards.TakeCard();
            }
        }
            
        if (card < 0)
        {
            turnUnit.FloatStateShow(turnUnit, "����", Color.black);
            for (int i = 0; i < -card; i++)
            {
                GameManager.instance.fightPlayerCards.haveCards[Koubot.Tool.Random.RandomTool.GenerateRandomInt(0, GameManager.instance.fightPlayerCards.haveCards.Count-1)].CardDestory();
            }
        }          
    }
    public virtual void SkillSettleAbandomCard(Unit turnUnit)
    {
        int card = FinalPoint(turnUnit);//ԭʼ����
        if (card > 0)
        {
            GameManager.instance.abandomCardNum += card;
        }
    }
    public virtual void SkillSettleEX(Unit turnUnit,Unit pointUnit)
    {


    }
   
    public void SkillRemove(Unit turnUnit)//�Ƴ�����
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
        //�����浵����Ϣ
        int temp;
        int index = GameManager.instance.heroUnit.IndexOf(turnUnit);//��ȡ������λ������
        temp=GameManager.instance.tempPlayer.GetComponent<FightPlayer>().fightHeroCode[index];
        GameManager.instance.tempPlayer.GetComponent<FightPlayer>().fightHeroCode[index] = GameManager.instance.tempPlayer.GetComponent<FightPlayer>().fightPrepareHeroCode[GameManager.instance.heroPreparePrefab.IndexOf(pointUnit.gameObject)];
        GameManager.instance.tempPlayer.GetComponent<FightPlayer>().fightPrepareHeroCode[GameManager.instance.heroPreparePrefab.IndexOf(pointUnit.gameObject)] = temp;
        
        //����ʵ�ʿ�¡��
        GameManager.instance.heroPrefab[index] = pointUnit.gameObject;
        GameManager.instance.heroPrefab[index].transform.SetParent(GameManager.instance.battleBackGround.transform);
        GameManager.instance.heroPrefab[index].transform.localPosition = GameManager.instance.playerStations[index].position;
        GameManager.instance.heroPrefab[index].GetComponent<SpriteRenderer>().sortingOrder = index;
        GameManager.instance.heroUnit[index]= GameManager.instance.heroPrefab[index].GetComponent<Unit>();//�滻unit���б�
        //��ȡ����
        GameManager.instance.Hub[index].SetHub(GameManager.instance.heroUnit[index]);
        GameManager.instance.Hub[index].gameObject.SetActive(true);//��ʾ��Ӧ��ɫ״̬��
        turnUnit.gameObject.transform.SetParent(GameManager.instance.heroPrepare.transform);
        turnUnit.gameObject.transform.localPosition=new Vector3(0,0,0);
        GameManager.instance.heroPreparePrefab[GameManager.instance.heroPreparePrefab.IndexOf(pointUnit.gameObject)] = turnUnit.gameObject;
    }

}

