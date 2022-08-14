using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Koubot.Tool;

public enum BattleState { NONE,START, PLAYERTURNSTART,PLAYERTURN, POINTALL,SKILL,CARDTURNUNIT,POINTENEMY,POINTPLAYER,POINTPREPAREHERO,TOACTION,ACTION,ACTIONFINISH,ABANDOMCARD,ENEMYTURNSTART, ENEMYTURN,ENEMYFINISH,OVER }


public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public bool over ;//战斗结束标志位
    public bool win ;//胜利条件标志位
    [HideInInspector] public bool AdjustCards = false;//调整卡牌位置标志位

    [Header("全物体List")]   
    public GameObject allListObject;
    public GameObject tempPlayer;
    [Header("画布设置")]
    public FightPlayerCards fightPlayerCards;//卡牌设置
    public TipsDelayed tipsDelayed;//延迟技能提示框
    public GameObject battleBackGround;//战斗背景
    public GameObject heroPrepare;//存放小队的地方
    public GameObject turnTipsObject;//提示框
    public Text turnNum;//回合数
    public Text tips;//提示框
    public GameObject backPanel;//返回画布
    public GameObject WinOrLost;//胜负画布
    public GameObject CardCanvas;//卡牌画布
    public GameObject AbandomCardCheck;//弃牌查看
    public GameObject exchange;//换人栏
    public GameObject skillImg;//技能栏
    public GameObject skillText;//技能介绍
    public GameObject addCancleBtn;//摸牌按钮
    public List<GameObject> skillBtns;//技能按钮   


    [Header("己方UI设置")]
    public BattleHub[] Hub;//状态栏
    public Transform[] playerStations;//设置角色坐标

    [Header("敌方UI设置")]
    public BattleHub[] enemyHub;//状态栏
    public Transform[] enemyStations;//设置角色坐标

    [Header("战斗存档设置")]
    public FightPrefs fightPrefs;//战斗存档
    public EnemyPrefs enemyPrefs;//敌人列表


    [Header("——————CHECKING——————")]
    public int abandomCardNum;
    public bool abandomCardSwitch;
    public BattleState state;

    [Header("Heros")]
    public List<GameObject> heroPrefab;//接收战斗列表角色
    public List<Unit> heroUnit;//获取战斗列表角色Unit脚本
    public List<GameObject> heroPreparePrefab;//接收小队列表角色

    [Header("enemyHeros")]
    public List<GameObject> enemyPrefab;//接收敌人列表角色
    public List<Unit> enemyUnit;//获取敌人列表角色Unit脚本
    // Start is called before the first frame update
    [Header("——————延时结算——————")]
    public bool delayedSwitch;
    public List<int> delayedTurn;//延迟的回合数
    public List<Unit> delayedTurnUnit;//延迟回合技能发动方
    public List<Skill> delayedSkill;//延迟回合的技能
    public List<Unit> delayedPointUnit;//延迟回合技能目标方
    [Header("——————FIGHTING——————")]

    public int turn;
    public List<Unit> turnUnit;//当前回合技能发动方
    public Skill useSkill;//角色技能
    public Cards useCard;//角色技能
    public int pointNumber;//目标数量
    public List<Unit> pointUnit;//当前回合技能目标方
    
    public void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
        //在fightprefs上setHero

        LeanTween.move(turnTipsObject, new Vector3(turnTipsObject.transform.position.x, turnTipsObject.transform.position.y-200f, turnTipsObject.transform.position.z), 0.8f);
        win = false;
        over = false;
        delayedSwitch = false;
        state = BattleState.PLAYERTURNSTART;
        pointNumber = 1;//默认值
        useSkill = null;//默认值       
        turn = 1;
        
    }
    private void Start()
    {
        StartCoroutine(Load());       
    }

    
    IEnumerator Load()
    {
        Debug.Log("加载游戏");
        yield return new WaitForSeconds(1f);//充分加载
        StartCoroutine(PlayerTurnStart());
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTips();  
        if ((heroUnit.Count== 0|| enemyUnit.Count == 0 )&& over==false)
        {
            over = true;           
            StartCoroutine(Over());
            GameReset();
        }
        CheckAbandomCardNum();

        

        if((state == BattleState.POINTPLAYER|| state == BattleState.POINTENEMY|| state == BattleState.POINTALL||state==BattleState.POINTPREPAREHERO) &&pointNumber==pointUnit.Count)
            StartCoroutine("ToAction");
        if (state == BattleState.ACTION)//当进入ACTION时，执行函数(携程）  
            StartCoroutine("Action");
        if(state == BattleState.ABANDOMCARD)
        {
            if(fightPlayerCards.haveCards.Count<= fightPlayerCards.maxCard)
            {
                StartCoroutine(EnemyTurnStart());
            }
                
        }
        if (AdjustCards)
        {
            AdjustCards=false;
            StartCoroutine(fightPlayerCards.CardAdjustPosition());
        }
    }
    
    public IEnumerator SetHeros()//在对应位置设置战斗队伍预置体以及状态栏
    {
        for (int i = 0; i < fightPrefs.fightHeros.Count; i++)
        {
            SetSingleHeros(i);
        }
        for (int j = 0; j < fightPrefs.fightPrepareHeros.Count; j++)
        {
            SetSinglePrepareHeros(j);
        }
        yield return null;
    }

    public void SetSingleHeros(int i)
    {
        heroPrefab.Add(Instantiate(fightPrefs.fightHeros[i], playerStations[i].position, playerStations[i].rotation));
        heroPrefab[i].transform.SetParent(battleBackGround.transform);
        heroPrefab[i].GetComponent<SpriteRenderer>().sortingOrder = i;
        heroUnit.Add(heroPrefab[i].GetComponent<Unit>());//添加unit进列表
                                                             //读取数据
        Hub[i].SetHub(heroUnit[i]);
        Hub[i].gameObject.SetActive(true);//显示对应角色状态栏
        LeanTween.move(Hub[i].gameObject, new Vector3(Hub[i].gameObject.transform.position.x + 350f, Hub[i].gameObject.transform.position.y, Hub[i].gameObject.transform.position.z), 0.8f);
    }
    public void SetSinglePrepareHeros(int i)
    {
        heroPreparePrefab.Add(Instantiate(fightPrefs.fightPrepareHeros[i], heroPrepare.transform.position, playerStations[i].rotation));
        heroPreparePrefab[i].transform.SetParent(heroPrepare.transform);

    }

    public IEnumerator SetEnemyHeros()//在对应位置设置战斗队伍预置体以及状态栏
    {
        for (int j = 0; j < enemyPrefs.enemyHeros.Count; j++)
        {
            enemyPrefab.Add(Instantiate(enemyPrefs.enemyHeros[j], enemyStations[j].position, enemyStations[j].rotation));
            enemyPrefab[j].transform.SetParent(battleBackGround.transform);
            enemyPrefab[j].GetComponent<SpriteRenderer>().sortingOrder = j;
            //读取数据
            enemyUnit.Add(enemyPrefab[j].GetComponent<Unit>());
            enemyHub[j].SetHub(enemyUnit[j]);
            enemyHub[j].gameObject.SetActive(true);//显示对应角色状态栏
            LeanTween.move(enemyHub[j].gameObject, new Vector3(enemyHub[j].gameObject.transform.position.x - 350f, enemyHub[j].gameObject.transform.position.y, enemyHub[j].gameObject.transform.position.z), 0.8f);
        }
        yield return null;
    }






    //————————————————————————UI——————————————————————————
    public void SkillShow(Unit unit)//显示技能栏(code为当前场上角色编号）
    {
        skillImg.SetActive(true);
        turnUnit.Add(unit);
        state= BattleState.SKILL;//切换回合状态
        BtnSet(unit);
       
    }
    public void BtnSet(Unit unit)
    {

        for (int i = 0; i < unit.heroSkillList.Count; i++)//设置按钮
        {
            skillBtns[i].GetComponent<SkillBtn>().skillInfo = unit.heroSkillList[i];
            skillBtns[i].SetActive(true);
            if (unit.heroSkillList[i].passiveType!=PassiveType.None)
            {
                skillBtns[i].GetComponent<Button>().interactable = false;
            }
        }
    }
    public void GameReset()//重置
    {     
        BtnHide();     
        
        pointNumber = 1;//默认值
        useSkill=null;//默认值
        useCard=null;
        foreach(var btn in exchange.GetComponent<Exchange>().herosBtn)
            btn.gameObject.SetActive(false);
        skillImg.SetActive(false);
        exchange.SetActive(false);
        CardCanvas.SetActive(true);
        turnUnit.Clear();
        pointUnit.Clear();
        AdjustCards = true; 
        StartCoroutine(TipsClear());
    }
    IEnumerator TipsClear()
    {
    yield return new WaitForSeconds(0.5f);
        tips.text = "";
    }


    public void BtnHide()
    {
        for (int i = 0; i < skillBtns.Count; i++)//隐藏按钮
        {
            skillBtns[i].SetActive(false);
            skillBtns[i].GetComponent<Button>().interactable = true;
            skillBtns[i].GetComponent<SkillBtn>().skillInfo = null;
        }
    }
    public void Back()//返回玩家回合
    {

            if (state == BattleState.CARDTURNUNIT||state == BattleState.PLAYERTURN || state == BattleState.SKILL || state == BattleState.POINTENEMY || state == BattleState.POINTPLAYER||state==BattleState.TOACTION)
        {
            StopCoroutine("ToAction");
            GameReset();
            state = BattleState.PLAYERTURN;
            foreach(var o in heroUnit)
                o.anim.Play("idle");
            abandomCardNum = 0;
        }
        CardCanvas.SetActive(true);    
    }

    IEnumerator DelayedPlayerSettle()
    {
        int tempDelayedCount = delayedTurn.Count;
        int temp = 0;
        for (int j = 0; j < tempDelayedCount; j++)
        {
            if (delayedTurn[temp] == turn)
            {
                if (delayedTurnUnit[temp].playerHero && (delayedSkill[temp].type != SkillType.AttributeAdjust))
                {
                    if (delayedTurnUnit[temp].currentHP > 0)
                    {
                        tips.text = delayedTurnUnit[temp].unitName + " 结算 " + delayedSkill[temp].skillName;
                        delayedSwitch = true;
                        delayedPointUnit[temp].SkillSettle(delayedTurnUnit[temp], delayedSkill[temp]);
                        delayedSwitch = false;
                    }
                    else
                        tips.text = delayedTurnUnit[temp].unitName + " 已死亡, " + delayedSkill[temp].skillName + " 结算失败 ";
                    delayedTurn.Remove(delayedTurn[temp]);
                    delayedTurnUnit.Remove(delayedTurnUnit[temp]);
                    delayedSkill.Remove(delayedSkill[temp]);
                    delayedPointUnit.Remove(delayedPointUnit[temp]);
                    yield return new WaitForSeconds(0.1f);
                }
                else if (delayedPointUnit[temp].playerHero && (delayedSkill[temp].type == SkillType.AttributeAdjust))
                {
                    if (delayedPointUnit[temp].currentHP > 0)
                    {
                        tips.text = delayedTurnUnit[temp].unitName + " " + delayedSkill[temp].skillName;
                        delayedSwitch = true;
                        delayedPointUnit[temp].SkillSettle(delayedTurnUnit[temp], delayedSkill[temp]);
                        delayedSwitch = false;
                    }
                    delayedTurn.Remove(delayedTurn[temp]);
                    delayedTurnUnit.Remove(delayedTurnUnit[temp]);
                    delayedSkill.Remove(delayedSkill[temp]);
                    delayedPointUnit.Remove(delayedPointUnit[temp]);
                }
                else
                {
                    temp = temp + 1;
                }

            }
            else
                temp += 1;
        }
    }
    IEnumerator DelayedEnemySettle()
    {
        int tempDelayedCount = delayedTurn.Count;
        int temp = 0;
        for (int j = 0; j < tempDelayedCount; j++)
        {
            if (delayedTurn[temp] == turn)
            {
                if (!delayedTurnUnit[temp].playerHero && (delayedSkill[temp].type != SkillType.AttributeAdjust))
                {
                    if (delayedTurnUnit[temp].currentHP > 0)
                    {
                        tips.text = delayedTurnUnit[temp].unitName + " 结算 " + delayedSkill[temp].skillName;
                        delayedSwitch = true;
                        delayedPointUnit[temp].SkillSettle(delayedTurnUnit[temp], delayedSkill[temp]);
                        delayedSwitch = false;
                    }
                    else
                        tips.text = delayedTurnUnit[temp].unitName + " 已死亡, " + delayedSkill[temp].skillName + " 结算失败 ";
                    delayedTurn.Remove(delayedTurn[temp]);
                    delayedTurnUnit.Remove(delayedTurnUnit[temp]);
                    delayedSkill.Remove(delayedSkill[temp]);
                    delayedPointUnit.Remove(delayedPointUnit[temp]);
                    yield return new WaitForSeconds(0.1f);
                }
                else if (!delayedPointUnit[temp].playerHero && (delayedSkill[temp].type == SkillType.AttributeAdjust))
                {
                    if (delayedPointUnit[temp].currentHP > 0)
                    {
                        tips.text = delayedTurnUnit[temp].unitName + " " + delayedSkill[temp].skillName;
                        delayedSwitch = true;
                        delayedPointUnit[temp].SkillSettle(delayedTurnUnit[temp], delayedSkill[temp]);
                        delayedSwitch = false;
                    }
                    delayedTurn.Remove(delayedTurn[temp]);
                    delayedTurnUnit.Remove(delayedTurnUnit[temp]);
                    delayedSkill.Remove(delayedSkill[temp]);
                    delayedPointUnit.Remove(delayedPointUnit[temp]);
                }
                else
                {
                    temp = temp + 1;
                }

            }
            else
                temp += 1;
        }

    }



    //————————————————————————阶段—————————————————————————
    //回合各阶段函数
    IEnumerator PlayerTurnStart()
    {
        if (over==true)
        {
            yield return null;
        }
        else
        {
            tips.text = "你的回合...";
            turnNum.text = turn.ToString();

            if (fightPlayerCards.playerCards.Count == 0)
            {
                fightPlayerCards.cardsObject.transform.GetChild(0).gameObject.SetActive(false);
                fightPlayerCards.ResetCards();
            }
                
            state = BattleState.PLAYERTURNSTART;
            //结算状态
            for (int i = 0; i < heroUnit.Count; i++)
            {
                heroUnit[i].PoisonDamage();
                heroUnit[i].TurnBeginSettle();
            }
                
            yield return new WaitForSeconds(0.5f);
            for (int i = 0; i < heroUnit.Count; i++)
                heroUnit[i].PassiveTurnStart();
            yield return new WaitForSeconds(0.5f);
            StartCoroutine(DelayedPlayerSettle());
            yield return new WaitForSeconds(0.5f);                 
            yield return new WaitUntil(() => abandomCardSwitch == false);
            tips.text = "";
            state = BattleState.PLAYERTURN;

        }


    }
    public void SkillToAction()
    {
        StartCoroutine(ToAction());
    }
    IEnumerator ToAction()//过度态
    {
        BtnHide();
        exchange.SetActive(false);
        skillImg.SetActive(false);
        CardCanvas.SetActive(true);
        abandomCardNum += useSkill.abandomCardNum;
        state = BattleState.TOACTION;
        yield return new WaitForSeconds(0.2f);
        yield return new WaitUntil(() => abandomCardSwitch == false);
        state = BattleState.ACTION;

    }
    //使用技能的text提示在SkillBtn里
    IEnumerator Action()//行动阶段函数
    {  
        state = BattleState.ACTIONFINISH;//及时切换state,防止多次运行此函数    
        if (over == true)
        {
            yield return null;
        }
        else
        {                 
            //重置动画
            foreach (var o in heroUnit)
            {
                o.anim.Play("idle");
            }
            foreach (var o in enemyUnit)
            {
                o.anim.Play("idle");
            }
            CardUse();
            TipsSkillPoint();
            yield return new WaitForSeconds(0.3f);
            if (useSkill != null)
            {
                if(Probility(useSkill.precent))
                    {

                    tips.text = "欧不！ " + useSkill.skillName + " 发动失败";
                    turnUnit[0].SkillCost(useSkill);
                    AdjustCards=true;
                    yield return new WaitForSeconds(0.5f);
                }
                else
                {
                    StartCoroutine(TurnUnitAnim());
                }
            }
            yield return new WaitForSeconds(1f);
            if (state == BattleState.ACTIONFINISH)
            {
                yield return new WaitUntil(() => abandomCardSwitch == false);
                StartCoroutine(ActionFinish());
            }
                

        }

    }
    public IEnumerator ActionFinish()
    {
        if (over == true)
        {
            yield return null;
        }
        else
        {         
            yield return new WaitForSeconds(1f);
            tips.text = "己方回合结束";
            for (int i = 0; i < heroUnit.Count; i++)
                heroUnit[i].PassiveTurnEnd();
            yield return new WaitForSeconds(1f);
            foreach (var o in heroUnit)
            {
                o.TurnEndSettle();
                if (o.tired > 0 && !turnUnit.Contains(o))
                {
                    o.tired--;
                    tips.text = o.unitName + " 减少1点疲劳";
                    yield return new WaitForSeconds(0.3f);
                }
            }
            GameReset();
            yield return new WaitForSeconds(1f);
            yield return new WaitUntil(() => abandomCardSwitch == false);
            StartCoroutine(AbandomCard());
        }
       
    }



    IEnumerator AbandomCard()
    {
        if (over == true)
        {
            yield return null;
        }
        else
        {         
            if (fightPlayerCards.haveCards.Count <= fightPlayerCards.maxCard)
                StartCoroutine(EnemyTurnStart());
            else
            {
                state = BattleState.ABANDOMCARD;
            }
        }
           
    }
    IEnumerator EnemyTurnStart()
    {
        if (over == true)
        {
            yield return null;
        }
        else
        {       
            state = BattleState.ENEMYTURNSTART;
            tips.text = "敌方回合";
            //结算状态
            for (int i = 0; i < enemyUnit.Count; i++)
            {
                enemyUnit[i].PoisonDamage();
                enemyUnit[i].TurnBeginSettle();
            }
                
            yield return new WaitForSeconds(0.5f);
            for (int i = 0; i < enemyUnit.Count; i++)
                enemyUnit[i].PassiveTurnStart();
            yield return new WaitForSeconds(0.5f);
            StartCoroutine(DelayedEnemySettle());
            yield return new WaitForSeconds(1f);
            yield return new WaitUntil(() => abandomCardSwitch == false);
            StartCoroutine(EnemyTurn());

        }
     
    }

    IEnumerator EnemyTurn()
    {
        if (over == true)
        {
            yield return null;
        }
        else
        {         
            state = BattleState.ENEMYTURN;
            tips.text = "等待敌方行动...";
            StartCoroutine(EnemyAI());
            yield return new WaitForSeconds(1f);
            TipsSkillPoint();
            yield return new WaitForSeconds(0.5f);
            if (useSkill != null)
            {
                if (Probility(useSkill.precent))
                {
                    tips.text = "欧不！ " + useSkill.skillName + " 发动失败";
                    turnUnit[0].SkillCost(useSkill);
                    yield return new WaitForSeconds(1f);
                }

                else
                {
                    StartCoroutine(TurnUnitAnim());
                }
            }
            yield return new WaitForSeconds(2f);
            yield return new WaitUntil(() => abandomCardSwitch == false);
            StartCoroutine(EnemyFinish());
        }
                   

    }
    IEnumerator EnemyFinish()
    {
        if (over == true)
        {
            yield return null;
        }
        else
        {         
            tips.text = "敌方回合结束";
            for (int i = 0; i < enemyUnit.Count; i++)
                enemyUnit[i].PassiveTurnEnd();
            yield return new WaitForSeconds(1f);
            foreach (var o in enemyUnit)
            {
                o.TurnEndSettle();
                if (o.tired > 0 && !turnUnit.Contains(o))
                {
                    o.tired--;
                    tips.text = o.unitName + " 减少1点疲劳";
                    yield return new WaitForSeconds(0.3f);
                }
            }
            GameReset();
            yield return new WaitForSeconds(1f);
            yield return new WaitUntil(() => abandomCardSwitch == false);
            turn++;
            StartCoroutine(PlayerTurnStart());
        }       
    }

    IEnumerator Over()
    {
        state= BattleState.OVER;
        Time.timeScale = 1;
        Time.timeScale = 1;
        yield return new WaitForSeconds(1f);
        if(enemyUnit.Count==0)
        {
            win = true;
            Debug.Log("《《《《你赢了》》》》");
        }
        else
            Debug.Log("《《《《你输了》》》》");
        WinOrLost.SetActive(true);
        
    }


    private void UpdateTips()
    {
        if (tips.text == "")
        {
            tips.transform.parent.gameObject.SetActive(false);
        }
        else
        {
            tips.transform.parent.gameObject.SetActive(true);
        }
        if(state==BattleState.POINTENEMY|| state == BattleState.POINTPLAYER|| state == BattleState.POINTALL)
            GameManager.instance.tips.text = "选择 " + GameManager.instance.useSkill.skillName + " 的目标("+GameManager.instance.pointUnit.Count+"/"+ GameManager.instance.pointNumber+")";       
        
        if(state == BattleState.ABANDOMCARD)
        {
            GameManager.instance.tips.text = "弃掉" + (fightPlayerCards.haveCards.Count - fightPlayerCards.maxCard).ToString() + "张牌";
        }
    }
    IEnumerator EnemyAI()
    {
        List<Unit> tempEnemy = new List<Unit>();
        foreach (var o in enemyUnit)
        {
            if (o.tired == 0&&o.currentHP>0)//提取0疲劳的敌人
            {
                foreach (var t in o.heroSkillList)
                {
                    if (o.currentMP >= t.needMP)//提取有技能可用的敌人
                    {
                        tempEnemy.Add(o);
                        break;
                    }
                }
            }
        }

        if (tempEnemy.Count == 0)
        {
            tempEnemy.Clear();
        }
        else
        {
            turnUnit.Add(tempEnemy[Koubot.Tool.Random.RandomTool.GenerateRandomInt(0, tempEnemy.Count - 1)]);//随机添加一个敌方
            tempEnemy.Clear();

            List<Skill> tempSkill = new List<Skill>();
            foreach (var t in turnUnit[0].heroSkillList)
            {
                if (turnUnit[0].currentMP >= t.needMP&&t.passiveType==PassiveType.None)
                    tempSkill.Add(t);
            }
            useSkill = tempSkill[Koubot.Tool.Random.RandomTool.GenerateRandomInt(0, tempSkill.Count - 1)];
            tempSkill.Clear();
            pointNumber = useSkill.pointNum;//添加技能目标数量
            useSkill.EnemyUse();
            yield return new WaitForSeconds(1f);
            if(useSkill.point==SkillPoint.Enemies)
            {
                while (pointNumber > pointUnit.Count)//添加玩家作为目标
                {
                    if (!useSkill.reChoose)
                    {                      
                        int player = Koubot.Tool.Random.RandomTool.GenerateRandomInt(0, heroUnit.Count - 1);
                        if (!pointUnit.Contains(heroUnit[player]))
                            pointUnit.Add(heroUnit[player]);
                    }
                    else
                    {
                        yield return new WaitForSeconds(0.1f);
                        pointUnit.Add(heroUnit[Koubot.Tool.Random.RandomTool.GenerateRandomInt(0, heroUnit.Count - 1)]);
                    }
                }
            }
            else if (useSkill.point == SkillPoint.Players)
            {
                while (pointNumber > pointUnit.Count)//添加玩家作为目标
                {
                    if (!useSkill.reChoose)
                    {
                        int player = Koubot.Tool.Random.RandomTool.GenerateRandomInt(0, enemyUnit.Count - 1);
                        if (!pointUnit.Contains(enemyUnit[player]))
                            pointUnit.Add(enemyUnit[player]);
                    }
                    else
                    {
                        pointUnit.Add(enemyUnit[Koubot.Tool.Random.RandomTool.GenerateRandomInt(0, enemyUnit.Count - 1)]);
                    }
                }
            }


        }
    }
    public void TipsSkillPoint()//文本函数
    {
        List<Unit> tempList = new List<Unit>(); 
        if (over==true)
            return;


        if (turnUnit.Count == 0)//为空时，可能是卡牌,可能是敌人无法使用技能
        {
            if(useSkill==null)
            {
                tips.text = "敌方无法行动";
                return;
            }
        }
            
        if(pointUnit.Count==1&&pointUnit[0]==turnUnit[0])//对自己使用
        {
            tips.text= turnUnit[0].unitName + " 对自己使用了 " + useSkill.skillName;
        }
        else if(useSkill.type==SkillType.Excharge)
        {
            tips.text = turnUnit[0].unitName + "准备跑路";
        }
        else
        {
            string tempText = " ";
            foreach (var o in pointUnit)
            {
                
                if(!tempList.Contains(o))
                {
                    tempList.Add(o);
                    tempText = tempText + o.unitName + " ";
                }
                
            }
            tips.text = turnUnit[0].unitName + " 对 " + tempText + " 使用了 " + useSkill.skillName;
        }
        tempList.Clear();
    }

    public void CardUse()
    {
        if (useCard != null)
        {
            StartCoroutine(CardFinish());
            LeanTween.move(useCard.gameObject, new Vector3(960f, 500f, 0f), 0.2f);
            LeanTween.scale(useCard.gameObject, new Vector3(0.7f, 0.7f, 0.7f), 0.2f);
            //useCard.CardDestory();

        }

    }
    IEnumerator CardFinish()
    {
        yield return new WaitForSeconds(0.25f);
        useCard.CardDestory();
    }
    IEnumerator TurnUnitAnim()//动画函数
    {  
        if(turnUnit[0].player==false)//非玩家本体
        {
            if (useSkill.animType == AnimType.Attack)
            {
                turnUnit[0].anim.Play("attack");
            }
        }
        yield return new WaitForSeconds(0.1f);
        if(turnUnit[0].player == false)
             yield return new WaitUntil(()=>turnUnit[0].anim.GetCurrentAnimatorStateInfo(0).IsName("idle"));//等待动画播放完毕
        foreach (var o in pointUnit)
        {
            o.SkillSettle(turnUnit[0], useSkill);
        }
    }


    public void ShowBackMenu()
    {
        backPanel.SetActive(true);
    }
    public void HideBackMenu()
    {
        backPanel.SetActive(false);
    }



    //——————————————————————————小功能————————————————————————————————————
    public bool Probility(int b)//成功率
    {
        int a;
        a = Koubot.Tool.Random.RandomTool.GenerateRandomInt(0, 99);
        Debug.Log("roll的概率为"+ a);
        if (a < b)
            return true;
        else
            return false;
    }
    public void CheckAbandomCardNum()
    {
        if (abandomCardNum > 0&&abandomCardSwitch==false)
        {
            if (abandomCardNum > fightPlayerCards.haveCards.Count&&state==BattleState.TOACTION)
            {
                tips.text = "手牌不足";
                Back();
            }
            else if (abandomCardNum == fightPlayerCards.haveCards.Count && useCard != null && state == BattleState.TOACTION)
            {
                tips.text = "手牌不足";
                Back();
            }
            else if(abandomCardNum > fightPlayerCards.haveCards.Count)
                abandomCardNum=fightPlayerCards.haveCards.Count;
            else
                abandomCardSwitch = true;              
        }
        if (abandomCardNum == 0 && abandomCardSwitch == true)
        {
            tips.text = "";
            abandomCardSwitch = false;
        }
        if (abandomCardNum < 0)
            abandomCardNum = 0;

        if(abandomCardSwitch == true)
            tips.text="需要弃置"+abandomCardNum+"张牌";
    }
}
