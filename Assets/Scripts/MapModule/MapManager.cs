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
    public Dictionary<CardType, int> library;
    public GameObject[] cardPosition;
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
        Debug.Log("Level:" + level + "ChildLevel:" + childLevel);
        CopyObject();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        InitializeMap();
        map[0, 0].GetComponent<PlaceCard>().cardState = CardState.back;
        startListening = true;
    }
    void CopyObject()//获取对象用于场景切换
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        background = GameObject.FindGameObjectWithTag("Background");
        cardPositions = GameObject.FindGameObjectWithTag("CardPosition");
        cards = GameObject.FindGameObjectWithTag("Card");
        border = GameObject.FindGameObjectWithTag("Border");
        canvas = GameObject.FindGameObjectWithTag("Canvas");
        eventSystem = GameObject.FindGameObjectWithTag("EventSystem");
        Debug.Log("获取成功");
    }
    public void BackUpMap()//备份地图
    {
        int k = 0;//一维数组下标
        for(int i = 0; i < mapRow; i++)
        {
            for(int j = 0; j < mapColumn; j++)
            {
                player.cardStates[k] = map[i, j].GetComponent<PlaceCard>().cardState;
                player.rotation[k] = map[i, j].transform.eulerAngles.y;
                Debug.Log(player.rotation[k]);
                k++;
            }
        }
        Debug.Log("备份地图成功");
    }
    public void RecoverMap()//恢复地图
    {
        int k = 0;//一维数组下标
        for (int i = 0; i < mapRow; i++)
        {
            for (int j = 0; j < mapColumn; j++)
            {
                map[i, j].GetComponent<PlaceCard>().cardState = player.cardStates[k];
                map[i, j].transform.rotation = Quaternion.Euler(0, player.rotation[k], 0);
                k++;
            }
        }
        Debug.Log("恢复地图成功");
    }
    public void FreezeMap()//冻结地图物件
    {
        mainCamera.SetActive(false);
        background.SetActive(false);
        cardPositions.SetActive(false);
        cards.SetActive(false);
        border.SetActive(false);
        canvas.SetActive(false);
        eventSystem.SetActive(false);
    }
    public void ActivateMap()//激活地图物件
    {
        mainCamera.SetActive(true);
        background.SetActive(true);
        cardPositions.SetActive(true);
        cards.SetActive(true);
        border.SetActive(true);
        canvas.SetActive(true);
        eventSystem.SetActive(true);
    }
    void InitializeMap()//初始化地图
    {
        InitializeMapSize();
        map = new GameObject[mapRow, mapColumn];
        player.InitializeArray(mapRow*mapColumn);
        InitializeCardPosition();
        InitializeLibrary();
        int maxRandomNumber = library.Count;
        int positionIndex = 0;
        GameObject cards = GameObject.Find("Cards");
        bool isSet;//卡牌是否被设置好
        for (int i = 0; i < mapRow; i++)
        {
            for (int j = 0; j < mapColumn; j++)
            {
                isSet = false;
                while (!isSet)
                {
                    int randomNumber = UnityEngine.Random.Range(0, maxRandomNumber);//随机函数需要被替换
                    Debug.Log(randomNumber);
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
        if (level != 0)
        {
            int method = UnityEngine.Random.Range(1, 3);//调节方法 //随机函数需要被替换
            AdjustMap(method);
        }
        EmbedSharps();
    }
    void InitializeMapSize()//初始化地图大小
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
        Debug.Log("初始化地图大小 "+mapRow+" "+mapColumn+" 成功");
    }
    void InitializeCardPosition()//卡牌位置
    {
        int positionIndex = 0;
        GameObject cardPositions = GameObject.FindGameObjectWithTag("CardPosition");
        int x, y;
        int z = 0;
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
                        //待补全
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
        Debug.Log("初始化卡牌位置成功");
    }
    void InitializeLibrary()//初始化牌库
    {
        library = new Dictionary<CardType, int>();
        if (level == 0)
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
            switch (childLevel)
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
        Debug.Log("初始化牌库成功");
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
    void AdjustMap(int method)//调整地图以适应随机需要
    {
        if (method == 1)
        {
            SwapCard(ref map[0, 0], ref map[mapRow - 1, mapColumn - 1]);
            SwapPosition(map[0, 0], map[mapRow - 1, mapColumn - 1]);
            SwapCard(ref map[0, mapColumn - 1], ref map[mapRow - 1, 0]);
            SwapPosition(map[0, mapColumn - 1], map[mapRow - 1, 0]);
            SwapCard(ref map[1, 1], ref map[mapRow - 2, mapColumn - 2]);
            SwapPosition(map[1, 1], map[mapRow - 2, mapColumn - 2]);
            SwapCard(ref map[mapRow - 2, 1], ref map[1, mapColumn - 2]);
            SwapPosition(map[mapRow - 2, 1], map[1, mapColumn - 2]);
            SwapCard(ref map[0, 2], ref map[mapRow - 1, mapColumn - 3]);
            SwapPosition(map[0, 2], map[mapRow - 1, mapColumn - 3]);
            if (childLevel > 1)
            {
                SwapCard(ref map[0, 3], ref map[mapRow - 1, 2]);
                SwapPosition(map[0, 3], map[mapRow - 1, 2]);
            }
            if(childLevel > 2)
            {
                SwapCard(ref map[1, 3], ref map[5, 3]);
                SwapPosition(map[1, 3], map[5, 3]);
            }
        }
        else if(method == 2) 
        {
            SwapCard(ref map[1, 0], ref map[mapRow - 2, mapColumn - 1]);
            SwapPosition(map[1, 0], map[mapRow - 2, mapColumn - 1]);
            SwapCard(ref map[0, 1], ref map[mapRow - 1, mapColumn - 2]);
            SwapPosition(map[0, 1], map[mapRow - 1, mapColumn - 2]);
            SwapCard(ref map[1, 2], ref map[mapRow - 2, mapColumn - 3]);
            SwapPosition(map[1, 2], map[mapRow - 2, mapColumn - 3]);
            SwapCard(ref map[mapRow - 1, 1], ref map[0, mapColumn - 2]);
            SwapPosition(map[mapRow - 1, 1], map[0, mapColumn - 2]);
            SwapCard(ref map[mapRow - 2, 0], ref map[1, mapColumn - 1]);
            SwapPosition(map[mapRow - 2, 0], map[1, mapColumn - 1]);
            if(childLevel > 1)
            {
                SwapCard(ref map[1, 3], ref map[mapRow - 2, 2]);
                SwapPosition(map[1, 3], map[mapRow - 2, 2]);
            }
            if(childLevel > 2)
            {
                SwapCard(ref map[0, 3], ref map[6, 3]);
                SwapPosition(map[0, 3], map[6, 3]);
            }
        }
        Debug.Log("调节成功");
    }
    void SwapCard(ref GameObject a,ref GameObject b)//交换卡牌
    {
        GameObject temp;
        temp = a;
        a = b;
        b = temp;
    }
    void SwapPosition(GameObject a, GameObject b)//交换卡牌位置
    {
        (b.transform.position, a.transform.position) = (a.transform.position, b.transform.position);
    }
    void EmbedSharps()//嵌入梦魇碎片
    {
        int row, column;
        GameObject target;
        for(nightmareSharps = GetSharpNumber(); nightmareSharps > 0; nightmareSharps--)
        {
            row = UnityEngine.Random.Range(0, mapRow - 1);//随机函数需要被替换
            column = UnityEngine.Random.Range(0, mapColumn - 1);//随机函数需要被替换
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
        Debug.Log("嵌入碎片成功");
    }
    int GetSharpNumber()
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
    void PlayerStateListener()
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
    // Update is called once per frame
    void Update()
    {
        CardStateListener();
        PlayerStateListener();
        player.level = level;
        player.childLevel = childLevel;
    }
}
