using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCImplement : Implement
{
    public override void BeginTurn()
    {
        base.BeginTurn();
        if (!hasMoved && move.CanMove)
        {
            move.IsTurn = true;
        } 
        if(!hasAttacked && battle.CanAttack)
        {
            battle.IsTurn = true;
        } 
        else if ((hasMoved || !move.CanMove) && (hasAttacked || !battle.CanAttack))
        {
            TurnManager.EndTurn();
        }
    }

    protected override void Subscribe()
    {
        base.Subscribe();
        move.EndEvent += TurnManager.EndTurn;
        battle.EndEvent += TurnManager.EndTurn;
    }

    protected override void UnSubscribe()
    {
        base.UnSubscribe();
        move.EndEvent -= TurnManager.EndTurn;
        battle.EndEvent -= TurnManager.EndTurn;
    }
}
