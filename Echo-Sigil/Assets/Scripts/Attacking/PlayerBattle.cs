using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBattle : JRPGBattle
{
    public new readonly bool leftSide = true;

    private void Update()
    {
        if (isTurn)
        {
            CheckAdjecent();
        }
    }

    private void CheckAdjecent()
    {
        JRPGBattle contender = FindNeighbors();
        if(contender != null && !contender.Equals(this))
        {
            SetCombatant(contender);
        }
    }
}
