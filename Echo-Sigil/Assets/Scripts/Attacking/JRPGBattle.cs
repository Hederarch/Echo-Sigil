using System;
using System.Collections;
using System.Collections.Generic;
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

    public event Action EndEvent;

    public void SetCombatant(JRPGBattle combatant)
    {
        Camera.main.GetComponent<TacticsMovementCamera>().enabled = false;
        JRPGBattleCamera battleCamera = Camera.main.GetComponent<JRPGBattleCamera>();
        battleCamera.units.Add(this);
        battleCamera.units.Add(combatant);
        battleCamera.enabled = true;
    }

    public void EndCombat()
    {
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

    public float GetHealthPercent()
    {
        if(maxHealth != 0)
        {
            return health / maxHealth;
        } 
        else
        {
            return 0;
        }
    }

    public float GetWillPercent()
    {
        if(maxWill != 0)
        {
            return will / maxWill;
        }
        else
        {
            return 0;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawRay(transform.position, Vector3.up);
        Gizmos.DrawRay(transform.position, Vector3.down);
        Gizmos.DrawRay(transform.position, Vector3.left);
        Gizmos.DrawRay(transform.position, Vector3.right);
    }
}
