using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

public enum SkillRoll {L1,L2,L3,M1,M2,M3,H1,H2,H3,T}
public class Unit : MonoBehaviour
{
    [HideInInspector]public Animator anim;//����
    [HideInInspector]public Unit danger;//��ʱ��¼�˺���Դ
    
    public Sprite normalSprite;
    public GameObject point;//ָ��
    public GameObject cardPoint;//��Ƭָ��
    public GameObject floatPoint;//�˺�
    public GameObject floatSkill;//���ܸ���
    public GameObject floatState;//���ܸ���
    public bool playerHero;//�ж��ǲ��Ǽ�����ɫ


    [Header("��ɫ��")]
    public string unitName;
    [Header("��������")]
    public int skillNum;
    public int unitLevel;
    public int maxLevel;
    public int[] nextExp;
    [Header("��")]
    public int AP;
    public int APDef;
    public int maxMP;
    [Header("��")]
    public int AD;
    public int Def;
    public int maxHP;
    [Header("��")]
    public int Spirit;
    public int Critical;
    public int Dodge;

    [Header("��Ϸ��״̬��")]
    public int currentHP;
    public int currentExp;
    public int getExp;

    [Header("ս��״̬��(ÿ��ս����ˢ�³�Ĭ��ֵ,����Ҫ�����ȡ)")]
    public int tired;
    public int sneer;//����
    public int shield;//��
    public int fragile;//����
    public int weakness;//����
    public int shieldDecrease;//����
    public int healDecrease;//���ָ�
    public int burn;
    public int cold;
    public int poison;   
    public int currentMP;   
    public List<SkillRoll> skillRoll;
    
    

    [Header("���ÿ���")]
    public int ADDecrease;
    public int ADPrecentDecrease;
    public int APDecrease;
    public int APPrecentDecrease;
    public int BurnDecrease;
    public int BurnPrecentDecrease;
    public int PoisonDecrease;
    public int PoisonPrecentDecrease;
    public int ColdDecrease;
    public int ColdPrecentDecrease;

    [Header("����")]
    public List<Skill> heroSkillList;

    [Header("���ܳ�")]
    public List<Skill> currencyLSkillList;
    public List<Skill> exclusiveLSkillList;
    public List<Skill> currencyMSkillList;
    public List<Skill> exclusiveMSkillList;
    public List<Skill> currencyHSkillList;  
    public List<Skill> exclusiveHSkillList;

    //[Header("���ܱ���б�")]
    public List<int> heroSkillListCode;

    //[Header("ս�����ܳر��(�洢�������")]
    public List<int> currencyFightLSkillList;
    public List<int> exclusiveFightLSkillList;
    public List<int> currencyFightMSkillList;
    public List<int> exclusiveFightMSkillList;
    public List<int> currencyFightHSkillList; 
    public List<int> exclusiveFightHSkillList;
    [Header("�Ƿ�Ϊ�������")]
    public bool player;

    [Header("���������б�")]
    public List<Skill> passiveTurnEndList;//�غϿ�ʼʱ����
    public List<Skill> passiveTurnStartList;//�غϿ�ʼʱ����
    public List<Skill> passiveHitList;//���˴���
    public List<Skill> passiveDeadList;//��������
    public List<Skill> passiveGameBeginList;//��������

    [Header("��ʱ���������б�(Ĭ��Ϊ��)")]
    public List<int> adjustTurn;
    public List<HeroAttribute> attributeAdjust;//�������������б�
    public List<float> attributeAdjustPoint;//�����б�
    [System.Serializable]
    public class SaveUnitData
    {
        public int skillNum;
        public int unitLevel;
        public int nextExp;

        public int AP;
        public int APDef;
        public int maxMP;

        public int AD;
        public int Def;
        public int maxHP;

        public int Spirit;
        public int Critical;
        public int Dodge;


        public int currentHP;
        public int currentExp;
        public int getExp;

        public int ADDecrease;
        public int ADPrecentDecrease;
        public int APDecrease;
        public int APPrecentDecrease;
        public int BurnDecrease;
        public int BurnPrecentDecrease;
        public int PoisonDecrease;
        public int PoisonPrecentDecrease;
        public int ColdDecrease;
        public int ColdPrecentDecrease;

        public List<int> currencyFightLSkillList;
        public List<int> currencyFightMSkillList;
        public List<int> currencyFightHSkillList;
        public List<int> exclusiveFightLSkillList;
        public List<int> exclusiveFightMSkillList;
        public List<int> exclusiveFightHSkillList;
        public List<int> heroSkillListCode;
    }
    protected virtual void Awake()//���Գ�ʼ��
    {
        //��ʼ������
        if (playerHero)
        {
            UnitBeginLoad();
            SetSkill();
        }
                      
    }
    protected virtual void Start()
    {
        nextExp = new int[maxLevel + 1];
        nextExp[1] = 100;
        for (int i = 2; i < maxLevel + 1; i++)
        {
            nextExp[i] = Mathf.RoundToInt(nextExp[i - 1] * 1.1f);
        }
        currentMP = maxMP;
        anim = GetComponent<Animator>();
        StartCoroutine(SetPassive());
    }

