using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public enum BattleState { START, PLAYERTURNSTART,PLAYERTURN, SKILL,CARD,POINTENEMY,POINTPLAYER,ACTION,ACTIONFINISH,ENEMYTURNSTART, ENEMYTURN,ENEMYFINISH,WIN, LOST,OVER }
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [Header("技能画布设置")]
    public GameObject WinOrLost;
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
        state = BattleState.START;//设置状态：开始
        SetHeros();//设置英雄
        pointNumber = 1;//默认值
        useSkill = null;//默认值
        StartCoroutine(PlayerTurnStart());

    }


    // Update is called once per frame
    void Update()
    {
        foreach (var o in playerUnit)
            o.HubUpdate();
        foreach (var o in enemyUnit)
            o.HubUpdate();

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
        state = BattleState.PLAYERTURNSTART;
        Debug.Log("回合开始");
        //结算状态
        yield return new WaitForSeconds(1f);
        if (state != BattleState.OVER)
        {
            state = BattleState.PLAYERTURN;
            Debug.Log("玩家回合");
        }
        
    }
    IEnumerator Action()//行动阶段函数
    {
        state = BattleState.ACTIONFINISH;//及时切换state,防止多次运行此函数
        BtnHide();
        //重置动画
        foreach(var o in playerUnit)
        {
            o.anim.Play("idle");
        }
        foreach (var o in enemyUnit)
        {
            o.anim.Play("idle");
        }
        
        Debug.Log(turnUnit[0].unitName + "发动了" + useSkill.skillName);        
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
        Debug.Log("回合结束");
        yield return new WaitForSeconds(2f);
        foreach (var o in playerUnit)
        {
            if (o.tired > 0)
            o.tired = o.tired - 1;
        }
        if (state != BattleState.OVER)
        {
            yield return new WaitForSeconds(1f);
            StartCoroutine(EnemyTurnStart());
        }
        
    }
    IEnumerator EnemyTurnStart()
    {
        state = BattleState.ENEMYTURNSTART;
        Debug.Log("敌方回合开始");
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
        System.Random r=new System.Random();
        turnUnit.Add(enemyUnit[r.Next(enemyUnit.Count)]);//随机添加一个敌方
        useSkill = turnUnit[0].heroSkillList[r.Next(turnUnit[0].heroSkillList.Count)];//随机添加一个技能
        pointNumber = useSkill.pointNum;//添加技能目标数量
        useSkill.EnemyUse();
        while(pointNumber>pointUnit.Count)//添加玩家作为目标
        {
            int player = r.Next(playerUnit.Count);
            if (!pointUnit.Contains(playerUnit[player]))
               pointUnit.Add(playerUnit[player]);
        }
        yield return new WaitForSeconds(1f);
        Debug.Log(turnUnit[0].unitName + "发动了" + useSkill.skillName);
        yield return new WaitForSeconds(1f);
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
        
        Debug.Log("敌方回合结束");
        foreach (var o in enemyUnit)
        {
            if (o.tired > 0)
                o.tired = o.tired - 1;
        }
        if (state != BattleState.OVER)
        {
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

}
