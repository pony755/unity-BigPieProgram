using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllList : MonoBehaviour
{
    public static AllList instance;
    public List<GameObject> allHero;
    public List<GameObject> allEnemyHero;
    public List<Skill> allSkillList;
    public List<Cards> allCardList;
    public List<item> allItemList;
    public void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }
}