    protected virtual void Update()
    {
        if (GameManager.instance.state == BattleState.CARDTURNUNIT && playerHero&&tired==0)
            cardPoint.SetActive(true);
        else
            cardPoint.SetActive(false);
        if (GameManager.instance. state != BattleState.POINTENEMY || GameManager.instance.state != BattleState.POINTPLAYER || GameManager.instance.state != BattleState.POINTALL)//���������׶Σ���ͼ��
            point.SetActive(false);
        if((GameManager.instance.state == BattleState.POINTENEMY&&!playerHero)|| (GameManager.instance.state == BattleState.POINTPLAYER && playerHero)|| GameManager.instance.state == BattleState.POINTALL)
        {
            if(currentHP > 0)
                 point.SetActive(true);
            if ((!GameManager.instance.useSkill.reChoose && GameManager.instance.pointUnit.Contains(this))|| (GameManager.instance.useSkill.noMe&&GameManager.instance.turnUnit[0]==this))
            {
                point.SetActive(false);
            }
            if(!playerHero&&SneerJudge()>0&&sneer==0)
            {
                point.SetActive(false);
            }
        }
        if (currentHP > maxHP)
        {
            currentHP = maxHP;
        }
        if (currentHP <= 0)
        {
            currentHP = 0;
            anim.Play("dead");
            Invoke("CheckDead", 0.2f);
        }
    }
    private void CheckDead()//�����жϽ��㺯��
    {
        if (currentHP == 0)
        {
            GetComponent<BoxCollider>().enabled = false;//�ر���ײ��ű�
            if (GameManager.instance.heroUnit.Contains(this))
            {
                GameManager.instance.heroUnit.Remove(this);
            }
            if (GameManager.instance.enemyUnit.Contains(this))
            {
                GameManager.instance.enemyUnit.Remove(this);
            }

        }
    }
    IEnumerator SetPassive()
    {
        
        foreach (var p in heroSkillList)
        {
            if (p.passiveType == PassiveType.Hit)
            {
                passiveHitList.Add(p);
            }
            else if (p.passiveType == PassiveType.Dead)
            {
                passiveDeadList.Add(p);
            }
            else if (p.passiveType == PassiveType.GameBegin)
            {
                passiveGameBeginList.Add(p);
            }
            else if (p.passiveType == PassiveType.TurnStart)
            {
                passiveTurnStartList.Add(p);
            }
            else if (p.passiveType == PassiveType.TurnEnd)
            {
                passiveTurnEndList.Add(p);
            }
        } 
        yield return new WaitForSeconds(0.5f);
        PassiveGameBegin();
    }

    
 
    private void AttributeSettle(Skill skill)//���㼼�ܷ��������Ա任
    {
        for (int i = 0; i < skill.attributeCost.Count; i++)
        {
            if (skill.attributeCost[i] == HeroAttribute.AP)
                SingleSettle(ref AP, skill.skillCost[i]);               
            else if (skill.attributeCost[i] == HeroAttribute.APDef)
                SingleSettle(ref APDef, skill.skillCost[i]);
            else if (skill.attributeCost[i] == HeroAttribute.maxMP)
                SingleSettle(ref maxMP, skill.skillCost[i]);
            else if (skill.attributeCost[i] == HeroAttribute.MP)
                SingleSettle(ref currentMP, skill.skillCost[i]);
            else if (skill.attributeCost[i] == HeroAttribute.AD)
                SingleSettle(ref AD, skill.skillCost[i]);
            else if (skill.attributeCost[i] == HeroAttribute.Def)
                SingleSettle(ref Def, skill.skillCost[i]);
            else if (skill.attributeCost[i] == HeroAttribute.maxHP)
                SingleSettle(ref maxHP, skill.skillCost[i]);           
            else if (skill.attributeCost[i] == HeroAttribute.HP)
                SingleSettle(ref currentHP, skill.skillCost[i]);
            else if (skill.attributeCost[i] == HeroAttribute.Spirit)
                SingleSettle(ref Spirit, skill.skillCost[i]);
            else if (skill.attributeCost[i] == HeroAttribute.Critical)
                SingleSettle(ref Critical, skill.skillCost[i]);
            else if (skill.attributeCost[i] == HeroAttribute.Dodge)
                SingleSettle(ref Dodge, skill.skillCost[i]);
            else if (skill.attributeCost[i] == HeroAttribute.Tired)
                SingleSettle(ref tired, skill.skillCost[i]);
            else if (skill.attributeCost[i] == HeroAttribute.fragile)
                SingleSettle(ref fragile, skill.skillCost[i]);
            else if (skill.attributeCost[i] == HeroAttribute.weakness)
                SingleSettle(ref weakness, skill.skillCost[i]);
            else if (skill.attributeCost[i] == HeroAttribute.shieldDecrease)
                SingleSettle(ref shieldDecrease, skill.skillCost[i]);
            else if (skill.attributeCost[i] == HeroAttribute.Burn)
                SingleSettle(ref burn, skill.skillCost[i]);
            else if (skill.attributeCost[i] == HeroAttribute.Cold)
                SingleSettle(ref cold, skill.skillCost[i]);
            else if (skill.attributeCost[i] == HeroAttribute.Poison)
                SingleSettle(ref poison, skill.skillCost[i]);
            else if (skill.attributeCost[i] == HeroAttribute.ADDecrease)
                SingleSettle(ref ADDecrease, skill.skillCost[i]);
            else if (skill.attributeCost[i] == HeroAttribute.ADPrecentDecrease)
                SingleSettle(ref ADPrecentDecrease, skill.skillCost[i]);
            else if (skill.attributeCost[i] == HeroAttribute.APDecrease)
                SingleSettle(ref APDecrease, skill.skillCost[i]);
            else if (skill.attributeCost[i] == HeroAttribute.APPrecentDecrease)
                SingleSettle(ref APPrecentDecrease, skill.skillCost[i]);
            else if (skill.attributeCost[i] == HeroAttribute.BurnDecrease)
                SingleSettle(ref BurnDecrease, skill.skillCost[i]);
            else if (skill.attributeCost[i] == HeroAttribute.BurnPrecentDecrease)
                SingleSettle(ref BurnPrecentDecrease, skill.skillCost[i]);
            else if (skill.attributeCost[i] == HeroAttribute.PoisonDecrease)
                SingleSettle(ref PoisonDecrease, skill.skillCost[i]);
            else if (skill.attributeCost[i] == HeroAttribute.PoisonPrecentDecrease)
                SingleSettle(ref PoisonPrecentDecrease, skill.skillCost[i]);
            else if (skill.attributeCost[i] == HeroAttribute.ColdDecrease)
                SingleSettle(ref ColdDecrease, skill.skillCost[i]);
            else if (skill.attributeCost[i] == HeroAttribute.ColdPrecentDecrease)
                SingleSettle(ref ColdPrecentDecrease, skill.skillCost[i]);
            else if (skill.attributeCost[i] == HeroAttribute.Sneer)
                SingleSettle(ref sneer, skill.skillCost[i]);

        }
    }

