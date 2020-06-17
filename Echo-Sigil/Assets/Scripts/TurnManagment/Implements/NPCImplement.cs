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
            tacticsMove.isTurn = true;
        } else
        {
            TurnManager.EndTurn();
        }

    }

    protected override void Subscribe()
    {
        base.Subscribe();
        tacticsMove.EndMoveEvent += TurnManager.EndTurn;
    }

    protected override void UnSubscribe()
    {
        base.UnSubscribe();
        tacticsMove.EndMoveEvent -= TurnManager.EndTurn;
    }
}
