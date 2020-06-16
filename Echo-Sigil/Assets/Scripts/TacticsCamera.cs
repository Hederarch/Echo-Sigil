using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TacticsCamera : MonoBehaviour
{
    public FacesTacticticsCamera foucus;
    public float speed = .5f;
    public Camera cam;
    public float offsetDistance = 4;
    public float offsetAngle = 4;
    public static Vector3 forward;
    public static Vector3 right;

    bool cameraMoved;

    float desieredAngle = (float)Math.PI;
    private float angle = 0;
    Vector2 previousMousePosition;

    // Update is called once per frame
    void Update()
    {
        PlayerInputs();
        FoucusInputs();
        SetCameraFace();
        SetSortMode();
    }

    private void FoucusInputs()
    {
        float lerpAngle;
        if(Mathf.Abs(desieredAngle - angle) > .5f)
        {
            lerpAngle = Mathf.Lerp(angle, desieredAngle, .1f);
        } else
        {
            lerpAngle = desieredAngle;
        }
        if (!Input.GetMouseButtonDown(1) && foucus != null)
        {
            transform.position = CalcPostion(lerpAngle);
            transform.rotation = CalcRotation(transform.position,lerpAngle);
            angle = lerpAngle;
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
        else if(Input.GetAxisRaw("Horizontal") != 0 && !cameraMoved)
        {
            desieredAngle -= Input.GetAxisRaw("Horizontal") * Mathf.PI/2;
            cameraMoved = true;
        }
        else if(Input.GetAxisRaw("Horizontal") == 0)
        {
            cameraMoved = false;
        }
        previousMousePosition = Input.mousePosition;
        transform.position = desierdPosition;
    }

    private Vector3 CalcPostion(float angle)
    {
        Vector3 offset = new Vector3((float)Math.Sin(angle), (float)Math.Cos(angle));
        offset *= offsetDistance;
        offset.z = -offsetAngle;
        return foucus.transform.position + offset;
    }

    private Quaternion CalcRotation(Vector3 position, float angle)
    {
        Vector3 rotation = Quaternion.LookRotation(foucus.transform.position - position).eulerAngles;
        rotation.z = -angle * Mathf.Rad2Deg;
        return Quaternion.Euler(rotation);
    }

    private void SetCameraFace()
    {
        forward = transform.forward;
        right = transform.right;
    }

    private void SetSortMode()
    {
        cam.transparencySortMode = TransparencySortMode.CustomAxis;
        cam.transparencySortAxis = transform.up;
    }
}
