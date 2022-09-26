using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static PlaceCard;

public class MapManager : MonoBehaviour
{
    public Player player;
    public string saveNumber;
    public int level;
    public int childLevel;
    public GameObject[,] map;
    private int mapRow;
    private int mapColumn;
    public GameObject[] cardPosition;
    private int shopLimit;//商店最大生成数
    private int shopCount = 0;//商店数量
    private int innLimit;//旅馆最大生成数
    private int innCount = 0;//旅馆数量
    private int treasureLimit;//宝藏最大生成数
    private int treasureCount = 0;//宝藏数量
    private int portalLimit;//传送门最大生成数
    private int portalCount = 0;//传送门数量
    private int placeOfGodLimit;//神意之地最大生成数
    private int placeOfGodCount = 0;//神意之地数量
    public int nightmareSharps;
    public bool isTurning = false;
    private bool startListening = false;
    private const string locatorName = "SaveLocator.sav";
    private const string selectorName = "ConstructionModeSelector.sav";
    [Header("地图物件")]
    public GameObject mainCamera;
    public GameObject background;
    public GameObject cardPositions;
    public GameObject cards;
    public GameObject border;
    public GameObject canvas;
    public GameObject eventSystem;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        BuildMap();
    }
    void Update()
    {
        CardStateListener();
        PlayerStateListener();
        player.level = level;
        player.childLevel = childLevel;
    }
    void Test()//测试函数
    {
        int k = 0;//一维数组下标
        for (int i = 0; i < mapRow; i++)
        {
            for (int j = 0; j < mapColumn; j++)
            {
                Debug.Log(player.cardTypes[k]);
                Debug.Log(player.cardStates[k]);
                Debug.Log(player.rotation[k]);
                Debug.Log(player.embededFlags[k]);
                k++;
            }
        }
        Debug.Log("Level:" + player.level);
        Debug.Log("ChildLevel:" + player.childLevel);
        Debug.Log("BP:" + player.BP);
    }
    void BuildMap()//建造地图
    {
        GetSaveNumber();
        int mode = Convert.ToInt32(GetConstructionMode());//获取构造模式，0为完全初始化，1为读取记录
        if (mode == 1)
        {
            GetLevel();

        }
        InitializeMap();
        player.awakeCount = 1;//测试用
        map[0, 0].GetComponent<PlaceCard>().cardState = CardState.back;
        if (mode == 1)
        {
            player.Load(saveNumber);
            //Test();
            RecoverMap();
        }
        BackUpMap();
        player.Save(saveNumber);
        startListening = true;
    }
    void GetLevel()//获取关卡号
    {
        Debug.Log(saveNumber);
        player.Load(saveNumber);
        level = player.level;
        childLevel = player.childLevel;
    }
    void GetSaveNumber()//获取存档编号
    {
        saveNumber = File.ReadAllText(Path.Combine(Application.persistentDataPath, locatorName));
    }
    string GetConstructionMode()
    {
        string mode = File.ReadAllText(Path.Combine(Application.persistentDataPath, selectorName));
        return mode;
    }
    void InitializeMap()//初始化地图
    {
        InitializeMapSize();
        map = new GameObject[mapRow, mapColumn];
        player.InitializeArray(mapRow*mapColumn);
        InitializeCardPosition();
        SetCardLimit();
        PresetSpecialCards();
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
        Debug.Log("初始化卡牌位置成功");
    }
    void SetCardLimit()//设置部分卡牌限制数量
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
    void PresetSpecialCards()//预置特定卡牌
    {

    }
    CardType RamdomCardType(double bp0, double bp1, double bp2, double bp3, double bp4, double bp5, double bp6)//随机卡牌类型
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
        Debug.Log("嵌入碎片成功");
    }
    int GetSharpNumber()//获取应嵌入地图的碎片数量
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
    void PlayerStateListener()//玩家状态监听器
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
    public void BackUpMap()//备份地图
    {
        int k = 0;//一维数组下标
        for (int i = 0; i < mapRow; i++)
        {
            for (int j = 0; j < mapColumn; j++)
            {
                player.cardTypes[k] = map[i, j].GetComponent<PlaceCard>().cardType;
                player.cardStates[k] = map[i, j].GetComponent<PlaceCard>().cardState;
                player.rotation[k] = map[i, j].transform.eulerAngles.y;
                player.embededFlags[k] = map[i, j].GetComponent<PlaceCard>().isEmbedded;
                Debug.Log(player.rotation[k]);
                k++;
            }
        }
        player.level = level;
        player.childLevel = childLevel;
        Debug.Log("备份地图成功");
    }
    public void RecoverMap()//恢复地图
    {
        int k = 0;//一维数组下标
        for (int i = 0; i < mapRow; i++)
        {
            for (int j = 0; j < mapColumn; j++)
            {
                map[i, j].GetComponent<PlaceCard>().cardType = player.cardTypes[k];
                map[i, j].transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("MapGraphics/Cardface_" + player.cardTypes[k].ToString());
                map[i, j].GetComponent<PlaceCard>().cardState = player.cardStates[k];
                map[i, j].transform.rotation = Quaternion.Euler(0, player.rotation[k], 0);
                map[i, j].GetComponent<PlaceCard>().isEmbedded = player.embededFlags[k];
                if(map[i, j].GetComponent<PlaceCard>().isEmbedded)
                {
                    map[i, j].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("MapGraphics/Cardback_Nightmare");
                }
                else
                {
                    map[i, j].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("MapGraphics/Cardback");
                }
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
    public void NextLevel()//加载下一关卡
    {
        childLevel = 1;
        File.WriteAllText(Path.Combine(Application.persistentDataPath, selectorName), "0");
        DestroyMap();
        BuildMap();
    }
    public void NextChildLevel()//加载下一子关卡
    {
        if (childLevel == 3)
        {
            NextLevel();
        }
        childLevel++;
        File.WriteAllText(Path.Combine(Application.persistentDataPath, selectorName),"0");
        DestroyMap();
        BuildMap();
    }
    void DestroyMap()//摧毁地图
    {
        for(int i = 0; i < map.Length; i++)
        {
            Destroy(cardPositions.transform.GetChild(i).gameObject);
            Destroy(cards.transform.GetChild(i).gameObject);
        }
    }

    /*//以下为开发者功能
    [UnityEditor.MenuItem("Developer/跳转到下一子关卡")]*/
    public static void DevNextChildLevel()
    {
        MapManager mapManager = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>();
        mapManager.NextChildLevel();
    }
}