    private void SingleSettle(ref int Attribute,int b)
    {
        Attribute += b;
        if (Attribute < 0)
            Attribute = 0;
        
    }

    IEnumerator Settle(Unit turnUnit, Skill skill)
    {
            if (skill.type == SkillType.AD)
                skill.SkillSettleAD(turnUnit, this);
            if (skill.type == SkillType.AP)
                skill.SkillSettleAP(turnUnit, this);
            if (skill.type == SkillType.ReallyDamage)
                skill.SkillSettleReallyDamage(turnUnit, this);
            if (skill.type == SkillType.Heal)
                skill.SkillSettleHeal(turnUnit, this);
            if (skill.type == SkillType.Shield)
                skill.SkillSettleShield(turnUnit, this);
            if (skill.type == SkillType.Burn)
                skill.SkillSettleBurn(turnUnit, this);
            if (skill.type == SkillType.Cold)
                skill.SkillSettleCold(turnUnit, this);
            if (skill.type == SkillType.Poison)
                skill.SkillSettlePoison(turnUnit, this);
            if (skill.type == SkillType.Card)
                skill.SkillSettleCard(turnUnit);
            if (skill.type == SkillType.AbandomCard)
                skill.SkillSettleAbandomCard(turnUnit);
            if (skill.type == SkillType.Excharge)
                skill.SkillSettleExchange(turnUnit, this);
            if (skill.type == SkillType.EX)
                skill.SkillSettleEX(turnUnit, this);    
        yield return null;

    }

    public void SkillCost(Skill skill)
    {
        //���㷢������
        AttributeSettle(skill);
        currentMP -= skill.needMP;
        tired += skill.skillTired;
    }

    public void UseSkillSettle(Skill skill,List<Unit> pointUnits)//���㼼��
    {

        SkillCost(skill);    
        if(skill.onlyOne)
        {
            skill.SkillRemove(this);
        }
        //�����Ƿ�Ϊ��ʱ
        if (skill.delayedTurn > 0&&!GameManager.instance.delayedSwitch)
        {

            GameManager.instance.delayedTurn.Add(GameManager.instance.turn+skill.delayedTurn);
            GameManager.instance.delayedTurnUnit.Add(this);
            GameManager.instance.delayedSkill.Add(skill);
            if (GameManager.instance.delayedPointUnit.Count < GameManager.instance.delayedTurn.Count)
            {
                List<Unit> tempUnits=new List<Unit>();
                GameManager.instance.delayedPointUnit.Add(tempUnits);
            }
            foreach(var p in pointUnits)
            GameManager.instance.delayedPointUnit[GameManager.instance.delayedTurn.Count-1].Add(p);
        }

        else
        {    
            foreach(var p in pointUnits)
                StartCoroutine(p.Settle(this,skill));
        }
    }
    public void UseSkillSettle(Skill skill, Unit pointUnits)//���㼼��
    {

        SkillCost(skill);
        if (skill.onlyOne)
        {
            skill.SkillRemove(this);
        }
        //�����Ƿ�Ϊ��ʱ
        if (skill.delayedTurn > 0 && !GameManager.instance.delayedSwitch)
        {

            GameManager.instance.delayedTurn.Add(GameManager.instance.turn + skill.delayedTurn);
            GameManager.instance.delayedTurnUnit.Add(this);
            GameManager.instance.delayedSkill.Add(skill);
            if (GameManager.instance.delayedPointUnit.Count < GameManager.instance.delayedTurn.Count)
            {
                List<Unit> tempUnits = new List<Unit>();
                GameManager.instance.delayedPointUnit.Add(tempUnits);
            }
            GameManager.instance.delayedPointUnit[GameManager.instance.delayedTurn.Count - 1].Add(this);
        }

        else
        {
                StartCoroutine(pointUnits.Settle(this, skill));
        }
    }

