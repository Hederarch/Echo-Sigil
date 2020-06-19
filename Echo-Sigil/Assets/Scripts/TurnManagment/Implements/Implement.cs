using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Character in the world
/// </summary>
public class Implement : FacesCamera, ITurn
{
    public IMovement move;
    public IBattle battle;

    public bool hasMoved;
    public bool hasAttacked;

    public static event Action<Implement> IsTurnEvent;

    private void Start()
    {
        move = GetComponent<TacticsMove>();
        battle = GetComponent<JRPGBattle>();
        Init();
    }

    public virtual void BeginTurn()
    {
        IsTurnEvent?.Invoke(this);
        Subscribe();
    }

    public virtual void EndTurn()
    {
        ResetUnit();
        UnSubscribe();
    }

    public string GetTag()
    {
       return gameObject.tag;
    }

    public virtual void Init()
    {
        TurnManager.AddUnit(this);
    }

    public void HasMoved()
    {
        hasMoved = true;
    }

    public void HasAttacked()
    {
        hasAttacked = true;
    }

    protected virtual void Subscribe()
    {
        move.EndEvent += HasMoved;
        battle.EndEvent += HasAttacked;
    }
    protected virtual void UnSubscribe()
    {
        move.EndEvent -= HasMoved;
        battle.EndEvent -= HasAttacked;
    }

    public virtual void ResetUnit()
    {
        hasMoved = false;
        hasAttacked = false;
    }

}
