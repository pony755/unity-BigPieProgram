using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BattleHub : MonoBehaviour
{
    private Unit hubUnit;
    public Image headImg;
    public Text nameText;
    public Text levelText;
    public Text tiredText;
    public Text MaxHP;
    public Text CurrentHP;
    public Image hurtHPImg;
    public Slider hpSlider;
    public Text MaxMP;
    public Text CurrentMP;
    public Slider mpSlider;
    private float scale;
    private float hurtSpeed=0.003f;
    public void SetHub(Unit unit)
    {
        hubUnit = unit;
        nameText.text = unit.unitName;
        levelText.text = "Lv " + unit.unitLevel;
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

    private void Start()
    {
        headImg.sprite = hubUnit.normalSprite;
    }

    private void CheckDead()//死亡判断结算函数
    {
        if (hubUnit.currentHP == 0)
        {
            hubUnit.GetComponent<BoxCollider>().enabled = false;//关闭碰撞体脚本
            if (GameManager.instance.playerUnit.Contains(hubUnit))
            {
                GameManager.instance.playerUnit.Remove(hubUnit);
            }
            if (GameManager.instance.enemyUnit.Contains(hubUnit))
            {
                GameManager.instance.enemyUnit.Remove(hubUnit);
            }
           
        }
    }

   
    private void Update()
    {
        if (hubUnit.currentHP > hubUnit.maxHP)
        {
            hubUnit.currentHP = hubUnit.maxHP;
        }
        if (hubUnit.currentHP <=0)
        {
            hubUnit.currentHP = 0;
            hubUnit.anim.Play("dead");
            Invoke("CheckDead",0.2f);
        }

        scale=(float)hubUnit.currentHP/ (float)hubUnit.maxHP;
        if (hurtHPImg.fillAmount > scale)
        {
            hurtHPImg.fillAmount = hurtHPImg.fillAmount - hurtSpeed;
        }
        else
        {
            hurtHPImg.fillAmount = scale;
        }



        tiredText.text = "疲劳 " + hubUnit.tired;
        hpSlider.value = hubUnit.currentHP;
        hpSlider.maxValue = hubUnit.maxHP;
        CurrentHP.text = hubUnit.currentHP.ToString();
        MaxHP.text = "/" + hubUnit.maxHP.ToString();

        mpSlider.value = hubUnit.currentMP;
        mpSlider.maxValue = hubUnit.maxMP;
        CurrentMP.text = hubUnit.currentMP.ToString();
        MaxMP.text = "/" + hubUnit.maxMP.ToString();


    }


}
