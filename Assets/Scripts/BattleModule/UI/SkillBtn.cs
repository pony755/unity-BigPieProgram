using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillBtn : MonoBehaviour
{
    public Image skillImg;
    [HideInInspector]public Skill skillInfo;//������Ϣ����


    private void Start()
    {
        skillImg.sprite=skillInfo.skillImg;
    }
    public void addSkillName()//������ȡʹ�õļ�����,�ر�skillText�����Ҹ���GameManager����Ŀ�������������жϽ�������״̬
    {
        GameManager.instance.useSkill = skillInfo;//��ȡʹ�õļ�������
        StartCoroutine(skillInfo.JudgePlayerSkill());  
        GameManager.instance.skillText.SetActive(false);//�رռ�����ʾ��
    }
    public void ShowSkillText()
    {
        if (skillInfo.type != skillType.Mix)
            StartCoroutine(TypeTextColor());
        else
            StartCoroutine(MixTypeTextColor());
        GameManager.instance.skillText.GetComponent<SkillText>().skillImg=skillImg;
        GameManager.instance.skillText.GetComponent<SkillText>().skillName.text=skillInfo.skillName;
        GameManager.instance.skillText.GetComponent<SkillText>().skillText.text ="         "+ skillInfo.description;
        if (GameManager.instance.state == BattleState.SKILL)
            GameManager.instance.skillText.SetActive(true);
    }
    public void HideSkillText()
    {
        StartCoroutine(GameManager.instance.skillText.GetComponent<SkillText>().Reset());
        GameManager.instance.skillText.SetActive(false);
    }
    IEnumerator TypeTextColor()//�ж������Լ���ɫ
    {
        int tempIndex = 0;
        if(skillInfo.passiveType!=passiveType.None)
        {
            GameManager.instance.skillText.GetComponent<SkillText>().skillType[tempIndex].text = "[����]";
            GameManager.instance.skillText.GetComponent<SkillText>().skillType[tempIndex].color = new Color32(190, 190, 190, 255);
            tempIndex++;
        }
        if (skillInfo.delayedTurn>0)
        {
            GameManager.instance.skillText.GetComponent<SkillText>().skillType[tempIndex].text = "[��ʱ]";
            GameManager.instance.skillText.GetComponent<SkillText>().skillType[tempIndex].color = Color.black;
            tempIndex++;
        }
       BaseType(skillInfo,tempIndex);
       
        yield return null;
    }
    IEnumerator MixTypeTextColor()//�ж������Լ���ɫ
    {
        int tempIndex = 0;
        List<skillType> tempList = new List<skillType>();//��ʱ�洢�Ӽ������ͣ�����Ӽ���������ӵ�У�������BaseType����
        if (skillInfo.passiveType != passiveType.None)
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
        
        foreach(var p in skillInfo.moreSkill)
        {
            
            if(!tempList.Contains(p.type))
            {              
                BaseType(p,tempIndex);
                tempList.Add(p.type);
                tempIndex++;
            }
        }
        yield return null;
    }


    private void BaseType(Skill skillInMix, int index)
    {
        
        if (skillInMix.type == skillType.AD)
        {
            GameManager.instance.skillText.GetComponent<SkillText>().skillType[index].text = "[����]";
            GameManager.instance.skillText.GetComponent<SkillText>().skillType[index].color = Color.red;
        }
        if (skillInMix.type == skillType.AP)
        {
            
            GameManager.instance.skillText.GetComponent<SkillText>().skillType[index].text = "[����]";
            GameManager.instance.skillText.GetComponent<SkillText>().skillType[index].color = Color.blue;
        }
        if (skillInMix.type == skillType.Heal)
        {
            GameManager.instance.skillText.GetComponent<SkillText>().skillType[index].text = "[����]";
            GameManager.instance.skillText.GetComponent<SkillText>().skillType[index].color = Color.green;
        }
        if (skillInMix.type == skillType.Shield)
        {
            GameManager.instance.skillText.GetComponent<SkillText>().skillType[index].text = "[����]";
            GameManager.instance.skillText.GetComponent<SkillText>().skillType[index].color = Color.grey;
        }
        if (skillInMix.type == skillType.Burn)
        {
            GameManager.instance.skillText.GetComponent<SkillText>().skillType[index].text = "[ȼ��]";
            GameManager.instance.skillText.GetComponent<SkillText>().skillType[index].color = new Color32(231, 115, 49, 255);
        }
        if (skillInMix.type == skillType.Cold)
        {
            GameManager.instance.skillText.GetComponent<SkillText>().skillType[index].text = "[����]";
            GameManager.instance.skillText.GetComponent<SkillText>().skillType[index].color = new Color32(97, 198, 236, 255);
        }
        if (skillInMix.type == skillType.Poison)
        {
            GameManager.instance.skillText.GetComponent<SkillText>().skillType[index].text = "[�ж�]";
            GameManager.instance.skillText.GetComponent<SkillText>().skillType[index].color = new Color32(157, 207, 73, 255);
        }
    }
    
}

