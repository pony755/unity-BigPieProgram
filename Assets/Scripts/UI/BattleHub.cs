using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BattleHub : MonoBehaviour
{

    public Text nameText;
    public Text levelText;
    public Text tiredText;
    public Text MaxHP;
    public Text CurrentHP;
    public Slider hpSlider;
    public Text MaxMP;
    public Text CurrentMP;
    public Slider mpSlider;

    public void SetHub(Unit unit)
    {
 
        nameText.text = unit.unitName;
        levelText.text = "Lvl " + unit.unitLevel;
        tiredText.text = "疲劳 " + unit.tired;
        hpSlider.value= unit.currentHP;       
        hpSlider.maxValue = unit.maxHP;
        CurrentHP.text = unit.currentHP.ToString();
        MaxHP.text = "/"+ unit.maxHP.ToString();

        mpSlider.value = unit.currentMP;       
        mpSlider.maxValue = unit.maxMP;
        CurrentMP.text = unit.currentMP.ToString();
        MaxMP.text = "/" + unit.maxMP.ToString();
    }
    

}
