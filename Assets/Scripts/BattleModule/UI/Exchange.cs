using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Exchange : MonoBehaviour
{

    public List<GameObject> herosBtn;
    private bool show;
    private bool exchange;
    void Start()
    {
        show = false;
        exchange = false;
    }

    private void Update()
    {
        if(this.gameObject.activeInHierarchy)
            show = true;
        if(show)
            ShowBtn();

    }
    public void ShowBtn()
    {
        for (int i = 0; i < GameManager.instance.heroPreparePrefab.Count; i++)
        {
            if (GameManager.instance.heroPreparePrefab[i].GetComponent<Unit>().currentHP > 0)
            {
                herosBtn[i].GetComponent<Image>().sprite = GameManager.instance.heroPreparePrefab[i].GetComponent<Unit>().normalSprite;
                herosBtn[i].transform.GetChild(0).GetComponent<Text>().text = "Lv " + GameManager.instance.heroPreparePrefab[i].GetComponent<Unit>().unitLevel.ToString();
                herosBtn[i].SetActive(true);
                exchange = true;
            }
            
        }
        if(exchange==true)
            GameManager.instance.tips.text = "选择交换角色";
        else
            GameManager.instance.tips.text = "无角色交换";
        show = false;
    }
}
