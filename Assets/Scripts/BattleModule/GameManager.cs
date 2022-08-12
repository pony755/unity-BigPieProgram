using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Koubot.Tool;

public enum BattleState { NONE,START, PLAYERTURNSTART,PLAYERTURN, POINTALL,SKILL,CARDTURNUNIT,POINTENEMY,POINTPLAYER,POINTPREPAREHERO,ACTION,ACTIONFINISH,ABANDOMCARD,ENEMYTURNSTART, ENEMYTURN,ENEMYFINISH,OVER }


public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public bool over ;//ս��������־λ
    public bool win ;//ʤ��������־λ
    [HideInInspector] public bool AdjustCards = false;//��������λ�ñ�־λ

    [Header("ȫ����List")]   
    public GameObject allListObject;
    public GameObject tempPlayer;
    [Header("��������")]
    public FightPlayerCards fightPlayerCards;//��������
    public TipsDelayed tipsDelayed;//�ӳټ�����ʾ��
    public GameObject battleBackGround;//ս������
    public GameObject heroPrepare;//���С�ӵĵط�
    public GameObject turnTipsObject;//��ʾ��
    public Text turnNum;//�غ���
    public Text tips;//��ʾ��
    public GameObject backPanel;//���ػ���
    public GameObject WinOrLost;//ʤ������
    public GameObject CardCanvas;//���ƻ���
    public GameObject AbandomCardCheck;//���Ʋ鿴
    public GameObject exchange;//������
    public GameObject skillImg;//������
    public GameObject skillText;//���ܽ���
    public GameObject addCancleBtn;//���ư�ť
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
    public List<GameObject> heroPrefab;//����ս���б��ɫ
    public List<Unit> heroUnit;//��ȡս���б��ɫUnit�ű�
    public List<GameObject> heroPreparePrefab;//����С���б��ɫ

    [Header("enemyHeros")]
    public List<GameObject> enemyPrefab;//���յ����б��ɫ
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
        over = false;
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
        if ((heroUnit.Count== 0|| enemyUnit.Count == 0 )&& over==false)
        {
            over = true;           
            StartCoroutine(Over());
            GameReset();
        }


        if((state == BattleState.POINTPLAYER|| state == BattleState.POINTENEMY|| state == BattleState.POINTALL||state==BattleState.POINTPREPAREHERO) &&pointNumber==pointUnit.Count)
            StartCoroutine(ToAction());
        if (state == BattleState.ACTION)//������ACTIONʱ��ִ�к���(Я�̣�  
            StartCoroutine(Action());
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
    IEnumerator ToAction()
    {
        state = BattleState.ACTION;
        yield return null;
    }
    public IEnumerator SetHeros()//�ڶ�Ӧλ������ս������Ԥ�����Լ�״̬��
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
        heroUnit.Add(heroPrefab[i].GetComponent<Unit>());//���unit���б�
                                                             //��ȡ����
        Hub[i].SetHub(heroUnit[i]);
        Hub[i].gameObject.SetActive(true);//��ʾ��Ӧ��ɫ״̬��
        LeanTween.move(Hub[i].gameObject, new Vector3(Hub[i].gameObject.transform.position.x + 350f, Hub[i].gameObject.transform.position.y, Hub[i].gameObject.transform.position.z), 0.8f);
    }
    public void SetSinglePrepareHeros(int i)
    {
        heroPreparePrefab.Add(Instantiate(fightPrefs.fightPrepareHeros[i], heroPrepare.transform.position, playerStations[i].rotation));
        heroPreparePrefab[i].transform.SetParent(heroPrepare.transform);

    }

    public IEnumerator SetEnemyHeros()//�ڶ�Ӧλ������ս������Ԥ�����Լ�״̬��
    {
        for (int j = 0; j < enemyPrefs.enemyHeros.Count; j++)
        {
            enemyPrefab.Add(Instantiate(enemyPrefs.enemyHeros[j], enemyStations[j].position, enemyStations[j].rotation));
            enemyPrefab[j].transform.SetParent(battleBackGround.transform);
            enemyPrefab[j].GetComponent<SpriteRenderer>().sortingOrder = j;
            //��ȡ����
            enemyUnit.Add(enemyPrefab[j].GetComponent<Unit>());
            enemyHub[j].SetHub(enemyUnit[j]);
            enemyHub[j].gameObject.SetActive(true);//��ʾ��Ӧ��ɫ״̬��
            LeanTween.move(enemyHub[j].gameObject, new Vector3(enemyHub[j].gameObject.transform.position.x - 350f, enemyHub[j].gameObject.transform.position.y, enemyHub[j].gameObject.transform.position.z), 0.8f);
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
        tips.text = "";
        pointNumber = 1;//Ĭ��ֵ
        useSkill=null;//Ĭ��ֵ
        useCard=null;
        foreach(var btn in exchange.GetComponent<Exchange>().herosBtn)
            btn.gameObject.SetActive(false);
        skillImg.SetActive(false);
        exchange.SetActive(false);
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
            foreach(var o in heroUnit)
                o.anim.Play("idle");
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



    //�������������������������������������������������׶Ρ�������������������������������������������������
    //�غϸ��׶κ���
    IEnumerator PlayerTurnStart()
    {
        if (over==true)
        {
            yield return null;
        }
        else
        {
            tips.text = "��Ļغ�...";
            turnNum.text = turn.ToString();

            if (fightPlayerCards.playerCards.Count == 0)
            {
                GameManager.instance.fightPlayerCards.cardsObject.transform.GetChild(0).gameObject.SetActive(false);
                GameManager.instance.fightPlayerCards.cardsObject.GetComponent<Animator>().Play("cards");
                fightPlayerCards.ResetCards();
            }
                
            state = BattleState.PLAYERTURNSTART;
            //����״̬
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
            tips.text = "";
            yield return new WaitForSeconds(0.5f);
            state = BattleState.PLAYERTURN;

        }


    }
    //ʹ�ü��ܵ�text��ʾ��SkillBtn��
    IEnumerator Action()//�ж��׶κ���
    {
        if (over == true)
        {
            yield return null;
        }
        else
        {
            state = BattleState.ACTIONFINISH;//��ʱ�л�state,��ֹ������д˺���     
            BtnHide();
            exchange.SetActive(false);
            skillImg.SetActive(false);
            CardCanvas.SetActive(true);
            //���ö���
            foreach (var o in heroUnit)
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
                if(Probility(useSkill.precent))
                    {

                    tips.text = "ŷ���� " + useSkill.skillName + " ����ʧ��";
                    turnUnit[0].SkillCost(useSkill);
                    AdjustCards=true;
                    yield return new WaitForSeconds(0.5f);
                }
                else
                {
                    TurnUnitAnim();
                }
            }
            yield return new WaitForSeconds(1f);
            if (state == BattleState.ACTIONFINISH)
                StartCoroutine(ActionFinish());

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
            tips.text = "�����غϽ���";
            for (int i = 0; i < heroUnit.Count; i++)
                heroUnit[i].PassiveTurnEnd();
            yield return new WaitForSeconds(1f);
            foreach (var o in heroUnit)
            {
                o.TurnEndSettle();
                if (o.tired > 0 && !turnUnit.Contains(o))
                {
                    o.tired--;
                    tips.text = o.unitName + " ����1��ƣ��";
                    yield return new WaitForSeconds(0.3f);
                }
            }
            GameReset();
            yield return new WaitForSeconds(1f);
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
            tips.text = "�з��غ�";
            //����״̬
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
            tips.text = "�ȴ��з��ж�...";
            StartCoroutine(EnemyAI());
            yield return new WaitForSeconds(1f);
            TipsSkillPoint();
            yield return new WaitForSeconds(0.5f);
            if (useSkill != null)
            {
                if (Probility(useSkill.precent))
                {
                    tips.text = "ŷ���� " + useSkill.skillName + " ����ʧ��";
                    turnUnit[0].SkillCost(useSkill);
                    yield return new WaitForSeconds(1f);
                }

                else
                {
                    TurnUnitAnim();
                }
            }
            yield return new WaitForSeconds(2f);
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
            tips.text = "�з��غϽ���";
            for (int i = 0; i < enemyUnit.Count; i++)
                enemyUnit[i].PassiveTurnEnd();
            yield return new WaitForSeconds(1f);
            foreach (var o in enemyUnit)
            {
                o.TurnEndSettle();
                if (o.tired > 0 && !turnUnit.Contains(o))
                {
                    o.tired--;
                    tips.text = o.unitName + " ����1��ƣ��";
                    yield return new WaitForSeconds(0.3f);
                }
            }
            GameReset();
            yield return new WaitForSeconds(1f);
            turn++;
            StartCoroutine(PlayerTurnStart());
        }       
    }

    IEnumerator Over()
    {
        state= BattleState.OVER;
        Time.timeScale = 1;
        yield return new WaitForSeconds(1f);
        if(enemyUnit.Count==0)
        {
            win = true;
            Debug.Log("����������Ӯ�ˡ�������");
        }
        else
            Debug.Log("����������Ӯ�ˡ�������");
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
            GameManager.instance.tips.text = "ѡ�� " + GameManager.instance.useSkill.skillName + " ��Ŀ��("+GameManager.instance.pointUnit.Count+"/"+ GameManager.instance.pointNumber+")";       
        
        if(state == BattleState.ABANDOMCARD)
        {
            GameManager.instance.tips.text = "����" + (fightPlayerCards.haveCards.Count - fightPlayerCards.maxCard).ToString() + "����";
        }
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
        if (over==true)
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
        else if(useSkill.type==SkillType.Excharge)
        {
            tips.text = turnUnit[0].unitName + "׼����·";
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



    //����������������������������������������������������С���ܡ�����������������������������������������������������������������������
    public bool Probility(int b)//�ɹ���
    {
        int a;
        a = Koubot.Tool.Random.RandomTool.GenerateRandomInt(0, 99);
        Debug.Log("roll�ĸ���Ϊ"+ a);
        if (a < b)
            return true;
        else
            return false;
    }
}
