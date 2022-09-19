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
    private int tempLv;//��ǰ�ȼ�
    private int tempFinalExp;//�ܹ�exp


    private void Start()
    {
        if(winHero != null)
        {
            ExpFinish = false;
            tempLv = winHero.unitLevel;
            tempFinalExp = winHero.currentExp+winHero.getExp;

            ExpSlider.fillAmount = (float)winHero.currentExp / (float)winHero.nextExp[winHero.unitLevel];     
            heroIMG.sprite = winHero.normalSprite;
            heroLV.text = "Lv " + winHero.unitLevel.ToString();
            Exp.text = winHero.currentExp.ToString();
            ExpNext.text = "/" + winHero.nextExp.ToString();
            ExpGet.text = "+" + winHero.getExp.ToString();
            winHero.SettleGetExp();//���㾭��
        }
        else
            ExpFinish= true;
        
    }

    private void Update()
    {
        //���ڣ�ʵʱ��¼����
        heroLV.text = "Lv " + tempLv.ToString();
        Exp.text = tempFinalExp.ToString();
        ExpNext.text = "/" + winHero.nextExp[tempLv].ToString();
        scale = (float)tempFinalExp / (float)winHero.nextExp[tempLv];

        if (ExpSlider.fillAmount < scale)
        {
            ExpSlider.fillAmount +=  ExpSliderSpeed;
        }
        if (ExpSlider.fillAmount == 1)
        {
           
            ExpSlider.fillAmount = 0;
            //������������
            StartCoroutine(ShowLevelUp());

        }
        if (ExpSlider.fillAmount >= scale)
        {           
            ExpSlider.fillAmount = scale;
            ExpFinish=true;
        }

    }

    IEnumerator ShowLevelUp()
    {
        levelUpObject.GetComponent<Animator>().Play("levelUp");
        tempFinalExp -=  winHero.nextExp[tempLv];
        tempLv++;      
        yield return null;
    }

   
}



