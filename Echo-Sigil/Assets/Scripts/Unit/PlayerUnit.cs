using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnit : Unit
{
    private void QOLEndTurn()
    {
        if (TurnManager.CurrentUnit.Equals(this) && ((hasMoved || !CanMove) && (hasAttacked || !CanAttack)))
        {
            TurnManager.EndTurn();
        }
    }

    void Move() => CanMove = !hasMoved;

    void Attack() => CanAttack = !hasAttacked;

    public override void HasMoved()
    {
        base.HasMoved();
        QOLEndTurn();
    }

    public override void HasAttacked()
    {
        base.HasAttacked();
        QOLEndTurn();
    }

    public override void BeginTurn()
    {
        base.BeginTurn();
        QOLEndTurn();
    }
}
