using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using UnityEngine;

public class JRPGBattle : MonoBehaviour , IBattle
{
    public readonly bool leftSide;

    public int health =5;
    public int maxHealth = 5;

    public int will =5;
    public int maxWill = 5;

    public List<Ability> abilites = new List<Ability>();
    public float reach;

    public bool isTurn;
    public bool inBattle;

    public event Action EndEvent;

    public void SetCombatant(JRPGBattle combatant)
    {
        if(combatant != null && !inBattle)
        {
            inBattle = true;
            //needs to be a corutine for the animation to work
            StartCoroutine(SetCombatantCoroutine(combatant));
        }
    }

    IEnumerator SetCombatantCoroutine(JRPGBattle combatant)
    {
        FightGUIScript.SetBattle();
        yield return new WaitForSeconds(.3f);
        Camera.main.GetComponent<TacticsMovementCamera>().enabled = false;
        JRPGBattleCamera battleCamera = Camera.main.GetComponent<JRPGBattleCamera>();
        battleCamera.units.Clear();
        battleCamera.units.Add(this);
        battleCamera.units.Add(combatant);
        battleCamera.enabled = true;
        FightGUIScript.SetMenu(this);
        FightGUIScript.SetStats();
    }

    public void EndCombat()
    {
        inBattle = false;
        StartCoroutine(EndCombatCorutine());
    }

    IEnumerator EndCombatCorutine()
    {
        FightGUIScript.UnSetBattle();
        yield return new WaitForSeconds(.1f);
        ResetBattleCamera(Camera.main.GetComponent<JRPGBattleCamera>());
        Camera.main.GetComponent<TacticsMovementCamera>().enabled = true;
        FightGUIScript.ResetMenuStats();
        EndEvent?.Invoke();
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
        if(Physics.Raycast(transform.position, direction,out RaycastHit hit))
        {
            return hit.collider.GetComponent<JRPGBattle>();
        }
        return null;
    }

    private void ResetBattleCamera(JRPGBattleCamera battleCamera)
    {
        throw new NotImplementedException();
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
}
