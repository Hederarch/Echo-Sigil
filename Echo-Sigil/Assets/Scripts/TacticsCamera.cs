using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TacticsCamera : MonoBehaviour
{
    public CameraFoucus foucus;
    public float speed = .5f;
    public Camera cam;
    public static Vector3 forward;
    public static Vector3 right;
    float angle = (float)Math.PI;
    Vector2 previousMousePosition;

    // Update is called once per frame
    void Update()
    {
        PlayerInputs();
        FoucusInputs();
        forward = transform.forward;
        right = transform.right;
        cam.transparencySortMode = TransparencySortMode.CustomAxis;
        cam.transparencySortAxis = transform.up;
    }

    private void FoucusInputs()
    {
        if (!Input.GetMouseButtonDown(1) && foucus != null)
        {
            transform.position = foucus.CalcPostion(angle);
            transform.rotation = foucus.CalcRotation(transform.position,angle);
        }
    }

    private void PlayerInputs()
    {
        Vector3 desierdPosition = transform.position;
        if (Input.GetMouseButton(1))
        {
            desierdPosition += transform.up * (previousMousePosition.x - Input.mousePosition.x) * (speed * Time.deltaTime);
            desierdPosition += transform.right * (previousMousePosition.y - Input.mousePosition.y) * (speed * Time.deltaTime);
        }
        else
        {
            angle -= Input.GetAxis("Horizontal") * (speed * Time.deltaTime);
        }
        previousMousePosition = Input.mousePosition;
        transform.position = desierdPosition;
    }
}
