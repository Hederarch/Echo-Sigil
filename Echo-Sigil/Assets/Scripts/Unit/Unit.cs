using MapEditor;
using System;
using System.Numerics;
using UnityEngine;

/// <summary>
/// Character in the world
/// </summary>
public class Unit : FacesCamera, ITurn, IMovement, IBattle
{
    public bool hasMoved;
    public bool hasAttacked;

    public int implementListIndex = 0;
    public Vector2Int posInGrid;
    public float curHeight;

    public string Tag { get => gameObject.tag; }
    public bool IsTurn { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public bool CanMove { get; set; }

    public bool CanAttack { get; set; }

    public float HealthPercent => throw new NotImplementedException();

    public float WillPercent => throw new NotImplementedException();



    public static event Action<Unit> IsTurnEvent;
    public event Action EndEvent;

    public virtual void BeginTurn()
    {
        TurnManager.CheckForWin();
        IsTurnEvent?.Invoke(this);
    }

    public virtual void EndTurn()
    {
        TurnManager.CheckForWin();
        ResetUnit();
    }

    public virtual void Init()
    {
        TurnManager.AddUnit(this);
    }

    public virtual void HasMoved()
    {
        hasMoved = true;
    }

    public virtual void HasAttacked()
    {
        hasAttacked = true;
    }

    public virtual void ResetUnit()
    {
        hasMoved = false;
        hasAttacked = false;
    }

    internal void SetValues(MapImplement.MovementSettings movementSettings)
    {
        throw new NotImplementedException();
    }

    internal void SetValues(MapImplement.BattleSettings battleSettings)
    {
        throw new NotImplementedException();
    }
}
