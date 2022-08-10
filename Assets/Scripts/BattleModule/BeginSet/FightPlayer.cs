using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightPlayer : MonoBehaviour
{

    [Header("己方角色")]
    public List<int> fightHeroCode;
    public List<int> fightPrepareHeroCode;
    [Header("敌人组合编号")]
    public int enemyBundleCode;
    [Header("卡组")]
    public int PstartCard;
    public int PmaxCard;
    public int PaddCardNum;
    public List<int> cardCode;
    

}
