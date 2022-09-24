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
    public bool over ;//ս��������־λ
    public bool win ;//ʤ��������־λ
    [HideInInspector] public bool AdjustCards = false;//��������λ�ñ�־λ

    [Header("ȫ����List")]   
    public GameObject tempPlayer;
    [Header("��������")]
    public FightPlayerInFight fightPlayer;//�������
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
    public int abandomCardNum;
    public bool abandomCardSwitch;
    public BattleState state;

    [Header("Heros")]
    [HideInInspector]public List<GameObject> heroPrefab;//����ս���б��ɫ
    public List<Unit> heroUnit;//��ȡս���б��ɫUnit�ű�
    public List<GameObject> heroPreparePrefab;//����С���б��ɫ
    public List<Unit> deadUnit;//��ȡս���б��ɫUnit�ű�

    [Header("enemyHeros")]
    [HideInInspector] public List<GameObject> enemyPrefab;//���յ����б��ɫ
    public List<Unit> enemyUnit;//��ȡ�����б��ɫUnit�ű�
    // Start is called before the first frame update
    [Header("��������������ʱ���㡪����������")]
    public bool delayedSwitch;
    public List<int> delayedTurn;//�ӳٵĻغ���
    public List<Unit> delayedTurnUnit;//�ӳٻغϼ��ܷ�����
    public List<Skill> delayedSkill;//�ӳٻغϵļ���
    public List<List<Unit>> delayedPointUnit=new List<List<Unit>>();//�ӳٻغϼ���Ŀ�귽
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

        //�жϸ��¼��Ƿ���������δ���������������
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
        //�жϸ��¼��Ƿ���������δ���������������
        StartCoroutine(Load());       
    }

    
    IEnumerator Load()
    {
        Debug.Log("������Ϸ");
        yield return new WaitForSeconds(1f);//��ּ���
        foreach(var i in fightPlayer.items)//������ƷЧ��
        {
            if(i.itemType == ItemBase.ItemType.GainEffectInFight)
            i.SettleGainEffectInFightItem();
        }
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
            foreach (var p in heroUnit)
                p.getExpAndCurrentHp();//���浱ǰѪ���ͻ�ȡ�ľ���ֵ
            foreach (var p in deadUnit)
                p.getExpAndCurrentHp();//���浱ǰѪ���ͻ�ȡ�ľ���ֵ
            foreach (var p in heroPreparePrefab)
                p.GetComponent<Unit>().getExpAndCurrentHp();//���浱ǰѪ���ͻ�ȡ�ľ���ֵ
            //���㿨�ƻ�ȡ(����浵)
            if (Koubot.Tool.Random.RandomTool.GenerateRandomInt(0, 99) < 70)
                fightPlayer.getCards.Add(GetCard.T);
            StartCoroutine(Over());//�����¼������꣬����Over����
            GameReset();
        }
        CheckAbandomCardNum();

        

        if((state == BattleState.POINTPLAYER|| state == BattleState.POINTENEMY|| state == BattleState.POINTALL||state==BattleState.POINTPREPAREHERO) &&pointNumber==pointUnit.Count)
            StartCoroutine("ToAction");
        if (state == BattleState.ACTION)//������ACTIONʱ��ִ�к���(Я�̣�  
            StartCoroutine("Action");
        if(state == BattleState.ABANDOMCARD)
        {
            if(fightPlayer.haveCards.Count<= fightPlayer.maxCard)
            {
                StartCoroutine(EnemyTurnStart());
            }
                
        }
        if (AdjustCards)
        {
            AdjustCards=false;
            StartCoroutine(fightPlayer.CardAdjustPosition());
        }
    }
    
    public IEnumerator SetHeros()//�ڶ�Ӧλ������ս������Ԥ�����Լ�״̬��
    {
        for (int i = 0; i < fightPrefs.fightHeros.Count; i++)
        {
            SetSingleHeros(i);
        }
        for (int j = 0; j < fightPrefs.fightPrepareHeros.Count; j++)
        {
            Debug.Log(j);
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
        Hub[i].SetHub(heroUnit[i]);
        Hub[i].gameObject.SetActive(true);//��ʾ��Ӧ��ɫ״̬��
        LeanTween.move(Hub[i].gameObject, new Vector3(Hub[i].gameObject.transform.position.x + 350f, Hub[i].gameObject.transform.position.y, Hub[i].gameObject.transform.position.z), 0.8f);
    }
    public void SetSinglePrepareHeros(int i)
    {
        heroPreparePrefab.Add(Instantiate(fightPrefs.fightPrepareHeros[i], heroPrepare.transform.position, fightPrefs.fightPrepareHeros[i].transform.rotation));
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
    public void SkillShow(Unit unit)//��ʾ������
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
            skillBtns[i].GetComponent<SkillBtn>().skillImg.sprite = unit.heroSkillList[i].skillImg;
            skillBtns[i].SetActive(true);
            if (unit.heroSkillList[i].passiveSkill==true)
            {
                skillBtns[i].GetComponent<Button>().interactable = false;
            }
        }
    }
    public void GameReset()//����
    {
        if (state == BattleState.POINTPREPAREHERO)
            fightPlayer.CardAdjustPositionImediately();

        AdjustCards = true;
        BtnReset();
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
        
        if (state == BattleState.POINTPREPAREHERO)
            state = BattleState.PLAYERTURN;
        
    }



    public void BtnReset()
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
            if (delayedTurn[temp] <= turn)
            {
                if (delayedTurnUnit[temp].playerHero)
                {
                    if (delayedTurnUnit[temp].currentHP > 0)
                    {                      
                        tips.text = delayedTurnUnit[temp].unitName + " ���� " + delayedSkill[temp].skillName;
                        delayedSwitch = true;
                        delayedTurnUnit[temp].UseSkillSettle(delayedSkill[temp],delayedPointUnit[temp] );                       
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
            if (delayedTurn[temp] <= turn)
            {
                if (!delayedTurnUnit[temp].playerHero)
                {
                    if (delayedTurnUnit[temp].currentHP > 0)
                    {
                        tips.text = delayedTurnUnit[temp].unitName + " ���� " + delayedSkill[temp].skillName;
                        delayedSwitch = true;
                        delayedTurnUnit[temp].UseSkillSettle(delayedSkill[temp], delayedPointUnit[temp]);
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

            if (fightPlayer.playerCards.Count == 0)
            {
                fightPlayer.cardsObject.transform.GetChild(0).gameObject.SetActive(false);
                fightPlayer.ResetCards();
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
            yield return new WaitUntil(() => abandomCardSwitch == false);
            tips.text = "";
            state = BattleState.PLAYERTURN;

        }


    }
    public void SkillToAction()
    {
        StartCoroutine(ToAction());
    }
    IEnumerator ToAction()//����̬
    {
        BtnReset();
        exchange.SetActive(false);
        skillImg.SetActive(false);
        CardCanvas.SetActive(true);
        abandomCardNum += useSkill.abandomCardNum;
        state = BattleState.TOACTION;
        yield return new WaitForSeconds(0.2f);
        yield return new WaitUntil(() => abandomCardSwitch == false);
        state = BattleState.ACTION;

    }
    //ʹ�ü��ܵ�text��ʾ��SkillBtn��
    IEnumerator Action()//�ж��׶κ���
    {  
        state = BattleState.ACTIONFINISH;//��ʱ�л�state,��ֹ������д˺���    
        if (over == true)
        {
            yield return null;
        }
        else
        {                 
            //���ö���
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
                if(Function.Probility(useSkill.precent))
                    {

                    tips.text = "ŷ���� " + useSkill.skillName + " ����ʧ��";
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
            if (fightPlayer.haveCards.Count <= fightPlayer.maxCard)
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
            tips.text = "�ȴ��з��ж�...";
            StartCoroutine(EnemyAI());
            yield return new WaitForSeconds(1f);
            TipsSkillPoint();
            yield return new WaitForSeconds(0.5f);
            if (useSkill != null)
            {
                if (Function.Probility(useSkill.precent))
                {
                    tips.text = "ŷ���� " + useSkill.skillName + " ����ʧ��";
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
            yield return new WaitUntil(() => abandomCardSwitch == false);
            turn++;
            StartCoroutine(PlayerTurnStart());
        }       
    }

    IEnumerator Over()
    {
        state= BattleState.OVER;
        Time.timeScale = 1;
        foreach (var h in heroUnit)
        {
            h.UnitLoad();
        }
        yield return new WaitForSeconds(1f);
        if(enemyUnit.Count==0)
        {
            win = true;
            Debug.Log("����������Ӯ�ˡ�������");
        }
        else
            Debug.Log("�������������ˡ�������");

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
            GameManager.instance.tips.text = "����" + (fightPlayer.haveCards.Count - fightPlayer.maxCard).ToString() + "����";
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
                if (turnUnit[0].currentMP >= t.needMP&&t.passiveSkill!=true)
                    tempSkill.Add(t);
            }
            useSkill = tempSkill[Koubot.Tool.Random.RandomTool.GenerateRandomInt(0, tempSkill.Count - 1)];
            tempSkill.Clear();
            pointNumber = useSkill.pointNum;//��Ӽ���Ŀ������
            StartCoroutine(useSkill.EnemyUse());
            yield return new WaitForSeconds(1f);
            


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
    IEnumerator TurnUnitAnim()//��������
    {  
        if(turnUnit[0].player==false)//����ұ���
        {
            if (useSkill.animType == AnimType.Attack)
            {
                turnUnit[0].anim.Play("attack");
            }
        }
        yield return new WaitForSeconds(0.1f);
        if(turnUnit[0].player == false)
             yield return new WaitUntil(()=>turnUnit[0].anim.GetCurrentAnimatorStateInfo(0).IsName("idle"));//�ȴ������������
        turnUnit[0].UseSkillSettle(useSkill,pointUnit);

    }


    public void ShowBackMenu()
    {
        backPanel.SetActive(true);
    }
    public void HideBackMenu()
    {
        backPanel.SetActive(false);
    }
    public void BackToMainScene()
    {
        StartCoroutine(test());
    }
    IEnumerator test()
    {
        tips.text = "������δ����";
        yield return new WaitForSeconds(0.8f);
        tips.text = "";
    }

    //����������������������������������������������������С���ܡ�����������������������������������������������������������������������
    
    public void CheckAbandomCardNum()
    {
        if (abandomCardNum > 0&&abandomCardSwitch==false)
        {
            if (abandomCardNum > fightPlayer.haveCards.Count&&state==BattleState.TOACTION)
            {
                tips.text = "���Ʋ���";
                Back();
            }
            else if (abandomCardNum == fightPlayer.haveCards.Count && useCard != null && state == BattleState.TOACTION)
            {
                tips.text = "���Ʋ���";
                Back();
            }
            else if(abandomCardNum > fightPlayer.haveCards.Count)
                abandomCardNum=fightPlayer.haveCards.Count;
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
            tips.text="��Ҫ����"+abandomCardNum+"����";
    }
}
