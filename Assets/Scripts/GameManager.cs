using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public enum BattleState { START, PLAYERTURNSTART,PLAYERTURN, SKILL,CARD,POINTENEMY,POINTPLAYER,ACTION,ACTIONFINISH,ENEMYTURNSTART, ENEMYTURN,ENEMYFINISH,WIN, LOST,OVER }
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [Header("���ܻ�������")]
    public GameObject WinOrLost;
    public GameObject skillImg;//���ܻ���
    public GameObject backBtn;
    public GameObject[] skillBtns;//���ܰ�ť   
    public Text[] skilText;//���ܰ�ť�ı�

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

    [Header("Btn")]
    public List<SkillBtn> skillBtnInfo;//���ܰ�ť�ű�
    [Header("Heros")]
    public GameObject[] playerPrefab;//����ս���б��ɫ
    public List<Unit> playerUnit;//��ȡս���б��ɫUnit�ű�

    [Header("enemyHeros")]
    public GameObject[] enemyPrefab;//���յ����б��ɫ
    public List<Unit> enemyUnit;//��ȡ�����б��ɫUnit�ű�
    // Start is called before the first frame update


    [Header("������������FIGHTING������������")]

    public List<Unit> turnUnit;//��ǰ�غϼ��ܷ�����
    public Skill useSkill;//��ɫ����
    public List<Unit> pointUnit;//��ǰ�غϼ���Ŀ�귽
    public int pointNumber;//Ŀ������
    public void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
        state = BattleState.START;//����״̬����ʼ
        SetHeros();//����Ӣ��
        pointNumber = 1;//Ĭ��ֵ
        useSkill = null;//Ĭ��ֵ
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
        if (state == BattleState.ACTION)//������ACTIONʱ��ִ�к���(Я�̣�
        {
            StartCoroutine(Action());
        }
                        
    }
    public void SetHeros()//�ڶ�Ӧλ������ս������Ԥ�����Լ�״̬��
    {
        for (int i = 0; i < fightPrefs.fightHeros.Length; i++)
        {
            playerPrefab[i]=Instantiate(fightPrefs.fightHeros[i],playerStations[i].position,playerStations[i].rotation);
            playerUnit.Add(playerPrefab[i].GetComponent<Unit>());//���unit���б�
            playerUnit[i].hub = Hub[i];
            playerUnit[i].HubUpdate();
            Hub[i].gameObject.SetActive(true);//��ʾ��Ӧ��ɫ״̬��          
        }

        for (int j = 0; j < enemyPrefs.enemyHeros.Length; j++)
        {
            enemyPrefab[j] = Instantiate(enemyPrefs.enemyHeros[j], enemyStations[j].position, enemyStations[j].rotation);
            enemyUnit.Add(enemyPrefab[j].GetComponent<Unit>());
            enemyUnit[j].hub = enemyHub[j];
            enemyUnit[j].HubUpdate();
            enemyHub[j].gameObject.SetActive(true);//��ʾ��Ӧ��ɫ״̬��          
        }
    }






    //������������������������������������������������UI����������������������������������������������������
    public void SkillShow(Unit unit)//��ʾ������(codeΪ��ǰ���Ͻ�ɫ��ţ�
    {
        backBtn.SetActive(true);
        skillImg.SetActive(true);
        turnUnit.Add(unit);
        state= BattleState.SKILL;//�л��غ�״̬
        BtnSet(unit);
       
    }
    public void BtnSet(Unit unit)
    {

        for (int i = 0; i < unit.heroSkillList.Count; i++)//���ð�ť
        {
            skillBtns[i].SetActive(true);
            skillBtnInfo.Add(skillBtns[i].GetComponent<SkillBtn>());//��ȡ�ű������в���
            skillBtnInfo[i].skillInfo = unit.heroSkillList[i];//��ť��ȡ���ܽű�
            skillBtnInfo[i].skillText.text =unit.heroSkillList[i].skillName;
        }
    }
    public void GameReset()//����
    {
        BtnHide();
        backBtn.SetActive(false);
        pointNumber = 1;//Ĭ��ֵ
        useSkill=null;//Ĭ��ֵ
        skillImg.SetActive(false);
        skillBtnInfo.Clear();
        turnUnit.Clear();
        pointUnit.Clear();
    }

    public void BtnHide()
    {
        backBtn.SetActive(false);
        for (int i = 0; i < skillBtnInfo.Count; i++)//���ذ�ť
        {
            skillBtns[i].SetActive(false);
            skillBtnInfo[i].skillInfo = null;//��հ�ť��skill
        }
    }
    public void Back()//������һغ�
    {
        
        if (state==BattleState.PLAYERTURN || state == BattleState.SKILL || state == BattleState.POINTENEMY || state == BattleState.POINTPLAYER)
        {
            
            GameReset();
            state = BattleState.PLAYERTURN;
        }     
    }
    public void HeroPointActive()//��ʾѡ����ҵı�־,�ɴ˺����ж��Ƿ������һ�׶�.
    {
        
        if (state != BattleState.POINTENEMY || state != BattleState.POINTPLAYER)//���������׶Σ���ͼ��
        {
            foreach (var o in enemyUnit)
                o.transform.GetChild(0).gameObject.SetActive(false);
            foreach (var o in playerUnit)
                o.transform.GetChild(0).gameObject.SetActive(false);
        }


        if (state==BattleState.POINTENEMY && pointNumber!=0)//ָ����˽׶Σ�ָ������Ϊ0
        {
            
            foreach(var o in enemyUnit)//����
            {
                o.transform.GetChild(0).gameObject.SetActive(true);
                if (pointUnit.Contains(o.GetComponent<Unit>())&&!useSkill.reChoose)//���������Ƿ���Ŀ���б����Ҽ��ܲ�֧���ظ�ѡ
                {
                    o.transform.GetChild(0).gameObject.SetActive(false);
                }
                if (pointUnit.Count == pointNumber)//����Ŀ���б�������ָ������ʱ,����ACTION
                {
                    state = BattleState.ACTION;
                }
            }           
        }

  

    }



    //�غϸ��׶κ���
    IEnumerator PlayerTurnStart()
    {
        state = BattleState.PLAYERTURNSTART;
        Debug.Log("�غϿ�ʼ");
        //����״̬
        yield return new WaitForSeconds(1f);
        if (state != BattleState.OVER)
        {
            state = BattleState.PLAYERTURN;
            Debug.Log("��һغ�");
        }
        
    }
    IEnumerator Action()//�ж��׶κ���
    {
        state = BattleState.ACTIONFINISH;//��ʱ�л�state,��ֹ������д˺���
        BtnHide();
        //���ö���
        foreach(var o in playerUnit)
        {
            o.anim.Play("idle");
        }
        foreach (var o in enemyUnit)
        {
            o.anim.Play("idle");
        }
        
        Debug.Log(turnUnit[0].unitName + "������" + useSkill.skillName);        
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
        Debug.Log("�غϽ���");
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
        Debug.Log("�з��غϿ�ʼ");
        //����״̬
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
        turnUnit.Add(enemyUnit[r.Next(enemyUnit.Count)]);//������һ���з�
        useSkill = turnUnit[0].heroSkillList[r.Next(turnUnit[0].heroSkillList.Count)];//������һ������
        pointNumber = useSkill.pointNum;//��Ӽ���Ŀ������
        useSkill.EnemyUse();
        while(pointNumber>pointUnit.Count)//��������ΪĿ��
        {
            int player = r.Next(playerUnit.Count);
            if (!pointUnit.Contains(playerUnit[player]))
               pointUnit.Add(playerUnit[player]);
        }
        yield return new WaitForSeconds(1f);
        Debug.Log(turnUnit[0].unitName + "������" + useSkill.skillName);
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
        
        Debug.Log("�з��غϽ���");
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
        Debug.Log("����������Ӯ�ˡ�������");
    }
    IEnumerator Lost()
    {
        state = BattleState.OVER;
        yield return new WaitForSeconds(1f);
        WinOrLost.SetActive(true);
        Debug.Log("�������������ˡ�������");
    }

}
