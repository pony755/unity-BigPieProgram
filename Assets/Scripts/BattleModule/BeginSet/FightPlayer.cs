using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GetCard { T}

public class FightPlayer : MonoBehaviour
{
    
    [Header("������ɫ")]
    public List<int> fightHeroCode;//��ս���
    public List<int> fightPrepareHeroCode;//��սС��
    [Header("������ϱ��")]
    public int enemyBundleCode;
    [Header("����")]
    public int PstartCard;
    public int PmaxCard;
    public int PaddCardNum;
    public List<int> cardCode;

    [Header("״̬��")]
    public List<GetCard> getCards;

    //����������������������������������������������ɫ��ÿ��Ʋ�������������������������������������������������������

}
