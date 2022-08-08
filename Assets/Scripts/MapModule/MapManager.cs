using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlaceCard;

public class MapManager : MonoBehaviour
{
    public Player player;
    public int level;
    public int childLevel;
    public GameObject[,] map;
    private int mapRow;
    private int mapColumn;
    public GameObject[] cardPosition;
    private int shopLimit;//�̵����������
    private int shopCount = 0;//�̵�����
    private int innLimit;//�ù����������
    private int innCount = 0;//�ù�����
    private int treasureLimit;//�������������
    private int treasureCount = 0;//��������
    private int portalLimit;//���������������
    private int portalCount = 0;//����������
    private int placeOfGodLimit;//����֮�����������
    private int placeOfGodCount = 0;//����֮������
    public int nightmareSharps;
    public bool isTurning = false;
    private bool startListening = false;
    private GameObject mainCamera;
    private GameObject background;
    private GameObject cardPositions;
    private GameObject cards;
    private GameObject border;
    private GameObject canvas;
    private GameObject eventSystem;

    // Start is called before the first frame update
    void Start()
    {
        CopyObject();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        player.awakeCount = 1;//������
        InitializeMap();
        map[0, 0].GetComponent<PlaceCard>().cardState = CardState.back;
        startListening = true;
    }
    void Update()
    {
        CardStateListener();
        PlayerStateListener();
        player.level = level;
        player.childLevel = childLevel;
    }
    void InitializeMap()//��ʼ����ͼ
    {
        InitializeMapSize();
        map = new GameObject[mapRow, mapColumn];
        player.InitializeArray(mapRow*mapColumn);
        InitializeCardPosition();
        SetCardLimit();
        PresetSpecialCards();
        int positionIndex = 0;
        GameObject cards = GameObject.Find("Cards");
        bool isSet;//�����Ƿ����ú�
        for (int i = 0; i < mapRow; i++)
        {
            for (int j = 0; j < mapColumn; j++)
            {
                isSet = false;
                while (!isSet)
                {
                    CardType randomType = RamdomCardType(35, 40, 60, 70, 80, 90, 95);
                    if(randomType == CardType.shop)
                    {
                        if (shopCount >= shopLimit)
                        {
                            continue;
                        }
                        shopCount++;
                    }
                    else if(randomType == CardType.inn)
                    {
                        if(innCount >= innLimit)
                        {
                            continue;
                        }
                        innCount++;
                    }
                    else if(randomType == CardType.treasure)
                    {
                        if(treasureCount >= treasureLimit)
                        {
                            continue;
                        }
                        treasureCount++;
                    }
                    else if(randomType == CardType.portal)
                    {
                        if(portalCount >= portalLimit)
                        {
                            continue;
                        }
                        portalCount++;
                    }
                    else if(randomType == CardType.placeOfGod)
                    {
                        if(placeOfGodCount >= placeOfGodLimit)
                        {
                            continue;
                        }
                        placeOfGodCount++;
                    }
                    InitializeCard(randomType, cards, positionIndex, i, j);
                    positionIndex++;
                    isSet = true;
                }
            }
        }
        EmbedSharps();
    }
    void InitializeMapSize()//��ʼ����ͼ��С
    {
        if (level == 0)
        {
            mapRow = 3;
            mapColumn = 3;
        }
        else
        {
            switch (childLevel)
            {
                case 1:
                    {
                        mapRow = 5;
                        mapColumn = 5;
                        break;
                    }
                case 2:
                    {
                        mapRow = 6;
                        mapColumn = 6;
                        break;
                    }
                case 3:
                    {
                        mapRow = 7;
                        mapColumn = 7;
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }
        Debug.Log("��ʼ����ͼ��С "+mapRow+" "+mapColumn+" �ɹ�");
    }
    void InitializeCardPosition()//����λ��
    {
        int positionIndex = 0;
        GameObject cardPositions = GameObject.FindGameObjectWithTag("CardPosition");
        float x, y;
        float z = 0;
        if (level == 0)
        {
            y = 3;
            for (int i = 0; i < mapRow; i++)
            {
                x = -2;
                for (int j = 0; j < mapColumn; j++)
                {
                    GameObject positionObject = new GameObject();
                    positionObject.name = "Position" + positionIndex;
                    positionObject.transform.position = new Vector3(x, y, z);
                    positionObject.transform.SetParent(cardPositions.transform);
                    cardPosition[positionIndex] = positionObject;
                    positionIndex++;
                    x += 2;
                }
                y -= 3;
            }
        }
        else
        {
            switch (childLevel)
            {
                case 1:
                    {
                        y = 6;
                        for (int i = 0; i < mapRow; i++)
                        {
                            x = -4;
                            for (int j = 0; j < mapColumn; j++)
                            {
                                GameObject positionObject = new GameObject();
                                positionObject.name = "Position" + positionIndex;
                                positionObject.transform.position = new Vector3(x, y, z);
                                positionObject.transform.SetParent(cardPositions.transform);
                                cardPosition[positionIndex] = positionObject;
                                positionIndex++;
                                x += 2;
                            }
                            y -= 3;
                        }
                        break;
                    }
                case 2:
                    {
                        y = 7.5f;
                        for (int i = 0; i < mapRow; i++)
                        {
                            x = -5;
                            for (int j = 0; j < mapColumn; j++)
                            {
                                GameObject positionObject = new GameObject();
                                positionObject.name = "Position" + positionIndex;
                                positionObject.transform.position = new Vector3(x, y, z);
                                positionObject.transform.SetParent(cardPositions.transform);
                                cardPosition[positionIndex] = positionObject;
                                positionIndex++;
                                x += 2;
                            }
                            y -= 3;
                        }
                        break;
                    }
                case 3:
                    {
                        y = 9;
                        for (int i = 0; i < mapRow; i++)
                        {
                            x = -6;
                            for (int j = 0; j < mapColumn; j++)
                            {
                                GameObject positionObject = new GameObject();
                                positionObject.name = "Position" + positionIndex;
                                positionObject.transform.position = new Vector3(x, y, z);
                                positionObject.transform.SetParent(cardPositions.transform);
                                cardPosition[positionIndex] = positionObject;
                                positionIndex++;
                                x += 2;
                            }
                            y -= 3;
                        }
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }
        Debug.Log("��ʼ������λ�óɹ�");
    }
    void SetCardLimit()//���ò��ֿ�����������
    {
        switch (childLevel)
        {
            case 1:
                {
                    shopLimit = 1;
                    innLimit = 1;
                    treasureLimit = 2;
                    portalLimit = 1;
                    placeOfGodLimit = 1;
                    break;
                }
            case 2:
                {
                    shopLimit = 2;
                    innLimit = 2;
                    treasureLimit = 3;
                    portalLimit = 2;
                    placeOfGodLimit = 2;
                    break;
                }
            case 3:
                {
                    shopLimit = 3;
                    innLimit = 3;
                    treasureLimit = 4;
                    portalLimit = 3;
                    placeOfGodCount = 3;
                    break;
                }
            default:
                {
                    break;
                }
        }
    }
    void PresetSpecialCards()//Ԥ���ض�����
    {

    }
    CardType RamdomCardType(double bp0, double bp1, double bp2, double bp3, double bp4, double bp5, double bp6)//�����������
    {
        double randomNumber = Koubot.Tool.Random.RandomTool.GenerateRandomDouble(1, 100);
        if (randomNumber >= 1 && randomNumber <= bp0)
        {
            return CardType.battle;
        }
        else if (randomNumber > bp0 && randomNumber <= bp1)
        {
            return CardType.eliteBattle;
        }
        else if (randomNumber > bp1 && randomNumber <= bp2)
        {
            return CardType.randomEvent;
        }
        else if(randomNumber > bp2 && randomNumber <= bp3)
        {
            return CardType.shop;
        }
        else if(randomNumber > bp3 && randomNumber <= bp4)
        {
            return CardType.inn;
        }
        else if(randomNumber > bp4 && randomNumber <= bp5)
        {
            return CardType.treasure;
        }
        else if(randomNumber > bp5 && randomNumber <= bp6)
        {
            return CardType.portal;
        }
        else
        {
            return CardType.placeOfGod;
        }
    }
    void InitializeCard(CardType cardType,GameObject cards,int positionIndex,int row,int column)//��ʼ������
    {
        GameObject card = Instantiate(Resources.Load<GameObject>("Prefabs/Card"));
        card.GetComponent<PlaceCard>().cardType = cardType;
        card.GetComponent<PlaceCard>().cardState = CardState.hide;
        card.name = "Card_" + cardType.ToString();
        card.transform.position = cardPosition[positionIndex].transform.position;
        GameObject cardFace = Instantiate(Resources.Load<GameObject>("Prefabs/Cardface"));
        cardFace.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("MapGraphics/Cardface_" + cardType.ToString());
        cardFace.transform.SetParent(card.transform);
        cardFace.transform.localPosition = new Vector3(0, 0, 0.01f);
        cardFace.transform.eulerAngles = new Vector3(0, 180, 0);
        map[row, column] = card;
        card.transform.SetParent(cards.transform);
    }
    void EmbedSharps()//Ƕ��������Ƭ
    {
        int row, column;
        GameObject target;
        for(nightmareSharps = GetSharpNumber(); nightmareSharps > 0; nightmareSharps--)
        {
            row = Koubot.Tool.Random.RandomTool.GenerateRandomInt(0, mapRow - 2);
            column = Koubot.Tool.Random.RandomTool.GenerateRandomInt(0, mapColumn - 2);
            target = map[row, column];
            if (target.GetComponent<PlaceCard>().isEmbedded)
            {
                nightmareSharps++;
                continue;
            }
            else
            {
                target.GetComponent<PlaceCard>().isEmbedded = true;
                target.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("MapGraphics/Cardback_Nightmare");
            }
        }
        Debug.Log("Ƕ����Ƭ�ɹ�");
    }
    int GetSharpNumber()//��ȡӦǶ���ͼ����Ƭ����
    {
        if(level == 0)
        {
            return 3;
        }
        else
        {
            switch (childLevel)
            {
                case 1:
                    {
                        return 5;
                    }
                case 2:
                    {
                        return 6;
                    }
                case 3:
                    {
                        return 7;
                    }
                default:
                    {
                        return 0;
                    }
            }
        }
    }
    void CardStateListener()//����״̬������
    {
        if (startListening)
        {
            for (int i = 0; i < mapRow; i++)
            {
                for (int j = 0; j < mapColumn; j++)
                {
                    if (map[i, j].GetComponent<PlaceCard>().isLinked)
                    {
                        continue;
                    }
                    else if (map[i, j].GetComponent<PlaceCard>().cardState.Equals(CardState.face))
                    {
                        TransformCardState(i, j);
                        map[i, j].GetComponent<PlaceCard>().isLinked = true;
                    }
                }
            }
        }
    }
    void TransformCardState(int row,int column)//�ı����ڿ���״̬
    {
        int newRow,newColumn;
        newRow = row - 1;
        newColumn = column;
        if (newRow >= 0)
        {
            map[newRow, newColumn].GetComponent<PlaceCard>().cardState = CardState.back;
        }
        newRow = row + 1;
        newColumn = column;
        if (newRow < mapRow)
        {
            map[newRow, newColumn].GetComponent<PlaceCard>().cardState = CardState.back;
        }
        newRow = row;
        newColumn = column - 1;
        if (newColumn >= 0)
        {
            map[newRow, newColumn].GetComponent<PlaceCard>().cardState = CardState.back;
        }
        newRow = row;
        newColumn = column + 1;
        if (newColumn < mapColumn)
        {
            map[newRow, newColumn].GetComponent<PlaceCard>().cardState = CardState.back;
        }
    }
    void PlayerStateListener()//���״̬������
    {
        if (player != null)
        {
            if (player.GetComponent<Player>().globalStateValue != 0)
            {
                ActivateMap();
                RecoverMap();
                player.GetComponent<Player>().globalStateValue--;
            }
        }
    }
    void CopyObject()//��ȡ�������ڳ����л�
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        background = GameObject.FindGameObjectWithTag("Background");
        cardPositions = GameObject.FindGameObjectWithTag("CardPosition");
        cards = GameObject.FindGameObjectWithTag("Card");
        border = GameObject.FindGameObjectWithTag("Border");
        canvas = GameObject.FindGameObjectWithTag("Canvas");
        eventSystem = GameObject.FindGameObjectWithTag("EventSystem");
        Debug.Log("��ȡ�ɹ�");
    }
    public void BackUpMap()//���ݵ�ͼ
    {
        int k = 0;//һά�����±�
        for (int i = 0; i < mapRow; i++)
        {
            for (int j = 0; j < mapColumn; j++)
            {
                player.cardStates[k] = map[i, j].GetComponent<PlaceCard>().cardState;
                player.rotation[k] = map[i, j].transform.eulerAngles.y;
                Debug.Log(player.rotation[k]);
                k++;
            }
        }
        Debug.Log("���ݵ�ͼ�ɹ�");
    }
    public void RecoverMap()//�ָ���ͼ
    {
        int k = 0;//һά�����±�
        for (int i = 0; i < mapRow; i++)
        {
            for (int j = 0; j < mapColumn; j++)
            {
                map[i, j].GetComponent<PlaceCard>().cardState = player.cardStates[k];
                map[i, j].transform.rotation = Quaternion.Euler(0, player.rotation[k], 0);
                k++;
            }
        }
        Debug.Log("�ָ���ͼ�ɹ�");
    }
    public void FreezeMap()//�����ͼ���
    {
        mainCamera.SetActive(false);
        background.SetActive(false);
        cardPositions.SetActive(false);
        cards.SetActive(false);
        border.SetActive(false);
        canvas.SetActive(false);
        eventSystem.SetActive(false);
    }
    public void ActivateMap()//�����ͼ���
    {
        mainCamera.SetActive(true);
        background.SetActive(true);
        cardPositions.SetActive(true);
        cards.SetActive(true);
        border.SetActive(true);
        canvas.SetActive(true);
        eventSystem.SetActive(true);
    }
}
