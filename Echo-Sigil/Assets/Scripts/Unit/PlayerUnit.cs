using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnit : Unit
{
    public const string playerTag = "Player";

    public override string Tag => playerTag;

    public override void BeginTurn()
    {
        base.BeginTurn();
        Selector.GetCursor();
    }
}
