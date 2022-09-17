using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillBtn : MonoBehaviour
{
    public Image skillImg;
    [HideInInspector] public Skill skillInfo;//������Ϣ����


    private void Start()
    {
        skillImg.sprite = skillInfo.skillImg;
    }
    public void AddSkillName()//������ȡʹ�õļ�����,�ر�skillText�����Ҹ���GameManager����Ŀ�������������жϽ�������״̬
    {
        GameManager.instance.useSkill = skillInfo;//��ȡʹ�õļ�������    
        StartCoroutine(skillInfo.JudgePlayerSkill());
        GameManager.instance.skillText.SetActive(false);//�رռ�����ʾ��
    }
    public void ShowSkillText()
    {

        StartCoroutine(MixTypeTextColor());
        GameManager.instance.skillText.GetComponent<SkillText>().skillImg.sprite = skillImg.sprite;
        GameManager.instance.skillText.GetComponent<SkillText>().skillName.text = skillInfo.skillName;
        GameManager.instance.skillText.GetComponent<SkillText>().skillText.text = skillInfo.description;
        GameManager.instance.skillText.GetComponent<SkillText>().TextMP.text = "����:" + skillInfo.needMP.ToString();
        GameManager.instance.skillText.GetComponent<SkillText>().TextTired.text = "ƣ��:" + skillInfo.skillTired.ToString();
        GameManager.instance.skillText.GetComponent<SkillText>().TextFail.text = "ʧ����:" + skillInfo.precent.ToString() + "%";
        if (GameManager.instance.state == BattleState.SKILL)
            GameManager.instance.skillText.SetActive(true);
    }
    public void HideSkillText()
    {
        StartCoroutine(GameManager.instance.skillText.GetComponent<SkillText>().Reset());
        GameManager.instance.skillText.SetActive(false);
    }

    IEnumerator MixTypeTextColor()//�ж������Լ���ɫ
    {
        int tempIndex = 0;
        if (skillInfo.passiveSkill == true)
        {
            GameManager.instance.skillText.GetComponent<SkillText>().skillType[tempIndex].text = "[����]";
            GameManager.instance.skillText.GetComponent<SkillText>().skillType[tempIndex].color = new Color32(190, 190, 190, 255);
            tempIndex++;
        }
        if (skillInfo.delayedTurn > 0)
        {
            GameManager.instance.skillText.GetComponent<SkillText>().skillType[tempIndex].text = "[��ʱ]";
            GameManager.instance.skillText.GetComponent<SkillText>().skillType[tempIndex].color = Color.black;
            tempIndex++;
        }
        if (skillInfo.onlyOne)
        {
            GameManager.instance.skillText.GetComponent<SkillText>().skillType[tempIndex].text = "[�޶�]";
            GameManager.instance.skillText.GetComponent<SkillText>().skillType[tempIndex].color = Color.magenta;
            tempIndex++;
        }
        if (skillInfo.typeTag.Count > 0)//�б�ǩʱ���Ա�ǩΪ��
        {
            for (int i = 0; i < skillInfo.typeTag.Count; i++)
            {
                BaseType(skillInfo.typeTag[i], ref tempIndex);
                tempIndex++;
            }
        }
        else//����Ĭ������Ϊ��
            BaseType(skillInfo.type, ref tempIndex);

        yield return null;
    }
    private void SetBaseType(int index, string text, Color color)//��index������tag
    {
        GameManager.instance.skillText.GetComponent<SkillText>().skillType[index].text = text;
        GameManager.instance.skillText.GetComponent<SkillText>().skillType[index].color = color;
    }
    private void BaseType(SkillType tag,ref int index)
    {

        if (tag == SkillType.AD)
        {
            SetBaseType(index, "[����]", Color.red);
        }
        else if (tag == SkillType.AP)
        {
            SetBaseType(index, "[����]", Color.blue);
        }
        else if (tag == SkillType.Heal)
        {
            SetBaseType(index, "[����]", Color.green);
        }
        else if (tag == SkillType.Shield)
        {
            SetBaseType(index, "[����]", Color.white);
        }
        else if (tag == SkillType.Burn)
        {
            SetBaseType(index, "[ȼ��]", new Color32(231, 115, 49, 255));
        }
        else if (tag == SkillType.Cold)
        {
            SetBaseType(index, "[����]", new Color32(97, 198, 236, 255));
        }
        else if (tag == SkillType.Poison)
        {
            SetBaseType(index, "[�ж�]", new Color32(157, 207, 73, 255));
        }
        else if (tag == SkillType.ReallyDamage)
        {
            SetBaseType(index, "[����]", Color.cyan);
        }
        else
            index--;
    }
}

