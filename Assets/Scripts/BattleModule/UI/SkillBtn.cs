using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillBtn : MonoBehaviour
{
    public Image skillImg;
    [HideInInspector]public Skill skillInfo;//技能信息加载


    private void Start()
    {
        skillImg.sprite=skillInfo.skillImg;
    }
    public void AddSkillName()//点击后获取使用的技能名,关闭skillText，并且更改GameManager技能目标数量变量。判断接下来的状态
    {
        GameManager.instance.useSkill = skillInfo;//读取使用的技能索引    
        StartCoroutine(skillInfo.JudgePlayerSkill());  
        GameManager.instance.skillText.SetActive(false);//关闭技能提示框
    }
    public void ShowSkillText()
    {
        if (skillInfo.type != SkillType.Mix)
            StartCoroutine(TypeTextColor());
        else
            StartCoroutine(MixTypeTextColor());
        GameManager.instance.skillText.GetComponent<SkillText>().skillImg.sprite=skillImg.sprite;
        GameManager.instance.skillText.GetComponent<SkillText>().skillName.text=skillInfo.skillName;
        GameManager.instance.skillText.GetComponent<SkillText>().skillText.text =skillInfo.description;
        GameManager.instance.skillText.GetComponent<SkillText>().TextMP.text = "耗蓝:" + skillInfo.needMP.ToString();
        GameManager.instance.skillText.GetComponent<SkillText>().TextTired.text = "疲劳:" + skillInfo.skillTired.ToString();
        GameManager.instance.skillText.GetComponent<SkillText>().TextFail.text = "失败率:" + skillInfo.precent.ToString()+"%";
        if (GameManager.instance.state == BattleState.SKILL)
            GameManager.instance.skillText.SetActive(true);
    }
    public void HideSkillText()
    {
        StartCoroutine(GameManager.instance.skillText.GetComponent<SkillText>().Reset());
        GameManager.instance.skillText.SetActive(false);
    }
    IEnumerator TypeTextColor()//判断类型以及颜色
    {
        int tempIndex = 0;
        if(skillInfo.passiveType!=PassiveType.None)
        {
            GameManager.instance.skillText.GetComponent<SkillText>().skillType[tempIndex].text = "[被动]";
            GameManager.instance.skillText.GetComponent<SkillText>().skillType[tempIndex].color = new Color32(190, 190, 190, 255);
            tempIndex++;
        }
        if (skillInfo.delayedTurn>0)
        {
            GameManager.instance.skillText.GetComponent<SkillText>().skillType[tempIndex].text = "[延时]";
            GameManager.instance.skillText.GetComponent<SkillText>().skillType[tempIndex].color = Color.black;
            tempIndex++;
        }
       BaseType(skillInfo,tempIndex);
       
        yield return null;
    }
    IEnumerator MixTypeTextColor()//判断类型以及颜色
    {
        int tempIndex = 0;
        List<SkillType> tempList = new List<SkillType>();//临时存储子技能类型，如果子技能类型已拥有，则跳过BaseType结算
        if (skillInfo.passiveType != PassiveType.None)
        {
            GameManager.instance.skillText.GetComponent<SkillText>().skillType[tempIndex].text = "[被动]";
            GameManager.instance.skillText.GetComponent<SkillText>().skillType[tempIndex].color = new Color32(190, 190, 190, 255);
            tempIndex++;
        }
        if (skillInfo.delayedTurn > 0)
        {
            GameManager.instance.skillText.GetComponent<SkillText>().skillType[tempIndex].text = "[延时]";
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
        
        if (skillInMix.type == SkillType.AD)
        {
            GameManager.instance.skillText.GetComponent<SkillText>().skillType[index].text = "[物理]";
            GameManager.instance.skillText.GetComponent<SkillText>().skillType[index].color = Color.red;
        }
        if (skillInMix.type == SkillType.AP)
        {
            
            GameManager.instance.skillText.GetComponent<SkillText>().skillType[index].text = "[法术]";
            GameManager.instance.skillText.GetComponent<SkillText>().skillType[index].color = Color.blue;
        }
        if (skillInMix.type == SkillType.Heal)
        {
            GameManager.instance.skillText.GetComponent<SkillText>().skillType[index].text = "[治疗]";
            GameManager.instance.skillText.GetComponent<SkillText>().skillType[index].color = Color.green;
        }
        if (skillInMix.type == SkillType.Shield)
        {
            GameManager.instance.skillText.GetComponent<SkillText>().skillType[index].text = "[护盾]";
            GameManager.instance.skillText.GetComponent<SkillText>().skillType[index].color = Color.white;
        }
        if (skillInMix.type == SkillType.Burn)
        {
            GameManager.instance.skillText.GetComponent<SkillText>().skillType[index].text = "[燃烧]";
            GameManager.instance.skillText.GetComponent<SkillText>().skillType[index].color = new Color32(231, 115, 49, 255);
        }
        if (skillInMix.type == SkillType.Cold)
        {
            GameManager.instance.skillText.GetComponent<SkillText>().skillType[index].text = "[冰冻]";
            GameManager.instance.skillText.GetComponent<SkillText>().skillType[index].color = new Color32(97, 198, 236, 255);
        }
        if (skillInMix.type == SkillType.Poison)
        {
            GameManager.instance.skillText.GetComponent<SkillText>().skillType[index].text = "[中毒]";
            GameManager.instance.skillText.GetComponent<SkillText>().skillType[index].color = new Color32(157, 207, 73, 255);
        }
    }
    
}

