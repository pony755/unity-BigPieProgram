using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class WinObject : MonoBehaviour
{

    public Button next;
    public List<WinHeroShow> Heros = new List<WinHeroShow>();
    public int heroIndex;
    public int showHeroInt;
    // Start is called before the first frame update
    void Start()
    {
        showHeroInt = 0;
        heroIndex = 0;
        for (int i = 0; i < GameManager.instance.heroUnit.Count; i++)
        {
            Heros[i].winHero = GameManager.instance.heroUnit[i];
            showHeroInt++;
            Heros[i].gameObject.SetActive(true);
        }

        //player.BP += 100;
    }

    // Update is called once per frame
    void Update()
    {
        if (heroIndex != showHeroInt)
        {
            if (Heros[heroIndex].ExpFinish)
                heroIndex++;
        }

        if (heroIndex == showHeroInt)
            next.gameObject.SetActive(true);
    }

}
