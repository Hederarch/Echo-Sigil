using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerImplement : Implement
{
    public override void Update()
    {
        base.Update();
        QOLEndTurn();
    }

    private void QOLEndTurn()
    {
        if(hasMoved /*&& hasAttacked*/)
        {
            TurnManager.EndTurn();
        }
    }

    void Move()
    {
        if (!hasMoved)
        {
            tacticsMove.isTurn = true;
        }
    }

    void Attack()
    {
        throw new NotImplementedException();
    }

    protected override void Subscribe()
    {
        PlayerInputs.MoveEvent += Move;
        PlayerInputs.AttackEvent += Attack;
        base.Subscribe();
    }

    protected override void UnSubscribe()
    {
        PlayerInputs.MoveEvent -= Move;
        PlayerInputs.AttackEvent -= Attack;
        base.UnSubscribe();
    }
}
