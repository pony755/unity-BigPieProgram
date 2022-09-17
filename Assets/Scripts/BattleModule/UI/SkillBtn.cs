using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillBtn : MonoBehaviour
{
    public Image skillImg;
    [HideInInspector] public Skill skillInfo;//技能信息加载


    private void Start()
    {
        skillImg.sprite = skillInfo.skillImg;
    }
    public void AddSkillName()//点击后获取使用的技能名,关闭skillText，并且更改GameManager技能目标数量变量。判断接下来的状态
    {
        GameManager.instance.useSkill = skillInfo;//读取使用的技能索引    
        StartCoroutine(skillInfo.JudgePlayerSkill());
        GameManager.instance.skillText.SetActive(false);//关闭技能提示框
    }
    public void ShowSkillText()
    {

        StartCoroutine(MixTypeTextColor());
        GameManager.instance.skillText.GetComponent<SkillText>().skillImg.sprite = skillImg.sprite;
        GameManager.instance.skillText.GetComponent<SkillText>().skillName.text = skillInfo.skillName;
        GameManager.instance.skillText.GetComponent<SkillText>().skillText.text = skillInfo.description;
        GameManager.instance.skillText.GetComponent<SkillText>().TextMP.text = "耗蓝:" + skillInfo.needMP.ToString();
        GameManager.instance.skillText.GetComponent<SkillText>().TextTired.text = "疲劳:" + skillInfo.skillTired.ToString();
        GameManager.instance.skillText.GetComponent<SkillText>().TextFail.text = "失败率:" + skillInfo.precent.ToString() + "%";
        if (GameManager.instance.state == BattleState.SKILL)
            GameManager.instance.skillText.SetActive(true);
    }
    public void HideSkillText()
    {
        StartCoroutine(GameManager.instance.skillText.GetComponent<SkillText>().Reset());
        GameManager.instance.skillText.SetActive(false);
    }

    IEnumerator MixTypeTextColor()//判断类型以及颜色
    {
        int tempIndex = 0;
        if (skillInfo.passiveSkill == true)
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
        if (skillInfo.onlyOne)
        {
            GameManager.instance.skillText.GetComponent<SkillText>().skillType[tempIndex].text = "[限定]";
            GameManager.instance.skillText.GetComponent<SkillText>().skillType[tempIndex].color = Color.magenta;
            tempIndex++;
        }
        if (skillInfo.typeTag.Count > 0)//有标签时，以标签为主
        {
            for (int i = 0; i < skillInfo.typeTag.Count; i++)
            {
                BaseType(skillInfo.typeTag[i], ref tempIndex);
                tempIndex++;
            }
        }
        else//技能默认属性为主
            BaseType(skillInfo.type, ref tempIndex);

        yield return null;
    }
    private void SetBaseType(int index, string text, Color color)//给index框添上tag
    {
        GameManager.instance.skillText.GetComponent<SkillText>().skillType[index].text = text;
        GameManager.instance.skillText.GetComponent<SkillText>().skillType[index].color = color;
    }
    private void BaseType(SkillType tag,ref int index)
    {

        if (tag == SkillType.AD)
        {
            SetBaseType(index, "[物理]", Color.red);
        }
        else if (tag == SkillType.AP)
        {
            SetBaseType(index, "[法术]", Color.blue);
        }
        else if (tag == SkillType.Heal)
        {
            SetBaseType(index, "[治疗]", Color.green);
        }
        else if (tag == SkillType.Shield)
        {
            SetBaseType(index, "[护盾]", Color.white);
        }
        else if (tag == SkillType.Burn)
        {
            SetBaseType(index, "[燃烧]", new Color32(231, 115, 49, 255));
        }
        else if (tag == SkillType.Cold)
        {
            SetBaseType(index, "[冰冻]", new Color32(97, 198, 236, 255));
        }
        else if (tag == SkillType.Poison)
        {
            SetBaseType(index, "[中毒]", new Color32(157, 207, 73, 255));
        }
        else if (tag == SkillType.ReallyDamage)
        {
            SetBaseType(index, "[真伤]", Color.cyan);
        }
        else
            index--;
    }
}

