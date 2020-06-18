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
        if(combatant != null)
        {
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
        inBattle = true;
    }

    public void EndCombat()
    {
        StartCoroutine(EndCombatCorutine());
    }

    IEnumerator EndCombatCorutine()
    {
        FightGUIScript.SetBattle();
        yield return new WaitForSeconds(.1f);
        ResetBattleCamera(Camera.main.GetComponent<JRPGBattleCamera>());
        Camera.main.GetComponent<TacticsMovementCamera>().enabled = true;
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

    protected bool CheckAdjecent()
    {
        JRPGBattle contender = FindNeighbors();
        if (contender != null && !contender.Equals(this))
        {
            SetCombatant(contender);
            return true;
        } 
        else
        {
            return false;
        }
    }

    public float GetHealthPercent()
    {
        if(maxHealth != 0)
        {
            print(health + "/" + maxHealth + "=" + health / maxHealth + "% health");
            return health / maxHealth;
        } 
        else
        {
            Debug.LogWarning(name + "has no health max");
            return .5f;
        }
    }

    public float GetWillPercent()
    {
        if(maxWill != 0)
        {
            print(will + "/" + maxWill + "=" + will / maxWill + "% will");
            return will / maxWill;
        }
        else
        {
            Debug.LogWarning(name + "has no will max");
            return .5f;
        }
    }

}
