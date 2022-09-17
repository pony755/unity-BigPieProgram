using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class RollSkillImage : MonoBehaviour
{
    public List<GetSkillBtn> getSkillBtn=new List<GetSkillBtn>();   
    public Image heroImg;
    public GameObject skillReplace;
    public Button nextBtn;
    public bool nextSwitch;
    private Unit tempUnit;//获取学习技能的Unit
    // Start is called before the first frame update
    void Start()
    {
           

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void StartShow(Unit unit)//初始化+显示函数
    {
        nextSwitch = false;
        tempUnit = unit;
        heroImg.sprite = tempUnit.normalSprite;
        SetSkillBtn();
        gameObject.SetActive(true);
        
    }
    private void SetSkillBtn()
    {
        List<int> tempList = new List<int>();
        if (tempUnit.skillRoll[0] == SkillRoll.T)
        {
            tempUnit.skillRoll.Remove(tempUnit.skillRoll[0]);//移除首元素
            tempList = SetSkillModeT();
        }

        for(int i=0;i<tempList.Count;i++)
        {
            Skill tempSkill=GameManager.instance.allListObject.GetComponent<AllList>().allSkillList[tempList[i]];
            getSkillBtn[i].skillText.text = tempSkill.description;
            getSkillBtn[i].skillName.text = tempSkill.skillName;
            getSkillBtn[i].skillImg.sprite = tempSkill.skillImg;
            getSkillBtn[i].skillIndex = tempList[i];
            getSkillBtn[i].gameObject.SetActive(true);
            int tempIndex = getSkillBtn[i].skillIndex;
            getSkillBtn[i].GetComponent<Button>().onClick.AddListener(delegate () {
                if (tempUnit.skillNum > tempUnit.heroSkillListCode.Count)
                {
                    tempUnit.UnitLearnSkill(tempIndex);
                    nextSwitch = true;
                }
                else
                    skillReplace.GetComponent<ReplaceImg>().ReplaceImgShow(tempUnit);
            });
        }
    }

    private List<int> SetSkillModeT()
    {      
        return tempUnit.SkillRollList(3, tempUnit.currencyFightLSkillList, 50, tempUnit.exclusiveFightLSkillList, 40, tempUnit.currencyFightMSkillList, 10);
    }

    

}
