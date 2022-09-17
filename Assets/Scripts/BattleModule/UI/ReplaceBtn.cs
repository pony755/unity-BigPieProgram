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
        skillImg.sprite=GameManager.instance.allListObject.GetComponent<AllList>().allSkillList[code].skillImg;
        skillName.text = GameManager.instance.allListObject.GetComponent<AllList>().allSkillList[code].skillName;
        skillDescription.text = GameManager.instance.allListObject.GetComponent<AllList>().allSkillList[code].description;
        gameObject.SetActive(true);
    }
}
