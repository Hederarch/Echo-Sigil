using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerImplement : Implement
{
    protected override void Update()
    {
        base.Update();
        QOLEndTurn();
    }

    private void QOLEndTurn()
    {
        if(hasMoved && hasAttacked)
        {
            TurnManager.EndTurn();
        }
    }

    void Move()
    {
        if (!hasMoved)
        {
            move.SetIsTurn();
        }
    }

    void Attack()
    {
        if (!hasAttacked)
        {
            battle.SetIsTurn();
        }
    }

    protected override void Subscribe()
    {
        PlayerGUIScript.MoveEvent += Move;
        PlayerGUIScript.AttackEvent += Attack;
        base.Subscribe();
    }

    protected override void UnSubscribe()
    {
        PlayerGUIScript.MoveEvent -= Move;
        PlayerGUIScript.AttackEvent -= Attack;
        base.UnSubscribe();
    }
}
