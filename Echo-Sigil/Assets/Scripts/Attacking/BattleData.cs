using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BattleData
{
    public static JRPGBattle instagator;
    public static JRPGBattle combatant;
    public static List<JRPGBattle> leftCombatants = new List<JRPGBattle>();
    public static List<JRPGBattle> rightCombatants = new List<JRPGBattle>();

    public static bool isLeftTurn;

    public static void SortIntoLists(JRPGBattle[] units)
    {
        leftCombatants.Clear();
        rightCombatants.Clear();
        foreach(JRPGBattle unit in units)
        {
            if(unit != null)
            {
                if (unit.leftSide)
                {
                    leftCombatants.Add(unit);
                }
                else
                {
                    rightCombatants.Add(unit);
                }
            }
        }
    }

    public static void Reset()
    {
        instagator = null;
        if(combatant != null)
        {
            combatant.inBattle = false;
            combatant = null;
        }
        leftCombatants.Clear();
        rightCombatants.Clear();
    }

    public static void CheckForDead()
    {
        if(instagator.GetHealthPercent() <= 0)
        {
            instagator.EndCombat();
            TurnManager.EndTurn();
            Object.Destroy(instagator.gameObject);
        } else if (combatant.GetHealthPercent() <= 0)
        {
            instagator.EndCombat();
            Object.Destroy(combatant.gameObject);
        } else if (instagator.GetWillPercent() <= 0)
        {
            instagator.EndCombat();
            Debug.Log(instagator.name + " has lost will fight");
        } else if (combatant.GetWillPercent() <= 0)
        {
            instagator.EndCombat();
            Debug.Log(combatant.name + " has lost will fight");
        }
    }

    public static void EndBattleTurn()
    {
        CheckForDead();
        isLeftTurn = !isLeftTurn;
    }
}
