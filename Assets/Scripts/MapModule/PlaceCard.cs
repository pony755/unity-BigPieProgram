using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlaceCard : MonoBehaviour
{
    public enum PlaceCardState
    {
        hide,back,front
    };
    public PlaceCardState cardState;

    public enum PlaceCardType
    {
        battle,randomEvent,shop,hotel,treasure,portal,placeOfGod
    };
    public PlaceCardType cardType;


    // Start is called before the first frame update
    void Start()
    {
        cardState = PlaceCardState.back;
    }

    public void OnMouseUp()
    {
        OpenPlaceCard();
    }

    void OpenPlaceCard()
    {
        transform.eulerAngles = new Vector3(0, 180, 0);
        cardState = PlaceCardState.front;
        StartCoroutine(EnterBattle());
    }

    IEnumerator EnterBattle()
    {
        yield return new WaitForSeconds(3.0f);
        SceneManager.LoadScene("BattleScene", LoadSceneMode.Additive);
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
