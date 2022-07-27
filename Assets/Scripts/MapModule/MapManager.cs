using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlaceCard;

public class MapManager : MonoBehaviour
{
    public List<PlaceCard> cardList;

    // Start is called before the first frame update
    void Start()
    {
        PlaceCard[,] levelMap = new PlaceCard[3, 3];
    }

    public void AddCardInList(PlaceCard card)
    {
        cardList.Add(card);
    }

    void InitializeCard(CardType cardtype)
    {
        GameObject card = Instantiate(Resources.Load<GameObject>("Prefabs/back"));
        card.GetComponent<PlaceCard>().cardType = cardtype;
        card.name = "Card_" + cardtype.ToString();

        GameObject cardFace = Instantiate(Resources.Load<GameObject>("MapGraphics/Cardface_Battle"));
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
