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
        if(!TipsDelayedShow(tempUnit))
        {
            tempUnit = null;
            this.gameObject.SetActive(false);

        }
    }

    public string TextSet(Unit unit)
    {
        tempUnit = unit;
        nameText.text = unit.unitName;
        for(int i = 0; i < GameManager.instance.delayedTurnUnit.Count; i++)
        {
            if (GameManager.instance.delayedTurnUnit[i] == unit && GameManager.instance.delayedSkill[i].type != SkillType.AttributeAdjust)
                delayedTextString += "第" + GameManager.instance.delayedTurn[i] + "回合 释放技能[" + GameManager.instance.delayedSkill[i].skillName + "]"+"\n";
        }
        for (int i = 0; i < GameManager.instance.delayedPointUnit.Count; i++)
        {
            if (GameManager.instance.delayedPointUnit[i] == unit && GameManager.instance.delayedSkill[i].type == SkillType.AttributeAdjust)
                delayedTextString += "第" + GameManager.instance.delayedTurn[i] + "回合 " + GameManager.instance.delayedSkill[i].skillName+"\n";
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
        for(int i = 0; i < GameManager.instance.delayedPointUnit.Count;i++)
        {
            if ((GameManager.instance.delayedTurnUnit[i] == unit && GameManager.instance.delayedSkill[i].type != SkillType.AttributeAdjust) || (GameManager.instance.delayedPointUnit[i] == unit && GameManager.instance.delayedSkill[i].type == SkillType.AttributeAdjust))
            {
                show = true;
                break;
            }
              
        }
        return show;
    }
}
