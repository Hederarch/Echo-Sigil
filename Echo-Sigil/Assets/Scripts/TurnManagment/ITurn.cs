using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITurn
{
    /// <summary>
    /// Should be called somewhere in Start()
    /// </summary>
    void Init();
    string Tag { get; }
    void BeginTurn();
    void EndTurn();
}
