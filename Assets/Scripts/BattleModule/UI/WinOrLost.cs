using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class WinOrLost : MonoBehaviour
{
    [HideInInspector]public Player player;
    //public GameObject levelUp;

    [HideInInspector] public enum SettleState { None,First,RollSkill,RollCard,Finish}
    public SettleState settleCurrentState;
    public GameObject firstImg;
    public GameObject rollSkillImg;
    public GameObject rollCardImg;



    private void Start()
    {
        settleCurrentState = SettleState.None;
        firstImg.SetActive(false);
        rollSkillImg.SetActive(false);
        rollCardImg.SetActive(false);
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
        //��ʼ
        if(settleCurrentState == SettleState.None&& !firstImg.activeInHierarchy)
        {
            settleCurrentState = SettleState.First;
            firstImg.SetActive(true);
        }

        //ʤ������
        if (firstImg.GetComponent<firstImg>().winNextSwitch==true&& settleCurrentState == SettleState.First)//ʤ�����������������һ�׶�
        {
            settleCurrentState = SettleState.RollSkill;
            firstImg.SetActive(false);
        }

        //roll���ܽ���
        if(settleCurrentState == SettleState.RollSkill)
        {
            if (CheckRollSkill()!=null && !rollSkillImg.activeInHierarchy)
            {
                rollSkillImg.GetComponent<RollSkillImage>().StartShow(CheckRollSkill());//��ʼ���鼼�ܲ���ʾ
            }
            if (rollSkillImg.GetComponent<RollSkillImage>().nextSwitch == true)
            {
                rollSkillImg.GetComponent<RollSkillImage>().resetRollSkillImage();//һ�γ鼼�ܽ����������״̬���ر�
            }
            if(CheckRollSkill()==null&& !rollSkillImg.activeInHierarchy)//���رյ�ʱ�����Խ�ɫ�鼼���ѽ�����ɽ�����һ�׶�
            {
                settleCurrentState = SettleState.RollCard;
            }
        }

        //roll���ƽ���
        if(settleCurrentState == SettleState.RollCard)
        {
            if(GameManager.instance.tempPlayer.GetComponent<FightPlayer>().getCards.Count>0)
            {
                rollCardImg.GetComponent<RollCards>().RollCardShow();
            }
            if(rollCardImg.GetComponent<RollCards>().nextSwitch==true)
            {
                rollCardImg.SetActive(false);
                settleCurrentState = SettleState.Finish;
            }
        }
    }









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
