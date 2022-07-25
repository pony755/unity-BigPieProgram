using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillBtn : MonoBehaviour
{
    public Text skillText;
    public Skill skillInfo;//技能信息加载
    


    public void addSkillName()//获取使用的技能名,并且更改GameManager技能目标数量变量。判断接下来的状态
    {
        skillInfo.JudgePlayerSkill();
    }
  
}
