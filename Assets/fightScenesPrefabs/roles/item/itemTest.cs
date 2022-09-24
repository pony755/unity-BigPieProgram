using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[CreateAssetMenu(fileName = "itemTest", menuName = "Create new item")]
public class itemTest : ItemBase
{
    
    public override void SettleGainEffectInFightItem()
    {
        GameManager.instance.tips.text = "Ã¿Ã¿Ã∆Ã∆";
        
    }

}
