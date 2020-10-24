using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour
{
    static Dictionary<string, List<ITurn>> units = new Dictionary<string, List<ITurn>>();
    static Queue<string> turnKey = new Queue<string>();
    static Queue<ITurn> turnTeam = new Queue<ITurn>();

    public static bool IsPlayerTurn { get => turnKey.Peek() == "Player" ; }
    public static ITurn CurrentUnit { get => turnTeam.Peek() ; }

    public static event Action GameWinEvent;
    public static event Action GameLoseEvent;

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
            ITurn i = turnTeam.Peek();
            if(i != null)
            {
                i.BeginTurn();
            } 
            else
            {
                turnTeam.Dequeue();
                StartTurn();
            }
            
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
    public static void CheckForWin()
    {
        if (units["Player"].Count <= 0)
        {
            GameLoseEvent?.Invoke();
        }
        if (units["NPC"].Count <= 0)
        {
            GameWinEvent?.Invoke();
        }
    }
    public static void AddUnit(ITurn unit)
    {
        List<ITurn> list;

        if (!units.ContainsKey(unit.Tag))
        {
            list = new List<ITurn>();
            units[unit.Tag] = list;

            if (!turnKey.Contains(unit.Tag))
            {
                turnKey.Enqueue(unit.Tag);
            }
        } 
        else
        {
            list = units[unit.Tag];
        }

        list.Add(unit);
    }
    public static void RemoveUnit(ITurn unit)
    {
        string unitTag = unit.Tag;
        List<ITurn> dictonaryList = units[unitTag];
        if (dictonaryList.Contains(unit))
        {
            dictonaryList.Remove(unit);
        }
        if (turnKey.Peek() == unitTag)
        {
            //Cant do the same thing for this. Bugger. Alright, I guess muder lets you move more, why not
            turnTeam.Clear();
            InitTeamTurnQueue();
        }
    }
}
