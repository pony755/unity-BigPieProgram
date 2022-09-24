using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//[CreateAssetMenu(fileName = "Item", menuName = "Character TestItem")]
public class ItemBase : ScriptableObject
{
    public enum ItemQuality { ��ͨ, ϡ�� }
    public ItemQuality itemQuality;
    public enum ItemType { Disposable,GainEffectInFight }//��ȡ���̽���;ս���ڽ�������
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
