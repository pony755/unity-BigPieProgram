using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Unit : MonoBehaviour
{
    [HideInInspector]public Animator anim;//����
    [HideInInspector]public Unit damger;//��ʱ��¼�˺���Դ
    public Sprite normalSprite;
    public GameObject point;//ָ��
    public GameObject floatPoint;//�˺�
    public GameObject floatSkill;//���ܸ���
    public bool playerHero;//�ж��ǲ��Ǽ�����ɫ
    public string unitName;
    public int unitLevel;

    public int tired;
    public int Atk;
    public int Def;

    public int maxHP;
    public int currentHP;
    public int maxMP;
    public int currentMP;
    public int currentExp;
    public int nextExp;
    public int getExp;

    [Header("����")]
    public List<Skill> heroSkillList;

    [Header("���������б�")]
    public List<Skill> passiveTurnEndList;//�غϿ�ʼʱ����
    public List<Skill> passiveTurnStartList;//�غϿ�ʼʱ����
    public List<Skill> passiveHitList;//���˴���
    public List<Skill> passiveDeadList;//��������
    public List<Skill> passiveGameBeginList;//��������

    private void Start()
    {
        

        anim = GetComponent<Animator>();
        StartCoroutine(SetPassive());

    }

    private void Update()
    {
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
            skill.SkillSettleAD(turnUnit,this);

        }
        yield return null;

    }
   

       
     
    public void SkillSettle(Unit turnUnit, Skill skill)//���㼼��
    {
        for (int i=0;i< skill.attributeCost.Count;i++)//���㷢������
        {
            if (skill.attributeCost[i] == HeroAttribute.Atk)
                turnUnit.Atk +=  skill.skillCost[i];
            else if (skill.attributeCost[i] == HeroAttribute.HP)
                turnUnit.currentHP +=  skill.skillCost[i];
        }
        turnUnit.currentMP -=  skill.needMP;
        turnUnit.tired+=skill.skillTired;
        
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
    public void PassiveTurnEnd()//�غϿ�ʼʱ
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
        damger = null;
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

        bool Go=true;//�ж��Ƿ��������
        int tempPointNum = o.pointNum;
        List<Unit> tempUnits = new List<Unit>();
        System.Random r = new System.Random();
        if (o.passiveTurn==PassiveTurn.E)
        {
            if (!(((GameManager.instance.state == BattleState.ENEMYTURN || GameManager.instance.state == BattleState.ENEMYTURNSTART || GameManager.instance.state == BattleState.ENEMYFINISH) && this.playerHero)
             || (GameManager.instance.state == BattleState.ACTIONFINISH || GameManager.instance.state == BattleState.PLAYERTURNSTART) && !this.playerHero))
                Go = false;
        }
        else if(o.passiveTurn==PassiveTurn.M)
        {
            if(!(((GameManager.instance.state == BattleState.ENEMYTURN || GameManager.instance.state == BattleState.ENEMYTURNSTART || GameManager.instance.state == BattleState.ENEMYFINISH) && !this.playerHero)
            || ((GameManager.instance.state == BattleState.ACTIONFINISH || GameManager.instance.state == BattleState.PLAYERTURNSTART) && this.playerHero)))
                Go=false;
        }

        if(Go)
        {        
            FloatSkillShow(this, o, new Color32(190, 190, 190, 255));

            if (o.passivePoint == PassivePoint.MDamager)
                if (damger != null)
                {             
                    damger.SkillSettle(this, o);
                }                   
            if (o.passivePoint == PassivePoint.MMyself)
                this.SkillSettle(this, o);
            if (o.passivePoint == PassivePoint.MAllEnemy)
            {
                    Debug.Log("sb");
                    if(playerHero)
                    {
                        for (int i = 0; i < GameManager.instance.enemyUnit.Count; i++)
                        {
                            GameManager.instance.enemyUnit[i].SkillSettle(this, o);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < GameManager.instance.playerUnit.Count; i++)
                        {
                            GameManager.instance.playerUnit[i].SkillSettle(this, o);
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
                        for (int i = 0; i < GameManager.instance.playerUnit.Count; i++)
                        {
                            GameManager.instance.playerUnit[i].SkillSettle(this, o);
                        }
                    }

                }
            if (o.passivePoint == PassivePoint.MEnemiesAuto)
            {

                if (!o.reChoose)
                {
                    if (playerHero && (o.pointNum > GameManager.instance.enemyUnit.Count))
                        tempPointNum = GameManager.instance.enemyUnit.Count;
                    else if (!playerHero && (o.pointNum > GameManager.instance.playerUnit.Count))
                        tempPointNum = GameManager.instance.playerUnit.Count;
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
                    foreach (var i in GameManager.instance.playerUnit)
                    {
                        tempUnits.Add(i);
                    }
                }
                for (int j = 0; j < tempPointNum; j++)
                {
                    int k = r.Next(tempUnits.Count);
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
                    if (playerHero && (o.pointNum > GameManager.instance.playerUnit.Count))
                        tempPointNum = GameManager.instance.playerUnit.Count;
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
                    foreach (var i in GameManager.instance.playerUnit)
                    {
                        tempUnits.Add(i);
                    }
                }
                for (int j = 0; j < tempPointNum; j++)
                {
                    int k = r.Next(tempUnits.Count);
                    tempUnits[k].SkillSettle(this, o);
                    if (!o.reChoose)
                        tempUnits.Remove(tempUnits[k]);
                    yield return new WaitForSeconds(0.05f);

                }

            }
        }
        
    }

    public void FloatSkillShow(Unit unit, Skill skill, Color color)//����������ʾ����
    {
        unit.floatSkill.transform.GetChild(0).GetComponent<TMP_Text>().text = skill.skillName;
        unit.floatSkill.transform.GetChild(0).GetComponent<TMP_Text>().color = color;
        Instantiate(unit.floatSkill, unit.transform.position, Quaternion.identity);
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


    //����������������������������������������¼���������������������������������������������������������
    private void OnMouseEnter()//����ѡ�񶯻�
    {
        if (GameManager.instance.backPanel.activeInHierarchy)
            return;
        if (GameManager.instance.state == BattleState.PLAYERTURN && playerHero)//������һغ�
        {
            if (tired == 0)
                anim.Play("choose");
        }
        if (point.activeInHierarchy)
             anim.Play("choose");

        

    }
    private void OnMouseExit()//�˳��ع�ԭ��
    {
        if (point.activeInHierarchy|| GameManager.instance.state == BattleState.PLAYERTURN)
           anim.Play("idle");

    }
    private void OnMouseDown()//���
    {
        if (GameManager.instance.backPanel.activeInHierarchy)
            return;
        if (tired == 0)
        {
            if ((GameManager.instance.state == BattleState.PLAYERTURN && playerHero))
            {
                anim.Play("idle");
                GameManager.instance.SkillShow(this);//�����ɫ
            }
        }

        if (point.activeInHierarchy)
        {
            GameManager.instance.pointUnit.Add(this);//�����ӦԤ����
            if (!GameManager.instance.useSkill.reChoose)
                anim.Play("idle");
        }
   
    }

}
