using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapUI : MonoBehaviour
{
    public enum ButtonType
    {
        library,team,equipment,settings
    } 
    public ButtonType buttonType;

    private void OnMouseOver()
    {
        if(buttonType == ButtonType.library)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("MapGraphics/LibraryButton2");
        }
        else if(buttonType == ButtonType.team)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("MapGraphics/TeamButton2");
        }
        else if(buttonType == ButtonType.equipment)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("MapGraphics/EquipmentButton2");
        }
        else if(buttonType == ButtonType.settings)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("MapGraphics/SettingsButton2");
        }
    }
    private void OnMouseExit()
    {
        if (buttonType == ButtonType.library)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("MapGraphics/LibraryButton1");
        }
        else if (buttonType == ButtonType.team)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("MapGraphics/TeamButton1");
        }
        else if (buttonType == ButtonType.equipment)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("MapGraphics/EquipmentButton1");
        }
        else if (buttonType == ButtonType.settings)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("MapGraphics/SettingsButton1");
        }
    }
    private void OnMouseUp()
    {
        
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
