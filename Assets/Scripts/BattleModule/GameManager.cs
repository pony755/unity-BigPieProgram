using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Koubot.Tool;

public enum BattleState { NONE,START, PLAYERTURNSTART,PLAYERTURN, POINTALL,SKILL,CARDTURNUNIT,POINTENEMY,POINTPLAYER,ACTION,ACTIONFINISH,ENEMYTURNSTART, ENEMYTURN,ENEMYFINISH,WIN, LOST,OVER }
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [HideInInspector]public bool win = false;//胜利条件布尔值

    [Header("玩家脚本体")]
    public FightPlayer player;

    [Header("技能画布设置")]
    public GameObject turnTipsObject;
    public Text turnNum;//回合数
    public Text tips;//提示框
    public GameObject backPanel;//返回画布
    public GameObject WinOrLost;//胜负画布
    public GameObject CardCanvas;//卡牌画布
    public GameObject AbandomCardCheck;//弃牌查看
    public GameObject skillImg;//技能栏
    public GameObject skillText;//技能介绍
    public GameObject addCancleBtn;
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
    
    public BattleState state;

    [Header("Heros")]
    public GameObject[] playerPrefab;//接收战斗列表角色
    public List<Unit> playerUnit;//获取战斗列表角色Unit脚本

    [Header("enemyHeros")]
    public GameObject[] enemyPrefab;//接收敌人列表角色
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
        if (playerUnit.Count== 0 && state!=BattleState.OVER)
        {
            state = BattleState.LOST;            
            StartCoroutine(Lost());
            GameReset();
        }

        if (enemyUnit.Count == 0 && state != BattleState.OVER)
        {
            state = BattleState.WIN;           
            StartCoroutine(Win());
            GameReset();
        }

        if((state == BattleState.POINTPLAYER|| state == BattleState.POINTENEMY|| state == BattleState.POINTALL)&&pointNumber==pointUnit.Count)
            StartCoroutine(ToAction());
        if (state == BattleState.ACTION)//当进入ACTION时，执行函数(携程）  
            StartCoroutine(Action());

                        
    }
    IEnumerator ToAction()
    {
        state = BattleState.ACTION;
        yield return null;
    }
    public IEnumerator SetHeros()//在对应位置设置战斗队伍预置体以及状态栏
    {
        for (int i = 0; i < fightPrefs.fightHeros.Length; i++)
        {
            playerPrefab[i]=Instantiate(fightPrefs.fightHeros[i],playerStations[i].position,playerStations[i].rotation);
            playerPrefab[i].GetComponent<SpriteRenderer>().sortingOrder = i;
            playerUnit.Add(playerPrefab[i].GetComponent<Unit>());//添加unit进列表
            //读取数据
            Hub[i].SetHub(playerUnit[i]);
            Hub[i].gameObject.SetActive(true);//显示对应角色状态栏
            LeanTween.move(Hub[i].gameObject, new Vector3(Hub[i].gameObject.transform.position.x+350f, Hub[i].gameObject.transform.position.y, Hub[i].gameObject.transform.position.z), 0.8f);
        }
        for (int j = 0; j < enemyPrefs.enemyHeros.Length; j++)
        {
            enemyPrefab[j] = Instantiate(enemyPrefs.enemyHeros[j], enemyStations[j].position, enemyStations[j].rotation);
            enemyPrefab[j].GetComponent<SpriteRenderer>().sortingOrder = j;
            //读取数据
            enemyUnit.Add(enemyPrefab[j].GetComponent<Unit>());
            enemyHub[j].SetHub(enemyUnit[j]);
            enemyHub[j].gameObject.SetActive(true);//显示对应角色状态栏
            LeanTween.move(enemyHub[j].gameObject, new Vector3(enemyHub[j].gameObject.transform.position.x -350f, enemyHub[j].gameObject.transform.position.y, enemyHub[j].gameObject.transform.position.z), 0.8f);
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
        foreach(var card in player.haveCards)
            card.BackCard();
        tips.text = "";
        pointNumber = 1;//默认值
        useSkill=null;//默认值
        useCard=null;
        skillImg.SetActive(false);
        CardCanvas.SetActive(true);
        turnUnit.Clear();
        pointUnit.Clear();
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
        
        if (GameManager.instance.state == BattleState.CARDTURNUNIT||state == BattleState.PLAYERTURN || state == BattleState.SKILL || state == BattleState.POINTENEMY || state == BattleState.POINTPLAYER)
        {
            
            GameReset();
            state = BattleState.PLAYERTURN;
            foreach(var o in playerUnit)
                o.anim.Play("idle");
        }
        CardCanvas.SetActive(true);    
    }

    IEnumerator DelayedPlayerSettle()
    {
        int tempDelayedCount = delayedTurn.Count;
        for (int j = 0; j < tempDelayedCount; j++)
        {
            int temp=0;
            if (delayedTurn[temp] == turn && delayedTurnUnit[temp].playerHero)
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
            else
            {
                temp ++;
            }
        }
    }
    IEnumerator DelayedEnemySettle()
    {
        int tempDelayedCount = delayedTurn.Count;
        for (int j = 0; j < tempDelayedCount; j++)
        {
            int temp = 0;
            if (delayedTurn[temp] == turn && !delayedTurnUnit[temp].playerHero)
            {
                if(delayedTurnUnit[temp].currentHP > 0)
                {
                    tips.text = delayedTurnUnit[temp].unitName + " 结算 " + delayedSkill[temp].skillName;
                    delayedSwitch = true;
                    delayedPointUnit[temp].SkillSettle(delayedTurnUnit[temp], delayedSkill[temp]);
                    delayedSwitch = false;
                }
                else
                    tips.text = delayedTurnUnit[temp].unitName + " 已死亡, " + delayedSkill[temp].skillName+" 结算失败 ";


                delayedTurn.Remove(delayedTurn[temp]);
                delayedTurnUnit.Remove(delayedTurnUnit[temp]);
                delayedSkill.Remove(delayedSkill[temp]);
                delayedPointUnit.Remove(delayedPointUnit[temp]);
                yield return new WaitForSeconds(0.3f);

            }
            else
            {
                temp ++;
            }
        }

    }



    //————————————————————————阶段—————————————————————————
    //回合各阶段函数
    IEnumerator PlayerTurnStart()
    {
        tips.text = "你的回合...";       
        turnNum.text = turn.ToString();
        state = BattleState.PLAYERTURNSTART;      
        //结算状态
        for (int i = 0; i < playerUnit.Count; i++)
            playerUnit[i].PassiveTurnStart();
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(DelayedPlayerSettle());        
        yield return new WaitForSeconds(0.5f);
        tips.text = "";
        yield return new WaitForSeconds(0.5f);
        if (state != BattleState.OVER|| state != BattleState.WIN|| state != BattleState.LOST)
        {
            state = BattleState.PLAYERTURN; 
        }
        
    }
    //使用技能的text提示在SkillBtn里
    IEnumerator Action()//行动阶段函数
    {
        state = BattleState.ACTIONFINISH;//及时切换state,防止多次运行此函数     
        BtnHide();
        skillImg.SetActive(false);
        CardCanvas.SetActive(true);
        //重置动画
        foreach(var o in playerUnit)
        {
            o.anim.Play("idle");
        }
        foreach (var o in enemyUnit)
        {
            o.anim.Play("idle");
        }
        TipsSkillPoint();
        yield return new WaitForSeconds(0.5f);
        if (useSkill != null)
        {
            if (Koubot.Tool.Random.RandomTool.GenerateRandomInt(0, 100) < useSkill.precent)
            {

                tips.text = "欧不！ " + useSkill.skillName + " 发动失败";
                yield return new WaitForSeconds(0.5f);
            }
            else
            {
                TurnUnitAnim();
            }
        }
        yield return new WaitForSeconds(1f);
        if (state != BattleState.OVER || state != BattleState.WIN || state != BattleState.LOST)
        {               
                if (state == BattleState.ACTIONFINISH)
                    StartCoroutine(ActionFinish());
        }            
    }

    public IEnumerator ActionFinish()
    {
        yield return new WaitForSeconds(1f);
        tips.text = "己方回合结束";
        for (int i = 0; i < playerUnit.Count; i++)
            playerUnit[i].PassiveTurnEnd();
        yield return new WaitForSeconds(1f);        
        if (state != BattleState.OVER || state != BattleState.WIN || state != BattleState.LOST)
        {           
            foreach (var o in playerUnit)
            {
                if (o.tired > 0&&!turnUnit.Contains(o))
                {
                    o.tired --;
                    tips.text = o.unitName + " 减少1点疲劳";
                    yield return new WaitForSeconds(0.3f);
                }               
            }
            GameReset();
            yield return new WaitForSeconds(1f);
        StartCoroutine(EnemyTurnStart());
        }
        
    }
    IEnumerator EnemyTurnStart()
    {       
        state = BattleState.ENEMYTURNSTART;
        tips.text = "敌方回合";
        //结算状态
        for (int i = 0; i < enemyUnit.Count; i++)
            enemyUnit[i].PassiveTurnStart();
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(DelayedEnemySettle());
        yield return new WaitForSeconds(1f);
        if (state != BattleState.OVER || state != BattleState.WIN || state != BattleState.LOST)
        {
            StartCoroutine(EnemyTurn());
        }
        
    }

    IEnumerator EnemyTurn()
    {
        state = BattleState.ENEMYTURN;
        tips.text = "等待敌方行动...";
        StartCoroutine( EnemyAI());
        yield return new WaitForSeconds(2f);
        TipsSkillPoint();
        yield return new WaitForSeconds(0.5f);
        if (useSkill != null)
        {
            if (Koubot.Tool.Random.RandomTool.GenerateRandomInt(0, 100) < useSkill.precent)
            {
                tips.text = "欧不！ " + useSkill.skillName + " 发动失败";
                yield return new WaitForSeconds(1f);
            }

            else
            {
                TurnUnitAnim();
            }
        }
        yield return new WaitForSeconds(1f);   
            if (state != BattleState.OVER || state != BattleState.WIN || state != BattleState.LOST)
            {
                StartCoroutine(EnemyFinish());
            }

        

    }
    IEnumerator EnemyFinish()
    {
        tips.text = "敌方回合结束";
        for (int i = 0; i < enemyUnit.Count; i++)
            enemyUnit[i].PassiveTurnEnd();
        yield return new WaitForSeconds(1f);
        if (state != BattleState.OVER || state != BattleState.WIN || state != BattleState.LOST)
        {
            foreach (var o in enemyUnit)
            {
                if (o.tired > 0 && !turnUnit.Contains(o))
                {
                    o.tired --;
                    tips.text = o.unitName + " 减少1点疲劳";
                    yield return new WaitForSeconds(0.3f);
                }
            }
            GameReset();
            yield return new WaitForSeconds(1f);
            turn ++;
            StartCoroutine(PlayerTurnStart());
        }
        
    }

    IEnumerator Win()
    {
        state= BattleState.OVER;
        yield return new WaitForSeconds(1f);
        win = true;
        Time.timeScale = 1;
        WinOrLost.SetActive(true);
        Debug.Log("《《《《你赢了》》》》");
    }
    IEnumerator Lost()
    {
        state = BattleState.OVER;
        yield return new WaitForSeconds(1f);
        Time.timeScale = 1;
        WinOrLost.SetActive(true);
        Debug.Log("《《《《你输了》》》》");
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
                        int player = Koubot.Tool.Random.RandomTool.GenerateRandomInt(0, playerUnit.Count - 1);
                        if (!pointUnit.Contains(playerUnit[player]))
                            pointUnit.Add(playerUnit[player]);
                    }
                    else
                    {
                        yield return new WaitForSeconds(0.1f);
                        pointUnit.Add(playerUnit[Koubot.Tool.Random.RandomTool.GenerateRandomInt(0, playerUnit.Count - 1)]);
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
        if (state == BattleState.WIN || state == BattleState.LOST|| state == BattleState.OVER)
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

    public void TurnUnitAnim()//动画函数
    {
        if (useCard != null)
        {
            useCard.gameObject.GetComponent<Animator>().enabled=true;
            useCard.gameObject.GetComponent<Animator>().Play("cardUse");
        }
            
        if(turnUnit[0].player==false)
        {
            if (useSkill.animType == AnimType.Attack)
            {
                turnUnit[0].anim.Play("attack");
            }
        }
        StartCoroutine(SettleSkill());
    }

    IEnumerator SettleSkill()
    {
        yield return new WaitForSeconds(0.3f);
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


}
