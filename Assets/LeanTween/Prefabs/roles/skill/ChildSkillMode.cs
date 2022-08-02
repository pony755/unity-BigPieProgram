using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
[CreateAssetMenu(fileName = "skill", menuName = "Create new child skill")]
public class ChildSkillMode : Skill
{
    public override void SkillSettleAD(Unit turnUnit,Unit pointUnit)
    {
        if (this.type == SkillType.AD)
        {
            bool add100=false;
            if(turnUnit.currentHP==turnUnit.maxHP)
            {
                this.baseInt +=100;
                add100=true;
            }   
            int damage = this.FinalPoint(turnUnit) - pointUnit.Def;
            if (damage < 0)
                damage = 0;

            if (damage > 0 && pointUnit.currentHP > 0)
            {
                pointUnit.damger = turnUnit;//暂时记录伤害来源
                pointUnit.currentHP -= damage;
                Debug.Log(pointUnit.unitName + "受到了" + damage + "点物理伤害");
                pointUnit.floatPoint.transform.GetChild(0).GetComponent<TMP_Text>().text = damage.ToString();
                pointUnit.floatPoint.transform.GetChild(0).GetComponent<TMP_Text>().color = Color.red;
                Instantiate(pointUnit.floatPoint, pointUnit.transform.position + new Vector3(0, 1, 0), Quaternion.identity);
                pointUnit.anim.Play("hit");
            }
            if(add100)
            {
                this.baseInt -=100;
                add100 = false;
            }
        }
    }
}
