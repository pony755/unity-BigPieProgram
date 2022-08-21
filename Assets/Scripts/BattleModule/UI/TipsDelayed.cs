using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TipsDelayed : MonoBehaviour
{
    public Unit tempUnit;
    public TMP_Text nameText;
    public TMP_Text delayedText;
    private string delayedTextString;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(tempUnit != null)
        {
            if (!TipsDelayedShow(tempUnit))
            {
                tempUnit = null;
                gameObject.SetActive(false);

            }
        }
        
    }

    public string TextSet(Unit unit)
    {
        tempUnit=unit;
        nameText.text = unit.unitName;
        int tempTurnNum = 0;
        for(int i = 0; i < GameManager.instance.delayedTurnUnit.Count; i++)
        {
            if (GameManager.instance.delayedTurnUnit[i] == unit)
            {
                tempTurnNum = GameManager.instance.delayedTurn[i] - GameManager.instance.turn;
                if (tempTurnNum < 0)
                    tempTurnNum = 0;
                delayedTextString += tempTurnNum+ "回合后 释放技能[" + GameManager.instance.delayedSkill[i].skillName + "]" + "\n";
            }
                
        }
        for (int j = 0; j < unit.adjustTurn.Count; j++)
        {
            delayedTextString += TipsDelayedAttributeSingle(unit.adjustTurn[j],unit.attributeAdjust[j],unit.attributeAdjustPoint[j]);
        }
        delayedText.text = delayedTextString;
        this.gameObject.SetActive(true);
        return delayedTextString;
    }
    public void Textclear()
    {
        gameObject.SetActive(false);     
        nameText.text = "";
        delayedText.text = "";
        delayedTextString = "";
        tempUnit = null;

    }

    public bool TipsDelayedShow(Unit unit)
    {
        bool show=false;
        if(unit.adjustTurn.Count>0)
            show = true;
        for (int i = 0; i < GameManager.instance.delayedTurnUnit.Count;i++)
        {
            if (GameManager.instance.delayedTurnUnit[i] == unit)
            {
                show = true;
                break;
            }
              
        }
        
        return show;
    }
    /*private string TipsDelayedAdjust(Unit unit)
    {
        string tempString="";
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
        return tempString;
    }*/
    private string TipsDelayedAttributeSingle(int turn,HeroAttribute a,float point)
    {
        string text = "";
        int tempTurnNum;
        tempTurnNum = turn - GameManager.instance.turn;
        if (tempTurnNum < 0)
            tempTurnNum = 0;
        if (point > 0)
            text = tempTurnNum + "回合后," + AttributeToString(a) + "增加" + point;
        else if (point < 0)
            text = tempTurnNum + "回合后," + AttributeToString(a) + "减少"+Mathf.Abs( point);
        return text;
    }
    private string AttributeToString(HeroAttribute a)
    {
        string tempString = "";
        if (a == HeroAttribute.AP)
            tempString = "法强";
        else if (a == HeroAttribute.APDef)
            tempString = "法抗";
        else if (a == HeroAttribute.maxMP)
            tempString = "法力上限";
        else if (a == HeroAttribute.MP)
            tempString = "法力值";
        else if (a == HeroAttribute.AD)
            tempString = "物强";
        else if (a == HeroAttribute.Def)
            tempString = "物抗";
        else if (a == HeroAttribute.maxHP)
            tempString = "生命上限";
        else if (a == HeroAttribute.HP)
            tempString = "生命值";
        else if (a == HeroAttribute.Spirit)
            tempString = "人相";
        else if (a == HeroAttribute.Critical)
            tempString = "精准率";
        else if (a == HeroAttribute.Dodge)
            tempString = "闪避率";
        else if (a == HeroAttribute.Tired)
            tempString = "疲劳";
        else if (a == HeroAttribute.fragile)
            tempString = "易伤";
        else if (a == HeroAttribute.weakness)
            tempString = "虚弱";
        else if (a == HeroAttribute.shieldDecrease)
            tempString = "护盾减免";
        else if (a == HeroAttribute.healDecrease)
            tempString = "治疗减免";
        else if (a == HeroAttribute.Burn)
            tempString = "烧伤";
        else if (a == HeroAttribute.Cold)
            tempString = "冰冻";
        else if (a == HeroAttribute.Poison)
            tempString = "中毒";
        else if (a == HeroAttribute.ADDecrease)
            tempString = "物理减免";
        else if (a == HeroAttribute.ADPrecentDecrease)
            tempString = "物理减免(百分比)";
        else if (a == HeroAttribute.APDecrease)
            tempString = "法强减免";
        else if (a == HeroAttribute.APPrecentDecrease)
            tempString = "法强减免(百分比)";
        else if (a == HeroAttribute.BurnDecrease)
            tempString = "烧伤减免";
        else if (a == HeroAttribute.BurnPrecentDecrease)
            tempString = "烧伤减免(百分比)";
        else if (a == HeroAttribute.PoisonDecrease)
            tempString = "中毒减免";
        else if (a == HeroAttribute.PoisonPrecentDecrease)
            tempString = "中毒减免(百分比)";
        else if (a == HeroAttribute.ColdDecrease)
            tempString = "冰冻减免";
        else if (a == HeroAttribute.ColdPrecentDecrease)
            tempString = "冰冻减免(百分比)";
        else if (a == HeroAttribute.Sneer)
            tempString = "嘲讽";
        return tempString;
    }
}
