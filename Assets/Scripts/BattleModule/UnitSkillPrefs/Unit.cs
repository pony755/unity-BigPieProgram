using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
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
    public int nextExp;
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

    [Header("״̬��")]
    public int tired;
    public int shield;//��
    public int fragile;//����
    public int weakness;//����
    public int shieldDecrease;//����
    public int healDecrease;//���ָ�
    public int burn;
    public int cold;
    public int poison;
    public int currentHP;
    public int currentMP;
    public int currentExp;
    public int getExp;

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

    [Header("���ܱ���б�")]
    public List<int> heroSkillListCode;

    [Header("����")]
    public List<Skill> heroSkillList;


    [Header("�Ƿ�Ϊ�������")]
    public bool player;

    [Header("���������б�")]
    public List<Skill> passiveTurnEndList;//�غϿ�ʼʱ����
    public List<Skill> passiveTurnStartList;//�غϿ�ʼʱ����
    public List<Skill> passiveHitList;//���˴���
    public List<Skill> passiveDeadList;//��������
    public List<Skill> passiveGameBeginList;//��������



    [System.Serializable]
    class SaveUnitData
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


        public int tired;
        public int shield;//��
        public int fragile;//����
        public int weakness;//����
        public int shieldDecrease;//����
        public int healDecrease;//���ָ�
        public int burn;
        public int cold;
        public int poison;
        public int currentHP;
        public int currentMP;
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


        public List<int> heroSkillListCode;
    }
    protected virtual void Awake()//���Գ�ʼ��
    {
        //��ʼ������
        //if (playerHero)
            //UnitLoad();
            SetSkill();

            
        
    }
    protected virtual void Start()
    {
        
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
        if (skill.delayedTurn > 0 && !GameManager.instance.delayedSwitch)
        {

            GameManager.instance.delayedTurn.Add(GameManager.instance.turn + skill.delayedTurn);
            GameManager.instance.delayedTurnUnit.Add(turnUnit);
            GameManager.instance.delayedSkill.Add(skill);
            GameManager.instance.delayedPointUnit.Add(this);
        }
        else
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
                skill.SkillSettleCard(turnUnit, this);
            if (skill.type == SkillType.AttributeAdjust)
                skill.SkillSettleAdjust(turnUnit, this);
            if (skill.type == SkillType.Excharge)
                skill.SkillSettleExchange(turnUnit, this);

        }
        yield return null;

    }

    public void SkillCost(Skill skill)
    {
        //���㷢������
        AttributeSettle(skill);
        currentMP -= skill.needMP;
        tired += skill.skillTired;
    }

    public void SkillSettle(Unit turnUnit, Skill skill)//���㼼��
    {

        turnUnit.SkillCost(skill);       
        //�����Ƿ�Ϊ��ʱ
        if (skill.delayedTurn > 0&&!GameManager.instance.delayedSwitch)
        {

            GameManager.instance.delayedTurn.Add(GameManager.instance.turn+skill.delayedTurn);
            GameManager.instance.delayedTurnUnit.Add(turnUnit);
            GameManager.instance.delayedSkill.Add(skill);
            GameManager.instance.delayedPointUnit.Add(this);
        }

        else
        {    

            if (skill.type == SkillType.Mix)
            {
                foreach (var o in skill.moreSkill)
                {
                    StartCoroutine( Settle(turnUnit, o));
                }
            }
            else
            {
                StartCoroutine(Settle(turnUnit, skill));
            }
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
            GameManager.instance.tips.text = "���� " + o.skillName + " ����MP����";
        }
        else if (GameManager.instance.Probility(o.precent))
        {
            GameManager.instance.tips.text = "ŷ���� ���� " + o.skillName + " ����ʧ��";
            SkillCost(o);

        }     
        else
        {
            bool Go = true;//�ж��Ƿ��������
            int tempPointNum = o.pointNum;
            List<Unit> tempUnits = new List<Unit>();
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
                        danger.SkillSettle(this, o);
                    }
                if (o.passivePoint == PassivePoint.MMyself)
                    this.SkillSettle(this, o);
                if (o.passivePoint == PassivePoint.MAllEnemy)
                {
                    if (playerHero)
                    {
                        for (int i = 0; i < GameManager.instance.enemyUnit.Count; i++)
                        {
                            GameManager.instance.enemyUnit[i].SkillSettle(this, o);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < GameManager.instance.heroUnit.Count; i++)
                        {
                            GameManager.instance.heroUnit[i].SkillSettle(this, o);
                        }
                    }

                }
                if (o.passivePoint == PassivePoint.MAllPlayers)
                {
                    if (!playerHero)
                    {
                        for (int i = 0; i < GameManager.instance.enemyUnit.Count; i++)
                        {
                            GameManager.instance.enemyUnit[i].SkillSettle(this, o);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < GameManager.instance.heroUnit.Count; i++)
                        {
                            GameManager.instance.heroUnit[i].SkillSettle(this, o);
                        }
                    }

                }
                if (o.passivePoint == PassivePoint.MEnemiesAuto)
                {

                    if (!o.reChoose)
                    {
                        if (playerHero && (o.pointNum > GameManager.instance.enemyUnit.Count))
                            tempPointNum = GameManager.instance.enemyUnit.Count;
                        else if (!playerHero && (o.pointNum > GameManager.instance.heroUnit.Count))
                            tempPointNum = GameManager.instance.heroUnit.Count;
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
                        tempUnits[k].SkillSettle(this, o);
                        if (!o.reChoose)
                            tempUnits.Remove(tempUnits[k]);
                        yield return new WaitForSeconds(0.05f);

                    }

                }
                if (o.passivePoint == PassivePoint.MPlayersAuto)
                {
                    if (!o.reChoose)
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
                        tempUnits[k].SkillSettle(this, o);
                        if (!o.reChoose)
                            tempUnits.Remove(tempUnits[k]);
                        yield return new WaitForSeconds(0.05f);

                    }

                }
            }
        }
       
        
    }


    //��������������������������������������������������������������������������������������������������������������������

    public void FloatPointShow(int number,Color color)
    {
        if(this.player)
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
        if (this.player)
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
    }



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
    public void SetSkill()
    {
        foreach (var code in heroSkillListCode)
        {
            heroSkillList.Add(GameManager.instance.allListObject.GetComponent<AllList>().allSkillList[code]);
        }
    }

    //������������������������������������������������������ȡɾ����������������������������������������������������������������

    /*string GetSaveNumber()
    {
        string saveNumber;
        string locatorName = "SaveLocator.sav";
        saveNumber = File.ReadAllText(Path.Combine(Application.persistentDataPath, locatorName));
        return saveNumber;
    }*/
    public void UnitCreat()//����浵
    {
        string unitSaveName = "Save_BattleHero_" + unitName + ".sav";
        SaveUnitData saveData = new SaveUnitData
        {
        skillNum = skillNum,
        unitLevel = unitLevel,
        nextExp= nextExp,

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
        currentMP= currentMP,
        currentExp= currentExp,
        getExp= getExp,


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
        heroSkillListCode= heroSkillListCode
        };

        SaveSystem.Save(unitSaveName, saveData);
    }

    public void UnitLoad()//���ش浵
    {
        string saveUnitName = "Save_BattleHero_" + unitName + ".sav";
        SaveUnitData saveData;
        if (File.Exists(Path.Combine(Application.persistentDataPath, saveUnitName)))
        {
            saveData = SaveSystem.Load<SaveUnitData>(saveUnitName);
            skillNum = saveData.skillNum;
            unitLevel = saveData.unitLevel;
            nextExp = saveData.nextExp;

            AP = saveData.AP;
            APDef = saveData.APDef;
            maxMP = saveData.maxMP;

            AD = saveData.AD;
            Def = saveData.Def;
            maxHP = saveData.maxHP;

            Spirit = saveData.Spirit;
            Critical = saveData.Critical;
            Dodge = saveData.Dodge;


            tired = saveData.tired;
            shield = saveData.shield;//��
            fragile = saveData.fragile;//����
            weakness = saveData.weakness;//����
            shieldDecrease = saveData.shieldDecrease;//����
            healDecrease = saveData.healDecrease;//���ָ�
            burn = saveData.burn;
            cold = saveData.cold;
            poison = saveData.poison;
            currentHP = saveData.currentHP;
            currentMP = saveData.currentMP;
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
            heroSkillListCode = saveData.heroSkillListCode;
        }
        else
        {
            UnitCreat();
        }

    }

    public void UnitDelete()//ɾ���浵
    {
        string saveUnitName = "Save_BattleHero_" + unitName + ".sav";
        SaveSystem.Delete(saveUnitName);
    }

}
