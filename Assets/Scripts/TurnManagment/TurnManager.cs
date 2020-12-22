using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TurnManager
{
    public static Queue<Team> teamQueue = new Queue<Team>();

    public static Action<bool> playerWinEvent;

    public static bool IsPlayerTurn => teamQueue.Peek().tag == "Player";
    public static ITurn Current => teamQueue.Peek().Current;

    public static void Reset()
    {
        teamQueue.Clear();
    }

    public static void StartTurn()
    {
        if (teamQueue.Count > 0)
        {
            CheckForWin();
            Team team = teamQueue.Peek();
            if (team != null)
            {
                if (team.MoveNext())
                {
                    team.Current.BeginTurn();
                }
                else
                {
                    team.Reset();
                    teamQueue.Enqueue(teamQueue.Dequeue());
                    StartTurn();
                }
            }
            else
            {
                teamQueue.Dequeue();
                StartTurn();
            }
        }
    }
    public static void EndTurn()
    {
        Current.EndTurn();
        StartTurn();
    }
    public static void CheckForWin()
    {
        foreach (Team entry in teamQueue)
        {
            if (entry.units.Count <= 0)
            {
                playerWinEvent?.Invoke(entry.tag == PlayerUnit.playerTag);
            }
        }
    }
    public static void AddUnit(ITurn unit)
    {
        //Do we already have this team?
        foreach (Team team in teamQueue)
        {
            if (team.tag == unit.Tag)
            {
                team.units.Enqueue(unit);
                return;
            }
        }
        //Create new team
        teamQueue.Enqueue(new Team(unit.Tag, unit, unit.GetTeamColor(), unit.GetTeamTexture()));

    }
    public static void RemoveUnit(ITurn unit)
    {
        //Do we already have this team?
        foreach (Team team in teamQueue)
        {
            if (team.tag == unit.Tag)
            {
                if (team.units.Contains(unit))
                {
                    if(unit == team.units.Peek())
                    {
                        team.units.Dequeue();
                    }
                    else
                    {
                        bool removed = false;
                        ITurn current = team.units.Peek();
                        while(!removed && current != team.units.Peek())
                        {
                            ITurn considered = team.units.Dequeue();
                            if(considered != unit)
                            {
                                team.units.Enqueue(considered);
                            }
                            else
                            {
                                removed = true;
                            }
                        }
                    }
                    CheckForWin();
                    return;
                }
                throw new NullReferenceException("Team " + unit.Tag + " does not contain " + unit.GetHashCode());
            }
        }
        throw new NullReferenceException("Team " + unit.Tag + " does not exist");
    }
}

public interface ITurn
{
    string Tag { get; }
    Color GetTeamColor();
    Texture2D GetTeamTexture();
    void BeginTurn();
    void EndTurn();
}

public class Team : IEnumerator<ITurn>, IEquatable<string>
{
    public Queue<ITurn> units = new Queue<ITurn>();
    public string tag;
    public Color color;
    private Texture2D icon;
    public Texture2D Icon { get => GetIcon(); set => icon = value; }

    private int curIndex;
    public ITurn Current => units.Peek();

    object IEnumerator.Current => Current;

    public Texture2D GetIcon()
    {
        if (icon == null)
        {
            Color[] colors = new Color[64 * 64];
            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = color;
            }
            Texture2D texture2D = new Texture2D(64, 64);
            texture2D.SetPixels(colors);
            texture2D.Apply();
            return texture2D;
        }
        return icon;

    }

    public bool MoveNext()
    {
        curIndex++;
        if (curIndex >= units.Count)
        {
            Reset();
            return false;
        }
        units.Enqueue(units.Dequeue());
        return true;
    }

    public void Reset()
    {
        curIndex = -1;
    }

    public void Dispose()
    {

    }

    public bool Equals(string other)
    {
        return tag.Equals(other);
    }

    public Team(string tag, ITurn first) : this(tag, first, Color.black) { }
    public Team(string tag, ITurn first, Color color, Texture2D icon = null)
    {
        Reset();
        this.tag = tag;
        units.Enqueue(first);
        this.color = color;
        this.icon = icon;
    }
}