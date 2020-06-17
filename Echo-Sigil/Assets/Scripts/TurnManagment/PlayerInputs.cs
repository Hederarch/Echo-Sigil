using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputs : MonoBehaviour
{
    public static event Action MoveEvent;
    public static event Action AttackEvent;
    public void Move()
    {
        if (TurnManager.isPlayerTurn)
        {
            MoveEvent?.Invoke();
        }
    }

    public void Attack()
    {
        if (TurnManager.isPlayerTurn)
        {
            AttackEvent?.Invoke();
        }
    }

    public void Skip()
    {
        if (TurnManager.isPlayerTurn)
        {
            TurnManager.EndTurn();
        }
    }
}
