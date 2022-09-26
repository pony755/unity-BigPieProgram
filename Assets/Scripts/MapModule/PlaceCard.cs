using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlaceCard : MonoBehaviour
{
    public Player player;
    public CardState cardState;
    public CardType cardType;
    public MapManager mapManager;
    public bool isLinked = false;//������־������Ƿ��Ѿ����������ڿ���״̬ת������
    public bool isEmbedded = false;//Ƕ���־������Ƿ���������Ƭ
    public enum CardState//����״̬
    {
        hide,back,face
    };
    public enum CardType//��������
    {
        battle,eliteBattle,randomEvent,shop,inn,treasure,portal,placeOfGod
    };
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        mapManager = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>();
    }
    private void OnMouseDown()//�����
    {
        if (mapManager.isTurning)
        {
            return;
        }
        if(cardState.Equals(CardState.hide)||cardState.Equals(CardState.face))
        {
            return;
        }
        mapManager.BackUpMap();
        player.Save(mapManager.saveNumber);
        StartCoroutine(TurnCard());
    }
    IEnumerator TurnCard()//����
    {
        mapManager.isTurning = true;
        cardState = CardState.face;
        if (isEmbedded)
        {
            player.nightmareSharps++;
        }
        StartCoroutine(TurnAnimation());
        if (cardType.Equals(CardType.battle))
        {
            yield return new WaitForSeconds(2.5f);
            EnterCardScene("BattleScene");
        }
    }
    IEnumerator TurnAnimation()//���ƶ���
    {
        float angle = 0;
        //60֡��1.5�����
        for(int i = 0; i < 120; i++)
        {
            angle += 1.5f;
            transform.eulerAngles = new Vector3(0, angle, 0);
            yield return new WaitForSeconds(0.001f);
        }
        mapManager.isTurning = false;
    }
    void EnterCardScene(string sceneName)
    {
        SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        mapManager.BackUpMap();
        mapManager.FreezeMap();
        Scene scene = SceneManager.GetSceneByName(sceneName);
        SceneManager.MoveGameObjectToScene(player.gameObject, scene);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
