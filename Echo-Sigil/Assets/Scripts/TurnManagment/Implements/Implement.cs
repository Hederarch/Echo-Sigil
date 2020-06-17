using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Character in the world
/// </summary>
public class Implement : FacesTacticticsCamera, ITurn
{
    protected TacticsMove tacticsMove;
    protected JRPGBattle jRPGBattle;

    protected bool hasMoved;
    protected bool hasAttacked;

    private void Start()
    {
        tacticsMove = GetComponent<TacticsMove>();
        jRPGBattle = GetComponent<JRPGBattle>();
        Init();
    }

    public virtual void BeginTurn()
    {
        Camera.main.GetComponent<TacticsCamera>().foucus = this;
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
        tacticsMove.EndMoveEvent += HasMoved;
    }
    protected virtual void UnSubscribe()
    {
        tacticsMove.EndMoveEvent -= HasMoved;
    }

    public virtual void ResetUnit()
    {
        hasMoved = false;
        hasAttacked = false;
    }

}
