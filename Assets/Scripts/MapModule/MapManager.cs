using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlaceCard;

public class MapManager : MonoBehaviour
{
    static int mapRow = 3;
    static int mapColumn = 3;
    public GameObject[,] map = new GameObject[mapRow,mapColumn];
    public List<CardType> typeList;
    public Transform[] cardPosition;

    // Start is called before the first frame update
    void Start()
    {
        InitializeMap();
    }
    void InitializeMap()//初始化地图
    {
        int positionIndex = 0;
        InitializeTypeList();
        int maxRandomNumber = typeList.Count;
        for(int i = 0; i < mapRow; i++)
        {
            for(int j = 0; j < mapColumn; j++)
            {
                int randomNumber = UnityEngine.Random.Range(0, maxRandomNumber);
                InitializeCard(typeList[randomNumber], positionIndex, i, j);
                positionIndex++;
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
    void InitializeCard(CardType cardType,int positionIndex,int row,int column)//初始化卡牌
    {
        GameObject card = Instantiate(Resources.Load<GameObject>("Prefabs/Card"));
        card.GetComponent<PlaceCard>().cardType = cardType;
        card.name = "Card_" + cardType.ToString();
        card.transform.position = cardPosition[positionIndex].position;
        GameObject cardFace = Instantiate(Resources.Load<GameObject>("Prefabs/Cardface"));
        cardFace.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("MapGraphics/Cardface_" + cardType.ToString());
        cardFace.transform.SetParent(card.transform);
        cardFace.transform.localPosition = new Vector3(0, 0, 0.01f);
        cardFace.transform.eulerAngles = new Vector3(0, 180, 0);
        map[row, column] = card;
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
        
    }
}
