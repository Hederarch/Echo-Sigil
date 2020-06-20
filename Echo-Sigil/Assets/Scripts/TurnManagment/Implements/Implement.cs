using System;

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

    public string GetTag()
    {
       return gameObject.tag;
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

    private void OnDestroy()
    {
        TurnManager.RemoveUnit(this);
    }
}
