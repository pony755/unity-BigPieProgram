using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class WinOrLost : MonoBehaviour
{
    public Image winLostImg;
    public Sprite winImg;
    public Sprite lostImg;
    public GameObject Win;
    public GameObject Lost;
    public List<WinHeroShow> Heros=new List<WinHeroShow>();
    public Text tips;

    //到时候逐行读取txt文件给其fu值
    private string winText= "米浴说的道理";
    private string lostText = "下次一定";
    private void Start()
    {
        if (GameManager.instance.win == true)
        {
            winLostImg.sprite = winImg;
            tips.text = winText;
            Win.SetActive(true);
            for(int i=0; i < GameManager.instance.playerUnit.Count; i++)
            {
                Heros[i].heroIMG.sprite = GameManager.instance.playerUnit[i].normalSprite;
                Heros[i].heroLV.text = "Lv "+ GameManager.instance.playerUnit[i].unitLevel.ToString();
                Heros[i].Exp.text =GameManager.instance.playerUnit[i].currentExp.ToString();
                Heros[i].ExpNext.text = "/" + GameManager.instance.playerUnit[i].nextExp.ToString();
                Heros[i].gameObject.SetActive(true);
            }          
        }

        else
        {
            winLostImg.sprite = lostImg;
            tips.text = lostText;
            Lost.SetActive(true);
        }          
    }

}
