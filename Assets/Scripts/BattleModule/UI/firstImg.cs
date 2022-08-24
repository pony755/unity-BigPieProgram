using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class firstImg : MonoBehaviour
{
    public Image winLostImg;
    public Sprite winImg;
    public Sprite lostImg;
    public GameObject winObject;
    public GameObject lostObject;
    public Text tips;
    //到时候逐行读取txt文件给其fu值
    private string winText = "米浴说的道理";
    private string lostText = "下次一定";
    // Start is called before the first frame update
    void Start()
    {
        if (GameManager.instance.win == true)
        {
            winLostImg.sprite = winImg;
            tips.text = winText;
            winObject.SetActive(true);
        }
        else
        {
            winLostImg.sprite = lostImg;
            tips.text = lostText;
            lostObject.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
