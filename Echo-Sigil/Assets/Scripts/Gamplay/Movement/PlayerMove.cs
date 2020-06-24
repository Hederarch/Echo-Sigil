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
        if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),out RaycastHit hit) && hit.collider.GetComponent<TileBehaviour>() && Input.GetMouseButtonDown(0) && hit.collider.GetComponent<TileBehaviour>().tile.selectable)
        {
            MoveToTile(hit.collider.GetComponent<TileBehaviour>().tile);
        }
    }
}
