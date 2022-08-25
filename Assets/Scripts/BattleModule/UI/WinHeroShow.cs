using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class WinHeroShow : MonoBehaviour
{
    public Unit winHero;
    public Image heroIMG;
    public Text heroLV;
    public Text ExpGet;
    public Image ExpSlider;
    public Text Exp;
    public Text ExpNext;
    public GameObject levelUpObject;

    public bool ExpFinish;
    private float scale;
    readonly private float ExpSliderSpeed = 0.01f;


    private void Start()
    {
        if(winHero != null)
        {
            ExpFinish = false;
            ExpSlider.fillAmount = (float)winHero.currentExp / (float)winHero.nextExp[winHero.unitLevel];
            winHero.currentExp +=winHero.getExp ;//直接加经验           
            heroIMG.sprite = winHero.normalSprite;
            heroLV.text = "Lv " + winHero.unitLevel.ToString();
            Exp.text = winHero.currentExp.ToString();
            ExpNext.text = "/" + winHero.nextExp.ToString();
            ExpGet.text = "+" + winHero.getExp.ToString();
            winHero.getExp = 0;//清0getExp;

        }
        else
            ExpFinish= true;
        
    }

    private void Update()
    {
        //开摆，实时记录数据
        heroLV.text = "Lv " + winHero.unitLevel.ToString();
        Exp.text = winHero.currentExp.ToString();
        ExpNext.text = "/" + winHero.nextExp[winHero.unitLevel].ToString();
        scale = (float)winHero.currentExp / (float)winHero.nextExp[winHero.unitLevel];

        if (ExpSlider.fillAmount < scale)
        {
            ExpSlider.fillAmount +=  ExpSliderSpeed;
        }
        if (ExpSlider.fillAmount == 1)
        {
           
            ExpSlider.fillAmount = 0;
            //调用升级函数
            StartCoroutine(LevelUp());

        }
        if (ExpSlider.fillAmount >= scale)
        {           
            ExpSlider.fillAmount = scale;
            ExpFinish=true;
        }

    }

    IEnumerator LevelUp()
    {
        levelUpObject.GetComponent<Animator>().Play("levelUp");
        winHero.currentExp -=  winHero.nextExp[winHero.unitLevel];
        winHero.unitLevel++;
        AdjudgeLvUp(winHero.unitLevel);
        yield return null;
    }

    private void AdjudgeLvUp(int a)//判断升级等级并调用相应升级函数
    {
        switch(a)
        {
            case 2:
                winHero.LevelUp2();
                break;
            case 3:
                winHero.LevelUp3();
                break;
            case 4:
                winHero.LevelUp4();
                break;
            case 5:
                winHero.LevelUp5();
                break;
            case 6:
                winHero.LevelUp6();
                break;
            case 7:
                winHero.LevelUp7();
                break;
            case 8:
                winHero.LevelUp8();
                break;
            case 9:
                winHero.LevelUp9();
                break;
            case 10:
                winHero.LevelUp10();
                break;
            case 11:
                winHero.LevelUp11();
                break;
            case 12:
                winHero.LevelUp12();
                break;
            case 13:
                winHero.LevelUp13();
                break;
            case 14:
                winHero.LevelUp14();
                break;
            case 15:
                winHero.LevelUp15();
                break;
            default:
                break;
        }
    }
}



