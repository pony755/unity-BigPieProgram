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
        
        delayedText.text = delayedTextString;
        this.gameObject.SetActive(true);
        return delayedTextString;
    }
    public void Textclear()
    {
        this.gameObject.SetActive(false);     
        nameText.text = "";
        delayedText.text = "";
        delayedTextString = "";
        tempUnit = null;

    }

    public bool TipsDelayedShow(Unit unit)
    {
        bool show=false;
        if(unit.adjustCount.Count>0)
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
    /*private string TipsDelayedSingleAdjust(int turn,HeroAttribute a,int point)
    {
        string tempString = "";
        if (a == HeroAttribute.AP)
            tempString=turn+"回合后,法强"+point
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
        return tempString;
    }*/
}
