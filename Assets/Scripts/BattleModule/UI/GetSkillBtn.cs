using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GetSkillBtn : MonoBehaviour
{
    public Text skillName;
    public Image skillImg;
    public Text skillText;
    public int skillIndex;
    public Unit unit;
    public void LearnSkillCode()
    {
        unit.heroSkillListCode.Add(skillIndex);
        if (unit.currencyFightLSkillList.Contains(skillIndex))
            unit.currencyFightLSkillList.Remove(skillIndex);
        else if (unit.currencyFightMSkillList.Contains(skillIndex))
            unit.currencyFightMSkillList.Remove(skillIndex);
        else if (unit.currencyFightHSkillList.Contains(skillIndex))
            unit.currencyFightHSkillList.Remove(skillIndex);
        else if (unit.exclusiveFightLSkillList.Contains(skillIndex))
            unit.exclusiveFightLSkillList.Remove(skillIndex);
        else if (unit.exclusiveFightMSkillList.Contains(skillIndex))
            unit.exclusiveFightMSkillList.Remove(skillIndex);
        else if (unit.exclusiveFightHSkillList.Contains(skillIndex))
            unit.exclusiveFightHSkillList.Remove(skillIndex);
    }

}
