using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public enum BattleState { START, PLAYERTURNSTART,PLAYERTURN, SKILL,CARD,POINTENEMY,POINTPLAYER,ACTION,ACTIONFINISH,ENEMYTURNSTART, ENEMYTURN,ENEMYFINISH,WIN, LOST,OVER }
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


    [Header("——————FIGHTING——————")]

    public int turn;
    public List<Unit> turnUnit;//当前回合技能发动方
    public Skill useSkill;//角色技能
    public List<Unit> pointUnit;//当前回合技能目标方
    public int pointNumber;//目标数量
    public void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
        SetHeros();//设置英雄
        pointNumber = 1;//默认值
        useSkill = null;//默认值
        turn = 0;
        StartCoroutine(PlayerTurnStart());

    }


    // Update is called once per frame
    void Update()
    {
        foreach (var o in playerUnit)
            o.HubUpdate();
        foreach (var o in enemyUnit)
            o.HubUpdate();


        UpdateTips();
        if(playerUnit.Count== 0 && state!=BattleState.OVER)
        {
            state = BattleState.LOST;
            StartCoroutine(Lost());
        }

        if (enemyUnit.Count == 0 && state != BattleState.OVER)
        {
            state = BattleState.WIN;
            StartCoroutine(Win());
        }

        HeroPointActive();
        if (state == BattleState.ACTION)//当进入ACTION时，执行函数(携程）
        {
            StartCoroutine(Action());
        }
                        
    }
    public void SetHeros()//在对应位置设置战斗队伍预置体以及状态栏
    {
        for (int i = 0; i < fightPrefs.fightHeros.Length; i++)
        {
            playerPrefab[i]=Instantiate(fightPrefs.fightHeros[i],playerStations[i].position,playerStations[i].rotation);
            playerUnit.Add(playerPrefab[i].GetComponent<Unit>());//添加unit进列表
            playerUnit[i].hub = Hub[i];
            playerUnit[i].HubUpdate();
            Hub[i].gameObject.SetActive(true);//显示对应角色状态栏          
        }

        for (int j = 0; j < enemyPrefs.enemyHeros.Length; j++)
        {
            enemyPrefab[j] = Instantiate(enemyPrefs.enemyHeros[j], enemyStations[j].position, enemyStations[j].rotation);
            enemyUnit.Add(enemyPrefab[j].GetComponent<Unit>());
            enemyUnit[j].hub = enemyHub[j];
            enemyUnit[j].HubUpdate();
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
    public void HeroPointActive()//显示选择玩家的标志,由此函数判断是否进入下一阶段.
    {
        
        if (state != BattleState.POINTENEMY || state != BattleState.POINTPLAYER)//非这两个阶段，灭图标
        {
            foreach (var o in enemyUnit)
                o.transform.GetChild(0).gameObject.SetActive(false);
            foreach (var o in playerUnit)
                o.transform.GetChild(0).gameObject.SetActive(false);
        }


        if (state==BattleState.POINTENEMY && pointNumber!=0)//指向敌人阶段，指向数不为0
        {
            
            foreach(var o in enemyUnit)//亮灯
            {
                o.transform.GetChild(0).gameObject.SetActive(true);
                if (pointUnit.Contains(o.GetComponent<Unit>())&&!useSkill.reChoose)//检测该物体是否在目标列表内且技能不支持重复选
                {
                    o.transform.GetChild(0).gameObject.SetActive(false);
                }
                if (pointUnit.Count == pointNumber)//接收目标列表数等于指定个数时,进入ACTION
                {
                    state = BattleState.ACTION;
                }
            }           
        }

  

    }



    //回合各阶段函数
    IEnumerator PlayerTurnStart()
    {
        tips.text = "你的回合...";
        turn = turn + 1;
        turnNum.text = turn.ToString();
        state = BattleState.PLAYERTURNSTART;     
        //结算状态
        yield return new WaitForSeconds(1f);
        if (state != BattleState.OVER)
        {
            state = BattleState.PLAYERTURN;
            tips.text = "选择行动...";
        }
        yield return new WaitForSeconds(1f);
        tips.text = "";
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
        yield return new WaitForSeconds(1f);
        foreach (var o in pointUnit)
        {
            o.skillSettle(turnUnit[0], useSkill);
        }
        GameReset();
        yield return new WaitForSeconds(1.5f);
        if (state != BattleState.OVER)
        {
            StartCoroutine(ActionFinish());
        }
        
             
    }
    IEnumerator ActionFinish()
    {     
        yield return new WaitForSeconds(0.5f);
        foreach (var o in playerUnit)
        {
            if (o.tired > 0)
            {
                o.tired = o.tired - 1;
                tips.text = o.unitName + " 减少1点疲劳";
            }
            yield return new WaitForSeconds(0.5f);
        }
        if (state != BattleState.OVER)
        {
            tips.text = "己方回合结束";
            yield return new WaitForSeconds(1f);
            StartCoroutine(EnemyTurnStart());
        }
        
    }
    IEnumerator EnemyTurnStart()
    {
        state = BattleState.ENEMYTURNSTART;
        tips.text = "敌方回合";
        //结算状态
        if (state != BattleState.OVER)
        {
            yield return new WaitForSeconds(1f);            
            StartCoroutine(EnemyTurn());
        }
        
    }

    IEnumerator EnemyTurn()
    {
        state = BattleState.ENEMYTURN;
        tips.text = "等待敌方行动...";
        yield return new WaitForSeconds(0.5f);
        EnemyAI();
        yield return new WaitForSeconds(1.5f);
        foreach (var o in pointUnit)
        {
            o.skillSettle(turnUnit[0], useSkill);
        }
        GameReset();
        if (state != BattleState.OVER)
        {
            StartCoroutine(EnemyFinish());
        }
              
    }
    IEnumerator EnemyFinish()
    {
       
        foreach (var o in enemyUnit)
        {
            if (o.tired > 0)
            {
                o.tired = o.tired - 1;
                tips.text = o.unitName + " 减少1点疲劳";
            }
            yield return new WaitForSeconds(0.5f);
        }
        if (state != BattleState.OVER)
        {
            tips.text = "敌方回合结束";
            yield return new WaitForSeconds(1f);
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



    private void EnemyAI()
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

        if (tempEnemy.Count == 0)//回合结束
        {
            tempEnemy.Clear();
            StartCoroutine(EnemyFinish());
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
                int player = r.Next(playerUnit.Count);
                if (!pointUnit.Contains(playerUnit[player]))
                    pointUnit.Add(playerUnit[player]);
            }
        }
        TipsSkillPoint();
    }
    public void TipsSkillPoint()//文本函数
    {
        if(pointUnit.Count==1&&pointUnit[0]==turnUnit[0])//对自己使用
        {
            tips.text= turnUnit[0].unitName + " 对自己使用了 " + useSkill.skillName;
        }
        else
        {
            string tempText=" ";
            foreach(var o in pointUnit)
            {
                tempText=tempText+o.unitName+" ";
            }
            tips.text = turnUnit[0].unitName + " 对 "+tempText+" 使用了 " + useSkill.skillName;
        }
    }
}
