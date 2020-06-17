using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    static Dictionary<string, List<ITurn>> units = new Dictionary<string, List<ITurn>>();
    static Queue<string> turnKey = new Queue<string>();
    static Queue<ITurn> turnTeam = new Queue<ITurn>();

    private void Update()
    {
        if (turnTeam.Count == 0)
        {
            InitTeamTurnQueue();
        }
    }
    static void InitTeamTurnQueue()
    {
        List<ITurn> teamList = units[turnKey.Peek()];

        foreach (ITurn unit in teamList)
        {
            turnTeam.Enqueue(unit);
        }

        StartTurn();
    }

    static void StartTurn()
    {
        if (turnTeam.Count > 0)
        {
            turnTeam.Peek().BeginTurn();
        }
    }

    public static void EndTurn()
    {
        ITurn unit = turnTeam.Dequeue();
        unit.EndTurn();

        if (turnTeam.Count > 0)
        {
            StartTurn();
        }
        else
        {
            string team = turnKey.Dequeue();
            turnKey.Enqueue(team);
            InitTeamTurnQueue();
        }
    }

    public static void AddUnit(ITurn unit)
    {
        List<ITurn> list;

        if (!units.ContainsKey(unit.GetTag()))
        {
            list = new List<ITurn>();
            units[unit.GetTag()] = list;

            if (!turnKey.Contains(unit.GetTag()))
            {
                turnKey.Enqueue(unit.GetTag());
            }
        } else
        {
            list = units[unit.GetTag()];
        }

        list.Add(unit);
    }
}
