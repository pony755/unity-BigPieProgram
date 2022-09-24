using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Function
{
    public static bool Probility(int b)//³É¹¦ÂÊ
    {
        int a;
        a = Koubot.Tool.Random.RandomTool.GenerateRandomInt(0, 99);
        if (a < b)
            return true;
        else
            return false;
    }
}
