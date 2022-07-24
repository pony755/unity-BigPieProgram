using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BattleHub : MonoBehaviour
{

    public Text nameText;
    public Text levelText;
    public Text tiredText;
    public Slider hpSlider;

    public void SetHub(Unit unit)
    {
 
        nameText.text = unit.unitName;
        levelText.text = "Lvl " + unit.unitLevel;
        tiredText.text = "疲劳 " + unit.tired;
        hpSlider.value= unit.currentHP;
        hpSlider.maxValue = unit.maxHP;
    }
    

}
