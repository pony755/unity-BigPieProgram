using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero000 : Unit
{
    public override void LevelUp2()
    {
        
        MaxHpUp(50);
        maxMP += 20;
        skillRoll.Add(SkillRoll.T);
    }
}