    //�����������������������������������������¼�(�󶨵���Ӧ����)��������������������������������������������������������
    public void PassiveGameBegin()
    {
        
        if (passiveGameBeginList.Count > 0)
        {
            foreach (var o in passiveGameBeginList)
            {             
                StartCoroutine(PassiveSettle(o));
            }
        }
    }
    public void PassiveTurnEnd()//�غϽ���ʱ
    {
        if (passiveTurnEndList.Count > 0)
        {
            foreach (var o in passiveTurnEndList)
            {
                StartCoroutine(PassiveSettle(o));
            }
        }
    }
    public void PassiveTurnStart()//�غϿ�ʼʱ
    {
        if (passiveTurnStartList.Count > 0)
        {
            foreach (var o in passiveTurnStartList)
            {
                StartCoroutine(PassiveSettle(o));
            }
        }
    }
    public void PassiveHit()//����ʱ
    {
        if(passiveHitList.Count>0)
        {
             foreach(var o in passiveHitList)
            {
                StartCoroutine(PassiveSettle(o));
            }
        }
        danger = null;
    }

    public void PassiveDead()//����ʱ
    {

        if (passiveDeadList.Count > 0)
        {
            foreach (var o in passiveDeadList)
            {

                StartCoroutine(PassiveSettle(o));

            }
        }
    }
    
    IEnumerator PassiveSettle(Skill o)
    {
        if (currentMP < o.needMP)
        {
            Debug.Log("���� " + o.skillName + " ����MP����");
        }
        else if (GameManager.instance.Probility(o.precent))
        {
            Debug.Log(o.skillName + " ����ʧ��");
            SkillCost(o);

        }     
        else
        {
            bool Go = true;//�ж��Ƿ��������
            int tempPointNum = o.pointNum;
            List<Unit> tempUnits = new List<Unit>();
            List<Unit> tempPointUnits = new List<Unit>();
            if (o.passiveTurn == PassiveTurn.E)
            {
                if (!(((GameManager.instance.state == BattleState.ENEMYTURN || GameManager.instance.state == BattleState.ENEMYTURNSTART || GameManager.instance.state == BattleState.ENEMYFINISH) && this.playerHero)
                 || (GameManager.instance.state == BattleState.ACTIONFINISH || GameManager.instance.state == BattleState.PLAYERTURNSTART) && !this.playerHero))
                    Go = false;
            }
            else if (o.passiveTurn == PassiveTurn.M)
            {
                if (!(((GameManager.instance.state == BattleState.ENEMYTURN || GameManager.instance.state == BattleState.ENEMYTURNSTART || GameManager.instance.state == BattleState.ENEMYFINISH) && !this.playerHero)
                || ((GameManager.instance.state == BattleState.ACTIONFINISH || GameManager.instance.state == BattleState.PLAYERTURNSTART) && this.playerHero)))
                    Go = false;
            }

            if (Go)
            {
                FloatTextShow(this, o.skillName, new Color32(190, 190, 190, 255));

                if (o.passivePoint == PassivePoint.MDamager)
                    if (danger != null)
                    {
                        UseSkillSettle( o, danger);
                    }
                if (o.passivePoint == PassivePoint.MMyself)
                    UseSkillSettle(o,this);
                if (o.passivePoint == PassivePoint.MAllEnemy)
                {
                    if (playerHero)
                    {
                            UseSkillSettle(o, GameManager.instance.enemyUnit);
                    }
                    else
                    {
                            UseSkillSettle(o,GameManager.instance.heroUnit);

                    }

                }
                if (o.passivePoint == PassivePoint.MAllPlayers)
                {
                    if (!playerHero)
                    {
                        for (int i = 0; i < GameManager.instance.enemyUnit.Count; i++)
                        {
                            UseSkillSettle(o, GameManager.instance.enemyUnit);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < GameManager.instance.heroUnit.Count; i++)
                        {
                            UseSkillSettle(o, GameManager.instance.heroUnit);
                        }
                    }

                }              
                if (o.passivePoint == PassivePoint.MEnemiesAuto)
                {
                    if (!o.reChoose)//ȷ��ѡ�������
                    {
                        if (!playerHero && (o.pointNum > GameManager.instance.heroUnit.Count))
                            tempPointNum = GameManager.instance.heroUnit.Count;
                        else if (playerHero && (o.pointNum > GameManager.instance.enemyUnit.Count))
                            tempPointNum = GameManager.instance.enemyUnit.Count;
                    }

                    if (playerHero)
                    {
                        foreach (var i in GameManager.instance.enemyUnit)
                        {
                            tempUnits.Add(i);
                        }
                    }
                    else
                    {
                        foreach (var i in GameManager.instance.heroUnit)
                        {
                            tempUnits.Add(i);
                        }
                    }
                    for (int j = 0; j < tempPointNum; j++)
                    {
                        int k = Koubot.Tool.Random.RandomTool.GenerateRandomInt(0, tempUnits.Count - 1);
                        Debug.Log("k:" + k);
                        //Debug.Log("kou:"+ Koubot.Tool.Random.RandomTool.GenerateRandomInt(0, tempUnits.Count - 1));
                        tempPointUnits.Add(tempUnits[k]);
                        if (!o.reChoose)
                            tempUnits.Remove(tempUnits[k]);
                        yield return new WaitForSeconds(0.05f);
                    }
                    UseSkillSettle(o, tempPointUnits);

                }
                if (o.passivePoint == PassivePoint.MPlayersAuto)
                {
                    if (!o.reChoose)//ȷ��ѡ�������
                    {
                        if (playerHero && (o.pointNum > GameManager.instance.heroUnit.Count))
                            tempPointNum = GameManager.instance.heroUnit.Count;
                        else if (!playerHero && (o.pointNum > GameManager.instance.enemyUnit.Count))
                            tempPointNum = GameManager.instance.enemyUnit.Count;
                    }

                    if (!playerHero)
                    {
                        foreach (var i in GameManager.instance.enemyUnit)
                        {
                            tempUnits.Add(i);
                        }
                    }
                    else
                    {
                        foreach (var i in GameManager.instance.heroUnit)
                        {
                            tempUnits.Add(i);
                        }
                    }
                    for (int j = 0; j < tempPointNum; j++)
                    {
                        int k = Koubot.Tool.Random.RandomTool.GenerateRandomInt(0, tempUnits.Count - 1);
                        Debug.Log("k:" + k);
                        //Debug.Log("kou:" + Koubot.Tool.Random.RandomTool.GenerateRandomInt(0, tempUnits.Count - 1));
                        tempPointUnits.Add(tempUnits[k]);
                        if (!o.reChoose)
                            tempUnits.Remove(tempUnits[k]);
                        yield return new WaitForSeconds(0.05f);
                    }
                    UseSkillSettle(o,tempPointUnits);

                }
            }
        }
       
        
    }


