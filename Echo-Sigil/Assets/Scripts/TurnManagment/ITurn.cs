using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITurn
{
    string Tag { get; }
    void BeginTurn();
    void EndTurn();
}
