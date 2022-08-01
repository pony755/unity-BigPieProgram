using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class WinOrLost : MonoBehaviour
{
    public GameObject levelUp;
    public Button next;
    public Image winLostImg;
    public Sprite winImg;
    public Sprite lostImg;
    public GameObject Win;
    public GameObject Lost;
    public List<WinHeroShow> Heros=new List<WinHeroShow>();
    public Text tips;

    public int heroIndex;
    //��ʱ�����ж�ȡtxt�ļ�����fuֵ
    private string winText= "��ԡ˵�ĵ���";
    private string lostText = "�´�һ��";
    private void Start()
    {

        heroIndex = 0;
        if (GameManager.instance.win == true)
        {
            winLostImg.sprite = winImg;
            tips.text = winText;
            Win.SetActive(true);
            for(int i=0; i < GameManager.instance.playerUnit.Count; i++)
            {
                Heros[i].winHero = GameManager.instance.playerUnit[i];
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
    private void Update()
    {
        if(heroIndex!=Heros.Count)
        {
            if(Heros[heroIndex].ExpFinish)
                heroIndex++;
        }
            
        if(heroIndex==Heros.Count)
            next.gameObject.SetActive(true);

    }

}
