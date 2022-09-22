using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddCancleBtn : MonoBehaviour
{
    public GameObject AddCardNumImg;
    void Update()
    {
        if (GameManager.instance.state == BattleState.PLAYERTURNSTART)
        {
            this.gameObject.transform.GetChild(0).GetComponent<Text>().text = "�غϿ�ʼ";
            this.gameObject.transform.GetChild(0).GetComponent<Text>().color = Color.magenta;
        }
            
        else if (GameManager.instance.state == BattleState.PLAYERTURN)
        {
            this.gameObject.transform.GetChild(0).GetComponent<Text>().text = "����";
            this.gameObject.transform.GetChild(0).GetComponent<Text>().color = Color.black;
        }

        else if (GameManager.instance.state == BattleState.CARDTURNUNIT || GameManager.instance.state == BattleState.SKILL || GameManager.instance.state == BattleState.CARDTURNUNIT || GameManager.instance.state == BattleState.POINTALL || GameManager.instance.state == BattleState.POINTENEMY || GameManager.instance.state == BattleState.POINTPLAYER)
        {
            this.gameObject.transform.GetChild(0).GetComponent<Text>().text = "����";
            this.gameObject.transform.GetChild(0).GetComponent<Text>().color = Color.black;
        }
        else if (GameManager.instance.state == BattleState.TOACTION)
        {
            this.gameObject.transform.GetChild(0).GetComponent<Text>().text = "�ȴ�����";
            this.gameObject.transform.GetChild(0).GetComponent<Text>().color = Color.magenta;
        }
        else if (GameManager.instance.state == BattleState.ACTION || GameManager.instance.state == BattleState.ACTIONFINISH)
        {
            this.gameObject.transform.GetChild(0).GetComponent<Text>().text = "����";
            this.gameObject.transform.GetChild(0).GetComponent<Text>().color = Color.magenta;
        }
        else if(GameManager.instance.over)
        {

            this.gameObject.transform.GetChild(0).GetComponent<Text>().text = "��Ϸ����";
            this.gameObject.transform.GetChild(0).GetComponent<Text>().color = Color.grey;

        }
        else
        {
            this.gameObject.transform.GetChild(0).GetComponent<Text>().text = "�з��غ�";
            this.gameObject.transform.GetChild(0).GetComponent<Text>().color = Color.grey;
        }

        if(this.gameObject.transform.GetChild(0).GetComponent<Text>().text == "����"|| this.gameObject.transform.GetChild(0).GetComponent<Text>().text == "����")
            GetComponent<Button>().enabled = true;
        else
            GetComponent<Button>().enabled = false;

        if (this.gameObject.transform.GetChild(0).GetComponent<Text>().text == "����")
            AddCardNumImg.SetActive(true);
        else
            AddCardNumImg.SetActive(false);

        AddCardNumImg.transform.GetChild(0).GetComponent<Text>().text = GameManager.instance.fightPlayer.addCardNum.ToString();

    }

    public void Click()
    {
        if (this.gameObject.transform.GetChild(0).GetComponent<Text>().text == "����")
            GameManager.instance.fightPlayer.ButtonTakeCard();
        if (this.gameObject.transform.GetChild(0).GetComponent<Text>().text == "����")
        {                  
            GameManager.instance.Back();
        }
            
    }
}
