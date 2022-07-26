using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceCard : MonoBehaviour
{
    public bool placeCardState;
    // Start is called before the first frame update
    void Start()
    {
        placeCardState = false;
    }

    public void OnMouseUp()
    {
        OpenPlaceCard();
    }

    void OpenPlaceCard()
    {
        transform.eulerAngles = new Vector3(0, 180, 0);
        placeCardState = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
