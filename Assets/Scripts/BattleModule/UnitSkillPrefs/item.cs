using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class item : MonoBehaviour
{
    public enum ItemType { Disposable,GainEffectInFight }//��ȡ���̽���;ս���ڽ�������
    public ItemType itemType;




    public virtual void SettleDisposableItem()
    {

    }
    public virtual void SettleGainEffectInFightItem()
    {

    }
}
