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
    readonly private float ExpSliderSpeed = 0.005f;


    private void Start()
    {
        if(winHero != null)
        {
            ExpFinish = false;
            ExpSlider.fillAmount = (float)winHero.currentExp / (float)winHero.nextExp;
            winHero.currentExp +=winHero.getExp ;//ֱ�ӼӾ���         
            heroIMG.sprite = winHero.normalSprite;
            heroLV.text = "Lv " + winHero.unitLevel.ToString();
            Exp.text = winHero.currentExp.ToString();
            ExpNext.text = "/" + winHero.nextExp.ToString();
            ExpGet.text = "+" + winHero.getExp.ToString();
            
        }
        else
            ExpFinish= true;
        
    }

    private void Update()
    {
        //���ڣ�ʵʱ��¼����
        heroLV.text = "Lv " + winHero.unitLevel.ToString();
        Exp.text = winHero.currentExp.ToString();
        ExpNext.text = "/" + winHero.nextExp.ToString();
        scale = (float)winHero.currentExp / (float)winHero.nextExp;

        if (ExpSlider.fillAmount < scale)
        {
            ExpSlider.fillAmount +=  ExpSliderSpeed;
        }
        if (ExpSlider.fillAmount == 1)
        {
            StartCoroutine(LevelUp());
            ExpSlider.fillAmount = 0;
            //������������
            winHero.unitLevel ++;
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
        winHero.currentExp -=  winHero.nextExp;
        yield return null;
    }

}



