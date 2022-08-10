using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Exchange : MonoBehaviour
{

    public List<GameObject> herosBtn;
    private bool show;
    void Start()
    {
        show = false;
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

            herosBtn[i].GetComponent<Image>().sprite = GameManager.instance.heroPreparePrefab[i].GetComponent<Unit>().normalSprite;
            herosBtn[i].transform.GetChild(0).GetComponent<Text>().text = "Lv " + GameManager.instance.heroPreparePrefab[i].GetComponent<Unit>().unitLevel.ToString();
            herosBtn[i].SetActive(true);
        }
        show = false;
    }
}
