using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BattleHub : MonoBehaviour
{
    private Unit hubUnit;
    public Image tipsDelayed;
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
    public GameObject shield;
    public GameObject burn;
    public GameObject cold;
    public GameObject poison;
    public GameObject shieldDecrease;
    public GameObject healDecrease;
    public GameObject weakness;
    public GameObject fragile;
    public List<GameObject> tempStateList;
    private float scale;
    readonly private float hurtSpeed=0.003f;
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
        shield.transform.SetParent(this.gameObject.transform);
        tempStateList.Add(shield);
        burn.transform.SetParent(this.gameObject.transform);
        tempStateList.Add(burn);
        cold.transform.SetParent(this.gameObject.transform);
        tempStateList.Add(cold);
        poison.transform.SetParent(this.gameObject.transform);
        tempStateList.Add(poison);
        shieldDecrease.transform.SetParent(this.gameObject.transform);
        tempStateList.Add(shieldDecrease);
        healDecrease.transform.SetParent(this.gameObject.transform);
        tempStateList.Add(healDecrease);
        weakness.transform.SetParent(this.gameObject.transform);
        tempStateList.Add(weakness);
        fragile.transform.SetParent(this.gameObject.transform);
        tempStateList.Add(fragile);
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
            hurtHPImg.fillAmount -= hurtSpeed;
        }
        else
        {
            hurtHPImg.fillAmount = scale;
        }

        if (GameManager.instance.tipsDelayed.TipsDelayedShow(hubUnit))
            tipsDelayed.gameObject.SetActive(true);
        else
        {
            tipsDelayed.gameObject.SetActive(false);
        }


        StatePosition(shield,hubUnit.shield);
    StatePosition(burn, hubUnit.burn);
    StatePosition(cold, hubUnit.cold);
    StatePosition(poison, hubUnit.poison);
    StatePosition(shieldDecrease, hubUnit.shieldDecrease);
    StatePosition(healDecrease, hubUnit.healDecrease);
    StatePosition(weakness, hubUnit.weakness);
    StatePosition(fragile, hubUnit.fragile);



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

    public void StatePosition(GameObject state,int a)
    {
        if (a == 0)
        {
            state.SetActive(false);
            if (tempStateList.Contains(state))
                tempStateList.Remove(state);
        }
        else
        {
            if (!tempStateList.Contains(state))
                tempStateList.Add(state);
            state.transform.localPosition = new Vector3(-28+(tempStateList.IndexOf(state)%5)*16,-22-(tempStateList.IndexOf(state) / 5)*15, 0);
            state.transform.GetChild(0).GetComponent<Text>().text = a.ToString();
            state.SetActive(true);
        }
    }

    public void TipsDelayed()//鼠标进入事件
    {
        GameManager.instance.tipsDelayed.TextSet(hubUnit);
    }
    public void TipsDelayedHide()
    {
        GameManager.instance.tipsDelayed.Textclear();
    }
}
