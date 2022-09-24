using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class iconImg : MonoBehaviour
{
    [HideInInspector]public ItemBase item;
    [HideInInspector]public iconShow show;

    public void ShowItemShow()
    {
        if(item != null)
        {
            show.transform.position = transform.position;
            show.iconName.text = item.itemName;
            if (item.itemQuality == ItemBase.ItemQuality.∆’Õ®)
                show.iconName.color = Color.white;
            if (item.itemQuality==ItemBase.ItemQuality.œ°”–)
                show.iconName.color = new Color32(255,175,0,255);
            show.iconDescription.text = item.itemDescription;
            show.gameObject.SetActive(true);
        }
        
    }
    public void HideItemShow()
    {
        if(item!=null)
        {
            show.transform.position = new Vector3(0, 0, 0);
            show.iconName.text = null;
            show.iconDescription.text = null;
            show.gameObject.SetActive(false);
        }
        
    }


    public void SetIcon(ItemBase i,iconShow tempShow)
    {
        show= tempShow;
        item = i;
        GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        GetComponent<Image>().sprite = i.itemIcon;
    }
    public void ResetIcon()
    {
        GetComponent<Image>().sprite = null;
        GetComponent<Image>().color = new Color32(255, 255, 255, 0);
        item = null;
        show = null;
    }
}
