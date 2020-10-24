using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCUnit : Unit
{
    public override void BeginTurn()
    {
        base.BeginTurn();
        CanMove = !hasMoved;
        CanAttack = !hasAttacked;
    }

}
