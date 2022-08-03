using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlaceCard;

public class MapManager : MonoBehaviour
{
    public Player player;
    public int level;
    public int childlevel;
    public GameObject[,] map;
    private int mapRow;
    private int mapColumn;
    public Dictionary<CardType, int> library;
    public GameObject[] cardPosition;
    public int nightmareSharps;
    public bool isTurning = false;
    private bool startListening = false;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        player.Load();//加载存档
        InitializeMap();
        map[0, 0].GetComponent<PlaceCard>().cardState = CardState.back;
        startListening = true;
    }
    void InitializeMap()//初始化地图
    {
        InitializeMapSize();
        map = new GameObject[mapRow, mapColumn];
        InitializeCardPosition();
        InitializeLibrary();
        int maxRandomNumber = library.Count;
        int positionIndex = 0;
        GameObject cards = GameObject.Find("Cards");
        bool isSet;//卡牌是否被设置好
        for (int i = 0; i < mapRow; i++)
        {
            for(int j = 0; j < mapColumn; j++)
            {
                isSet = false;
                while (!isSet)
                {
                    int randomNumber = UnityEngine.Random.Range(0, maxRandomNumber);
                    CardType randomType = (CardType)randomNumber;
                    if (library[randomType] != 0)
                    {
                        library[randomType]--;
                        InitializeCard(randomType, cards, positionIndex, i, j);
                        positionIndex++;
                        isSet = true;
                    }
                }
            }
        }
        EmbedSharps();
    }
    void InitializeMapSize()//初始化地图大小
    {
        if (level == 0)
        {
            {
                mapRow = 3;
                mapColumn = 3;
            }
        }
        else
        {
            switch (childlevel)
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
    }
    void InitializeCardPosition()//卡牌位置
    {
        int positionIndex = 0;
        GameObject cardPositions = GameObject.FindGameObjectWithTag("CardPosition");
        int x, y;
        int z = 0;
        if(level == 0)
        {
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
        }
        else
        {
            switch (level)
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
                        break ;
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
    }
    void InitializeLibrary()//初始化牌库
    {
        library = new Dictionary<CardType, int>();
        if(level == 0)
        {
            library.Add(CardType.battle, 1);
            library.Add(CardType.eliteBattle, 1);
            library.Add(CardType.randomEvent, 1);
            library.Add(CardType.shop, 1);
            library.Add(CardType.inn, 1);
            library.Add(CardType.treasure, 1);
            library.Add(CardType.portal, 2);
            library.Add(CardType.placeOfGod, 1);
        }
        else
        {
            switch (childlevel)
            {
                case 1:
                    {
                        library.Add(CardType.battle, 7);
                        library.Add(CardType.eliteBattle, 2);
                        library.Add(CardType.randomEvent, 7);
                        library.Add(CardType.shop, 2);
                        library.Add(CardType.inn, 2);
                        library.Add(CardType.treasure, 2);
                        library.Add(CardType.portal, 2);
                        library.Add(CardType.placeOfGod, 1);
                        break;
                    }
                case 2:
                    {
                        library.Add(CardType.battle, 10);
                        library.Add(CardType.eliteBattle, 3);
                        library.Add(CardType.randomEvent, 10);
                        library.Add(CardType.shop, 3);
                        library.Add(CardType.inn, 3);
                        library.Add(CardType.treasure, 3);
                        library.Add(CardType.portal, 2);
                        library.Add(CardType.placeOfGod, 2);
                        break;
                    }
                case 3:
                    {
                        library.Add(CardType.battle, 13);
                        library.Add(CardType.eliteBattle, 5);
                        library.Add(CardType.randomEvent, 13);
                        library.Add(CardType.shop, 4);
                        library.Add(CardType.inn, 4);
                        library.Add(CardType.treasure, 4);
                        library.Add(CardType.portal, 3);
                        library.Add(CardType.placeOfGod, 3);
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }
    }
    void InitializeCard(CardType cardType,GameObject cards,int positionIndex,int row,int column)//初始化卡牌
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
    void EmbedSharps()//嵌入梦魇碎片
    {
        int row, column;
        GameObject target;
        for(nightmareSharps = 5; nightmareSharps > 0; nightmareSharps--)
        {
            row = UnityEngine.Random.Range(0, mapRow - 1);
            column = UnityEngine.Random.Range(0, mapColumn - 1);
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
    }
    void CardStateListener()//卡牌状态监听器
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
    void TransformCardState(int row,int column)//改变相邻卡牌状态
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
    public void FreezeMap()//冻结地图物件
    {
        for(int i = 0; i < mapRow; i++)
        {
            for(int j =0; j < mapColumn; j++)
            {
                map[i,j].SetActive(false);
            }
        }
    }
    public void ActivateMap()//激活地图物件
    {
        for (int i = 0; i < mapRow; i++)
        {
            for (int j = 0; j < mapColumn; j++)
            {
                map[i, j].SetActive(true);
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        CardStateListener();

    }
}