    //��������������������������������������������������������������������������������������������������������������������

    public void FloatPointShow(int number,Color color)
    {
        if(player)
        {
            return;
        }
        floatPoint.transform.GetChild(0).GetComponent<TMP_Text>().text = number.ToString();
        floatPoint.transform.GetChild(0).GetComponent<TMP_Text>().color = color;
        Instantiate(floatPoint, this.transform.position + new Vector3(0, 1, 0), Quaternion.identity);
    }

    public void FloatTextShow(Unit unit, string text, Color color)//����������ʾ����
    {
        if (this.player)
        {
            return;
        }
        unit.floatSkill.transform.GetChild(0).GetComponent<TMP_Text>().text = text;
        unit.floatSkill.transform.GetChild(0).GetComponent<TMP_Text>().color = color;
        Instantiate(unit.floatSkill, unit.transform.position, Quaternion.identity);
    }
    public void FloatStateShow(Unit unit, string text, Color color)//����������ʾ����
    {
        if (player)
        {
            return;
        }
        unit.floatState.transform.GetChild(0).GetComponent<TMP_Text>().text = text;
        unit.floatState.transform.GetChild(0).GetComponent<TMP_Text>().color = color;
        Instantiate(unit.floatState, this.transform.position + new Vector3(0, 1, 0), Quaternion.identity);
    }

    //������������������������������������������������������ɫ��������������������������������������������������������������������


    public virtual void LevelUp2()
    {
        return;
    }
    public virtual void LevelUp3()
    {
        return;
    }
    public virtual void LevelUp4()
    {
        return;
    }
    public virtual void LevelUp5()
    {
        return;
    }
    public virtual void LevelUp6()
    {
        return;
    }
    public virtual void LevelUp7()
    {
        return;
    }
    public virtual void LevelUp8()
    {
        return;
    }
    public virtual void LevelUp9()
    {
        return;
    }
    public virtual void LevelUp10()
    {
        return;
    }
    public virtual void LevelUp11()
    {
        return;
    }
    public virtual void LevelUp12()
    {
        return;
    }
    public virtual void LevelUp13()
    {
        return;
    }
    public virtual void LevelUp14()
    {
        return;
    }
    public virtual void LevelUp15()
    {
        return;
    }
    public virtual void Awaking()
    {
        return;
    }
    //������������������������������������roll���ܺ�������������������������������������������������������
    private void SingleSkillRoll(List<int> fList,List<int> tempList)
    {
        if(tempList.Count>0)
        {
            int tempIndex = Koubot.Tool.Random.RandomTool.GenerateRandomInt(0, tempList.Count - 1);
            fList.Add(tempList[tempIndex]);
            tempList.Remove(tempList[tempIndex]);
        }
    }
    private void CloneList(List<int> a,List<int>b)
    {
        foreach(var x in a)
            b.Add(x);
    }
    public List<int> SkillRollList(int count,List<int> aList, int aPrecent,List<int> bList, int bPrecent)//roll���ܱ��
    {
        List<int> finalList = new List<int>();
        List<int> tempLista = new List<int>();
        CloneList(aList,tempLista);
        List<int> tempListb = new List<int>();
        CloneList(bList, tempListb);
        int a = aPrecent;
        int b = aPrecent+bPrecent;
        int rollInt;

        while (finalList.Count < count)
        {
            rollInt = Koubot.Tool.Random.RandomTool.GenerateRandomInt(0, 99);
            Debug.Log("rollInt:"+rollInt);
            if (rollInt<a)
            {
                SingleSkillRoll(finalList, tempLista);
            }
            else if(a<=rollInt&&rollInt<b)
            {
                SingleSkillRoll(finalList, tempListb);
            }
        }
        foreach (var temp in finalList)
            Debug.Log("���ܱ��:" + temp);
        return finalList;
    }
    public List<int> SkillRollList(int count, List<int> aList, int aPrecent, List<int> bList, int bPrecent, List<int> cList, int cPrecent)//roll���ܱ��
    {
        List<int> finalList = new List<int>();
        List<int> tempLista = new List<int>();
        CloneList(aList,tempLista);
        List<int> tempListb = new List<int>();
        CloneList(bList,tempListb);
        List<int> tempListc = new List<int>();
        CloneList(cList,tempListc); 
        int a = aPrecent;
        int b = aPrecent + bPrecent;
        int c = aPrecent + bPrecent + cPrecent;
        int rollInt;
        while(finalList.Count<count)
        {
            rollInt = Koubot.Tool.Random.RandomTool.GenerateRandomInt(0, 99);
            Debug.Log("rollInt:" + rollInt);
            if (rollInt < a)
            {
                SingleSkillRoll(finalList, tempLista);
            }
            else if (a <= rollInt && rollInt < b)
            {
                SingleSkillRoll(finalList, tempListb);
            }
            else if (b <= rollInt && rollInt < c)
            {
                SingleSkillRoll(finalList, tempListc);
            }
        }
        foreach (var temp in finalList)
            Debug.Log("���ܱ��:"+temp);
        return finalList;
    }
    //���������������������������������������ܼ��㺯������������������������������������������������������
    public int Decrease(int final,int decrease,float decreasePrecent)
    {
        int a = (int)((final - decrease) * (float)(100 - decreasePrecent) / 100);
        if (a < 0)
            a = 0;
        return a;
    }

