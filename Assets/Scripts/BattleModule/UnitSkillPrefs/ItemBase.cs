using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//[CreateAssetMenu(fileName = "Item", menuName = "Character TestItem")]
public class ItemBase : ScriptableObject
{
    public enum ItemQuality { 普通, 稀有 }
    public ItemQuality itemQuality;
    public enum ItemType { Disposable,GainEffectInFight }//获取即刻结算;战斗内结算增益
    public ItemType itemType;
    public Sprite itemIcon;
    public string itemName;
    public string itemDescription;

    
    public virtual void SettleDisposableItem()
    {

    }
    public virtual void SettleGainEffectInFightItem()
    {

    }
}
