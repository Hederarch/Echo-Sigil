using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BattleData
{
    public static JRPGBattle instagator;
    public static JRPGBattle combatant;
    public static List<JRPGBattle> leftCombatants = new List<JRPGBattle>();
    public static List<JRPGBattle> rightCombatants = new List<JRPGBattle>();

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
        combatant = null;
        leftCombatants.Clear();
        rightCombatants.Clear();
    }
}
