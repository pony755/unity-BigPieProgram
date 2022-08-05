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
    [HideInInspector]public bool win = false;//ʤ����������ֵ

    [Header("��ҽű���")]
    public FightPlayer player;

    [Header("���ܻ�������")]
    public GameObject turnTipsObject;
    public Text turnNum;//�غ���
    public Text tips;//��ʾ��
    public GameObject backPanel;//���ػ���
    public GameObject WinOrLost;//ʤ������
    public GameObject CardCanvas;//���ƻ���
    public GameObject AbandomCardCheck;//���Ʋ鿴
    public GameObject skillImg;//������
    public GameObject skillText;//���ܽ���
    public GameObject addCancleBtn;
    public List<GameObject> skillBtns;//���ܰ�ť   


    [Header("����UI����")]
    public BattleHub[] Hub;//״̬��
    public Transform[] playerStations;//���ý�ɫ����

    [Header("�з�UI����")]
    public BattleHub[] enemyHub;//״̬��
    public Transform[] enemyStations;//���ý�ɫ����

    [Header("ս���浵����")]
    public FightPrefs fightPrefs;//ս���浵
    public EnemyPrefs enemyPrefs;//�����б�


    [Header("������������CHECKING������������")]
    
    public BattleState state;

    [Header("Heros")]
    public GameObject[] playerPrefab;//����ս���б��ɫ
    public List<Unit> playerUnit;//��ȡս���б��ɫUnit�ű�

    [Header("enemyHeros")]
    public GameObject[] enemyPrefab;//���յ����б��ɫ
    public List<Unit> enemyUnit;//��ȡ�����б��ɫUnit�ű�
    // Start is called before the first frame update
    [Header("��������������ʱ���㡪����������")]
    public bool delayedSwitch;
    public List<int> delayedTurn;//�ӳٵĻغ���
    public List<Unit> delayedTurnUnit;//�ӳٻغϼ��ܷ�����
    public List<Skill> delayedSkill;//�ӳٻغϵļ���
    public List<Unit> delayedPointUnit;//�ӳٻغϼ���Ŀ�귽
    [Header("������������FIGHTING������������")]

    public int turn;
    public List<Unit> turnUnit;//��ǰ�غϼ��ܷ�����
    public Skill useSkill;//��ɫ����
    public Cards useCard;//��ɫ����
    public int pointNumber;//Ŀ������
    public List<Unit> pointUnit;//��ǰ�غϼ���Ŀ�귽
    
    public void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
        //��fightprefs��setHero

        LeanTween.move(turnTipsObject, new Vector3(turnTipsObject.transform.position.x, turnTipsObject.transform.position.y-200f, turnTipsObject.transform.position.z), 0.8f);
        win = false;
        delayedSwitch = false;
        state = BattleState.PLAYERTURNSTART;
        pointNumber = 1;//Ĭ��ֵ
        useSkill = null;//Ĭ��ֵ       
        turn = 1;
        
    }
    private void Start()
    {
        StartCoroutine(Load());       

    }

    
    IEnumerator Load()
    {
        Debug.Log("������Ϸ");
        yield return new WaitForSeconds(1f);//��ּ���
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
        if (state == BattleState.ACTION)//������ACTIONʱ��ִ�к���(Я�̣�  
            StartCoroutine(Action());

                        
    }
    IEnumerator ToAction()
    {
        state = BattleState.ACTION;
        yield return null;
    }
    public IEnumerator SetHeros()//�ڶ�Ӧλ������ս������Ԥ�����Լ�״̬��
    {
        for (int i = 0; i < fightPrefs.fightHeros.Length; i++)
        {
            playerPrefab[i]=Instantiate(fightPrefs.fightHeros[i],playerStations[i].position,playerStations[i].rotation);
            playerPrefab[i].GetComponent<SpriteRenderer>().sortingOrder = i;
            playerUnit.Add(playerPrefab[i].GetComponent<Unit>());//���unit���б�
            //��ȡ����
            Hub[i].SetHub(playerUnit[i]);
            Hub[i].gameObject.SetActive(true);//��ʾ��Ӧ��ɫ״̬��
            LeanTween.move(Hub[i].gameObject, new Vector3(Hub[i].gameObject.transform.position.x+350f, Hub[i].gameObject.transform.position.y, Hub[i].gameObject.transform.position.z), 0.8f);
        }
        for (int j = 0; j < enemyPrefs.enemyHeros.Length; j++)
        {
            enemyPrefab[j] = Instantiate(enemyPrefs.enemyHeros[j], enemyStations[j].position, enemyStations[j].rotation);
            enemyPrefab[j].GetComponent<SpriteRenderer>().sortingOrder = j;
            //��ȡ����
            enemyUnit.Add(enemyPrefab[j].GetComponent<Unit>());
            enemyHub[j].SetHub(enemyUnit[j]);
            enemyHub[j].gameObject.SetActive(true);//��ʾ��Ӧ��ɫ״̬��
            LeanTween.move(enemyHub[j].gameObject, new Vector3(enemyHub[j].gameObject.transform.position.x -350f, enemyHub[j].gameObject.transform.position.y, enemyHub[j].gameObject.transform.position.z), 0.8f);
        }
        yield return null;
    }


    



    //������������������������������������������������UI����������������������������������������������������
    public void SkillShow(Unit unit)//��ʾ������(codeΪ��ǰ���Ͻ�ɫ��ţ�
    {
        skillImg.SetActive(true);
        turnUnit.Add(unit);
        state= BattleState.SKILL;//�л��غ�״̬
        BtnSet(unit);
       
    }
    public void BtnSet(Unit unit)
    {

        for (int i = 0; i < unit.heroSkillList.Count; i++)//���ð�ť
        {
            skillBtns[i].GetComponent<SkillBtn>().skillInfo = unit.heroSkillList[i];
            skillBtns[i].SetActive(true);
            if (unit.heroSkillList[i].passiveType!=PassiveType.None)
            {
                skillBtns[i].GetComponent<Button>().interactable = false;
            }
        }
    }
    public void GameReset()//����
    {
        BtnHide();
        foreach(var card in player.haveCards)
            card.BackCard();
        tips.text = "";
        pointNumber = 1;//Ĭ��ֵ
        useSkill=null;//Ĭ��ֵ
        useCard=null;
        skillImg.SetActive(false);
        CardCanvas.SetActive(true);
        turnUnit.Clear();
        pointUnit.Clear();
    }

    public void BtnHide()
    {
        for (int i = 0; i < skillBtns.Count; i++)//���ذ�ť
        {
            skillBtns[i].SetActive(false);
            skillBtns[i].GetComponent<Button>().interactable = true;
            skillBtns[i].GetComponent<SkillBtn>().skillInfo = null;
        }
    }
    public void Back()//������һغ�
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
                    tips.text = delayedTurnUnit[temp].unitName + " ���� " + delayedSkill[temp].skillName;
                    delayedSwitch = true;
                    delayedPointUnit[temp].SkillSettle(delayedTurnUnit[temp], delayedSkill[temp]);
                    delayedSwitch = false;
                }
                else
                    tips.text = delayedTurnUnit[temp].unitName + " ������, " + delayedSkill[temp].skillName + " ����ʧ�� ";
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
                    tips.text = delayedTurnUnit[temp].unitName + " ���� " + delayedSkill[temp].skillName;
                    delayedSwitch = true;
                    delayedPointUnit[temp].SkillSettle(delayedTurnUnit[temp], delayedSkill[temp]);
                    delayedSwitch = false;
                }
                else
                    tips.text = delayedTurnUnit[temp].unitName + " ������, " + delayedSkill[temp].skillName+" ����ʧ�� ";


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



    //�������������������������������������������������׶Ρ�������������������������������������������������
    //�غϸ��׶κ���
    IEnumerator PlayerTurnStart()
    {
        tips.text = "��Ļغ�...";       
        turnNum.text = turn.ToString();
        state = BattleState.PLAYERTURNSTART;      
        //����״̬
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
    //ʹ�ü��ܵ�text��ʾ��SkillBtn��
    IEnumerator Action()//�ж��׶κ���
    {
        state = BattleState.ACTIONFINISH;//��ʱ�л�state,��ֹ������д˺���     
        BtnHide();
        skillImg.SetActive(false);
        CardCanvas.SetActive(true);
        //���ö���
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

                tips.text = "ŷ���� " + useSkill.skillName + " ����ʧ��";
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
        tips.text = "�����غϽ���";
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
                    tips.text = o.unitName + " ����1��ƣ��";
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
        tips.text = "�з��غ�";
        //����״̬
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
        tips.text = "�ȴ��з��ж�...";
        StartCoroutine( EnemyAI());
        yield return new WaitForSeconds(2f);
        TipsSkillPoint();
        yield return new WaitForSeconds(0.5f);
        if (useSkill != null)
        {
            if (Koubot.Tool.Random.RandomTool.GenerateRandomInt(0, 100) < useSkill.precent)
            {
                tips.text = "ŷ���� " + useSkill.skillName + " ����ʧ��";
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
        tips.text = "�з��غϽ���";
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
                    tips.text = o.unitName + " ����1��ƣ��";
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
        Debug.Log("����������Ӯ�ˡ�������");
    }
    IEnumerator Lost()
    {
        state = BattleState.OVER;
        yield return new WaitForSeconds(1f);
        Time.timeScale = 1;
        WinOrLost.SetActive(true);
        Debug.Log("�������������ˡ�������");
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
            GameManager.instance.tips.text = "ѡ�� " + GameManager.instance.useSkill.skillName + " ��Ŀ��("+GameManager.instance.pointUnit.Count+"/"+ GameManager.instance.pointNumber+")";       
            
    }
    IEnumerator EnemyAI()
    {
        List<Unit> tempEnemy = new List<Unit>();
        foreach (var o in enemyUnit)
        {
            if (o.tired == 0&&o.currentHP>0)//��ȡ0ƣ�͵ĵ���
            {
                foreach (var t in o.heroSkillList)
                {
                    if (o.currentMP >= t.needMP)//��ȡ�м��ܿ��õĵ���
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
            turnUnit.Add(tempEnemy[Koubot.Tool.Random.RandomTool.GenerateRandomInt(0, tempEnemy.Count - 1)]);//������һ���з�
            tempEnemy.Clear();

            List<Skill> tempSkill = new List<Skill>();
            foreach (var t in turnUnit[0].heroSkillList)
            {
                if (turnUnit[0].currentMP >= t.needMP&&t.passiveType==PassiveType.None)
                    tempSkill.Add(t);
            }
            useSkill = tempSkill[Koubot.Tool.Random.RandomTool.GenerateRandomInt(0, tempSkill.Count - 1)];
            tempSkill.Clear();
            pointNumber = useSkill.pointNum;//��Ӽ���Ŀ������
            useSkill.EnemyUse();
            yield return new WaitForSeconds(1f);
            if(useSkill.point==SkillPoint.Enemies)
            {
                while (pointNumber > pointUnit.Count)//��������ΪĿ��
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
                while (pointNumber > pointUnit.Count)//��������ΪĿ��
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
    public void TipsSkillPoint()//�ı�����
    {
        List<Unit> tempList = new List<Unit>(); 
        if (state == BattleState.WIN || state == BattleState.LOST|| state == BattleState.OVER)
            return;
        if (turnUnit.Count == 0)//Ϊ��ʱ�������ǿ���,�����ǵ����޷�ʹ�ü���
        {
            if(useSkill==null)
            {
                tips.text = "�з��޷��ж�";
                return;
            }
        }
            
        if(pointUnit.Count==1&&pointUnit[0]==turnUnit[0])//���Լ�ʹ��
        {
            tips.text= turnUnit[0].unitName + " ���Լ�ʹ���� " + useSkill.skillName;
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
            tips.text = turnUnit[0].unitName + " �� " + tempText + " ʹ���� " + useSkill.skillName;
        }
        tempList.Clear();
    }

    public void TurnUnitAnim()//��������
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
