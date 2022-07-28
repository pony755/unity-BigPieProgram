using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlaceCard;

public class MapManager : MonoBehaviour
{
    public int level;
    private int mapRow;
    private int mapColumn;
    public GameObject[,] map;
    public List<CardType> typeList;
    public Dictionary<CardType, int> cardLibrary;
    public int[] cardNumber;
    public GameObject[] cardPosition;
    public bool isTurning = false;
    private bool startListening = false;

    // Start is called before the first frame update
    void Start()
    {
        InitializeMap();
        map[0, 0].GetComponent<PlaceCard>().cardState = CardState.back;
        startListening = true;
    }
    void InitializeMap()//初始化地图
    {
        InitializeMapSize();
        map = new GameObject[mapRow, mapColumn];
        InitializeCardPosition();
        InitializeTypeList();
        int maxRandomNumber = typeList.Count;
        int positionIndex = 0;
        GameObject cards = GameObject.Find("Cards");
        for (int i = 0; i < mapRow; i++)
        {
            for(int j = 0; j < mapColumn; j++)
            {
                int randomNumber = UnityEngine.Random.Range(0, maxRandomNumber);
                InitializeCard(typeList[randomNumber], cards, positionIndex, i, j);
                positionIndex++;
            }
        }
    }
    void InitializeMapSize()//初始化地图大小
    {
        switch (level)
        {
            case 0:
                {
                    mapRow = 3;
                    mapColumn = 3;
                    break;
                }
            case 1:
            case 2:
                {
                    mapRow = 5;
                    mapColumn = 5;
                    break;
                }
            case 3:
            case 4:
                {
                    mapRow = 6;
                    mapColumn = 6;
                    break;
                }
            case 5:
                {
                    mapRow = 7;
                    mapRow = 7;
                    break;
                }
            default:
                {
                    break;
                }
        }
    }
    void InitializeCardPosition()//卡牌位置
    {
        int positionIndex = 0;
        GameObject cardLocations = GameObject.FindGameObjectWithTag("CardPosition");
        int x, y;
        int z = 0;
        switch (level)
        {
            case 0:
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
                            positionObject.transform.SetParent(cardLocations.transform);
                            cardPosition[positionIndex] = positionObject;
                            positionIndex++;
                            x += 2;
                        }
                        y -= 3;
                    }
                    break;
                }
            case 1:
            case 2:
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
                            positionObject.transform.SetParent(cardLocations.transform);
                            cardPosition[positionIndex] = positionObject;
                            positionIndex++;
                            x += 2;
                        }
                        y -= 3;
                    }
                    break;
                }
            case 3:
            case 4:
                {
                    break;
                }
            case 5:
                {
                    break;
                }
            default:
                {
                    break;
                }
        }
    }
    void InitializeTypeList()//将卡牌类型从枚举转入列表
    {
        Array array = Enum.GetValues(typeof(CardType));
        foreach (var type in array)
        {
            typeList.Add((CardType)type);
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
    void CardStateListener()//卡牌状态监听器
    {
        if (startListening)
        {
            for (int i = 0; i < mapRow; i++)
            {
                for (int j = 0; j < mapColumn; j++)
                {
                    if (map[i, j].GetComponent<PlaceCard>().linked)
                    {
                        continue;
                    }
                    else if (map[i, j].GetComponent<PlaceCard>().cardState.Equals(CardState.face))
                    {
                        TransformCardState(i, j);
                        map[i, j].GetComponent<PlaceCard>().linked = true;
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
                map[i,j].gameObject.SetActive(false);
            }
        }
    }
    public void ActivateMap()//激活地图物件
    {
        for (int i = 0; i < mapRow; i++)
        {
            for (int j = 0; j < mapColumn; j++)
            {
                map[i, j].gameObject.SetActive(true);
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        CardStateListener();
    }
}
