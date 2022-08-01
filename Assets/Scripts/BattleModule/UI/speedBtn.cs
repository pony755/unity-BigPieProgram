using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class speedBtn : MonoBehaviour
{
    public Text speedText;
    private bool speed=false;

    public void speedUpDown()
    {
        if(speed==false)
        {
            speed=true;
            Time.timeScale=2;
            speedText.text = "¡Á2";
        }
        else
        {
            speed = false;
            Time.timeScale = 1;
            speedText.text = "¡Á1";
        }
    }
}
