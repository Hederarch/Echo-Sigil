using System;

/// <summary>
/// Character in the world
/// </summary>
public class Unit : FacesCamera, ITurn
{
    public IMovement move;
    public IBattle battle;

    public bool hasMoved;
    public bool hasAttacked;

    public string Tag { get => gameObject.tag; }

    public static event Action<Unit> IsTurnEvent;

    private void Start()
    {
        move = GetComponent<TacticsMove>();
        battle = GetComponent<JRPGBattle>();
    }

    public virtual void BeginTurn()
    {
        TurnManager.CheckForWin();
        IsTurnEvent?.Invoke(this);
        Subscribe();
    }

    public virtual void EndTurn()
    {
        TurnManager.CheckForWin();
        ResetUnit();
        UnSubscribe();
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

    public virtual void DeInit()
    {
        TurnManager.RemoveUnit(this);
    }
}
