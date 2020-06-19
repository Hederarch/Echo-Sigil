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
        BattleData.combatant.will -= willDamage;
        BattleData.combatant.health -= healthDameage;

        BattleData.instagator.will -= willCost;
    }
}
