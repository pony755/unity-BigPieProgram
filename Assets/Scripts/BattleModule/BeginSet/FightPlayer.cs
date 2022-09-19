using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GetCard { T}

public class FightPlayer : MonoBehaviour
{
    
    [Header("己方角色")]
    public List<int> fightHeroCode;//出战编号
    public List<int> fightPrepareHeroCode;//备战小队
    [Header("敌人组合编号")]
    public int enemyBundleCode;
    [Header("卡组")]
    public int PstartCard;
    public int PmaxCard;
    public int PaddCardNum;
    public List<int> cardCode;

    [Header("状态量")]
    public List<GetCard> getCards;

    //——————————————————————角色获得卡牌操作——————————————————————————

}