    //������������������������������������״̬��������������������������������������������������������
    public void BurnDamage()
    {
        if (burn == 0)
            return;
        currentHP -= Decrease(burn,BurnDecrease,BurnPrecentDecrease);
        FloatPointShow(Decrease(burn, BurnDecrease, BurnPrecentDecrease), new Color32(231, 115, 49, 255));
        burn -= 3;
        if(burn < 0)
            burn = 0;
    }
    public void PoisonDamage()
    {
        if (poison == 0)
            return;
        currentHP -= Decrease(poison, PoisonDecrease,PoisonPrecentDecrease);
        FloatPointShow(Decrease(poison, PoisonDecrease, PoisonPrecentDecrease), new Color32(157,207,73, 255));
        poison -= 2;
        if (poison < 0)
            poison = 0;
    }
    public void ColdDecreaseDamage(ref int damage)
    {
        if (cold == 0)
            return;
        damage-= Decrease(cold, ColdDecrease, ColdPrecentDecrease);
        Debug.Log(damage);
        FloatStateShow(this,"����",new Color32(97,198,236,255));
        cold -= 5;
        if (cold < 0)
            cold = 0;
    }

    public void TurnBeginSettle()//�غϿ�ʼ״̬����
    {
        
        if (shield > 0)
            shield -= 5;
        if(shield <= 0)
            shield = 0;
        SettleAdjust();
    }
    public void TurnEndSettle()
    {
        if (burn > 0)
            burn -= 2;
        if(burn <= 0)
            burn = 0;

        if(cold > 0)
            cold -= 3;
        if (cold <= 0)
            cold = 0;

        if(sneer>0)
            sneer -= 1;
    }
     
