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
        CursorBehaviour.GetCursor();
    }
    public override void EndTurn()
    {
        base.EndTurn();
        CursorBehaviour.HideCursor();
    }

    public override Color GetTeamColor()
    {
        return Color.green;
    }

    public override Texture2D GetTeamTexture()
    {
        return base.GetTeamTexture();
    }
}
