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
    private bool replacing;//判断是否在更换技能
    // Start is called before the first frame update
    void Start()
    {
        nextSwitch = false;
        nextBtn.GetComponent<Button>().onClick.AddListener(delegate () {
            nextSwitch = true;

            //+20Bp
            GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().BP += 20;
        });
    }

    // Update is called once per frame
    void Update()
    {
        if(replacing==true)
        {
            if(skillReplace.GetComponent<ReplaceImg>().finishSwitch==true)
            {
                skillReplace.GetComponent<ReplaceImg>().finishSwitch = false;
                replacing = false;
                nextSwitch = true;
            }
        }
    }
    public void StartShow(Unit unit)//初始化+显示函数
    {             
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
            int a = new int();
            a = i;
            Skill tempSkill= AllList.instance.allSkillList[tempList[a]];
            getSkillBtn[a].skillText.text = tempSkill.description;
            getSkillBtn[a].skillName.text = tempSkill.skillName;
            getSkillBtn[a].skillImg.sprite = tempSkill.skillImg;
            getSkillBtn[a].skillIndex = tempList[a];
            getSkillBtn[a].gameObject.SetActive(true);
            int tempIndex = getSkillBtn[a].skillIndex;        
            
            //添加按钮功能
            getSkillBtn[a].GetComponent<Button>().onClick.AddListener(delegate () {
                if (tempUnit.skillNum > tempUnit.heroSkillListCode.Count)
                {
                    tempUnit.UnitLearnSkill(tempIndex);
                    nextSwitch = true;
                }
                else
                {
                    skillReplace.GetComponent<ReplaceImg>().ReplaceImgShow(tempUnit, tempIndex);
                    replacing=true;
                }               
            });
        }
    }


    public void resetRollSkillImage()
    {
        nextSwitch = false;
        foreach(var b in getSkillBtn)
            b.GetComponent<Button>().onClick.RemoveAllListeners();
        gameObject.SetActive(false);
    }
    private List<int> SetSkillModeT()
    {      
        return tempUnit.SkillRollList(3, tempUnit.currencyFightLSkillList, 50, tempUnit.exclusiveFightLSkillList, 40, tempUnit.currencyFightMSkillList, 10);
    }

    

}
