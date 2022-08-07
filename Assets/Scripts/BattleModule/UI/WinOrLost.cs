using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class WinOrLost : MonoBehaviour
{
    public Player player;
    public GameObject levelUp;
    public Button next;
    public Image winLostImg;
    public Sprite winImg;
    public Sprite lostImg;
    public GameObject Win;
    public GameObject Lost;
    public List<WinHeroShow> Heros=new List<WinHeroShow>();
    public Text tips;

    public int heroIndex;
    //到时候逐行读取txt文件给其fu值
    private string winText= "米浴说的道理";
    private string lostText = "下次一定";
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        next.onClick.AddListener(delegate () { ChangeScene(); });
        heroIndex = 0;
        if (GameManager.instance.win == true)
        {
            winLostImg.sprite = winImg;
            tips.text = winText;
            Win.SetActive(true);
            for(int i=0; i < GameManager.instance.playerUnit.Count; i++)
            {
                Heros[i].winHero = GameManager.instance.playerUnit[i];
                Heros[i].gameObject.SetActive(true);
            }
            player.BP += 100;
        }
        else
        {
            winLostImg.sprite = lostImg;
            tips.text = lostText;
            Lost.SetActive(true);
        }
    }
    private void Update()
    {
        if(heroIndex!=Heros.Count)
        {
            if(Heros[heroIndex].ExpFinish)
                heroIndex++;
        }
            
        if(heroIndex==Heros.Count)
            next.gameObject.SetActive(true);

        if (player != null)
        {
            if (player.globalStateValue == 0)
            {
                player.globalStateValue++;
            }
        }

    }
    private void ChangeScene()
    {
        Scene scene = SceneManager.GetSceneByName("MapScene");
        SceneManager.MoveGameObjectToScene(player.gameObject, scene);
        SceneManager.UnloadSceneAsync("BattleScene");
    }
}
