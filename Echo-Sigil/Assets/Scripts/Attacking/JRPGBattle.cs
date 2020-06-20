using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JRPGBattle : MonoBehaviour , IBattle
{
    public bool leftSide;

    public int health =5;
    public int maxHealth = 5;

    public int will =5;
    public int maxWill = 5;

    public List<Ability> abilites = new List<Ability>();
    public float reach;

    public bool isTurn;
    public bool inBattle;

    public event Action EndEvent;

    private void Update()
    {
        if (isTurn && !inBattle)
        {
            JRPGBattle j = CheckAdjecent();
            if (j != null)
            {
                SetCombatant(j);
            }
        }
    }

    public void SetCombatant(JRPGBattle combatant)
    {
        if(combatant != null && !inBattle && isTurn)
        {
            inBattle = true;
            //needs to be a corutine for the animation to work
            StartCoroutine(SetCombatantCoroutine(combatant));
        }
    }

    IEnumerator SetCombatantCoroutine(JRPGBattle combatant)
    {
        //wait for black
        FightGUIScript.SetBattleAnimations();
        yield return new WaitForSeconds(.3f);

        BattleData.Reset();

        //who is punching who here?
        BattleData.instagator = this;
        BattleData.combatant = combatant;
        combatant.inBattle = true;

        //later, there will be an ability to pull firends into the mess
        JRPGBattle[] j = new JRPGBattle[2];
        j[0] = this;
        j[1] = combatant;
        BattleData.SortIntoLists(j);

        Camera.main.GetComponent<JRPGBattleCamera>().SwitchCamera(true);

        //set all the guis off
        BattleData.isLeftTurn = leftSide;
        FightGUIScript.SetMenu();
        FightGUIScript.SetStats();
        BattleData.CheckForDead();
    }

    public void EndCombat()
    {
        inBattle = false;
        isTurn = false;
        FightGUIScript.UnSetBattleAnimations();
        FightGUIScript.ResetMenuAndStats();
        Camera.main.GetComponent<JRPGBattleCamera>().SwitchCamera(false);
        EndEvent?.Invoke();
        TurnManager.CheckForWin();
    }


    protected JRPGBattle FindNeighbors()
    {
        JRPGBattle output = null;
        if(FindNeighbor(Vector3.up) != null)
        {
            output = FindNeighbor(Vector3.up);
        }
        if (FindNeighbor(Vector3.down) != null)
        {
            output = FindNeighbor(Vector3.down);
        }
        if (FindNeighbor(Vector3.left) != null)
        {
            output = FindNeighbor(Vector3.left);

        }
        if (FindNeighbor(Vector3.right) != null)
        {
            output = FindNeighbor(Vector3.right);
        }
        return output;
    }

    JRPGBattle FindNeighbor(Vector3 direction)
    {
        if(Physics.Raycast(transform.position, direction,out RaycastHit hit, reach))
        {
            return hit.collider.GetComponent<JRPGBattle>();
        }
        return null;
    }

    public void SetIsTurn()
    {
        isTurn = true;
        
    }

    protected JRPGBattle CheckAdjecent()
    {
        JRPGBattle contender = FindNeighbors();
        if (contender != null && !contender.Equals(this))
        {
            return contender;
        } 
        else
        {
            return null;
        }
    }

    public float GetHealthPercent()
    {
        if(maxHealth > 0)
        {
            //if you dont set it to float it will only return interger values
            float health = this.health;
            float maxHealth = this.maxHealth;
            return health / maxHealth;
        } 
        else
        {
            Debug.LogWarning(name + "has invalid health max");
            return 0;
        }
    }

    public float GetWillPercent()
    {
        if(maxWill != 0)
        {
            //if you dont set it to float it will only return interger values
            float will = this.will;
            float maxWill = this.maxWill;
            return will / maxWill;
        }
        else
        {
            Debug.LogWarning(name + "has invlaid will max");
            return .5f;
        }
    }

    public bool GetCanAttack()
    {
        JRPGBattle j = CheckAdjecent();
        if(j != null)
        {
            return true;
        }
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, reach);
    }
}
