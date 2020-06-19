using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Run Away", menuName = "Battle Element/Absocond", order = 0)]
public class Abscond : Ability
{
    public override void ActivateAbility()
    {
        BattleData.instagator.EndCombat();
    }
}
