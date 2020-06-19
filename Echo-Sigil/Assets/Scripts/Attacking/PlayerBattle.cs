using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBattle : JRPGBattle
{
    private void Start()
    {
        leftSide = true;
    }

    private void Update()
    {
        if (isTurn && !inBattle)
        {
            JRPGBattle j = CheckAdjecent();
            if(j != null)
            {
                SetCombatant(j);
            }
        }
    }
}
