using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Run Away", menuName = "Battle Element/Absocond", order = 0)]
public class Abscond : Ability
{
    public override void ActivateAbility()
    {
        if (BattleData.isLeftTurn)
        {
            BattleData.rightCombatants[0].will -= willDamage;
            BattleData.rightCombatants[0].health -= healthDameage;

            BattleData.leftCombatants[0].will -= willCost;
        }
        else
        {
            BattleData.leftCombatants[0].will -= willDamage;
            BattleData.leftCombatants[0].health -= healthDameage;

            BattleData.rightCombatants[0].will -= willCost;
        }

        BattleData.CheckForDead();

        BattleData.instagator.EndCombat();
    }
}
