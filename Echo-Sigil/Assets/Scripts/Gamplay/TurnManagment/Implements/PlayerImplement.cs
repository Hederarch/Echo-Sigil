using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerImplement : Implement
{
    private void QOLEndTurn()
    {
        if (TurnManager.CurrentUnit.Equals(this) && ((hasMoved || !move.GetCanMove()) && (hasAttacked || !battle.GetCanAttack())))
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

    public override void HasMoved()
    {
        base.HasMoved();
        PlayerGUIScript.staticThis.SetAsCurrentUnit(this);
        QOLEndTurn();

    }

    public override void HasAttacked()
    {
        base.HasAttacked();
        PlayerGUIScript.staticThis.SetAsCurrentUnit(this);
        QOLEndTurn();
    }

    public override void BeginTurn()
    {
        base.BeginTurn();
        QOLEndTurn();
    }
}
