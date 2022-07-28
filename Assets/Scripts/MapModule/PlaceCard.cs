using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlaceCard : MonoBehaviour
{
    public CardState cardState;
    public CardType cardType;
    public MapManager mapManager;
    public bool linked = false;//������־������Ƿ��Ѿ����������ڿ���״̬ת������
    public enum CardState//����״̬
    {
        hide,back,face
    };
    public enum CardType//��������
    {
        battle,eliteBattle,randomEvent,shop,hotel,treasure,portal,placeOfGod
    };
    // Start is called before the first frame update
    void Start()
    {
        mapManager = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>();
    }
    public void OnMouseUp()//�����
    {
        if (mapManager.isTurning)
        {
            return;
        }
        if(cardState.Equals(CardState.hide)||cardState.Equals(CardState.face))
        {
            return;
        }
        TurnCard();
    }
    void TurnCard()//����
    {
        mapManager.isTurning = true;
        cardState = CardState.face;
        StartCoroutine(TurnAnimation());
        if (cardType.Equals(CardType.battle))
        {
            StartCoroutine(EnterBattle());
        }
    }
    IEnumerator TurnAnimation()//���ƶ���
    {
        float angle = 0;
        for(int i = 0; i < 360; i++)
        {
            angle = angle + 0.5f;
            transform.eulerAngles = new Vector3(0, angle, 0);
            yield return new WaitForSeconds(0.0025f);
        }
        mapManager.isTurning = false;
    }
    IEnumerator EnterBattle()//����ս��
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("BattleScene", LoadSceneMode.Additive);
        mapManager.FreezeMap();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
