using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class RollSkillImage : MonoBehaviour
{
    public List<GetSkillBtn> getSkillBtn=new List<GetSkillBtn>();
    public Image heroImg;
    public Button nrxtBtn;
    private Unit tempUnit;//获取学习技能的Unit
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Show(Unit unit)
    {
        tempUnit = unit;
        gameObject.SetActive(true);
    }
}
