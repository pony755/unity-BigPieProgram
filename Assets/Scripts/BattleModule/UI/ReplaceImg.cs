using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ReplaceImg : MonoBehaviour
{
    public List<ReplaceBtn> replaceBtns = new List<ReplaceBtn>();
    public bool finishSwitch;
    public int newSkillCode;
    // Start is called before the first frame update
    void Start()
    {
        finishSwitch = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void ReplaceImgShow(Unit unit,int tempIndex)
    {
        newSkillCode = tempIndex;
        gameObject.SetActive(true);
        for(int i = 0; i < unit.heroSkillListCode.Count; i++)
        {
            int a = new int();
            a = i;
            replaceBtns[a].SetReplaceBtn(unit.heroSkillListCode[a]);
            replaceBtns[a].GetComponent<Button>().onClick.AddListener(delegate () {
                unit.heroSkillListCode.Remove(replaceBtns[a].skillCode);
                unit.UnitLearnSkill(tempIndex);
                finishSwitch = true;
                CloseReplaceImg();
            });
        }
            
    }
    public void CloseReplaceImg()
    {
        
        foreach(var p in replaceBtns)
        {
            p.GetComponent<Button>().onClick.RemoveAllListeners();
            p.gameObject.SetActive(false);
        }
        gameObject.SetActive(false);
    }
}