    protected void SettleAdjust()
    {
        
        int temp = 0;
        int count = adjustTurn.Count;
        for (int i = 0; i < count; i++)
        {             
            if (adjustTurn[temp] <=GameManager.instance.turn)
            {
                SettleSingleAdjust(attributeAdjust[i], attributeAdjustPoint[i]);
                adjustTurn.Remove(adjustTurn[temp]);
                attributeAdjust.Remove(attributeAdjust[temp]);
                attributeAdjustPoint.Remove(attributeAdjustPoint[temp]);
            }
            else
                temp++;
        }
    }
    protected virtual void SettleSingleAdjust(HeroAttribute a,float point)//���Ե���
    {

        if (a == HeroAttribute.AP)
            AP += (int)point;
        else if (a == HeroAttribute.APDef)
            APDef += (int)point;
        else if (a == HeroAttribute.maxMP)
            maxMP += (int)point;
        else if (a == HeroAttribute.MP)
            currentMP += (int)point;
        else if (a == HeroAttribute.AD)
            AD += (int)point;
        else if (a == HeroAttribute.Def)
            Def += (int)point;
        else if (a == HeroAttribute.maxHP)
            maxHP += (int)point;
        else if (a == HeroAttribute.HP)
            currentHP += (int)point;
        else if (a == HeroAttribute.Spirit)
            Spirit += (int)point;
        else if (a == HeroAttribute.Critical)
            Critical += (int)point;
        else if (a == HeroAttribute.Dodge)
            Dodge += (int)point;
        else if (a == HeroAttribute.Tired)
            tired += (int)point;
        else if (a == HeroAttribute.Sneer)
            sneer += (int)point;
        else if (a == HeroAttribute.fragile)
            fragile += (int)point;
        else if (a == HeroAttribute.weakness)
            weakness += (int)point;
        else if (a == HeroAttribute.shieldDecrease)
            shieldDecrease += (int)point;
        else if (a == HeroAttribute.Burn)
            burn += (int)point;
        else if (a == HeroAttribute.Cold)
            cold += (int)point;
        else if (a == HeroAttribute.Poison)
            poison += (int)point;
        else if (a == HeroAttribute.ADDecrease)
            ADDecrease += (int)point;
        else if (a == HeroAttribute.ADPrecentDecrease)
            ADPrecentDecrease += (int)point;
        else if (a == HeroAttribute.APDecrease)
            APDecrease += (int)point;
        else if (a == HeroAttribute.APPrecentDecrease)
            APPrecentDecrease += (int)point;
        else if (a == HeroAttribute.BurnDecrease)
            BurnDecrease += (int)point;
        else if (a == HeroAttribute.BurnPrecentDecrease)
            BurnPrecentDecrease += (int)point;
        else if (a == HeroAttribute.PoisonDecrease)
            PoisonDecrease += (int)point;
        else if (a == HeroAttribute.PoisonPrecentDecrease)
            PoisonPrecentDecrease += (int)point;
        else if (a == HeroAttribute.ColdDecrease)
            ColdDecrease += (int)point;
        else if (a == HeroAttribute.ColdPrecentDecrease)
            ColdPrecentDecrease += (int)point;

    }
    public int SneerJudge()//�жϼ���������û���˳���
    {
        int sneerNum=0;
        if(playerHero)
        {
            foreach(var p in GameManager.instance.heroUnit)
            {
                if(p.sneer>0)
                {
                    sneerNum+=1;
                    break;
                }
            }
        }
        else if (!playerHero)
        {
            foreach (var p in GameManager.instance.enemyUnit)
            {
                if (p.sneer > 0)
                {
                    sneerNum +=1;
                    break;
                }
            }
        }
        return sneerNum;
    }//�ж϶�������û�г���

    //����������������������������������������¼���������������������������������������������������������
    private void OnMouseEnter()//����ѡ�񶯻�
    {
        if (GameManager.instance.backPanel.activeInHierarchy|| GameManager.instance.AbandomCardCheck.activeInHierarchy)
            return;
        if (GameManager.instance.state == BattleState.PLAYERTURN && playerHero)//������һغ�
        {
            if (tired == 0)
                anim.Play("choose");
        }
        if (point.activeInHierarchy|| cardPoint.activeInHierarchy)
             anim.Play("choose");

        

    }
    private void OnMouseExit()//�˳��ع�ԭ��
    {
        if (point.activeInHierarchy|| GameManager.instance.state == BattleState.PLAYERTURN)
           anim.Play("idle");
        if (cardPoint.activeInHierarchy || GameManager.instance.state == BattleState.PLAYERTURN)
            anim.Play("idle");

    }
    private void OnMouseDown()//���
    {
        if (GameManager.instance.backPanel.activeInHierarchy || GameManager.instance.AbandomCardCheck.activeInHierarchy)
            return;
        if (tired == 0)
        {
            if ((GameManager.instance.state == BattleState.PLAYERTURN && playerHero))
            {
                anim.Play("idle");
                GameManager.instance.SkillShow(this);//�����ɫ
                GameManager.instance.CardCanvas.SetActive(false);
            }
        }

        if (point.activeInHierarchy)
        {
            GameManager.instance.pointUnit.Add(this);//�����ӦԤ����
            if (!GameManager.instance.useSkill.reChoose)
                anim.Play("idle");
        }
        if (cardPoint.activeInHierarchy)
        {
            GameManager.instance.turnUnit.Add(this);//�����ӦԤ����
                anim.Play("idle");
            StartCoroutine( GameManager.instance.useSkill.JudgePlayerSkill());
        }

    }



    //����������������������������������������������������ʼ��������������������������������������������������������
    private void SetSkill()
    {
        //�����ԭ��Ԥ��ĳ�ʼ���ܣ��ٶ�ȡ�������ݵļ���
        heroSkillList.Clear();
        foreach (var code in heroSkillListCode)
            heroSkillList.Add(GameManager.instance.allListObject.GetComponent<AllList>().allSkillList[code]);

    }

    //������������������������������������������������������ȡɾ����������������������������������������������������������������

    /*string GetSaveNumber()
    {
        string saveNumber;
        string locatorName = "SaveLocator.sav";
        saveNumber = File.ReadAllText(Path.Combine(Application.persistentDataPath, locatorName));
        return saveNumber;
    }*/
    public void UnitSave()//�����浵
    {
        string unitSaveName = "Save_BattleHero_" + unitName + ".sav";
        SaveUnitData saveData = new SaveUnitData
        {
        skillNum = skillNum,
        unitLevel = unitLevel,


        AP= AP,
        APDef= APDef,
        maxMP= maxMP,

        AD= AD,
        Def= Def,
        maxHP= maxHP,

        Spirit= Spirit,
        Critical= Critical,
        Dodge= Dodge,

        currentHP= currentHP,
        currentExp= currentExp,

        ADDecrease = ADDecrease,
        ADPrecentDecrease= ADPrecentDecrease,
        APDecrease= APDecrease,
        APPrecentDecrease= APPrecentDecrease,
        BurnDecrease= BurnDecrease,
        BurnPrecentDecrease= BurnPrecentDecrease,
        PoisonDecrease= PoisonDecrease,
        PoisonPrecentDecrease= PoisonPrecentDecrease,
        ColdDecrease = ColdDecrease,
        ColdPrecentDecrease= ColdPrecentDecrease,
        currencyFightLSkillList= currencyFightLSkillList,
        currencyFightMSkillList= currencyFightMSkillList,
        currencyFightHSkillList=currencyFightHSkillList,
        exclusiveFightLSkillList= exclusiveFightLSkillList,
        exclusiveFightMSkillList= exclusiveFightMSkillList,
        exclusiveFightHSkillList= exclusiveFightHSkillList,
        heroSkillListCode = heroSkillListCode
        };

        SaveSystem.Save(unitSaveName, saveData);
    }

