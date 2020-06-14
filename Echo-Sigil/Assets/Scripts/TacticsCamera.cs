using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TacticsCamera : MonoBehaviour
{
    public Transform foucus;
    Vector3 previousFoucusPosition;
    Vector2 previousMousePosition;

    // Update is called once per frame
    void Update()
    {
        PlayerInputs();
        if (foucus != null)
        {
            FoucusInputs();
        }
    }

    private void FoucusInputs()
    {
        transform.position += previousFoucusPosition - foucus.position;
        previousFoucusPosition = foucus.position;
    }

    private void PlayerInputs()
    {
        Vector3 desierdMovement = new Vector2();
        desierdMovement.x += Input.GetAxis("Horizontal");
        desierdMovement.y += Input.GetAxis("Vertical");
        if (Input.GetMouseButton(1))
        {
            desierdMovement.x += previousMousePosition.x - Input.mousePosition.x;
            desierdMovement.y += previousMousePosition.y - Input.mousePosition.y;
        }
        previousMousePosition = Input.mousePosition;
        transform.position += desierdMovement;


    }
}
