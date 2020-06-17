using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Character in the world
/// </summary>
public class Implement : FacesTacticticsCamera, ITurn
{
    TacticsMove tacticsMove;

    private void Start()
    {
        tacticsMove = GetComponent<TacticsMove>();
        Init();
    }

    public void BeginTurn()
    {
        tacticsMove.isTurn = true;
        Camera.main.GetComponent<TacticsCamera>().foucus = this;
        tacticsMove.EndMoveEvent += TurnManager.EndTurn;
    }

    public void EndTurn()
    {
        tacticsMove.isTurn = false;
        tacticsMove.EndMoveEvent -= TurnManager.EndTurn;
    }

    public string GetTag()
    {
       return gameObject.tag;
    }

    public void Init()
    {
        TurnManager.AddUnit(this);
    }

}