    public void getExpAndCurrentHp()//���浱ǰgetExp��Ѫ
    {
        string unitSaveName = "Save_BattleHero_" + unitName + ".sav";
        SaveUnitData saveData = SaveSystem.Load<SaveUnitData>(unitSaveName);
        saveData.currentHP = currentHP;
        saveData.getExp = getExp;
        SaveSystem.Save(unitSaveName, saveData);
    }
    private void UnitBeginLoad()//��ȡ�浵
    {      
        if(!UnitLoad())//�޴浵ʱ����ս�����ܳغͼ��ܱ�úŸ�ֵ���ٴ����浵
        {
            //�ȶ�ս�����ӵĸ�����б��ʼ��,�ٴ����ʼ����
            SetAllSkillList();
            UnitSave();         
        }

    }

    private void SetFightSkillList(List<Skill> tempSkills,List<int> tempCodes)//������Ԥ���б�ת���ɱ���б�
    {
        foreach(var s in tempSkills)
        {
            int Code = GameManager.instance.allListObject.GetComponent<AllList>().allSkillList.IndexOf(s);
            if(!heroSkillListCode.Contains(Code))
                 tempCodes.Add(Code);
        }
            
    }
    private void SetAllSkillList()
    {
        //�ȴ����м��ܽ�����б�
        foreach (var s in heroSkillList)
           heroSkillListCode.Add(GameManager.instance.allListObject.GetComponent<AllList>().allSkillList.IndexOf(s));
        SetFightSkillList(currencyLSkillList,currencyFightLSkillList);
        SetFightSkillList(currencyMSkillList, currencyFightMSkillList);
        SetFightSkillList(currencyHSkillList, currencyFightHSkillList);
        SetFightSkillList(exclusiveLSkillList, exclusiveFightLSkillList);
        SetFightSkillList(exclusiveMSkillList, exclusiveFightMSkillList);
        SetFightSkillList(exclusiveHSkillList, exclusiveFightHSkillList);
    }
    public void UnitDelete()//ɾ���浵
    {
        string saveUnitName = "Save_BattleHero_" + unitName + ".sav";
        SaveSystem.Delete(saveUnitName);
    }

    public bool UnitLoad() //��ȡ�浵����,���ڶ����ݵ���
    {
        bool loadSwitch=false;
        string saveUnitName = "Save_BattleHero_" + unitName + ".sav";
        SaveUnitData saveData;
        if (File.Exists(Path.Combine(Application.persistentDataPath, saveUnitName)))
        {
            loadSwitch=true;
            saveData = SaveSystem.Load<SaveUnitData>(saveUnitName);
            skillNum = saveData.skillNum;
            unitLevel = saveData.unitLevel;


            AP = saveData.AP;
            APDef = saveData.APDef;
            maxMP = saveData.maxMP;

            AD = saveData.AD;
            Def = saveData.Def;
            maxHP = saveData.maxHP;

            Spirit = saveData.Spirit;
            Critical = saveData.Critical;
            Dodge = saveData.Dodge;

            currentHP = saveData.currentHP;
            currentExp = saveData.currentExp;
            getExp = saveData.getExp;

            ADDecrease = saveData.ADDecrease;
            ADPrecentDecrease = saveData.ADPrecentDecrease;
            APDecrease = saveData.APDecrease;
            APPrecentDecrease = saveData.APPrecentDecrease;
            BurnDecrease = saveData.BurnDecrease;
            BurnPrecentDecrease = saveData.BurnPrecentDecrease;
            PoisonDecrease = saveData.PoisonDecrease;
            PoisonPrecentDecrease = saveData.PoisonPrecentDecrease;
            ColdDecrease = saveData.ColdDecrease;
            ColdPrecentDecrease = saveData.ColdPrecentDecrease;
            currencyFightLSkillList = saveData.currencyFightLSkillList;
            currencyFightMSkillList = saveData.currencyFightMSkillList;
            currencyFightHSkillList = saveData.currencyFightHSkillList;
            exclusiveFightLSkillList = saveData.exclusiveFightLSkillList;
            exclusiveFightMSkillList = saveData.exclusiveFightMSkillList;
            exclusiveFightHSkillList = saveData.exclusiveFightHSkillList;
            heroSkillListCode = saveData.heroSkillListCode;
        }
        return loadSwitch;
    }

    protected void MaxHpUp(int a)//���maxHp�ķ���
    {
        maxHP += a;
        currentHP += a;
    }
}
