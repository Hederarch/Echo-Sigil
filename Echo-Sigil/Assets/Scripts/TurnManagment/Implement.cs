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
    public Text bugText;

    private void Start()
    {
        tacticsMove = GetComponent<TacticsMove>();
        Init();
    }

    public void BeginTurn()
    {
        bugText.text +=" is turn of " + gameObject.name;
        tacticsMove.isTurn = true;
        Camera.main.GetComponent<TacticsCamera>().foucus = this;
        tacticsMove.EndMoveEvent += TurnManager.EndTurn;
    }

    public void EndTurn()
    {
        bugText.text =" Was turn of " + gameObject.name;
        tacticsMove.isTurn = false;
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
