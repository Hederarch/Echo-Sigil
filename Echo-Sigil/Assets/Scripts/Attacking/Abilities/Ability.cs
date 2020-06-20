using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "punch",menuName = "Battle Element/Ability",order = 0)]
public class Ability : ScriptableObject
{
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
