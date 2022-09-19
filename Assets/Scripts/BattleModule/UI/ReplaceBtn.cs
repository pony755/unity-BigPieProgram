using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReplaceBtn : MonoBehaviour
{
    public Image skillImg;
    public Text skillName;
    public Text skillDescription;
    public int skillCode;

    public void SetReplaceBtn(int code)
    {
        skillCode = code;
        skillImg.sprite= AllList.instance.allSkillList[code].skillImg;
        skillName.text = AllList.instance.allSkillList[code].skillName;
        skillDescription.text = AllList.instance.allSkillList[code].description;      
        gameObject.SetActive(true);
    }
}
