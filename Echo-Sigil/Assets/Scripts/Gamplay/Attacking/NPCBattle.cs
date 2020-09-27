using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NPCBattle : JRPGBattle
{
    private void Start()
    {
        leftSide = false;
    }

    private void Update()
    {
        if (!BattleData.isLeftTurn && inBattle)
        {
            int damage = 0;
            Ability use = null;
            foreach(Ability a in abilites.Keys.ToArray())
            {
                if(a.healthDameage > damage)
                {
                    damage = a.healthDameage;
                    use = a;
                }
            }
            use.ActivateAbility();
        }
    }
}
