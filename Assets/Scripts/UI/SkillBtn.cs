using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillBtn : MonoBehaviour
{
    public Text skillText;
    public Skill skillInfo;//������Ϣ����
    


    public void addSkillName()//��ȡʹ�õļ�����,���Ҹ���GameManager����Ŀ�������������жϽ�������״̬
    {
        skillInfo.JudgePlayerSkill();
    }
  
}
