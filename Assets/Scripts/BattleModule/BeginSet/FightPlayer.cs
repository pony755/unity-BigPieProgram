using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class FightPlayer : MonoBehaviour
{
    //�������������¼����
    public string playerID;
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
    [Header("����")]
    public int AD;
    public int AP;
    [Header("��Ʒ��")]
    public List<int> itemsCode;
    
}
