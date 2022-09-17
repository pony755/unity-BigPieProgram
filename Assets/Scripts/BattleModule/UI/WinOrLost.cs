using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class WinOrLost : MonoBehaviour
{
    [HideInInspector]public Player player;
    //public GameObject levelUp;

    [HideInInspector] public enum SettleState { None,First,RollSkill,RollCard}
    public SettleState settleCurrentState;
    public GameObject firstImg;
    public GameObject rollSkillImg;
    private bool rollSkillIng;//�жϵ�ǰ�Ƿ���ѡ���ܽ���

    
    
    private void Start()
    {
        settleCurrentState = SettleState.None;
        rollSkillIng = false;
        /*player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        next.onClick.AddListener(delegate () {
            if (player != null)
            {
                if (player.globalStateValue == 0)
                {
                    player.globalStateValue++;
                }
            }
            ChangeScene(); });*/
               
    }
    private void Update()
    {
        if(settleCurrentState == SettleState.None&& !firstImg.activeInHierarchy)
        {
            settleCurrentState = SettleState.First;
            firstImg.SetActive(true);
        }
        if(firstImg.GetComponent<firstImg>().winNextSwitch==true&& settleCurrentState == SettleState.First)
        {
            settleCurrentState = SettleState.RollSkill;
            firstImg.SetActive(false);
        }


        if(settleCurrentState == SettleState.RollSkill&&CheckRollSkill()&&!rollSkillImg.activeInHierarchy)
        {
            rollSkillImg.GetComponent<RollSkillImage>().StartShow(CheckRollSkill());
        }
        if(settleCurrentState == SettleState.RollSkill && rollSkillImg.GetComponent<RollSkillImage>().nextSwitch==true)
        {
            rollSkillImg.SetActive(false);
        }
    }







/*    private void ToSkillRoll()
    {
        if (firstImg.activeInHierarchy)
            firstImg.SetActive(false);
        if (CheckRollSkill()!=null)
        {
            rollSkillImg.GetComponent<RollSkillImage>().StartShow(CheckRollSkill());
        }      
        else
        {
            //ToRollCard();
        }
    }//ǰ��ҡ���ܽ���*/
   
    

    private Unit CheckRollSkill()
    {
        Unit unit = null;
        foreach(var p in GameManager.instance.heroUnit)
        {
            if (p.skillRoll.Count > 0)
            {
                unit = p;
                break;
            }
        }
        return unit;
    }//���μ���ɫ״̬�����н�ɫѧ�Ἴ���򷵻ظ�Unit
}
