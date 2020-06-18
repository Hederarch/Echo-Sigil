using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCImplement : Implement
{
    public override void BeginTurn()
    {
        base.BeginTurn();
        if (!hasMoved)
        {
            move.SetIsTurn();
        } 
        else
        {
            TurnManager.EndTurn();
        }
    }

    protected override void Subscribe()
    {
        base.Subscribe();
        move.EndEvent += TurnManager.EndTurn;
    }

    protected override void UnSubscribe()
    {
        base.UnSubscribe();
        move.EndEvent -= TurnManager.EndTurn;
    }
}
