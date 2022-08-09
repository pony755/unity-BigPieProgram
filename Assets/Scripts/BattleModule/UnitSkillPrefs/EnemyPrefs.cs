using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPrefs : MonoBehaviour
{

    public List<GameObject> enemyHeros;

    [Header("µÐÈË×éºÏ")]
    public List<GameObject> enemyBundle000;
    public List<GameObject> enemyBundle001;
    public List<GameObject> enemyBundle002;
    public List<GameObject> enemyBundle003;
    public List<GameObject> enemyBundle004;
    public List<GameObject> enemyBundle005;

    // Start is called before the first frame update

    void Start()
    {
        SetEnemyBundle(GameManager.instance.tempPlayer.GetComponent<FightPlayer>().enemyBundleCode);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetEnemyBundle(int a)
    {
        if(a==0)
        {
            foreach(var o in enemyBundle000)
                enemyHeros.Add(o);
        }
        if (a == 1)
        {
            foreach (var o in enemyBundle001)
                enemyHeros.Add(o);
        }
        if (a == 2)
        {
            foreach (var o in enemyBundle002)
                enemyHeros.Add(o);
        }
        if (a == 3)
        {
            foreach (var o in enemyBundle003)
                enemyHeros.Add(o);
        }
        if (a == 4)
        {
            foreach (var o in enemyBundle004)
                enemyHeros.Add(o);
        }

        StartCoroutine(GameManager.instance.SetEnemyHeros());
    }
}
