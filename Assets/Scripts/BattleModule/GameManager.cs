using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public enum BattleState { NONE,START, PLAYERTURNSTART,PLAYERTURN, POINTALL,SKILL,CARD,POINTENEMY,POINTPLAYER,ACTION,ACTIONFINISH,ENEMYTURNSTART, ENEMYTURN,ENEMYFINISH,WIN, LOST,OVER }
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [Header("技能画布设置")]
    public Text turnNum;//回合数
    public Text tips;//
    public GameObject WinOrLost;//胜负画布
    public GameObject skillImg;//技能画布
    public GameObject backBtn;
    public GameObject[] skillBtns;//技能按钮   
    public Text[] skilText;//技能按钮文本

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

    [Header("Btn")]
    public List<SkillBtn> skillBtnInfo;//技能按钮脚本
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
    public int pointNumber;//目标数量
    public List<Unit> pointUnit;//当前回合技能目标方
    
    public void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
        SetHeros();//设置英雄
        delayedSwitch = false;
        state = BattleState.PLAYERTURNSTART;
        pointNumber = 1;//默认值
        useSkill = null;//默认值       
        turn = 1;
        
    }
    private void Start()
    {

        StartCoroutine(load());

    }
    IEnumerator load()
    {
        Debug.Log("加载游戏");
        yield return new WaitForSeconds(1f);//充分加载
        foreach (var player in playerUnit)
            player.PassiveGameBegin();
        foreach (var player in enemyUnit)
            player.PassiveGameBegin();
        yield return new WaitForSeconds(1f);//充分结算
        StartCoroutine(PlayerTurnStart());
    }

    // Update is called once per frame
    void Update()
    {

        
        UpdateTips();


        
        if (playerUnit.Count== 0 && state!=BattleState.OVER)
        {
            state = BattleState.LOST;
            GameReset();
            StartCoroutine(Lost());
        }

        if (enemyUnit.Count == 0 && state != BattleState.OVER)
        {
            state = BattleState.WIN;
            GameReset();
            StartCoroutine(Win());
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
    public void SetHeros()//在对应位置设置战斗队伍预置体以及状态栏
    {
        for (int i = 0; i < fightPrefs.fightHeros.Length; i++)
        {
            playerPrefab[i]=Instantiate(fightPrefs.fightHeros[i],playerStations[i].position,playerStations[i].rotation);
            playerPrefab[i].GetComponent<SpriteRenderer>().sortingOrder = i;
            playerUnit.Add(playerPrefab[i].GetComponent<Unit>());//添加unit进列表
            Hub[i].SetHub(playerUnit[i]);
            Hub[i].gameObject.SetActive(true);//显示对应角色状态栏          
        }

        for (int j = 0; j < enemyPrefs.enemyHeros.Length; j++)
        {
            enemyPrefab[j] = Instantiate(enemyPrefs.enemyHeros[j], enemyStations[j].position, enemyStations[j].rotation);
            enemyPrefab[j].GetComponent<SpriteRenderer>().sortingOrder = j;
            enemyUnit.Add(enemyPrefab[j].GetComponent<Unit>());
            enemyHub[j].SetHub(enemyUnit[j]);
            enemyHub[j].gameObject.SetActive(true);//显示对应角色状态栏          
        }
    }






    //————————————————————————UI——————————————————————————
    public void SkillShow(Unit unit)//显示技能栏(code为当前场上角色编号）
    {
        tips.text = "选择技能...";
        backBtn.SetActive(true);
        skillImg.SetActive(true);
        turnUnit.Add(unit);
        state= BattleState.SKILL;//切换回合状态
        BtnSet(unit);
       
    }
    public void BtnSet(Unit unit)
    {

        for (int i = 0; i < unit.heroSkillList.Count; i++)//设置按钮
        {
            skillBtns[i].SetActive(true);
            skillBtnInfo.Add(skillBtns[i].GetComponent<SkillBtn>());//获取脚本，进行操作
            skillBtnInfo[i].skillInfo = unit.heroSkillList[i];//按钮获取技能脚本
            skillBtnInfo[i].skillText.text =unit.heroSkillList[i].skillName;
            if(unit.heroSkillList[i].passiveType!=passiveType.None)
            {
                skillBtns[i].GetComponent<Button>().interactable = false;
            }
        }
    }
    public void GameReset()//重置
    {
        BtnHide();
        tips.text = "";
        backBtn.SetActive(false);
        pointNumber = 1;//默认值
        useSkill=null;//默认值
        skillImg.SetActive(false);
        skillBtnInfo.Clear();
        turnUnit.Clear();
        pointUnit.Clear();
    }

    public void BtnHide()
    {
        backBtn.SetActive(false);
        for (int i = 0; i < skillBtnInfo.Count; i++)//隐藏按钮
        {
            skillBtns[i].SetActive(false);
            skillBtns[i].GetComponent<Button>().interactable = true;
            skillBtnInfo[i].skillInfo = null;//清空按钮的skill
        }
    }
    public void Back()//返回玩家回合
    {
        
        if (state==BattleState.PLAYERTURN || state == BattleState.SKILL || state == BattleState.POINTENEMY || state == BattleState.POINTPLAYER)
        {
            
            GameReset();
            state = BattleState.PLAYERTURN;
        }     
    }

    IEnumerator delayedPlayerSettle()
    {
        int tempDelayedCount = delayedTurn.Count;
        for (int j = 0; j < tempDelayedCount; j++)
        {
            int temp = 0;
            if (delayedTurn[temp] == turn && delayedTurnUnit[temp].playerHero)
            {
                tips.text = delayedTurnUnit[temp].unitName + " 结算 " + delayedSkill[temp].skillName;
                delayedSwitch = true;
                delayedPointUnit[temp].skillSettle(delayedTurnUnit[temp], delayedSkill[temp]);
                delayedTurn.Remove(delayedTurn[temp]);
                delayedTurnUnit.Remove(delayedTurnUnit[temp]);
                delayedSkill.Remove(delayedSkill[temp]);
                delayedPointUnit.Remove(delayedPointUnit[temp]);
                delayedSwitch = false;
                yield return new WaitForSeconds(0.1f);
                
            }
            else
            {
                temp = temp + 1;
            }
        }
    }
    IEnumerator delayedEnemySettle()
    {
        int tempDelayedCount = delayedTurn.Count;
        for (int j = 0; j < tempDelayedCount; j++)
        {
            int temp = 0;
            if (delayedTurn[temp] == turn && !delayedTurnUnit[temp].playerHero)
            {
                tips.text = delayedTurnUnit[temp].unitName + " 结算 " + delayedSkill[temp].skillName;
                delayedSwitch = true;
                delayedPointUnit[temp].skillSettle(delayedTurnUnit[temp], delayedSkill[temp]);
                delayedTurn.Remove(delayedTurn[temp]);
                delayedTurnUnit.Remove(delayedTurnUnit[temp]);
                delayedSkill.Remove(delayedSkill[temp]);
                delayedPointUnit.Remove(delayedPointUnit[temp]);
                delayedSwitch = false;
                yield return new WaitForSeconds(0.1f);

            }
            else
            {
                temp = temp + 1;
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
        StartCoroutine(delayedPlayerSettle());        
        yield return new WaitForSeconds(0.5f);
        if (state != BattleState.OVER)
        {
            state = BattleState.PLAYERTURN;
            tips.text = "";
        }
    }
    //使用技能的text提示在SkillBtn里
    IEnumerator Action()//行动阶段函数
    {
        state = BattleState.ACTIONFINISH;//及时切换state,防止多次运行此函数     
        BtnHide();
        skillImg.SetActive(false);
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
        if(useSkill.myself)
        {
            GameManager.instance.pointNumber = GameManager.instance.pointNumber + 1;
            GameManager.instance.pointUnit.Add(turnUnit[0]);
        }
        yield return new WaitForSeconds(1.5f);
        System.Random r = new System.Random();
        if (useSkill != null)
        {
            if (r.Next(101) < useSkill.precent)
            {

                tips.text = "欧不！ " + useSkill.skillName + " 发动失败";
                yield return new WaitForSeconds(1f);
            }
            else
            {
                TurnUnitAnim();
                yield return new WaitForSeconds(0.3f);
                foreach (var o in pointUnit)
                {
                        o.skillSettle(turnUnit[0], useSkill);
                }

            }
        }       
        if (state != BattleState.OVER)
        {

                yield return new WaitForSeconds(1.5f);
                if (state == BattleState.ACTIONFINISH)
                    StartCoroutine(ActionFinish());
        }            
    }

    IEnumerator ActionFinish()
    {
        tips.text = "己方回合结束";
        for (int i = 0; i < playerUnit.Count; i++)
            playerUnit[i].PassiveTurnEnd();
        yield return new WaitForSeconds(1f);        
        if (state != BattleState.OVER)
        {           
            foreach (var o in playerUnit)
            {
                if (o.tired > 0&&!turnUnit.Contains(o))
                {
                    o.tired = o.tired - 1;
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
        StartCoroutine(delayedEnemySettle());
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(EnemyTurn());
    }

    IEnumerator EnemyTurn()
    {
        state = BattleState.ENEMYTURN;
        tips.text = "等待敌方行动...";
        StartCoroutine( EnemyAI());
        yield return new WaitForSeconds(1.5f);
        TipsSkillPoint();
        yield return new WaitForSeconds(1f);
        System.Random r = new System.Random();
        if (useSkill != null)
        {
            if (r.Next(101) < useSkill.precent)
            {
                tips.text = "欧不！ " + useSkill.skillName + " 发动失败";
                yield return new WaitForSeconds(1f);
            }

            else
            {
                TurnUnitAnim();
                yield return new WaitForSeconds(0.3f);
                foreach (var o in pointUnit)
                {
                    o.skillSettle(turnUnit[0], useSkill);
                }

            }
        }
        yield return new WaitForSeconds(1f);
  
           
            if (state != BattleState.OVER)
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
        if (state != BattleState.OVER)
        {
            foreach (var o in enemyUnit)
            {
                if (o.tired > 0 && !turnUnit.Contains(o))
                {
                    o.tired = o.tired - 1;
                    tips.text = o.unitName + " 减少1点疲劳";
                    yield return new WaitForSeconds(0.3f);
                }
            }
            GameReset();
            yield return new WaitForSeconds(1f);
            turn = turn + 1;
            StartCoroutine(PlayerTurnStart());
        }
        
    }

    IEnumerator Win()
    {
        state= BattleState.OVER;
        yield return new WaitForSeconds(1f);
        WinOrLost.SetActive(true);
        Debug.Log("《《《《你赢了》》》》");
    }
    IEnumerator Lost()
    {
        state = BattleState.OVER;
        yield return new WaitForSeconds(1f);
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
    }
    IEnumerator EnemyAI()
    {
        System.Random r = new System.Random();
        List<Unit> tempEnemy = new List<Unit>();
        foreach (var o in enemyUnit)
        {
            if (o.tired == 0)//提取0疲劳的敌人
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
            turnUnit.Add(tempEnemy[r.Next(tempEnemy.Count)]);//随机添加一个敌方
            tempEnemy.Clear();

            List<Skill> tempSkill = new List<Skill>();
            foreach (var t in turnUnit[0].heroSkillList)
            {
                if (turnUnit[0].currentMP >= t.needMP)
                    tempSkill.Add(t);
            }
            useSkill = tempSkill[r.Next(tempSkill.Count)];
            tempSkill.Clear();
            pointNumber = useSkill.pointNum;//添加技能目标数量
            useSkill.EnemyUse();

            while (pointNumber > pointUnit.Count)//添加玩家作为目标
            {
                if(!useSkill.reChoose)
                {
                    int player = r.Next(playerUnit.Count);
                    if (!pointUnit.Contains(playerUnit[player]))
                        pointUnit.Add(playerUnit[player]);
                }
                else
                {
                    yield return new WaitForSeconds(0.1f);
                    pointUnit.Add(playerUnit[r.Next(playerUnit.Count)]);
                }
                
            }
   
        }
    }
    public void TipsSkillPoint()//文本函数
    {
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
                tempText = tempText + o.unitName + " ";
            }
            tips.text = turnUnit[0].unitName + " 对 " + tempText + " 使用了 " + useSkill.skillName;
        }
    }

    public void TurnUnitAnim()//动画函数
    {
        if (useSkill.animType==animType.Attack)
        {
            turnUnit[0].anim.Play("attack");
        }
            

    }

    public void UsePassiveSkill()//使用被动函数
    {
        state = BattleState.SKILL;              
        StartCoroutine(useSkill.JudgePlayerSkill());

    }
}
