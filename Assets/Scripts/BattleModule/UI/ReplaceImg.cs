using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplaceImg : MonoBehaviour
{
    public List<ReplaceBtn> replaceBtns = new List<ReplaceBtn>();
    public bool finishSwitch;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void ReplaceImgShow(Unit unit)
    {
        gameObject.SetActive(true);
        for(int i = 0; i < unit.heroSkillListCode.Count; i++)
            replaceBtns[i].SetReplaceBtn(unit.heroSkillListCode[i]);
    }
    public void CloseReplaceImg()
    {
        gameObject.SetActive(false);
        foreach(var p in replaceBtns)
            p.gameObject.SetActive(false);
    }
}
