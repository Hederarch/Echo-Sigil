using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : TacticsMove
{
    // Update is called once per frame
    void Update()
    {
        //Tactitics movement
        if (isTurn)
        {
            if (!moveing)
            {
                FindSelectableTiles();
                CheckMouse();
            }
            else
            {
                Move();
            }
        }
    }

    void CheckMouse()
    {
        if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),out RaycastHit hit) && hit.collider.GetComponent<Tile>() && Input.GetMouseButtonDown(0) && hit.collider.GetComponent<Tile>().selectable)
        {
            MoveToTile(hit.collider.GetComponent<Tile>());
        }
    }
}
