using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public Animator anim;//����
    public BattleHub hub;//״̬��

    public bool playerHero;//�ж��ǲ��Ǽ�����ɫ
    public string unitName;
    public int unitLevel;

    public int tired;
    public int Atk;
    public int Def;

    public int maxHP;
    public int currentHP;

    [Header("����")]
    public List<Skill> heroSkillList;


    [Header("��Ϸ��������")]
    public int gameCode;//��Ϸ�ڽ�ɫ���
    public void Start()
    {
         anim=GetComponent<Animator>();
    }





    //---------------------------------��������---------------------------------------------
    public void Bling()
    {
        anim.Play("bling");
    }
    public void Idle()
    {
        anim.Play("idle");
    }


    public void HubUpdate()
    {
        hub.SetHub(this);
    }

    private void CheckDead()//�����жϽ��㺯��
    {
        if (currentHP <= 0)
        {
            currentHP = 0;
            this.GetComponent<BoxCollider>().enabled = false;//�ر���ײ��ű�
            if(GameManager.instance.playerUnit.Contains(this))
            {
                GameManager.instance.playerUnit.Remove(this);
            }
            if (GameManager.instance.enemyUnit.Contains(this))
            {
                GameManager.instance.enemyUnit.Remove(this);
            }
            anim.Play("dead");
        }
    }
    private void Settle(Unit turnUnit, Skill skill)
    {
        if(skill.type==skillType.AD)
        {
            Debug.Log(unitName + "�ܵ���" + (skill.finalPoint(turnUnit) - Def) + "�������˺�");
            currentHP=currentHP-(skill.finalPoint(turnUnit)-Def);

        }
        HubUpdate();
        CheckDead();
    }
    public void skillSettle(Unit turnUnit, Skill skill)//���㼼��
    {
        turnUnit.tired=turnUnit.tired+skill.skillTired;
        if(skill.type==skillType.Mix)
        {
            foreach(var o in skill.moreSkill)
            {
                Settle(turnUnit, o);
            }
        }
        else
        {
            Settle(turnUnit, skill);
        }
    }
    //����������������������������������������¼���������������������������������������������������������
    private void OnMouseEnter()//������˸
    {
        if(tired==0)
        {
            if (GameManager.instance.state == BattleState.PLAYERTURN && playerHero)//������һغ�
                anim.Play("bling");
        }
        


        if (GameManager.instance.state == BattleState.POINTENEMY && !playerHero&&!GameManager.instance.pointUnit.Contains(this))
            anim.Play("bling");
    }
    private void OnMouseExit()//�˳��ع�ԭ��
    {
        if (GameManager.instance.state == BattleState.PLAYERTURN && playerHero)
            anim.Play("idle");

        if (GameManager.instance.state == BattleState.POINTENEMY && !playerHero)
            anim.Play("idle");
    }
    private void OnMouseDown()//�����ʾ������
    {
        if (tired == 0)
        {
            if (GameManager.instance.state == BattleState.PLAYERTURN && playerHero)
            {
                anim.Play("idle");
                GameManager.instance.SkillShow(gameCode);//�����ɫ���
            }
        }
        

        if (GameManager.instance.state == BattleState.POINTENEMY && !playerHero)
        {
            if(!GameManager.instance.pointUnit.Contains(this))//�����б��ڣ�������б�
            {
                GameManager.instance.pointUnit.Add(GameManager.instance.enemyUnit[gameCode]);//�����Ӧ����Ԥ����
            }
           
        }
        
    }

}
