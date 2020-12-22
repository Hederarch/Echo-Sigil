using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCUnit : Unit
{
    public override string Tag => "NPC";

    public override Color GetTeamColor()
    {
        return Color.red;
    }

    public override Texture2D GetTeamTexture()
    {
        return base.GetTeamTexture();
    }
}
