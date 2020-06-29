using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Ability
{
    public string name;

    public int willDamage;
    public int healthDameage;

    public int willCost;

    public string menuPath;

    public virtual void ActivateAbility()
    {
        if (BattleData.isLeftTurn)
        {
            BattleData.rightCombatants[0].will -= willDamage;
            BattleData.rightCombatants[0].health -= healthDameage;

            BattleData.leftCombatants[0].will -= willCost;
        } else
        {
            BattleData.leftCombatants[0].will -= willDamage;
            BattleData.leftCombatants[0].health -= healthDameage;

            BattleData.rightCombatants[0].will -= willCost;
        }

        BattleData.EndBattleTurn();
    }
}
