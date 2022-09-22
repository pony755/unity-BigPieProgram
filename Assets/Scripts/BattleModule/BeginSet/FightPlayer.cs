using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class FightPlayer : MonoBehaviour
{
    //这个类仅仅负责记录数据
    public string playerID;
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
    [Header("属性")]
    public int AD;
    public int AP;
    [Header("饰品栏")]
    public List<int> itemsCode;
    
}
